<template>
  <div class="book-appointment">
    <div class="container-fluid py-4">
      <h2 class="text-center mb-4">Reservar Cita - Horarios Disponibles</h2>

      <!-- Selector de Semana -->
      <div class="card mb-4">
        <div class="card-body">
          <div class="row align-items-center">
            <div class="col-md-4">
              <label class="form-label">Seleccionar Semana</label>
              <input type="date" v-model="fechaInicio" class="form-control" @change="cargarTodosLosHorarios" />
            </div>
            <div class="col-md-8 text-end">
              <span class="badge bg-primary" v-if="diasSemana.length > 0">{{ formatoFecha(diasSemana[0]?.fecha) }} al {{ formatoFecha(diasSemana[4]?.fecha) }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Grid de Técnicos con Horarios -->
      <div v-if="cargando" class="text-center py-5">
        <div class="spinner-border text-primary"></div>
        <p>Cargando horarios...</p>
      </div>

      <div v-else>
        <div v-for="tecnico in tecnicos" :key="tecnico.idUnico" class="tecnico-section mb-4">
          <div class="card">
            <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
              <h5 class="mb-0">
                <i class="fa-solid fa-user-tie me-2"></i>
                {{ tecnico.nombre }} {{ tecnico.apellido1 }}
                <small class="ms-2">({{ tecnico.email }})</small>
              </h5>
              <button class="btn btn-light btn-sm" @click="toggleTecnico(tecnico.idUnico)">
                <i :class="isOpen(tecnico.idUnico) ? 'bi bi-chevron-up' : 'bi bi-chevron-down'"></i>
                <span class="ms-1">{{ isOpen(tecnico.idUnico) ? 'Ocultar horario' : 'Ver horario' }}</span>
              </button>
            </div>
            <div v-show="isOpen(tecnico.idUnico)" class="card-body">
              <!-- Calendario Semanal -->
              <div class="calendar-grid">
                <div v-for="dia in diasSemana" :key="dia.fecha" class="day-column">
                  <div class="day-header">
                    <strong>{{ dia.nombre }}</strong>
                    <small>{{ formatoFecha(dia.fecha) }}</small>
                  </div>
                  <div class="time-slots">
                    <div
                      v-for="horario in obtenerHorariosTecnicoDia(tecnico.idUnico, dia.fecha)"
                      :key="horario.idHorario"
                      class="time-slot disponible"
                      @click="seleccionarHorario(tecnico, horario)"
                    >
                      <i class="fa-solid fa-clock"></i>
                      {{ horario.horaInicio }}
                    </div>
                    <div v-if="obtenerHorariosTecnicoDia(tecnico.idUnico, dia.fecha).length === 0" class="no-slots">
                      Sin horarios
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de Reserva -->
    <div v-if="modalReserva" class="modal-overlay" @click.self="cerrarModal">
      <div class="modal-box">
        <h4>Confirmar Reserva de Cita</h4>
        <div class="resumen-modal mb-3">
          <p><strong>Técnico:</strong> {{ tecnicoSeleccionado?.nombre }} {{ tecnicoSeleccionado?.apellido1 }}</p>
          <p><strong>Fecha:</strong> {{ formatoFechaLarga(horarioSeleccionado?.fecha) }}</p>
          <p><strong>Hora:</strong> {{ horarioSeleccionado?.horaInicio }} - {{ horarioSeleccionado?.horaFin }}</p>
        </div>

        <form @submit.prevent="confirmarReserva" novalidate>
          <div class="mb-3">
            <label class="form-label">Nombre Completo *</label>
            <input
              ref="inputNombre"
              v-model="nombreCliente"
              type="text"
              class="form-control"
              placeholder="Nombre completo del cliente"
              @keydown.enter.prevent="focusNextInput('inputEmail')"
            />
            <div v-if="errores.nombreCliente" class="text-danger small mt-1">{{ errores.nombreCliente }}</div>
          </div>
          <div class="mb-3">
            <label class="form-label">Correo Electrónico *</label>
            <input
              ref="inputEmail"
              v-model="emailCliente"
              type="email"
              class="form-control"
              placeholder="correo@ejemplo.com"
              @keydown.enter.prevent="focusNextInput('inputCedula')"
            />
            <div v-if="errores.emailCliente" class="text-danger small mt-1">{{ errores.emailCliente }}</div>
          </div>
          <div class="mb-3">
            <label class="form-label">Cédula *</label>
            <input
              ref="inputCedula"
              v-model="cedulaCliente"
              type="text"
              class="form-control"
              placeholder="123456789"
              @keydown.enter.prevent="focusNextInput('inputDescripcion')"
            />
            <div v-if="errores.cedulaCliente" class="text-danger small mt-1">{{ errores.cedulaCliente }}</div>
          </div>
          <div class="mb-3">
            <label class="form-label">Descripción del Problema *</label>
            <textarea
              ref="inputDescripcion"
              v-model="descripcionProblema"
              class="form-control"
              rows="3"
              placeholder="Explica brevemente el problema..."
              @keydown.enter.prevent="focusNextInput('inputDireccion')"
            ></textarea>
            <div v-if="errores.descripcionProblema" class="text-danger small mt-1">{{ errores.descripcionProblema }}</div>
          </div>
          <div class="mb-3">
            <label class="form-label">Dirección *</label>
            <input
              ref="inputDireccion"
              v-model="direccion"
              type="text"
              class="form-control"
              placeholder="Dirección del servicio"
              @keydown.enter.prevent="focusNextInput('inputTelefono')"
            />
            <div v-if="errores.direccion" class="text-danger small mt-1">{{ errores.direccion }}</div>
          </div>
          <div class="mb-3">
            <label class="form-label">Teléfono de Contacto *</label>
            <input
              ref="inputTelefono"
              v-model="telefono"
              type="tel"
              class="form-control"
              placeholder="8888-9999"
              @keydown.enter.prevent="confirmarReserva"
            />
            <div v-if="errores.telefono" class="text-danger small mt-1">{{ errores.telefono }}</div>
          </div>

          <div class="d-flex justify-content-end gap-2">
            <button type="button" class="btn btn-secondary" @click="cerrarModal">Cancelar</button>
            <button type="submit" class="btn btn-success" :disabled="enviando">
              <span v-if="enviando" class="spinner-border spinner-border-sm me-2"></span>
              Confirmar Reserva
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal de Éxito -->
    <div v-if="modalExito" class="modal-overlay" @click="cerrarModalExito">
      <div class="modal-box success">
        <i class="fa-solid fa-check-circle success-icon"></i>
        <h4>¡Cita Reservada con Éxito!</h4>
        <p>Recibirás un correo de confirmación en breve.</p>
        <button class="btn btn-primary" @click="cerrarModalExito">Aceptar</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { usuarioService } from '@/services/UserService'
import CitaService from '@/services/CitaService'

const router = useRouter()
const tecnicos = ref([])
const abiertos = ref(new Set())
const tecnicoSeleccionado = ref(null)
const horarioSeleccionado = ref(null)
const horariosMap = ref({}) // Map de idTecnico -> horarios
const cargando = ref(false)
const nombreCliente = ref('')
const emailCliente = ref('')
const cedulaCliente = ref('')
const descripcionProblema = ref('')
const direccion = ref('')
const telefono = ref('')
const enviando = ref(false)
const modalReserva = ref(false)
const modalExito = ref(false)
const fechaInicio = ref(new Date().toISOString().split('T')[0])
const errores = ref({
  nombreCliente: '',
  emailCliente: '',
  cedulaCliente: '',
  descripcionProblema: '',
  direccion: '',
  telefono: ''
})
const inputNombre = ref(null)
const inputEmail = ref(null)
const inputCedula = ref(null)
const inputDescripcion = ref(null)
const inputDireccion = ref(null)
const inputTelefono = ref(null)

const diasSemana = computed(() => {
  const selected = new Date(fechaInicio.value + 'T00:00:00')
  const day = selected.getDay()
  const diffToMonday = (day === 0 ? -6 : 1 - day)
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
  return dias
})

async function cargarTecnicos () {
  try {
    const usuarios = await usuarioService.getTecnicosPublicos()
    tecnicos.value = usuarios
    // Por defecto, ninguno abierto
    abiertos.value = new Set()
  } catch (error) {
    console.error('Error cargando técnicos:', error)
  }
}

async function cargarTodosLosHorarios () {
  cargando.value = true
  horariosMap.value = {}

  try {
    const selected = new Date(fechaInicio.value + 'T00:00:00')
    const day = selected.getDay()
    const diffToMonday = (day === 0 ? -6 : 1 - day)
    const monday = new Date(selected)
    monday.setDate(selected.getDate() + diffToMonday)

    const friday = new Date(monday)
    friday.setDate(monday.getDate() + 4)

    const fechaDesde = monday.toISOString().split('T')[0]
    const fechaHasta = friday.toISOString().split('T')[0]

    for (const tecnico of tecnicos.value) {
      try {
        const horarios = await CitaService.obtenerHorariosDisponibles(
          tecnico.idUnico,
          fechaDesde,
          fechaHasta
        )
        horariosMap.value[tecnico.idUnico] = horarios.filter(h => h.disponibleReal)
      } catch (error) {
        console.error(`Error cargando horarios para ${tecnico.nombre}:`, error)
        horariosMap.value[tecnico.idUnico] = []
      }
    }
  } catch (error) {
    console.error('Error cargando horarios:', error)
  } finally {
    cargando.value = false
  }
}

function obtenerHorariosTecnicoDia (idTecnico, fecha) {
  const horarios = horariosMap.value[idTecnico] || []
  return horarios.filter(h => h.fecha.split('T')[0] === fecha)
}

function toggleTecnico (id) {
  const s = new Set(abiertos.value)
  if (s.has(id)) s.delete(id)
  else s.add(id)
  abiertos.value = s
}

function isOpen (id) {
  return abiertos.value.has(id)
}

function seleccionarHorario (tecnico, horario) {
  tecnicoSeleccionado.value = tecnico
  horarioSeleccionado.value = horario
  modalReserva.value = true
}

function formatoFecha (fecha) {
  if (!fecha) return ''
  const fechaSolo = fecha.split('T')[0]
  const d = new Date(fechaSolo + 'T00:00:00')
  return d.toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit' })
}

function formatoFechaLarga (fecha) {
  if (!fecha) return ''
  const fechaSolo = fecha.split('T')[0]
  const d = new Date(fechaSolo + 'T00:00:00')
  return d.toLocaleDateString('es-ES', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })
}

