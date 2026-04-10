<template>
  <div class="container-fluid mt-4 sala-carousel-container">
    <h2 class="mb-4">{{ titulo }}</h2>
    <div class="position-relative">
      <div v-if="isLoading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Cargando...</span>
        </div>
      </div>
      <div v-else-if="error" class="alert alert-danger">
        {{ error }}
      </div>
      <div v-else-if="salasRecomendadas.length === 0" class="alert alert-info">
        No hay salas recomendadas disponibles.
      </div>
      <div v-else>
        <div class="sala-carousel-wrapper" ref="wrapper">
          <div class="sala-carousel-track" ref="track">
            <div class="card h-100 shadow sala-card-fixed fade-card sala-card" v-for="sala in salasRecomendadas" :key="sala.idSala" @click="goToSala(sala.idSala)" style="cursor:pointer;">
              <div class="ratio-container position-relative">
                <div v-if="!imagenCargada[sala.idSala]" class="image-spinner-overlay">
                  <div class="spinner-border text-primary" role="status" style="width: 2.5rem; height: 2.5rem;"></div>
                </div>
                <img
                  :src="`${backendUrl}${sala.imagenPrincipal}`"
                  class="card-img-top img-ratio"
                  :alt="'Imagen de ' + sala.nombre"
                  @load="handleImageLoad(sala.idSala)"
                  @error="onImageError"
                  :style="!imagenCargada[sala.idSala] ? 'opacity:0;' : 'opacity:1; transition: opacity 0.3s;'"
                >
                <span v-if="sala.descuento > 0" class="offer-badge" style="position: absolute; top: 12px; left: 12px; background-color: #dc3545; color: white; padding: 0.35rem 0.7rem; border-radius: 6px; font-size: 0.95rem; font-weight: bold; z-index: 3; box-shadow: 0 2px 8px rgba(0,0,0,0.08); letter-spacing: 0.5px;">{{ sala.descuento }}% OFF</span>
              </div>
              <div class="card-body d-flex flex-column">
                <h5 class="card-title text-primary-custom">{{ sala.nombre }}</h5>
                <p class="card-text text-secondary-custom">{{ sala.descripcionCorta }}</p>
                <div class="mt-auto d-flex w-100 align-items-end justify-content-between gap-2" style="min-height:56px;">
                  <div class="price-container-responsive" style="padding-bottom: 6px;">
                    <template v-if="sala.descuento > 0">
                      <div class="price-container">
                        <span class="original-price" style="color: #6c757d; text-decoration: line-through; font-size: 1.0em;">${{ sala.precioBase.toFixed(2) }}</span>
                        <span class="precio-oferta text-danger d-block" style="font-weight: bold; font-size: 1.15em;">${{ (sala.precioBase * (1 - sala.descuento / 100)).toFixed(2) }}</span>
                      </div>
                    </template>
                    <template v-else>
                      <span class="precio text-primary-custom" style="font-weight: bold; font-size: 1.15em; color: #1A4456;">${{ sala.precioBase.toFixed(2) }}</span>
                    </template>
                  </div>
                  <div class="d-flex align-items-end gap-2 flex-wrap justify-content-end">
                    <router-link :to="`/salas/${sala.idSala}`" class="btn btn-secondary" style="min-width:90px;white-space:nowrap;" @click.stop>
                      Ver Sala
                    </router-link>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <button v-if="showArrows" class="carousel-control prev" @click.stop="scroll(-1)">
          <span class="carousel-control-prev-icon" aria-hidden="true"></span>
        </button>
        <button v-if="showArrows" class="carousel-control next" @click.stop="scroll(1)">
          <span class="carousel-control-next-icon" aria-hidden="true"></span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch, onBeforeUnmount, nextTick, defineProps } from 'vue'
import { useRouter } from 'vue-router'
import { getTodasLasSalas } from '@/services/RoomService'
import { useUserStore } from '@/stores/userStore'

