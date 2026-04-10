<template>
  <div class="tech-schedule-container">
    <LoadingOverlay v-if="processingGlobal" />
    <h2>Mi Horario</h2>

    <div class="schedule-header">
      <div class="week-navigation">
        <button @click="previousWeek" class="btn btn-outline">
          <i class="bi bi-chevron-left"></i> Anterior
        </button>
        <div class="current-week">
          <strong>Semana del {{ formatearFecha(fechaInicio) }}</strong>
        </div>
        <button @click="nextWeek" class="btn btn-outline">
          Siguiente <i class="bi bi-chevron-right"></i>
        </button>
      </div>
    </div>

    <div v-if="loading" class="loading">Cargando horario...</div>

    <div v-else class="schedule-grid">
      <div class="schedule-calendar">
        <div class="calendar-header">
          <div class="time-column-header">Hora</div>
          <div v-for="dia in diasSemana" :key="dia.fecha" class="day-header">
            <div class="day-name">{{ dia.nombre }}</div>
            <div class="day-date">{{ formatearFechaCorta(dia.fecha) }}</div>
          </div>
        </div>

        <div class="calendar-body">
          <div v-for="hora in horasLaborales" :key="hora" class="time-row">
            <div class="time-label">{{ hora }}</div>
            <div v-for="dia in diasSemana" :key="dia.fecha" class="schedule-cell">
              <div
                v-if="getCitaEnSlot(dia.fecha, hora)"
                class="cita-block"
                :class="`estado-${getCitaEnSlot(dia.fecha, hora).estado.toLowerCase()}`"
                @click="verDetalleCita(getCitaEnSlot(dia.fecha, hora))"
              >
                <div class="cita-time">{{ hora }}</div>
                <div class="cita-client">{{ (getCitaEnSlot(dia.fecha, hora).nombreCliente || getCitaEnSlot(dia.fecha, hora).clienteNombre || getCitaEnSlot(dia.fecha, hora).cliente?.nombre) ?? 'Sin nombre' }}</div>
                <div class="cita-badge">
                  <span :class="`badge badge-${getCitaEnSlot(dia.fecha, hora).estado.toLowerCase()}`">
                    {{ getCitaEnSlot(dia.fecha, hora).estado }}
                  </span>
                </div>
              </div>
              <div v-else-if="isSlotDisponible(dia.fecha, hora)" class="slot-disponible">
                <i class="bi bi-check-circle"></i>
              </div>
              <div v-else class="slot-no-laboral"></div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de detalle de cita -->
    <teleport to="body">
      <transition name="modal-fade">
        <div v-if="citaSeleccionada" class="modal-overlay" @click="cerrarDetalle">
          <div class="modal-container" @click.stop>
            <div class="modal-header">
              <h3>Detalle de la Cita</h3>
              <button @click="cerrarDetalle" class="btn-close">
                <i class="bi bi-x-lg"></i>
              </button>
            </div>
            <div class="modal-body">
              <div class="detail-item">
                <strong>Cliente:</strong>
                <span>{{ citaSeleccionada.nombreCliente }}</span>
              </div>
              <div class="detail-item">
                <strong>Fecha:</strong>
                <span>{{ formatearFecha(citaSeleccionada.fechaCita) }}</span>
              </div>
              <div class="detail-item">
                <strong>Hora:</strong>
                <span>{{ citaSeleccionada.horaInicio }} - {{ citaSeleccionada.horaFin }}</span>
              </div>
              <div class="detail-item">
                <strong>Teléfono:</strong>
                <span>{{ citaSeleccionada.telefonoContacto || 'N/A' }}</span>
              </div>
              <div class="detail-item">
                <strong>Dirección:</strong>
                <span>{{ citaSeleccionada.direccion || 'N/A' }}</span>
              </div>
              <div class="detail-item" v-if="citaSeleccionada.descripcionProblema">
                <strong>Problema:</strong>
                <span>{{ citaSeleccionada.descripcionProblema }}</span>
              </div>
              <div class="detail-item">
                <strong>Estado:</strong>
                <span :class="`badge badge-${citaSeleccionada.estado.toLowerCase()}`">
                  {{ citaSeleccionada.estado }}
                </span>
              </div>
            </div>
            <div class="modal-footer">
              <button @click="cerrarDetalle" class="btn btn-secondary">Cerrar</button>
            </div>
          </div>
        </div>
      </transition>
    </teleport>

    <!-- Modales reutilizables -->
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

    <!-- Listado interactivo de citas de la semana -->
    <div v-if="!loading && citas.length" class="card mt-4">
      <div class="card-body">
        <h5 class="mb-3">Citas de esta semana</h5>
        <div class="table-responsive">
          <table class="table align-middle">
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
              <tr v-for="c in citas" :key="c.idCita">
                <td>{{ formatearFecha(c.fechaCita) }}</td>
                <td>{{ normalizarHora(c.horaInicio) }}</td>
                <td>{{ c.nombreCliente || c.clienteNombre || (c.cliente && c.cliente.nombre) || 'Sin nombre' }}</td>
                <td>{{ c.descripcionProblema }}</td>
                <td><span :class="`badge bg-${getEstadoColor(c.estado)}`">{{ c.estado }}</span></td>
                <td>
                  <button v-if="c.estado === 'PENDIENTE'" class="btn btn-sm btn-success me-2" @click="confirmarCita(c.idCita)">
                    <i class="bi bi-check-circle"></i> Confirmar
                  </button>
                  <button v-if="c.estado === 'CONFIRMADA'" class="btn btn-sm btn-primary me-2" @click="completarCita(c.idCita)">
                    <i class="bi bi-check-all"></i> Completar
                  </button>
                  <button v-if="c.estado !== 'CANCELADA' && c.estado !== 'COMPLETADA'" class="btn btn-sm btn-danger" @click="cancelarCita(c.idCita)">
                    <i class="bi bi-x-circle"></i> Cancelar
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import CitaService from '@/services/CitaService'
import { useUserStore } from '@/stores/userStore'
import { usuarioService } from '@/services/UserService'
import ConfirmModal from '@/components/ConfirmModal.vue'
import LoadingOverlay from './LoadingOverlay.vue'