function validarFormulario () {
  let valido = true
  errores.value = {
    nombreCliente: '',
    emailCliente: '',
    cedulaCliente: '',
    descripcionProblema: '',
    direccion: '',
    telefono: ''
  }

  if (!nombreCliente.value.trim()) {
    errores.value.nombreCliente = 'El nombre completo es requerido'
    valido = false
  }

  if (!emailCliente.value.trim()) {
    errores.value.emailCliente = 'El correo electrónico es requerido'
    valido = false
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailCliente.value)) {
    errores.value.emailCliente = 'El correo electrónico no es válido'
    valido = false
  }

  if (!cedulaCliente.value.trim()) {
    errores.value.cedulaCliente = 'La cédula es requerida'
    valido = false
  }

  if (!descripcionProblema.value.trim()) {
    errores.value.descripcionProblema = 'La descripción del problema es requerida'
    valido = false
  }

  if (!direccion.value.trim()) {
    errores.value.direccion = 'La dirección es requerida'
    valido = false
  }

  if (!telefono.value.trim()) {
    errores.value.telefono = 'El teléfono de contacto es requerido'
    valido = false
  }

  return valido
}

function focusNextInput (refName) {
  const refs = {
    inputNombre: inputNombre.value,
    inputEmail: inputEmail.value,
    inputCedula: inputCedula.value,
    inputDescripcion: inputDescripcion.value,
    inputDireccion: inputDireccion.value,
    inputTelefono: inputTelefono.value
  }

  if (refs[refName]) {
    refs[refName].focus()
  }
}

