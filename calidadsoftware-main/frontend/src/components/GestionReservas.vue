<template>
  <div class="reservas-container">
    <h2 class="mb-4">Gestión de Reservas</h2>
    <!-- Filtros -->
    <div class="filters mb-4 p-3 bg-light rounded">
      <div class="row">
        <div class="col-md-4 mb-2">
          <label class="form-label">Filtrar por estado:</label>
          <select v-model="filtroEstado" class="form-select">
            <option value="">Todos</option>
            <option value="Pendiente">Pendiente</option>
            <option value="Confirmada">Confirmada</option>
            <option value="Completada">Completada</option>
            <option value="Cancelada">Cancelada</option>
          </select>
        </div>
        <div class="col-md-4 mb-2">
          <label class="form-label">Filtrar por fecha:</label>
          <input type="date" v-model="filtroFecha" class="form-control">
        </div>
        <div class="col-md-4 mb-2">
          <label class="form-label">Buscar por nombre de sala:</label>
          <input type="text" v-model="filtroSala" placeholder="Nombre de sala" class="form-control">
        </div>
      </div>
    </div>

    <!-- Tabla de reservas -->
    <div class="table-responsive">
      <table class="table table-hover">
        <thead class="table-dark">
          <tr>
            <th>ID</th>
            <th>Sala</th>
            <th>Usuario</th>
            <th>Fecha Inicio</th>
            <th>Fecha Fin</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
            <tr v-for="reserva in reservasFiltradas" :key="reserva.idReserva"
                :class="{
                'table-warning': reserva.estado === 'Pendiente',
                'table-success': reserva.estado === 'Confirmada',
                'table-secondary': reserva.estado === 'Completada' || reserva.estado === 'Expirada',
                'table-danger': reserva.estado === 'Cancelada'
                }">
            <td>{{ reserva.idReserva }}</td>
            <td>{{ reserva.salaNombre }}</td>
            <td>{{ reserva.idUsuario }}</td>
            <td>{{ formatFecha(reserva.fechaInicio) }}</td>
            <td>{{ formatFecha(reserva.fechaFin) }}</td>
            <td>
                <span class="badge" :class="{
                    'bg-warning': reserva.estado === 'Pendiente',
                    'bg-success': reserva.estado === 'Confirmada',
                    'bg-secondary': reserva.estado === 'Completada' || reserva.estado === 'Expirada',
                    'bg-danger': reserva.estado === 'Cancelada'
                    }">
                    {{ reserva.estado }}
                </span>
            </td>
            <td>
              <router-link :to="`/reserva/detalle/${reserva.idReserva}`" class="btn btn-sm btn-outline-primary ms-1">
                Detalles
              </router-link>
              <button v-if="reserva.estado.toLowerCase() !== 'cancelada' && reserva.estado.toLowerCase() !== 'confirmada'"
                @click="mostrarModalCancelar(reserva.idReserva)"
                class="btn btn-sm btn-outline-danger">
                Cancelar
            </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Mensaje cuando no hay reservas -->
    <div v-if="reservasFiltradas.length === 0" class="alert alert-info mt-3">
      No se encontraron reservas con los filtros seleccionados.
    </div>

    <!-- Modal de confirmación para cancelar -->
    <div v-if="showCancelModal" class="modal-backdrop fade show"></div>
    <div v-if="showCancelModal" class="modal fade show d-block" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirmar cancelación</h5>
            <button type="button" class="btn-close" @click="showCancelModal = false"></button>
          </div>
          <div class="modal-body">
            <p>¿Estás seguro que deseas cancelar esta reserva?</p>
            <p class="text-danger">Esta acción no se puede deshacer.</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="showCancelModal = false">Cancelar</button>
            <button type="button" class="btn btn-danger" @click="cancelarReserva">Confirmar</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Cargando -->
    <div v-if="loading" class="text-center mt-4">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Cargando...</span>
      </div>
      <p>Cargando reservas...</p>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, watch } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toastification'

const API_URL = 'https://localhost:5001/api/reservas'

