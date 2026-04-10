<template>
  <div class="tech-appointments-container">
    <LoadingOverlay v-if="processingGlobal" />
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

    <h2>Mis Citas</h2>

    <div class="filters">
      <label>
        Desde:
        <input type="date" v-model="fechaDesde" @change="cargarCitas" />
      </label>
      <label>
        Hasta:
        <input type="date" v-model="fechaHasta" @change="cargarCitas" />
      </label>
    </div>

    <div v-if="loading" class="loading">Cargando citas...</div>

    <div v-else-if="citas.length === 0" class="no-citas">
      No tienes citas programadas en este período.
    </div>

    <div v-else class="citas-list">
      <div v-for="cita in citas" :key="cita.idCita" class="cita-card" :class="`estado-${cita.estado.toLowerCase()}`">
        <div class="cita-header">
          <div class="cita-fecha">
            <i class="bi bi-calendar-event"></i>
            {{ formatearFecha(cita.fechaCita) }}
          </div>
          <div class="cita-hora">
            <i class="bi bi-clock"></i>
            {{ cita.horaInicio }} - {{ cita.horaFin }}
          </div>
          <div class="cita-estado">
            <span :class="`badge badge-${cita.estado.toLowerCase()}`">
              {{ cita.estado }}
            </span>
          </div>
        </div>

        <div class="cita-body">
          <div class="cita-info">
            <strong>Cliente:</strong>
            {{ cita.nombreCliente || cita.clienteNombre || (cita.cliente && cita.cliente.nombre) || 'Sin nombre' }}
          </div>
          <div class="cita-info">
            <strong>Teléfono:</strong> {{ cita.telefonoContacto || 'N/A' }}
          </div>
          <div class="cita-info">
            <strong>Dirección:</strong> {{ cita.direccion || 'N/A' }}
          </div>
          <div class="cita-info" v-if="cita.descripcionProblema">
            <strong>Problema:</strong> {{ cita.descripcionProblema }}
          </div>
        </div>

        <div class="cita-actions" v-if="cita.estado === 'PENDIENTE' || cita.estado === 'CONFIRMADA'">
          <button
            v-if="cita.estado === 'PENDIENTE'"
            @click="confirmarCita(cita.idCita)"
            class="btn btn-success"
          >
            <i class="bi bi-check-circle"></i> Confirmar
          </button>
          <button
            v-if="cita.estado === 'CONFIRMADA'"
            @click="completarCita(cita.idCita)"
            class="btn btn-primary"
          >
            <i class="bi bi-check-all"></i> Completar
          </button>
          <button
            @click="cancelarCita(cita.idCita)"
            class="btn btn-danger"
          >
            <i class="bi bi-x-circle"></i> Cancelar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import CitaService from '@/services/CitaService'
import ConfirmModal from '@/components/ConfirmModal.vue'
import LoadingOverlay from './LoadingOverlay.vue'

const fechaDesde = ref(new Date().toISOString().split('T')[0])
const fechaHasta = ref(new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0])
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

const cargarCitas = async () => {
  loading.value = true
  processingGlobal.value = true
  try {
    citas.value = await CitaService.obtenerMisCitasTecnico(fechaDesde.value, fechaHasta.value)
  } catch (error) {
    console.error('Error al cargar citas:', error)
    alert('Error al cargar citas')
  } finally {
    loading.value = false
    processingGlobal.value = false
  }
}

const confirmarCita = async (idCita) => {
  modalConfirm.value = {
    show: true,
    title: 'Confirmar Cita',
    message: '¿Desea confirmar esta cita?',
    type: 'question',
    onConfirm: async () => {
      try {
        await CitaService.confirmarCita(idCita)
        modalAlert.value = {
          show: true,
          title: 'Éxito',
          message: 'Cita confirmada exitosamente',
          type: 'success'
        }
        cargarCitas()
      } catch (error) {
        console.error('Error al confirmar cita:', error)
        modalAlert.value = {
          show: true,
          title: 'Error',
          message: 'Error al confirmar cita: ' + (error.response?.data?.message || error.message),
          type: 'danger'
        }
      }
    }
  }
}