async function confirmarReserva () {
  if (!validarFormulario()) {
    return
  }

  enviando.value = true
  try {
    const fechaSolo = horarioSeleccionado.value.fecha.split('T')[0]

    const citaData = {
      nombreCliente: nombreCliente.value,
      emailCliente: emailCliente.value,
      cedulaCliente: cedulaCliente.value,
      idUsuarioTecnico: tecnicoSeleccionado.value.idUnico,
      fechaCita: fechaSolo,
      horaInicio: horarioSeleccionado.value.horaInicio,
      descripcionProblema: descripcionProblema.value,
      direccion: direccion.value,
      telefonoContacto: telefono.value
    }

    await CitaService.crearCita(citaData)

    modalReserva.value = false
    modalExito.value = true
    limpiarFormulario()
    await cargarTodosLosHorarios()
  } catch (error) {
    console.error('Error creando cita:', error)
    errores.value.general = 'Error al crear la cita: ' + (error.response?.data?.message || error.message)
  } finally {
    enviando.value = false
  }
}

function cerrarModal () {
  modalReserva.value = false
  limpiarFormulario()
}

function cerrarModalExito () {
  modalExito.value = false
  router.push('/')
}

function limpiarFormulario () {
  nombreCliente.value = ''
  emailCliente.value = ''
  cedulaCliente.value = ''
  descripcionProblema.value = ''
  direccion.value = ''
  telefono.value = ''
  tecnicoSeleccionado.value = null
  horarioSeleccionado.value = null
  errores.value = {
    nombreCliente: '',
    emailCliente: '',
    cedulaCliente: '',
    descripcionProblema: '',
    direccion: '',
    telefono: ''
  }
}

