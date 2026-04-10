<template>
  <div class="admin-tech-schedules">
    <LoadingOverlay v-if="processingGlobal" />
    <!-- Debug Info -->
    <UserDebugInfo />

    <div class="header-section">
      <h2>Gestión de Horarios de Técnicos</h2>
      <p class="lead">Administrar disponibilidad y citas de técnicos</p>
    </div>

    <!-- Modales personalizados -->
    <ConfirmModal
      v-model="modalConfirm.show"
      :title="modalConfirm.title"
      :message="modalConfirm.message"
      :type="modalConfirm.type"
      :show-cancel="true"
      @confirm="modalConfirm.onConfirm"
    />

    <ConfirmModal
      v-model="modalAlert.show"
      :title="modalAlert.title"
      :message="modalAlert.message"
      :type="modalAlert.type"
      :show-cancel="false"
    />

    <!-- Selector de técnico -->
    <div class="card mb-3">
      <div class="card-body">
        <div class="row align-items-end">
          <div class="col-md-4">
            <label class="form-label">Seleccionar Técnico</label>
            <select v-model="tecnicoSeleccionado" class="form-select" @change="cargarDatos">
              <option :value="null">-- Seleccione un técnico --</option>
              <option v-for="tec in tecnicos" :key="tec.idUnico" :value="tec.idUnico">
                {{ tec.nombre }} {{ tec.apellido1 }}
              </option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label">Semana</label>
            <input type="date" v-model="fechaInicio" class="form-control" @change="cargarDatos" />
          </div>
          <div class="col-md-3">
            <button class="btn btn-primary" @click="generarHorarios" :disabled="!tecnicoSeleccionado">
              <i class="fa-solid fa-calendar-plus"></i> Generar Semana
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Calendario semanal -->
    <div v-if="tecnicoSeleccionado" class="calendar-grid">
      <div class="calendar-header">Horarios de la Semana</div>
      <div class="days-row">
        <div v-for="dia in diasSemana" :key="dia.fecha" class="day-column">
          <div class="day-header">
            <strong>{{ dia.nombre }}</strong>
            <small>{{ formatoFecha(dia.fecha) }}</small>
          </div>
          <div class="time-slots">
            <div
              v-for="slot in obtenerSlotsDelDia(dia.fecha)"
              :key="slot.idHorario"
              class="time-slot"
              :class="estadoSlot(slot)"
            >
              <div class="slot-time">{{ slot.horaInicio }}</div>
              <div class="slot-status">
                <span v-if="!slot.disponible && slot.motivoBloqueo" class="badge bg-warning">
                  Bloqueado
                </span>
                <span v-else-if="!slot.disponibleReal" class="badge bg-danger">Ocupado</span>
                <span v-else class="badge bg-success">Disponible</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Citas del técnico -->
    <div v-if="tecnicoSeleccionado && citas.length > 0" class="card mt-4">
      <div class="card-body">
        <h5>Citas Programadas</h5>
        <div class="table-responsive">
          <table class="table">
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Hora</th>
                <th>Cliente</th>
                <th>Problema</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="cita in citas" :key="cita.idCita">
                <td>{{ formatoFecha(cita.fechaCita) }}</td>
                <td>{{ cita.horaInicio }}</td>
                <td>{{ cita.nombreCliente }}</td>
                <td>{{ cita.descripcionProblema }}</td>
                <td><span :class="`badge bg-${getEstadoColor(cita.estado)}`">{{ cita.estado }}</span></td>
                <td>
                  <button
                    v-if="cita.estado === 'PENDIENTE'"
                    class="btn btn-sm btn-success me-1"
                    @click="confirmarCita(cita.idCita)"
                  >
                    <i class="bi bi-check-circle"></i> Confirmar
                  </button>
                  <button
                    v-if="cita.estado === 'CONFIRMADA'"
                    class="btn btn-sm btn-primary me-1"
                    @click="completarCita(cita.idCita)"
                  >
                    <i class="bi bi-check-all"></i> Completar
                  </button>
                  <button
                    v-if="cita.estado !== 'CANCELADA' && cita.estado !== 'COMPLETADA'"
                    class="btn btn-sm btn-danger"
                    @click="cancelarCita(cita.idCita)"
                  >
                    <i class="bi bi-x-circle"></i> Cancelar
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Loading overlay -->
    <div v-if="loading" class="loading-overlay">
      <div class="spinner-border text-primary" role="status"></div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import CitaService from '@/services/CitaService'
import { usuarioService } from '@/services/UserService'
import ConfirmModal from '@/components/ConfirmModal.vue'
import UserDebugInfo from '@/components/UserDebugInfo.vue'
import LoadingOverlay from './LoadingOverlay.vue'

const tecnicoSeleccionado = ref(null)
const tecnicos = ref([])
const fechaInicio = ref(new Date().toISOString().split('T')[0])
const horarios = ref([])
const citas = ref([])
const loading = ref(false)
const processingGlobal = ref(false)

// Modales personalizados
const modalConfirm = ref({
  show: false,
  title: '',
  message: '',
  type: 'question',
  onConfirm: () => {}
})