const completarCita = async (idCita) => {
  modalConfirm.value = {
    show: true,
    title: 'Completar Cita',
    message: '¿Marcar esta cita como completada?',
    type: 'question',
    onConfirm: async () => {
      try {
        await CitaService.completarCita(idCita)
        modalAlert.value = {
          show: true,
          title: 'Éxito',
          message: 'Cita completada exitosamente',
          type: 'success'
        }
        cargarCitas()
      } catch (error) {
        console.error('Error al completar cita:', error)
        modalAlert.value = {
          show: true,
          title: 'Error',
          message: 'Error al completar cita: ' + (error.response?.data?.message || error.message),
          type: 'danger'
        }
      }
    }
  }
}

const cancelarCita = async (idCita) => {
  modalConfirm.value = {
    show: true,
    title: 'Cancelar Cita',
    message: '¿Estás seguro de cancelar esta cita? Esta acción no se puede deshacer.',
    type: 'danger',
    onConfirm: async () => {
      try {
        await CitaService.cancelarCita(idCita)
        modalAlert.value = {
          show: true,
          title: 'Éxito',
          message: 'Cita cancelada exitosamente',
          type: 'success'
        }
        cargarCitas()
      } catch (error) {
        console.error('Error al cancelar cita:', error)
        modalAlert.value = {
          show: true,
          title: 'Error',
          message: 'Error al cancelar cita: ' + (error.response?.data?.message || error.message),
          type: 'danger'
        }
      }
    }
  }
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

onMounted(() => {
  cargarCitas()
})
</script>

<style scoped>
.tech-appointments-container {
  padding: 20px;
  max-width: 1000px;
  margin: 0 auto;
}

h2 {
  margin-bottom: 20px;
}

.filters {
  margin-bottom: 20px;
  display: flex;
  gap: 20px;
}

.filters label {
  display: flex;
  align-items: center;
  gap: 10px;
  font-weight: 500;
}

.filters input {
  padding: 5px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.loading {
  text-align: center;
  padding: 40px;
  font-size: 1.1em;
  color: #666;
}

.no-citas {
  text-align: center;
  padding: 40px;
  color: #666;
  background-color: #f8f9fa;
  border-radius: 8px;
}

.citas-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.cita-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  background: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.2s;
}

.cita-card:hover {
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

.cita-card.estado-pendiente {
  border-left: 4px solid #ffc107;
}

.cita-card.estado-confirmada {
  border-left: 4px solid #0d6efd;
}

.cita-card.estado-completada {
  border-left: 4px solid #198754;
}

.cita-card.estado-cancelada {
  border-left: 4px solid #dc3545;
  opacity: 0.7;
}

.cita-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 15px;
  border-bottom: 1px solid #eee;
}

.cita-fecha,
.cita-hora {
  font-weight: 500;
}

.badge {
  padding: 5px 10px;
  border-radius: 4px;
  font-size: 0.85em;
  font-weight: 500;
}

.badge-pendiente {
  background-color: #fff3cd;
  color: #856404;
}

.badge-confirmada {
  background-color: #cfe2ff;
  color: #084298;
}

.badge-completada {
  background-color: #d1e7dd;
  color: #0f5132;
}

.badge-cancelada {
  background-color: #f8d7da;
  color: #842029;
}

.cita-body {
  margin-bottom: 15px;
}

.cita-info {
  margin-bottom: 8px;
}

.cita-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  display: inline-flex;
  align-items: center;
  gap: 5px;
  transition: all 0.2s;
}

.btn-success {
  background-color: #198754;
  color: white;
}

.btn-success:hover {
  background-color: #157347;
}

.btn-primary {
  background-color: #0d6efd;
  color: white;
}

.btn-primary:hover {
  background-color: #0b5ed7;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-danger:hover {
  background-color: #bb2d3b;
}
</style>
