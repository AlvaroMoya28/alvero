<template>
  <div class="book-appointment-container">
    <h2>Reservar Cita de Soporte Técnico</h2>
    
    <div class="filters">
      <label>
        Semana:
        <input type="date" v-model="fechaInicio" @change="cargarDisponibilidad" />
      </label>
    </div>

    <div v-if="loading" class="loading">Cargando disponibilidad...</div>

    <div v-else class="disponibilidad-grid">
      <div class="day-header" v-for="dia in diasSemana" :key="dia.fecha">
        <strong>{{ dia.nombre }}</strong>
        <br />
        <span>{{ formatearFecha(dia.fecha) }}</span>
      </div>

      <div
        v-for="hora in horasDisponibles"
        :key="hora"
        class="hour-row"
      >
        <div class="hour-label">{{ hora }}:00</div>
        <div
          v-for="dia in diasSemana"
          :key="`${dia.fecha}-${hora}`"
          class="time-slot"
          :class="getSlotClass(dia.fecha, hora)"
          @click="seleccionarSlot(dia.fecha, hora)"
        >
          <div class="slot-info">
            <span v-if="getSlotDisponibilidad(dia.fecha, hora) > 0">
              {{ getSlotDisponibilidad(dia.fecha, hora) }} disponible(s)
            </span>
            <span v-else class="no-disponible">No disponible</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de reserva -->
    <div
      v-if="slotSeleccionado"
      class="modal-overlay"
      @click.self="cerrarModal"
    >
      <div class="modal-content">
        <div class="modal-header">
          <h3>Confirmar Cita de Soporte</h3>
          <button class="modal-close" @click="cerrarModal" type="button" aria-label="Cerrar">&times;</button>
        </div>

        <div class="modal-body">
          <div class="cita-info-box">
            <div class="info-item">
              <i class="bi bi-calendar-event"></i>
              <span><strong>Fecha:</strong> {{ formatearFecha(slotSeleccionado.fecha) }}</span>
            </div>
            <div class="info-item">
              <i class="bi bi-clock"></i>
              <span><strong>Hora:</strong> {{ slotSeleccionado.hora }}:00</span>
            </div>
            <div class="info-item">
              <i class="bi bi-person-badge"></i>
              <span><strong>Técnico:</strong> {{ slotSeleccionado.tecnicos[0].nombreTecnico }}</span>
            </div>
          </div>

          <form @submit.prevent="confirmarReserva" class="reserva-form">
            <div class="form-group">
              <label>Nombre Completo *</label>
              <input 
                type="text" 
                v-model="formulario.nombreCliente" 
                placeholder="Nombre completo del cliente"
                required 
              />
            </div>
            
            <div class="form-row">
              <div class="form-group">
                <label>Correo Electrónico *</label>
                <input 
                  type="email" 
                  v-model="formulario.emailCliente" 
                  placeholder="correo@ejemplo.com"
                  required 
                />
              </div>
              <div class="form-group">
                <label>Cédula *</label>
                <input 
                  type="text" 
                  v-model="formulario.cedulaCliente" 
                  placeholder="123456789"
                  required 
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label>Teléfono de Contacto *</label>
                <input 
                  type="tel" 
                  v-model="formulario.telefonoContacto" 
                  placeholder="8888-9999"
                  required 
                />
              </div>
              <div class="form-group">
                <label>Dirección *</label>
                <input 
                  type="text" 
                  v-model="formulario.direccion" 
                  placeholder="Dirección del servicio"
                  required 
                />
              </div>
            </div>

            <div class="form-group">
              <label>Descripción del Problema *</label>
              <textarea 
                v-model="formulario.descripcionProblema" 
                rows="3" 
                placeholder="Explica brevemente el problema..."
                required
              ></textarea>
            </div>

            <div class="modal-actions">
              <button type="submit" class="btn-primary">Confirmar Reserva</button>
              <button type="button" @click="cerrarModal" class="btn-secondary">Cancelar</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import CitaService from '@/services/CitaService'

const fechaInicio = ref(new Date().toISOString().split('T')[0])
const disponibilidad = ref([])
const loading = ref(false)
const slotSeleccionado = ref(null)
const formulario = ref({
  nombreCliente: '',
  emailCliente: '',
  cedulaCliente: '',
  telefonoContacto: '',
  direccion: '',
  descripcionProblema: ''
})

const horasDisponibles = [8, 9, 10, 11, 13, 14, 15, 16]

const diasSemana = computed(() => {
  const selected = new Date(fechaInicio.value)
  const day = selected.getDay()
  const diffToMonday = (day === 0 ? -6 : 1 - day)
  const monday = new Date(selected)
  monday.setDate(selected.getDate() + diffToMonday)

  const dias = []
  for (let i = 0; i < 5; i++) {
    const fecha = new Date(monday)
    fecha.setDate(monday.getDate() + i)
    dias.push({
      fecha: fecha.toISOString().split('T')[0],
      nombre: ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'][fecha.getDay()]
    })
  }
  return dias
})