export default {
  setup () {
    const toast = useToast()
    const reservas = ref([])
    const loading = ref(false)
    const showCancelModal = ref(false)
    const reservaACancelar = ref(null)

    // Filtros
    const filtroEstado = ref('')
    const filtroFecha = ref('')
    const filtroSala = ref('')

    // Obtener todas las reservas
    const obtenerReservas = async () => {
      loading.value = true
      try {
        const response = await axios.get(API_URL)
        reservas.value = response.data.map(reserva => ({
          idReserva: reserva.idReserva,
          idUsuario: reserva.idUsuario,
          fechaInicio: reserva.fechaInicio,
          fechaFin: reserva.fechaFin,
          estado: reserva.estado || 'Pendiente',
          fechaCreacion: reserva.fechaCreacion,
          idSala: reserva.idSala,
          salaNombre: reserva.salaNombre || 'Sin nombre',
          salaDescripcionCorta: reserva.salaDescripcionCorta,
          precioTotal: reserva.precioTotal || 0
        }))
      } catch (error) {
        toast.error('Error al cargar las reservas: ' + error.message)
        console.error('Error:', error)
      } finally {
        loading.value = false
      }
    }

    // Formatear fecha
    const formatFecha = (fechaString) => {
      if (!fechaString) return ''
      const fecha = new Date(fechaString)
      return fecha.toLocaleDateString() + ' ' + fecha.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    }

    // Mostrar modal de cancelación
    const mostrarModalCancelar = (idReserva) => {
      reservaACancelar.value = idReserva
      showCancelModal.value = true
    }

    // Cancelar reserva
    const cancelarReserva = async () => {
      try {
        await axios.put(`${API_URL}/cancelar/${reservaACancelar.value}`)
        toast.success('Reserva cancelada correctamente')
        // Actualizar el estado localmente
        const reservaIndex = reservas.value.findIndex(r => r.idReserva === reservaACancelar.value)
        if (reservaIndex !== -1) {
          reservas.value[reservaIndex].estado = 'Cancelada'
        }
      } catch (error) {
        toast.error('Error al cancelar la reserva: ' + error.message)
        console.error('Error:', error)
      } finally {
        showCancelModal.value = false
        reservaACancelar.value = null
      }
    }

    // Reservas filtradas
    const reservasFiltradas = computed(() => {
      return reservas.value.filter(reserva => {
        // Filtro por estado
        if (filtroEstado.value && reserva.estado?.toLowerCase() !== filtroEstado.value.toLowerCase()) {
          return false
        }

        // Filtro por fecha
        if (filtroFecha.value) {
          try {
            const fechaFiltro = new Date(filtroFecha.value)
            const fechaInicio = new Date(reserva.fechaInicio)

            // Comparar solo día, mes y año
            if (
              fechaInicio.getFullYear() !== fechaFiltro.getFullYear() ||
              fechaInicio.getMonth() !== fechaFiltro.getMonth() ||
              fechaInicio.getDate() !== fechaFiltro.getDate()
            ) {
              return false
            }
          } catch (error) {
            console.error('Error al comparar fechas:', error)
            return false
          }
        }

        // Filtro por nombre de sala
        if (filtroSala.value &&
            !reserva.salaNombre.toLowerCase().includes(filtroSala.value.toLowerCase())) {
          return false
        }

        return true
      })
    })

    // Cargar reservas al montar el componente
    onMounted(() => {
      obtenerReservas()
      // Debug: ver valores reales
      watch([filtroEstado, filtroFecha, filtroSala], () => {
        console.log('Filtros actuales:', {
          estado: filtroEstado.value,
          fecha: filtroFecha.value,
          sala: filtroSala.value
        })
        console.log('Reservas filtradas:', reservasFiltradas.value)
      })
    })

    return {
      reservas,
      reservasFiltradas,
      loading,
      showCancelModal,
      filtroEstado,
      filtroFecha,
      filtroSala,
      formatFecha,
      mostrarModalCancelar,
      cancelarReserva
    }
  }
}
</script>

<style scoped>
.reservas-container {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
  color: #1A4456; /* Color principal para textos */
}

h2 {
  color: #1A4456 !important;
}

.modal-backdrop {
  opacity: 0.5;
}

/* Textos generales */
body, .form-label, .table, .alert, .modal-title, .modal-body {
  color: #1A4456 !important;
}

/* Badges personalizados */
.badge {
  font-size: 0.85em;
  padding: 0.5em 0.75em;
  min-width: 90px;
  display: inline-block;
  text-align: center;
  font-weight: 500;
  letter-spacing: 0.5px;
  border-radius: 4px;
  color: #1A4456;
}

/* Estado: Pendiente */
.badge.bg-warning {
  background-color: #FFC107 !important;
  color: #1A4456 !important;
  border: 1px solid #FFA000;
}

/* Estado: Confirmada */
.badge.bg-success {
  background-color: #28A745 !important;
  color: white !important;
  border: 1px solid #218838;
}

/* Estado: Completada/Expirada */
.badge.bg-secondary {
  background-color: #6C757D !important;
  color: white !important;
  border: 1px solid #5A6268;
}

/* Estado: Cancelada */
.badge.bg-danger {
  background-color: #DC3545 !important;
  color: white !important;
  border: 1px solid #C82333;
}

/* Tablas */
.table {
  color: #1A4456;
}

.table th {
  white-space: nowrap;
  color: #1A4456;
  background-color: #f8f9fa;
}

.table-responsive {
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.debug-border {
  border: 1px solid red;
}

/* Filas con colores de estado */
.table-warning {
  --bs-table-bg: #FFF3CD;
  --bs-table-striped-bg: #F2E7C3;
  --bs-table-hover-bg: #FFE8A1;
  color: #1A4456;
}

.table-success {
  --bs-table-bg: #D1E7DD;
  --bs-table-striped-bg: #C7DBD2;
  --bs-table-hover-bg: #BCD0C7;
  color: #1A4456;
}

.table-secondary {
  --bs-table-bg: #E2E3E5;
  --bs-table-striped-bg: #D7D8DA;
  --bs-table-hover-bg: #CBCCCE;
  color: #1A4456;
}

.table-danger {
  --bs-table-bg: #F8D7DA;
  --bs-table-striped-bg: #ECCCCF;
  --bs-table-hover-bg: #DFC2C4;
  color: #1A4456;
}

/* Modal */
.modal-title, .modal-body {
  color: #1A4456 !important;
}

/* Botones */
.btn-outline-primary {
  color: #1A4456;
  border-color: #1A4456;
}

.btn-outline-primary:hover {
  background-color: #1A4456;
  color: white;
}

/* Alertas */
.alert-info {
  background-color: #D1ECF1;
  border-color: #BEE5EB;
  color: #1A4456;
}

/* Filtros */
.filters {
  background-color: #F8F9FA;
  border: 1px solid #DEE2E6;
}

.form-control, .form-select {
  color: #1A4456;
  border-color: #CED4DA;
}

/* Cargando */
.text-primary {
  color: #1A4456 !important;
}
</style>