const fechaInicio = ref(getMondayOfWeek(new Date()))
const citas = ref([])
const horarios = ref([])
const loading = ref(false)
const processingGlobal = ref(false)
const citaSeleccionada = ref(null)
const idTecnico = ref(null)

// Modales para confirmar acciones y mostrar mensajes
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

const horasLaborales = ['08:00', '09:00', '10:00', '11:00', '13:00', '14:00', '15:00', '16:00']

const diasSemana = computed(() => {
  const dias = []
  const monday = new Date(fechaInicio.value)

  for (let i = 0; i < 5; i++) {
    const fecha = new Date(monday)
    fecha.setDate(monday.getDate() + i)
    const diaSemana = fecha.getDay()
    dias.push({
      fecha: fecha.toISOString().split('T')[0],
      nombre: ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'][diaSemana]
    })
  }
  return dias
})

function getMondayOfWeek (date) {
  const d = new Date(date)
  const day = d.getDay()
  const diff = d.getDate() - day + (day === 0 ? -6 : 1)
  const monday = new Date(d.setDate(diff))
  monday.setHours(0, 0, 0, 0)
  return monday
}

const resolverIdTecnico = async () => {
  if (idTecnico.value) return idTecnico.value
  const userStore = useUserStore()
  const user = userStore.getUserFromToken()
  const idUsuario = user?.idUsuario || user?.sub
  if (!idUsuario) return null
  // Buscar el usuario actual en la lista para obtener idUnico (usado por endpoints admin)
  const usuarios = await usuarioService.getAllUsers()
  const yo = usuarios.find(u => u.idUsuario === idUsuario)
  idTecnico.value = yo?.idUnico || yo?.id || null
  return idTecnico.value
}

const cargarDatos = async () => {
  loading.value = true
  processingGlobal.value = true
  try {
    const monday = new Date(fechaInicio.value)
    const friday = new Date(monday)
    friday.setDate(monday.getDate() + 4)

    const fechaDesde = monday.toISOString().split('T')[0]
    const fechaHasta = friday.toISOString().split('T')[0]

    // Resolver id del técnico actual y usar los mismos endpoints que admin
    const idTec = await resolverIdTecnico()
    if (!idTec) throw new Error('No se pudo resolver el identificador del técnico')

    // Horarios con disponibilidad real (como admin)
    horarios.value = await CitaService.obtenerHorariosDisponibles(idTec, fechaDesde, fechaHasta)
    // Citas del técnico por id (como admin)
    citas.value = await CitaService.obtenerCitasTecnico(idTec, fechaDesde, fechaHasta)
  } catch (error) {
    console.error('Error al cargar horario:', error)
  } finally {
    loading.value = false
    processingGlobal.value = false
  }
}

const normalizarHora = (h) => (h || '').slice(0, 5)