const router = useRouter()
const salas = ref([])
const isLoading = ref(true)
const error = ref(null)
const backendUrl = 'https://localhost:5001'
const imagenCargada = ref({})

const track = ref(null)
const wrapper = ref(null)

const userStore = useUserStore()
const usuario = userStore.getUserFromToken()
const usuarioId = usuario?.id || usuario?.sub

// Permitir filtrar solo ofertas si se pasa la prop onlyOfertas
const props = defineProps({
  onlyOfertas: { type: Boolean, default: false },
  titulo: { type: String, default: 'Recomendaciones' }
})
// Filtramos según la prop: si onlyOfertas, solo con descuento; si no, solo sin descuento
const salasRecomendadas = computed(() => {
  if (props.onlyOfertas) {
    return salas.value.filter(sala => sala.descuento > 0)
  } else {
    return salas.value.filter(sala => sala.descuento === 0)
  }
})

const showArrows = ref(false)
function checkShowArrows () {
  nextTick(() => {
    if (!wrapper.value || !track.value) {
      showArrows.value = false
      return
    }
    // Si el ancho del track es mayor al del wrapper, hay overflow
    // Usar scrollWidth del wrapper para detectar overflow real
    showArrows.value = wrapper.value.scrollWidth > wrapper.value.clientWidth + 2 // margen de error
  })
}

// Funcionalidad de favoritos eliminada

async function cargarRecomendaciones () {
  isLoading.value = true
  error.value = null
  try {
    const response = await getTodasLasSalas()
    salas.value = response
  } catch (err) {
    console.error('Error al cargar recomendaciones:', err)
    error.value = err.message || 'No se pudieron cargar las recomendaciones.'
  } finally {
    isLoading.value = false
  }
}

const handleImageLoad = (salaId) => {
  imagenCargada.value[salaId] = true
}

const onImageError = (event) => {
  event.target.src = 'https://via.placeholder.com/400x300.png?text=Imagen+no+disponible'
}

const scroll = (direction) => {
  if (!wrapper.value || !track.value) return
  const card = track.value.children[0]
  if (!card) return
  const cardWidth = card.offsetWidth
  const gap = parseInt(getComputedStyle(track.value).gap) || 20
  const containerWidth = wrapper.value.offsetWidth
  const visibleCards = Math.floor(containerWidth / (cardWidth + gap))
  const scrollAmount = (cardWidth + gap) * visibleCards
  wrapper.value.scrollBy({ left: direction * scrollAmount, behavior: 'smooth' })
}

const goToSala = (idSala) => {
  router.push(`/salas/${idSala}`)
}

onMounted(async () => {
  await Promise.all([cargarRecomendaciones()])
  checkShowArrows()
  window.addEventListener('resize', checkShowArrows)
})

watch(salasRecomendadas, () => {
  nextTick(() => checkShowArrows())
}, { immediate: true })
onBeforeUnmount(() => {
  window.removeEventListener('resize', checkShowArrows)
})

// Eliminado: favoritos

// ...existing code...

</script>

<style lang="scss" scoped>
.image-spinner-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255,255,255,0.7);
  z-index: 2;
}
/* Estilos se mantienen igual que en el componente original */
.sala-carousel-container {
  color: #1A4456;
  padding: 0 50px;
}
.sala-carousel-wrapper {
  width: 100%;
  overflow-x: auto;
  cursor: grab;
  -webkit-overflow-scrolling: touch;
  scrollbar-width: thin;
  scrollbar-color: #B88B4A #f8f9fa;
}
.sala-carousel-track {
  display: flex;
  gap: 20px;
  padding: 10px 0;
  will-change: transform;
  transition: transform 0.5s ease-out;
  user-select: none;
}

