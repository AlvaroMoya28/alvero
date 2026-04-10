<template>
  <div class="tech-dashboard">
    <div class="header-section">
      <h2>Panel de Técnico</h2>
      <p class="lead">Gestiona tu horario y citas</p>
    </div>

    <!-- Selector de semana -->
    <div class="card mb-3">
      <div class="card-body">
        <div class="row align-items-end">
          <div class="col-md-4">
            <label class="form-label">Semana</label>
            <input type="date" v-model="fechaInicio" class="form-control" @change="cargarDatos" />
          </div>
          <div class="col-md-4">
            <button class="btn btn-secondary" @click="cargarDatos">
              <i class="fa-solid fa-refresh"></i> Actualizar
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Calendario semanal del técnico -->
    <div class="calendar-grid mb-4">
      <div class="calendar-header">Mi Horario de la Semana</div>
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
              @click="toggleBloqueo(slot)"
            >
              <div class="slot-time">{{ slot.horaInicio }}</div>
              <div class="slot-status">
                <span v-if="!slot.disponible && slot.motivoBloqueo" class="badge bg-warning text-dark">
                  <i class="fa-solid fa-lock"></i> Bloqueado
                </span>
                <span v-else-if="!slot.disponibleReal" class="badge bg-danger">
                  <i class="fa-solid fa-calendar-check"></i> Cita
                </span>
                <span v-else class="badge bg-success">
                  <i class="fa-solid fa-check"></i> Libre
                </span>
              </div>
              <small v-if="slot.motivoBloqueo" class="motivo-text">{{ slot.motivoBloqueo }}</small>
            </div>
          </div>
        </div>
      </div>
      <div class="legend mt-2">
        <span class="legend-item"><span class="badge bg-success">Libre</span> Disponible para citas</span>
        <span class="legend-item"><span class="badge bg-danger">Cita</span> Con cita agendada</span>
        <span class="legend-item"><span class="badge bg-warning text-dark">Bloqueado</span> No disponible (click para editar)</span>
      </div>
    </div>

    <!-- Lista de citas -->
    <div class="card">
      <div class="card-body">
        <h5>Mis Citas Programadas</h5>
        <div v-if="citas.length === 0" class="text-muted">No hay citas programadas para esta semana.</div>
        <div v-else class="table-responsive">
          <table class="table">
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Hora</th>
                <th>Cliente</th>
                <th>Problema</th>
                <th>Contacto</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="cita in citas" :key="cita.idCita">
                <td>{{ formatoFecha(cita.fechaCita) }}</td>
                <td>{{ cita.horaInicio }} - {{ cita.horaFin }}</td>
                <td>{{ cita.nombreCliente || cita.clienteNombre || (cita.cliente && cita.cliente.nombre) || 'Sin nombre' }}</td>
                <td>{{ cita.descripcionProblema || 'Sin descripción' }}</td>
                <td>{{ cita.telefonoContacto || 'N/A' }}</td>
                <td><span :class="`badge bg-${getEstadoColor(cita.estado)}`">{{ cita.estado }}</span></td>
                <td>
                  <button
                    v-if="cita.estado === 'PENDIENTE'"
                    class="btn btn-sm btn-success me-1"
                    @click="confirmarCita(cita.idCita)"
                  >
                    Confirmar
                  </button>
                  <button
                    v-if="cita.estado === 'CONFIRMADA'"
                    class="btn btn-sm btn-primary"
                    @click="completarCita(cita.idCita)"
                  >
                    Completar
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Modal bloqueo/desbloqueo -->
    <div v-if="modalBloqueo" class="modal-overlay" @click.self="cerrarModal">
      <div class="modal-box">
        <h5>{{ slotActual?.disponible ? 'Bloquear Horario' : 'Desbloquear Horario' }}</h5>
        <p class="mb-3">
          {{ formatoFecha(slotActual?.fecha) }} - {{ slotActual?.horaInicio }}
        </p>
        <div v-if="slotActual?.disponible" class="mb-3">
          <label class="form-label">Motivo del bloqueo</label>
          <textarea v-model="motivoBloqueo" class="form-control" rows="3" placeholder="Ej: Reunión interna"></textarea>
        </div>
        <div class="d-flex justify-content-end gap-2">
          <button class="btn btn-secondary" @click="cerrarModal">Cancelar</button>
          <button
            class="btn"
            :class="slotActual?.disponible ? 'btn-warning' : 'btn-success'"
            @click="aplicarBloqueo"
          >
            {{ slotActual?.disponible ? 'Bloquear' : 'Desbloquear' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-overlay">
      <div class="spinner-border text-primary" role="status"></div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import CitaService from '@/services/CitaService'
import { useUserStore } from '@/stores/userStore'

const userStore = useUserStore()
const fechaInicio = ref(new Date().toISOString().split('T')[0])
const horarios = ref([])
const citas = ref([])
const loading = ref(false)
const modalBloqueo = ref(false)
const slotActual = ref(null)
const motivoBloqueo = ref('')

const diasSemana = computed(() => {
  const inicio = new Date(fechaInicio.value)
  const dias = []
  for (let i = 0; i < 7; i++) {
    const fecha = new Date(inicio)
    fecha.setDate(inicio.getDate() + i)
    const diaSemana = fecha.getDay()
    if (diaSemana >= 1 && diaSemana <= 5) {
      dias.push({
        fecha: fecha.toISOString().split('T')[0],
        nombre: ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'][diaSemana]
      })
    }
  }
  return dias
})

async function cargarDatos () {
  loading.value = true
  try {
    const inicio = new Date(fechaInicio.value)
    const fin = new Date(inicio)
    fin.setDate(inicio.getDate() + 7)

    const idTecnico = userStore.user.idUsuario

    horarios.value = await CitaService.obtenerHorariosDisponibles(
      idTecnico,
      inicio.toISOString(),
      fin.toISOString()
    )

    citas.value = await CitaService.obtenerMisCitasTecnico(
      inicio.toISOString(),
      fin.toISOString()
    )
  } catch (error) {
    console.error('Error cargando datos:', error)
  } finally {
    loading.value = false
  }
}

function obtenerSlotsDelDia (fecha) {
  return horarios.value.filter(h => h.fecha.split('T')[0] === fecha)
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

function toggleBloqueo (slot) {
  // Solo permitir bloqueo/desbloqueo si no hay cita
  if (!slot.disponibleReal && !slot.motivoBloqueo) return

  slotActual.value = slot
  motivoBloqueo.value = slot.motivoBloqueo || ''
  modalBloqueo.value = true
}

function cerrarModal () {
  modalBloqueo.value = false
  slotActual.value = null
  motivoBloqueo.value = ''
}

async function aplicarBloqueo () {
  if (!slotActual.value) return

  loading.value = true
  try {
    if (slotActual.value.disponible) {
      // Bloquear
      if (!motivoBloqueo.value.trim()) {
        alert('Ingresa un motivo para el bloqueo')
        loading.value = false
        return
      }
      await CitaService.bloquearHorario(
        slotActual.value.fecha,
        slotActual.value.horaInicio,
        motivoBloqueo.value
      )
    } else {
      // Desbloquear
      await CitaService.desbloquearHorario(
        slotActual.value.fecha,
        slotActual.value.horaInicio
      )
    }
    cerrarModal()
    await cargarDatos()
  } catch (error) {
    console.error('Error aplicando bloqueo:', error)
    alert('Error al aplicar el cambio')
  } finally {
    loading.value = false
  }
}

async function confirmarCita (idCita) {
  try {
    await CitaService.confirmarCita(idCita)
    await cargarDatos()
  } catch (error) {
    console.error('Error confirmando cita:', error)
  }
}

async function completarCita (idCita) {
  try {
    await CitaService.completarCita(idCita)
    await cargarDatos()
  } catch (error) {
    console.error('Error completando cita:', error)
  }
}

onMounted(() => {
  cargarDatos()
})
</script>

<style scoped>
.tech-dashboard { padding: 1.5rem; background: var(--dp-bg); min-height: 100vh; }
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
.time-slot { padding: .4rem; border-radius: 6px; font-size: .85rem; text-align: center; cursor: pointer; transition: transform .15s; }
.time-slot:hover { transform: scale(1.02); }
.slot-available { background: rgba(0,200,83,.08); border: 1px solid rgba(0,200,83,.2); }
.slot-busy { background: rgba(220,53,69,.08); border: 1px solid rgba(220,53,69,.2); cursor: default; }
.slot-blocked { background: rgba(255,193,7,.08); border: 1px solid rgba(255,193,7,.2); }
.slot-time { font-weight: 600; color: var(--dp-darkest); }
.slot-status .badge { font-size: .7rem; }
.motivo-text { display: block; font-size: .7rem; color: #0b2136; opacity: .75; margin-top: .2rem; }

.legend { display: flex; gap: 1rem; font-size: .85rem; color: #0b2136; opacity: .85; }
.legend-item { display: flex; align-items: center; gap: .35rem; }

.modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,.5); display: grid; place-items: center; z-index: 9999; }
.modal-box { background: #fff; border-radius: 12px; padding: 1.5rem; width: 90%; max-width: 480px; box-shadow: 0 10px 30px rgba(0,0,0,.3); }
.modal-box h5 { color: var(--dp-primary); margin-bottom: .5rem; }

.loading-overlay { position: fixed; inset: 0; background: rgba(0,0,0,.3); display: grid; place-items: center; z-index: 9999; }
</style>