const getCitaEnSlot = (fecha, hora) => {
  const h = normalizarHora(hora)
  return citas.value.find(cita => {
    const citaFecha = (cita.fechaCita || '').split('T')[0]
    const citaHora = normalizarHora(cita.horaInicio)
    return citaFecha === fecha && citaHora === h
  })
}

const isSlotDisponible = (fecha, hora) => {
  const hayFecha = diasSemana.value.some(d => d.fecha === fecha)
  const hayHora = horasLaborales.includes(hora)
  if (!hayFecha || !hayHora) return false
  const h = normalizarHora(hora)
  const slot = horarios.value.find(s => (s.fecha || '').split('T')[0] === fecha && normalizarHora(s.horaInicio) === h)
  if (getCitaEnSlot(fecha, hora)) return false
  if (slot) {
    // Disponible sólo si ambas banderas lo permiten
    return slot.disponible && (slot.disponibleReal ?? true)
  }
  // Si no hay info del slot, asumimos no disponible para evitar falsos positivos
  return false
}

const verDetalleCita = (cita) => {
  citaSeleccionada.value = cita
}

const cerrarDetalle = () => {
  citaSeleccionada.value = null
}

const getEstadoColor = (estado) => {
  const colores = {
    PENDIENTE: 'warning',
    CONFIRMADA: 'info',
    COMPLETADA: 'success',
    CANCELADA: 'secondary'
  }
  return colores[estado] || 'secondary'
}

const confirmarCita = (idCita) => {
  modalConfirm.value = {
    show: true,
    title: 'Confirmar Cita',
    message: '¿Deseas confirmar esta cita?',
    type: 'question',
    onConfirm: async () => {
      processingGlobal.value = true
      try {
        await CitaService.confirmarCita(idCita)
        modalAlert.value = { show: true, title: 'Éxito', message: 'Cita confirmada', type: 'success' }
        await cargarDatos()
      } catch (error) {
        const msg = error.response?.data?.message || error.message || 'Error al confirmar'
        modalAlert.value = { show: true, title: 'Error', message: msg, type: 'danger' }
      } finally {
        processingGlobal.value = false
      }
    }
  }
}

const cancelarCita = (idCita) => {
  modalConfirm.value = {
    show: true,
    title: 'Cancelar Cita',
    message: '¿Seguro que deseas cancelar esta cita?',
    type: 'danger',
    onConfirm: async () => {
      processingGlobal.value = true
      try {
        await CitaService.cancelarCita(idCita)
        modalAlert.value = { show: true, title: 'Éxito', message: 'Cita cancelada', type: 'success' }
        await cargarDatos()
      } catch (error) {
        const msg = error.response?.data?.message || error.message || 'Error al cancelar'
        modalAlert.value = { show: true, title: 'Error', message: msg, type: 'danger' }
      } finally {
        processingGlobal.value = false
      }
    }
  }
}

const completarCita = (idCita) => {
  modalConfirm.value = {
    show: true,
    title: 'Completar Cita',
    message: '¿Marcar esta cita como completada?',
    type: 'question',
    onConfirm: async () => {
      processingGlobal.value = true
      try {
        await CitaService.completarCita(idCita)
        modalAlert.value = { show: true, title: 'Éxito', message: 'Cita completada', type: 'success' }
        await cargarDatos()
      } catch (error) {
        const msg = error.response?.data?.message || error.message || 'Error al completar'
        modalAlert.value = { show: true, title: 'Error', message: msg, type: 'danger' }
      } finally {
        processingGlobal.value = false
      }
    }
  }
}

const previousWeek = () => {
  const newDate = new Date(fechaInicio.value)
  newDate.setDate(newDate.getDate() - 7)
  fechaInicio.value = newDate
  cargarDatos()
}

const nextWeek = () => {
  const newDate = new Date(fechaInicio.value)
  newDate.setDate(newDate.getDate() + 7)
  fechaInicio.value = newDate
  cargarDatos()
}