.sala-card {
  min-width: 330px;
  max-width: 330px;
  width: 330px;
  background: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  height: 490px;
  max-height: 490px;
  min-height: 490px;

  &:hover {
    transform: translateY(-5px);
    box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
  }

  .card-body {
    padding: 1.25rem;
    display: flex;
    flex-direction: column;
    flex-grow: 1;
    min-height: 0;
    max-height: 100%;
    overflow: hidden;

    .card-title {
      font-size: 1.15rem;
      font-weight: 600;
      margin-bottom: 0.5rem;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .card-text {
      font-size: 1rem;
      color: #1A4456;
      margin-bottom: 0.5rem;
      overflow-y: auto;
      max-height: 6em;
      text-overflow: ellipsis;
      white-space: normal;
      /* Si el texto es muy largo, muestra scroll vertical (ahora hasta 3 renglones) */
    }

    .button-container {
      margin-top: auto;
      padding-top: 1rem;
    }
  }
}

/* Eliminado bloque duplicado e inválido de .sala-card */
.carousel-control {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 40px;
  height: 40px;
  background: #1A4456;
  border: none;
  border-radius: 50%;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
  z-index: 10;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0.8;
  cursor: pointer;

  &:hover {
    opacity: 1;
  }

  &.prev {
    left: -15px;
  }

  &.next {
    right: -15px;
  }

  &-prev-icon,
  &-next-icon {
    background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' fill='%23fff'%3e%3cpath d='M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z'/%3e%3c/svg%3e");
    background-repeat: no-repeat;
    background-size: 50% 50%;
    background-position: center;
    width: 20px;
    height: 20px;
  }

  &.next .carousel-control-next-icon {
    transform: rotate(180deg);
  }
}
.ratio-container {
  position: relative;
  width: 100%;
  padding-top: 75%;
  overflow: hidden;
  background-color: #f8f9fa;
}
.img-ratio {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
  max-width: 100%;
  max-height: 100%;
}
.image-loader {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  background-color: rgba(255, 255, 255, 0.8);
  z-index: 1;
}
.loader {
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid #007bff;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  animation: spin 0.8s linear infinite;
}
@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}
@media (max-width: 768px) {
  .sala-carousel-container {
    padding: 0 15px;
  }

  .carousel-control {
    display: none !important;
  }

  .carousel-control {
    &.prev {
      left: -5px;
    }

    &.next {
      right: -5px;
    }
  }
}
.btn {
  --bs-btn-color: #fff;
  --bs-btn-bg: #1A4456;
  --bs-btn-border-color: #1A4456;
  --bs-btn-hover-color: #fff;
  --bs-btn-hover-bg: #122e3a;
  --bs-btn-hover-border-color: #122e3a;
  --bs-btn-focus-shadow-rgb: 60, 153, 110;
  --bs-btn-active-color: #fff;
  --bs-btn-active-bg: #122e3a;
  --bs-btn-active-border-color: #122e3a;
  --bs-btn-active-shadow: inset 0 3px 5px rgba(0, 0, 0, 0.125);
  --bs-btn-disabled-color: #fff;
  --bs-btn-disabled-bg: #1A4456;
  --bs-btn-disabled-border-color: #1A4456;
  box-shadow: none;
}
.btn:hover {
  background-color: #122e3a;
  border-color: #122e3a;
  color: #fff;
}
.btn-heart {
  background: transparent !important;
  border: none !important;
  padding: 0 !important;
  margin-right: 0.5rem;
  cursor: pointer;
  font-size: 1.25rem;
  line-height: 1;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: none !important;
  outline: none !important;
}
.btn-heart:focus {
  outline: none;
  box-shadow: none;
}
.btn-heart:hover {
  background: transparent !important;
  box-shadow: none !important;
}
.btn-heart i {
  color: #1A4456 !important;
  background: none !important;
  box-shadow: none !important;
  border-radius: 0 !important;
  font-size: 1.5rem;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: color 0.2s, transform 0.2s;
}
.btn-heart:hover i {
  transform: none !important;
}
</style>
