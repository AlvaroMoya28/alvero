<template>
  <div v-if="reserva" class="reserva-card sala-card card shadow mb-4">
    <div class="card-body d-flex flex-column">
      <h5 class="card-title text-primary-custom">{{ reserva.salaNombre }}</h5>
      <p class="card-text text-secondary-custom mb-1">{{ reserva.salaDescripcionCorta }}</p>
      <p class="card-text mb-1"><strong>Desde:</strong> {{ formatDate(reserva.fechaInicio) }}</p>
      <p class="card-text mb-1"><strong>Hasta:</strong> {{ formatDate(reserva.fechaFin) }}</p>
      <p class="card-text mb-1"><strong>Estado:</strong>
        <span
          style="margin-left: 0.25em;"
          :class="{
            'text-success': reserva.estado === 'CONFIRMADA',
            'text-warning': reserva.estado === 'PENDIENTE',
            'text-danger': reserva.estado === 'EXPIRADA'
          }"
        >
          {{ reserva.estado === 'EXPIRADA' ? 'EXPIRADA' : reserva.estado }}
          <span
            v-if="reserva.estado === 'EXPIRADA'"
            class="ms-1 info-icon-wrapper"
            tabindex="0"
          >
            <i
              class="bi bi-info-circle info-icon"
              aria-label="Información sobre reserva expirada"
              style="color: #dc3545;"
              @mouseenter="showTooltip('expirada')"
              @mouseleave="hideTooltip"
            ></i>
            <div
              v-if="tooltip.show && tooltip.estado === 'expirada'"
              class="custom-tooltip-absolute"
            >
              {{ tooltip.text }}
            </div>
          </span>
          <span
            v-else-if="reserva.estado === 'PENDIENTE'"
            class="ms-1 info-icon-wrapper"
            tabindex="0"
          >
            <i
              class="bi bi-info-circle info-icon"
              aria-label="Información sobre reserva pendiente"
              style="color: #ffc107;"
              @mouseenter="showTooltip('pendiente')"
              @mouseleave="hideTooltip"
            ></i>
            <div
              v-if="tooltip.show && tooltip.estado === 'pendiente'"
              class="custom-tooltip-absolute"
            >
              {{ tooltip.text }}
            </div>
          </span>
          <span
            v-else-if="reserva.estado === 'CONFIRMADA'"
            class="ms-1 info-icon-wrapper"
            tabindex="0"
          >
            <i
              class="bi bi-info-circle info-icon"
              aria-label="Información sobre reserva confirmada"
              style="color: #198754;"
              @mouseenter="showTooltip('confirmada')"
              @mouseleave="hideTooltip"
            ></i>
            <div
              v-if="tooltip.show && tooltip.estado === 'confirmada'"
              class="custom-tooltip-absolute"
            >
              {{ tooltip.text }}
            </div>
          </span>
        </span>
      </p>
      <p class="card-text mb-1"><strong>Precio:</strong> ${{ reserva.precioTotal != null ? reserva.precioTotal : reserva.salaPrecioBase }}</p>
      <div class="button-container mt-auto d-flex flex-row justify-content-between align-items-center">
        <span v-if="reserva.estado !== 'PENDIENTE' && reserva.estado !== 'EXPIRADA' && reserva.estado !== 'CANCELADA'" class="text-muted">Pagada</span>
        <span v-if="reserva.estado === 'EXPIRADA'" class="text-danger">Expirada</span>
        <span v-if="reserva.estado === 'CANCELADA'" class="text-danger">Cancelada</span>
        <button v-if="reserva.estado === 'PENDIENTE'" @click="emit('pagar', reserva)" class="btn btn-secondary btn-sm">Pagar</button>
        <button v-if="reserva.estado === 'PENDIENTE'" @click="emit('cancelar', reserva)" class="btn btn-danger btn-sm" > Cancelar </button>

        <!-- Nuevo botón para ver detalles -->
        <RouterLink
          :to="`/reserva/detalle/${reserva.idReserva}`"
          class="btn btn-outline-primary btn-sm ms-auto me-2"
          @click.native="navOpen = false">
          Ver detalles
        </RouterLink>
        <button
          v-if="mostrarAgregarResena && reserva.estado !== 'EXPIRADA' && reserva.estado !== 'CANCELADA'"
          @click="emit('agregar-resena', reserva)"
          class="btn btn-secondary btn-sm ms-auto"
          :disabled="deshabilitarAgregarResena"
        >
          <span v-if="deshabilitarAgregarResena">Reseña enviada</span>
          <span v-else>Agregar reseña</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits, ref } from 'vue'