const formatearFecha = (fecha) => {
  const date = new Date(fecha)
  return date.toLocaleDateString('es-ES', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const formatearFechaCorta = (fecha) => {
  const date = new Date(fecha)
  return date.toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit' })
}

onMounted(() => {
  cargarDatos()
})
</script>

<style scoped>
.tech-schedule-container {
  padding: 24px;
  max-width: 1400px;
  margin: 0 auto;
}

h2 {
  color: #2c3e50;
  margin-bottom: 24px;
  font-weight: 600;
}

.schedule-header {
  margin-bottom: 24px;
}

.week-navigation {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 24px;
}

.current-week {
  font-size: 1.1rem;
  color: #495057;
}

.btn {
  padding: 10px 20px;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 8px;
}

.btn-outline {
  background: white;
  border: 2px solid #667eea;
  color: #667eea;
}

.btn-outline:hover {
  background: #667eea;
  color: white;
}

.loading {
  text-align: center;
  padding: 48px;
  color: #6c757d;
  font-size: 1.1rem;
}

.schedule-grid {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.schedule-calendar {
  overflow-x: auto;
}

.calendar-header {
  display: grid;
  grid-template-columns: 80px repeat(5, 1fr);
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.time-column-header {
  padding: 16px 12px;
  text-align: center;
  font-weight: 600;
  border-right: 1px solid rgba(255, 255, 255, 0.2);
}

.day-header {
  padding: 16px 12px;
  text-align: center;
  border-right: 1px solid rgba(255, 255, 255, 0.2);
}

.day-header:last-child {
  border-right: none;
}

.day-name {
  font-weight: 600;
  font-size: 1rem;
  margin-bottom: 4px;
}

.day-date {
  font-size: 0.85rem;
  opacity: 0.9;
}

.calendar-body {
  min-width: 800px;
}

.time-row {
  display: grid;
  grid-template-columns: 80px repeat(5, 1fr);
  border-bottom: 1px solid #e9ecef;
}

.time-row:last-child {
  border-bottom: none;
}

.time-label {
  padding: 16px 12px;
  text-align: center;
  font-weight: 600;
  color: #495057;
  background: #f8f9fa;
  border-right: 1px solid #e9ecef;
  display: flex;
  align-items: center;
  justify-content: center;
}

.schedule-cell {
  padding: 8px;
  border-right: 1px solid #e9ecef;
  min-height: 80px;
  position: relative;
}

.schedule-cell:last-child {
  border-right: none;
}

.cita-block {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 12px;
  border-radius: 8px;
  cursor: pointer;
  height: 100%;
  display: flex;
  flex-direction: column;
  gap: 4px;
  transition: transform 0.2s, box-shadow 0.2s;
}

.cita-block:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.cita-block.estado-pendiente {
  background: linear-gradient(135deg, #ffc107 0%, #ff9800 100%);
}

.cita-block.estado-confirmada {
  background: linear-gradient(135deg, #0d6efd 0%, #0b5ed7 100%);
}

.cita-block.estado-completada {
  background: linear-gradient(135deg, #198754 0%, #157347 100%);
}

.cita-time {
  font-size: 0.85rem;
  opacity: 0.9;
  font-weight: 500;
}

.cita-client {
  font-weight: 600;
  font-size: 0.95rem;
}

.cita-badge {
  margin-top: auto;
}

.badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

.slot-disponible {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #198754;
  font-size: 1.5rem;
  opacity: 0.3;
}

.slot-no-laboral {
  background: #f8f9fa;
  height: 100%;
}

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  backdrop-filter: blur(2px);
}

.modal-container {
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  max-width: 600px;
  width: 90%;
  max-height: 90vh;
  overflow: hidden;
}

.modal-header {
  padding: 20px 24px;
  border-bottom: 1px solid #e9ecef;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.btn-close {
  background: none;
  border: none;
  color: white;
  font-size: 1.25rem;
  cursor: pointer;
  padding: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: all 0.2s;
}

.btn-close:hover {
  background: rgba(255, 255, 255, 0.2);
}

.modal-body {
  padding: 24px;
}

.detail-item {
  margin-bottom: 16px;
  display: flex;
  justify-content: space-between;
  align-items: start;
  gap: 16px;
}

.detail-item strong {
  color: #495057;
  min-width: 120px;
}

.detail-item span {
  text-align: right;
  color: #212529;
}

.modal-footer {
  padding: 16px 24px;
  background: #f8f9fa;
  border-top: 1px solid #e9ecef;
  display: flex;
  justify-content: flex-end;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background: #5c636a;
}

.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

@media (max-width: 768px) {
  .tech-schedule-container {
    padding: 16px;
  }

  .week-navigation {
    flex-direction: column;
    gap: 12px;
  }

  .schedule-calendar {
    font-size: 0.85rem;
  }

  .time-label,
  .day-header {
    padding: 12px 8px;
  }

  .schedule-cell {
    min-height: 60px;
    padding: 4px;
  }

  .cita-block {
    padding: 8px;
  }
}
</style>