const cargarDisponibilidad = async () => {
  loading.value = true
  try {
    const inicio = diasSemana.value[0].fecha
    const fin = diasSemana.value[4].fecha
    disponibilidad.value = await CitaService.obtenerDisponibilidadGeneral(inicio, fin)
  } catch (error) {
    console.error('Error al cargar disponibilidad:', error)
    alert('Error al cargar disponibilidad')
  } finally {
    loading.value = false
  }
}

const getSlotDisponibilidad = (fecha, hora) => {
  const slot = disponibilidad.value.find(d =>
    d.fecha.split('T')[0] === fecha && d.horaInicio === `${hora.toString().padStart(2, '0')}:00`
  )
  return slot ? slot.tecnicosDisponibles : 0
}

const getSlotClass = (fecha, hora) => {
  const cant = getSlotDisponibilidad(fecha, hora)
  if (cant === 0) return 'slot-no-disponible'
  if (cant === 1) return 'slot-disponible-uno'
  return 'slot-disponible-varios'
}

const seleccionarSlot = (fecha, hora) => {
  const cant = getSlotDisponibilidad(fecha, hora)
  if (cant === 0) return

  const slot = disponibilidad.value.find(d =>
    d.fecha.split('T')[0] === fecha && d.horaInicio === `${hora.toString().padStart(2, '0')}:00`
  )

  slotSeleccionado.value = {
    fecha,
    hora,
    tecnicosDisponibles: cant,
    tecnicos: slot.tecnicos
  }
  document.body.classList.add('modal-open')
}

const cerrarModal = () => {
  slotSeleccionado.value = null
  formulario.value = {
    nombreCliente: '',
    emailCliente: '',
    cedulaCliente: '',
    telefonoContacto: '',
    direccion: '',
    descripcionProblema: ''
  }
  document.body.classList.remove('modal-open')
}

const confirmarReserva = async () => {
  try {
    const tecnicoSeleccionado = slotSeleccionado.value.tecnicos[0].idTecnico

    const citaData = {
      nombreCliente: formulario.value.nombreCliente,
      emailCliente: formulario.value.emailCliente,
      cedulaCliente: formulario.value.cedulaCliente,
      idUsuarioTecnico: tecnicoSeleccionado,
      fechaCita: slotSeleccionado.value.fecha,
      horaInicio: `${slotSeleccionado.value.hora.toString().padStart(2, '0')}:00`,
      telefonoContacto: formulario.value.telefonoContacto,
      direccion: formulario.value.direccion,
      descripcionProblema: formulario.value.descripcionProblema
    }

    await CitaService.crearCita(citaData)
    cerrarModal()
    alert('Cita reservada exitosamente. Recibirá un correo de confirmación.')
    cargarDisponibilidad()
  } catch (error) {
    console.error('Error al crear cita:', error)
    alert('Error al crear la cita: ' + (error.response?.data?.message || error.message))
  }
}

const formatearFecha = (fecha) => {
  const date = new Date(fecha + 'T00:00:00')
  return date.toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit' })
}

onMounted(() => {
  cargarDisponibilidad()
})
</script>