onMounted(async () => {
  // Cargar datos sin requerir autenticación (reserva pública)
  await cargarTecnicos()
  await cargarTodosLosHorarios()
})
</script>

<style scoped>
.book-appointment {
  min-height: 100vh;
  background: #f8f9fa;
  padding-bottom: 2rem;
}

.container-fluid {
  max-width: 1400px;
  margin: 0 auto;
}

h2 {
  color: #1A4456;
  font-weight: 700;
  text-shadow: none;
}

.card {
  border: none;
  border-radius: 10px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  background-color: white;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.card:hover {
  transform: translateY(-5px);
  box-shadow: 0 12px 32px rgba(0,0,0,0.2);
}

.card-header {
  border-radius: 10px 10px 0 0 !important;
  padding: 1.25rem;
  background-color: #1A4456 !important;
}

.card-header h5 {
  font-size: 1.1rem;
  font-weight: 600;
}

.card-header .btn.btn-light.btn-sm {
  color: #1A4456;
  background: #fff;
  border: none;
  border-radius: 20px;
  padding: 0.35rem 0.6rem;
  display: inline-flex;
  align-items: center;
}

.badge {
  padding: 0.5rem 1rem;
  font-size: 0.95rem;
  font-weight: 500;
  background-color: #1A4456 !important;
  color: white;
}

/* Grid de Calendario */
.calendar-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 1rem;
  margin-top: 1rem;
}

.day-column {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 10px;
  padding: 1rem;
  min-height: 300px;
  display: flex;
  flex-direction: column;
}

.day-header {
  text-align: center;
  padding: 0.75rem;
  background-color: #1A4456;
  color: white;
  border-radius: 8px;
  margin-bottom: 1rem;
  box-shadow: none;
}

.day-header strong {
  display: block;
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.day-header small {
  font-size: 0.85rem;
  opacity: 0.9;
}

/* Time Slots */
.time-slots {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  flex-grow: 1;
}

.time-slot {
  padding: 1rem;
  border-radius: 10px;
  text-align: center;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  position: relative;
  overflow: hidden;
}

.time-slot::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.3), transparent);
  transition: left 0.5s;
}

.time-slot:hover::before {
  left: 100%;
}

.time-slot.disponible {
  background-color: #1A4456;
  color: white;
  border: 2px solid transparent;
}

.time-slot.disponible:hover {
  transform: translateY(-3px) scale(1.05);
  background-color: #B88B4A;
  box-shadow: 0 6px 20px rgba(184, 139, 74, 0.4);
  border-color: white;
}

.time-slot.disponible:active {
  transform: translateY(-1px) scale(1.02);
}

.time-slot i {
  margin-right: 0.5rem;
  font-size: 1.1rem;
}

.no-slots {
  text-align: center;
  color: #6c757d;
  padding: 2rem 1rem;
  font-style: italic;
  opacity: 0.7;
}

/* Modales */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(5px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  animation: fadeIn 0.3s ease;
  padding: 20px;
  overflow-y: auto;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.modal-box {
  background: white;
  padding: 2rem;
  border-radius: 20px;
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0,0,0,0.5);
  animation: slideUp 0.3s ease;
  margin: auto;
}