const props = defineProps({
  reserva: { type: Object, required: true },
  mostrarAgregarResena: { type: Boolean, default: false },
  deshabilitarAgregarResena: { type: Boolean, default: false }
})
const reserva = props.reserva
const emit = defineEmits(['pagar', 'agregar-resena', 'ver-detalles', 'cancelar'])

function formatDate (dateStr) {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  if (isNaN(date)) return dateStr
  return date.toLocaleString('es-CL', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false
  })
}

const tooltip = ref({ show: false, text: '', estado: '' })

function showTooltip (estado) {
  let text = ''
  if (estado === 'expirada') {
    text = 'La reserva ha expirado porque no fue pagada a tiempo. Si necesitas ayuda, contacta a soporte.'
  } else if (estado === 'pendiente') {
    text = 'La reserva está pendiente de pago. Realiza el pago para confirmar tu reserva.'
  } else if (estado === 'confirmada') {
    text = 'La reserva está confirmada y pagada. ¡Te esperamos!'
  }
  tooltip.value = {
    show: true,
    text,
    estado
  }
}

function hideTooltip () {
  tooltip.value.show = false
}
</script>

<style scoped>
.btn-outline-primary {
  border-color: #1A4456;
  color: #1A4456;
  transition: all 0.2s ease;
}

.btn-outline-primary:hover {
  background-color: #1A4456;
  color: white;
}

/* Ajustamos el contenedor de botones para mejor distribución */
.button-container {
  gap: 0.5rem;
  flex-wrap: wrap;
}

.button-container > * {
  flex: 1 1 auto;
}

/* Para pantallas pequeñas, hacemos que los botones ocupen todo el ancho */
@media (max-width: 576px) {
  .button-container {
    flex-direction: column;
  }

  .button-container > button {
    width: 100%;
  }
}

.sala-card {
  min-width: 330px;
  background: white;
  border-radius: 10px;
  overflow: visible; /* Allow content to expand */
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  /* Remove fixed height */
  margin: 0 auto 1.5rem auto;
}
.sala-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
}
.card-body {
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
  flex-grow: 1;
}
.button-container {
  margin-top: auto;
  padding-top: 1rem;
}
.btn-secondary {
  background: #1A4456;
  border-color: #1A4456;
  color: #fff;
  transition: background 0.2s, border 0.2s;
}
.btn-secondary:hover {
  background: #122e3a;
  border-color: #122e3a;
  color: #fff;
}
.text-primary-custom {
  color: #1A4456;
}
.text-secondary-custom {
  color: #6c757d;
}
.info-icon-wrapper {
  display: inline-flex;
  align-items: center;
  vertical-align: middle;
  margin-left: 0.05em;
  position: relative;
.custom-tooltip-absolute {
  position: absolute;
  left: 50%;
  bottom: calc(100% + 6px);
  transform: translateX(-50%);
  z-index: 9999;
  background: #fff;
  color: #1A4456;
  border: 1px solid #1A4456;
  border-radius: 6px;
  padding: 0.5em 1.5em;
  font-size: 0.95em;
  box-shadow: 0 4px 16px rgba(26,68,86,0.10);
  pointer-events: none;
  opacity: 0.97;
  max-width: 410px;
  min-width: 250px;
  white-space: pre-line;
  transition: opacity 0.15s;
}
}
.custom-tooltip-absolute::after {
  content: '';
  position: absolute;
  top: 100%;
  left: 50%;
  transform: translateX(-50%);
  border-width: 6px;
  border-style: solid;
  border-color: #1A4456 transparent transparent transparent;
}
.info-icon-wrapper {
  display: inline-flex;
  align-items: center;
  vertical-align: middle;
  margin-left: 0.05em;
}
.info-icon {
  font-size: 1em;
  line-height: 1;
  vertical-align: middle;
  margin-bottom: 1px;
  cursor: pointer;
}
</style>