<style scoped>
.book-appointment-container {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

h2 {
  margin-bottom: 20px;
}

.filters {
  margin-bottom: 20px;
}

.filters label {
  margin-right: 15px;
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

.disponibilidad-grid {
  display: grid;
  grid-template-columns: 80px repeat(5, 1fr);
  gap: 5px;
  margin-top: 20px;
}

.day-header {
  text-align: center;
  padding: 10px;
  background-color: #f8f9fa;
  border-radius: 4px;
  font-size: 0.9em;
}

.hour-row {
  display: contents;
}

.hour-label {
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 500;
  background-color: #f8f9fa;
  border-radius: 4px;
  padding: 10px;
}

.time-slot {
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
  text-align: center;
}

.slot-disponible-uno {
  background-color: #fff3cd;
}

.slot-disponible-varios {
  background-color: #d1e7dd;
}

.slot-no-disponible {
  background-color: #f8d7da;
  cursor: not-allowed;
}

.time-slot:hover:not(.slot-no-disponible) {
  transform: scale(1.05);
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.slot-info {
  font-size: 0.9em;
}

.no-disponible {
  color: #721c24;
}

/* Modal Styles - Fixed for responsiveness */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  padding: 20px;
  overflow-y: auto;
}

.modal-content {
  background: white;
  border-radius: 12px;
  width: 100%;
  max-width: 600px;
  max-height: 85vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);
  animation: modalSlideIn 0.3s ease-out;
  position: relative;
  margin: auto;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.modal-header {
  padding: 20px 24px 0;
  border-bottom: none;
  flex-shrink: 0;
}

.modal-header h3 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.4rem;
  text-align: center;
  padding-right: 40px;
}

.modal-close {
  position: absolute;
  top: 16px;
  right: 16px;
  background: none;
  border: none;
  font-size: 28px;
  color: #666;
  cursor: pointer;
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: all 0.2s;
  z-index: 10;
}

.modal-close:hover {
  background-color: #f0f0f0;
  color: #333;
}

.modal-body {
  padding: 20px 24px 24px;
  overflow-y: auto;
  flex: 1;
  display: flex;
  flex-direction: column;
}

.reserva-form {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.cita-info-box {
  background: #f8f9fa;
  border-left: 4px solid #007bff;
  padding: 16px;
  border-radius: 8px;
  margin-bottom: 24px;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 10px;
}

.info-item:last-child {
  margin-bottom: 0;
}

.info-item i {
  color: #007bff;
  font-size: 1.1rem;
  min-width: 16px;
}

.info-item span {
  color: #495057;
  font-size: 0.95rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-group {
  margin-bottom: 18px;
}

.form-group label {
  display: block;
  margin-bottom: 6px;
  font-weight: 600;
  color: #495057;
  font-size: 0.9rem;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 12px;
  border: 1px solid #ced4da;
  border-radius: 6px;
  font-family: inherit;
  font-size: 0.95rem;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
}

.form-group input::placeholder,
.form-group textarea::placeholder {
  color: #adb5bd;
}

.form-group textarea {
  resize: vertical;
  min-height: 80px;
}

.modal-actions {
  display: flex;
  gap: 12px;
  margin-top: auto;
  padding-top: 20px;
  border-top: 1px solid #dee2e6;
  flex-shrink: 0;
}

.btn-primary {
  flex: 2;
  padding: 14px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.2s;
}

.btn-primary:hover {
  background-color: #0056b3;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

.btn-secondary {
  flex: 1;
  padding: 14px 20px;
  background-color: #6c757d;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.2s;
}

.btn-secondary:hover {
  background-color: #5a6268;
  transform: translateY(-1px);
}

/* Responsive Design */
@media (max-width: 768px) {
  .modal-overlay {
    padding: 15px;
    align-items: center;
  }
  
  .modal-content {
    max-height: 80vh;
    max-width: 90%;
    margin: auto;
    box-shadow: 0 10px 40px rgba(0, 0, 0, 0.6);
  }
  
  .modal-header {
    padding: 14px 16px 0;
  }
  
  .modal-body {
    padding: 14px 16px 16px;
  }
  
  .modal-header h3 {
    font-size: 1.2rem;
    padding-right: 35px;
  }

  .modal-close {
    top: 12px;
    right: 12px;
    font-size: 24px;
    width: 32px;
    height: 32px;
  }
  
  .form-row {
    grid-template-columns: 1fr;
    gap: 0;
  }
  
  .form-group {
    margin-bottom: 14px;
  }

  .form-group label {
    font-size: 0.85rem;
  }

  .form-group input,
  .form-group textarea {
    padding: 10px;
    font-size: 0.9rem;
  }
  
  .modal-actions {
    flex-direction: column;
    gap: 8px;
    padding-top: 16px;
  }
  
  .btn-primary,
  .btn-secondary {
    flex: none;
    width: 100%;
    padding: 12px 16px;
    font-size: 0.95rem;
  }
  
  .cita-info-box {
    padding: 12px;
    margin-bottom: 16px;
  }
  
  .info-item {
    font-size: 0.85rem;
    gap: 10px;
    margin-bottom: 8px;
  }

  .info-item i {
    font-size: 1rem;
  }
}

@media (max-width: 480px) {
  .modal-overlay {
    padding: 12px;
  }
  
  .modal-content {
    max-height: 75vh;
    max-width: 92%;
    border-radius: 10px;
    box-shadow: 0 8px 35px rgba(0, 0, 0, 0.7);
  }
  
  .modal-header h3 {
    font-size: 1.1rem;
  }
  
  .modal-body {
    padding: 12px 14px 14px;
  }
  
  .form-group {
    margin-bottom: 12px;
  }
  
  .form-group input,
  .form-group textarea {
    padding: 9px;
    font-size: 0.85rem;
  }

  .form-group textarea {
    min-height: 70px;
  }

  .btn-primary,
  .btn-secondary {
    padding: 11px 14px;
    font-size: 0.9rem;
  }

  .cita-info-box {
    padding: 10px;
  }

  .info-item {
    font-size: 0.8rem;
  }
}

/* For very small screens */
@media (max-height: 600px) {
  .modal-overlay {
    align-items: flex-start;
    padding-top: 10px;
  }
  
  .modal-content {
    max-height: 95vh;
    margin-top: 0;
  }

  .modal-body {
    overflow-y: auto;
  }

  .form-group {
    margin-bottom: 10px;
  }

  .cita-info-box {
    margin-bottom: 12px;
  }
}
</style>

<style>
/* Global styles to lock page scroll when modal is open */
body.modal-open {
  overflow: hidden !important;
  position: fixed;
  width: 100%;
  height: 100%;
}
</style>