const modalAlert = ref({
  show: false,
  title: '',
  message: '',
  type: 'success'
})

const diasSemana = computed(() => {
  // Normalizar al lunes de la semana de la fecha seleccionada
  const selected = new Date(fechaInicio.value + 'T00:00:00')
  const day = selected.getDay() // 0=Dom,1=Lun,...
  const diffToMonday = (day === 0 ? -6 : 1 - day) // si Domingo, retrocede 6; si otro, hasta lunes
  const monday = new Date(selected)
  monday.setDate(selected.getDate() + diffToMonday)

  const dias = []
  for (let i = 0; i < 5; i++) {
    const fecha = new Date(monday)
    fecha.setDate(monday.getDate() + i)
    const diaSemana = fecha.getDay()
    dias.push({
      fecha: fecha.toISOString().split('T')[0],
      nombre: ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'][diaSemana]
    })
  }
  console.log('diasSemana computed:', dias)
  return dias
})

async function cargarTecnicos () {
  try {
    const usuarios = await usuarioService.getAllUsers()
    console.log('Usuarios recibidos:', usuarios)
    console.log('Primer usuario:', usuarios[0])
    tecnicos.value = usuarios.filter(u => u.tipoUsuario === 'TECNICO')
    console.log('Técnicos filtrados:', tecnicos.value)
  } catch (error) {
    console.error('Error cargando técnicos:', error)
  }
}

async function cargarDatos () {
  if (!tecnicoSeleccionado.value) return
  loading.value = true
  processingGlobal.value = true
  try {
    // Calcular el lunes de la semana seleccionada
    const selected = new Date(fechaInicio.value + 'T00:00:00')
    const day = selected.getDay()
    const diffToMonday = (day === 0 ? -6 : 1 - day)
    const monday = new Date(selected)
    monday.setDate(selected.getDate() + diffToMonday)
    // Calcular el viernes (4 días después del lunes)
    const friday = new Date(monday)
    friday.setDate(monday.getDate() + 4)

    const fechaDesde = monday.toISOString().split('T')[0]
    const fechaHasta = friday.toISOString().split('T')[0]

    console.log('=== CARGAR DATOS ===')
    console.log('Fecha seleccionada:', fechaInicio.value)
    console.log('Día de la semana:', day, '(0=Dom, 1=Lun...)')
    console.log('Diff to Monday:', diffToMonday)
    console.log('Lunes calculado:', fechaDesde)
    console.log('Viernes calculado:', fechaHasta)

    horarios.value = await CitaService.obtenerHorariosDisponibles(
      tecnicoSeleccionado.value,
      fechaDesde,
      fechaHasta
    )

    console.log('Horarios recibidos:', horarios.value.length, 'horarios')
    console.log('Fechas de horarios:', [...new Set(horarios.value.map(h => h.fecha.split('T')[0]))])

    citas.value = await CitaService.obtenerCitasTecnico(
      tecnicoSeleccionado.value,
      fechaDesde,
      fechaHasta
    )
  } catch (error) {
    console.error('Error cargando datos:', error)
  } finally {
    loading.value = false
    processingGlobal.value = false
  }
}

function obtenerSlotsDelDia (fecha) {
  const slots = horarios.value.filter(h => {
    const fechaHorario = h.fecha?.split('T')[0]
    return fechaHorario === fecha
  })
  if (fecha === '2025-11-24') {
    console.log('Buscando slots para 2025-11-24:', slots.length, 'encontrados')
    console.log('Primer horario completo:', horarios.value[0])
  }
  return slots
}

function estadoSlot (slot) {
  if (!slot.disponible && slot.motivoBloqueo) return 'slot-blocked'
  if (!slot.disponibleReal) return 'slot-busy'
  return 'slot-available'
}

function formatoFecha (fecha) {
  const d = new Date(fecha)
  return d.toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit' })
}

function getEstadoColor (estado) {
  const colores = {
    PENDIENTE: 'warning',
    CONFIRMADA: 'info',
    COMPLETADA: 'success',
    CANCELADA: 'secondary'
  }
  return colores[estado] || 'secondary'
}

async function generarHorarios () {
  if (!tecnicoSeleccionado.value) return
  loading.value = true
  try {
    // Calcular el lunes de la semana seleccionada
    const selected = new Date(fechaInicio.value + 'T00:00:00')
    const day = selected.getDay()
    const diffToMonday = (day === 0 ? -6 : 1 - day)
    const monday = new Date(selected)
    monday.setDate(selected.getDate() + diffToMonday)
    const lunesStr = monday.toISOString().split('T')[0]
    console.log('=== GENERAR HORARIOS ===')
    console.log('Fecha seleccionada:', fechaInicio.value)
    console.log('Generando horarios desde el lunes:', lunesStr)
    processingGlobal.value = true
    await CitaService.generarHorariosSemana(tecnicoSeleccionado.value, lunesStr)
    // Esperar un momento para que la BD se actualice
    await new Promise(resolve => setTimeout(resolve, 500))
    await cargarDatos()
    alert('Horarios generados exitosamente')
  } catch (error) {
    console.error('Error generando horarios:', error)
    alert('Error al generar horarios: ' + error.message)
  } finally {
    loading.value = false
    processingGlobal.value = false
  }
}