@keyframes slideUp {
  from {
    transform: translateY(50px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-box h4 {
  color: #1A4456;
  font-weight: 700;
  margin-bottom: 1.5rem;
  text-align: center;
}

.resumen-modal {
  background: linear-gradient(135deg, #f5f7fa 0%, #e8eef2 100%);
  padding: 1.25rem;
  border-radius: 12px;
  border-left: 4px solid #1A4456;
}

.resumen-modal p {
  margin-bottom: 0.5rem;
  color: #2c3e50;
}

.resumen-modal p:last-child {
  margin-bottom: 0;
}

.form-label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.form-control {
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  padding: 0.75rem;
  transition: all 0.3s ease;
}

.form-control:focus {
  border-color: #1A4456;
  box-shadow: 0 0 0 0.2rem rgba(26, 68, 86, 0.25);
  transform: translateY(-2px);
}

.btn {
  padding: 0.75rem 1.5rem;
  border-radius: 10px;
  font-weight: 600;
  transition: all 0.3s ease;
  border: none;
}

.btn-success {
  background-color: #1A4456;
  color: white;
  box-shadow: 0 4px 12px rgba(26, 68, 86, 0.3);
}

.btn-success:hover {
  transform: translateY(-2px);
  background-color: #B88B4A;
  box-shadow: 0 6px 20px rgba(184, 139, 74, 0.4);
}

.btn-success:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

.btn-secondary {
  background: #6c757d;
  box-shadow: 0 4px 12px rgba(108, 117, 125, 0.3);
}

.btn-secondary:hover {
  background: #5a6268;
  transform: translateY(-2px);
}

.btn-primary {
  background-color: #1A4456;
  color: white;
  box-shadow: 0 4px 12px rgba(26, 68, 86, 0.3);
}

.btn-primary:hover {
  transform: translateY(-2px);
  background-color: #B88B4A;
  box-shadow: 0 6px 20px rgba(184, 139, 74, 0.4);
}

/* Modal de Éxito */
.modal-box.success {
  text-align: center;
}

.success-icon {
  font-size: 4rem;
  color: #1A4456;
  margin-bottom: 1rem;
  animation: scaleIn 0.5s ease;
}

@keyframes scaleIn {
  0% {
    transform: scale(0);
    opacity: 0;
  }
  50% {
    transform: scale(1.2);
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

.modal-box.success h4 {
  color: #1A4456;
}

.modal-box.success p {
  color: #6c757d;
  margin-bottom: 1.5rem;
}

/* Loading Spinner */
.spinner-border {
  width: 3rem;
  height: 3rem;
  border-width: 0.3rem;
}

/* Técnico Section */
.tecnico-section {
  animation: fadeInUp 0.5s ease;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive */
@media (max-width: 1200px) {
  .calendar-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 768px) {
  .calendar-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 0.75rem;
  }

  .day-column {
    padding: 0.75rem;
  }

  .time-slot {
    padding: 0.75rem;
    font-size: 0.9rem;
  }

  .modal-overlay {
    padding: 15px;
  }

  .modal-box {
    padding: 1.5rem;
    width: 92%;
    max-height: 85vh;
    border-radius: 16px;
  }

  .modal-box h4 {
    font-size: 1.3rem;
  }

  .resumen-modal {
    padding: 1rem;
    font-size: 0.9rem;
  }

  .form-control {
    padding: 0.65rem;
    font-size: 0.95rem;
  }

  .btn {
    padding: 0.65rem 1.25rem;
    font-size: 0.95rem;
  }
}

@media (max-width: 576px) {
  .calendar-grid {
    grid-template-columns: 1fr;
  }

  .modal-overlay {
    padding: 12px;
  }

  .modal-box {
    padding: 1.25rem;
    width: 95%;
    max-height: 88vh;
    border-radius: 12px;
  }

  .modal-box h4 {
    font-size: 1.2rem;
    margin-bottom: 1rem;
  }

  .resumen-modal {
    padding: 0.85rem;
    font-size: 0.85rem;
  }

  .form-label {
    font-size: 0.9rem;
    margin-bottom: 0.4rem;
  }

  .form-control {
    padding: 0.6rem;
    font-size: 0.9rem;
  }

  .btn {
    padding: 0.6rem 1rem;
    font-size: 0.9rem;
    width: 100%;
  }

  .mb-3 {
    margin-bottom: 0.75rem !important;
  }
}
</style>