async function confirmarCita (idCita) {
  modalConfirm.value = {
    show: true,
    title: 'Confirmar Cita',
    message: '¿Desea confirmar esta cita?',
    type: 'question',
    onConfirm: async () => {
      processingGlobal.value = true
      try {
        // Debug: Verificar token y usuario
        const token = localStorage.getItem('token')
        console.log('Token presente:', !!token)
        if (token) {
          try {
            const payload = JSON.parse(atob(token.split('.')[1]))
            console.log('Usuario del token:', payload)
            console.log('Rol del token:', payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'])
          } catch (e) {
            console.error('Error decodificando token:', e)
          }
        }

        await CitaService.confirmarCita(idCita)
        modalAlert.value = {
          show: true,
          title: 'Éxito',
          message: 'Cita confirmada exitosamente',
          type: 'success'
        }
        await cargarDatos()
      } catch (error) {
        console.error('Error confirmando cita:', error)
        console.error('Detalles del error:', error.response)
        let errorMsg = 'Error desconocido'
        if (error.response?.status === 403) {
          errorMsg = 'No tienes permisos para confirmar esta cita. Verifica tu sesión.'
        } else if (error.response?.data?.message) {
          errorMsg = error.response.data.message
        } else if (error.message) {
          errorMsg = error.message
        }
        modalAlert.value = {
          show: true,
          title: 'Error',
          message: errorMsg,
          type: 'danger'
        }
      } finally {
        processingGlobal.value = false
      }
    }
  }
}

async function completarCita (idCita) {
  modalConfirm.value = {
    show: true,
    title: 'Completar Cita',
    message: '¿Marcar esta cita como completada?',
    type: 'question',
    onConfirm: async () => {
      processingGlobal.value = true
      try {
        await CitaService.completarCita(idCita)
        modalAlert.value = {
          show: true,
          title: 'Éxito',
          message: 'Cita completada exitosamente',
          type: 'success'
        }
        await cargarDatos()
      } catch (error) {
        console.error('Error completando cita:', error)
        modalAlert.value = {
          show: true,
          title: 'Error',
          message: 'Error al completar cita: ' + (error.response?.data?.message || error.message),
          type: 'danger'
        }
      } finally {
        processingGlobal.value = false
      }
    }
  }
}

async function cancelarCita (idCita) {
  modalConfirm.value = {
    show: true,
    title: 'Cancelar Cita',
    message: '¿Estás seguro de cancelar esta cita? Esta acción no se puede deshacer.',
    type: 'danger',
    onConfirm: async () => {
      processingGlobal.value = true
      try {
        await CitaService.cancelarCita(idCita)
        modalAlert.value = {
          show: true,
          title: 'Éxito',
          message: 'Cita cancelada exitosamente',
          type: 'success'
        }
        await cargarDatos()
      } catch (error) {
        console.error('Error cancelando cita:', error)
        modalAlert.value = {
          show: true,
          title: 'Error',
          message: 'Error al cancelar cita: ' + (error.response?.data?.message || error.message),
          type: 'danger'
        }
      } finally {
        processingGlobal.value = false
      }
    }
  }
}

onMounted(() => {
  cargarTecnicos()
})
</script>

<style scoped>
.admin-tech-schedules { padding: 1.5rem; background: var(--dp-bg); min-height: 100vh; }
.header-section { margin-bottom: 1.5rem; }
.header-section h2 { color: var(--dp-primary); margin-bottom: .25rem; }
.lead { color: #0b2136; opacity: .85; }

.calendar-grid { background: #fff; border-radius: 12px; padding: 1rem; box-shadow: 0 6px 16px rgba(3,33,63,0.06); }
.calendar-header { font-weight: 700; color: var(--dp-darkest); margin-bottom: .75rem; }
.days-row { display: grid; grid-template-columns: repeat(5, 1fr); gap: .75rem; }
.day-column { border: 1px solid rgba(3,33,63,.1); border-radius: 8px; padding: .5rem; }
.day-header { text-align: center; padding: .5rem; background: rgba(11,91,215,.06); border-radius: 6px; margin-bottom: .5rem; }
.day-header strong { display: block; color: var(--dp-primary); }
.day-header small { color: #0b2136; opacity: .75; }

.time-slots { display: flex; flex-direction: column; gap: .35rem; }
.time-slot { padding: .4rem; border-radius: 6px; font-size: .85rem; text-align: center; }
.slot-available { background: rgba(0,200,83,.08); border: 1px solid rgba(0,200,83,.2); }
.slot-busy { background: rgba(220,53,69,.08); border: 1px solid rgba(220,53,69,.2); }
.slot-blocked { background: rgba(255,193,7,.08); border: 1px solid rgba(255,193,7,.2); }
.slot-time { font-weight: 600; color: var(--dp-darkest); }
.slot-status .badge { font-size: .7rem; }

.loading-overlay { position: fixed; inset: 0; background: rgba(0,0,0,.3); display: grid; place-items: center; z-index: 9999; }
</style>
