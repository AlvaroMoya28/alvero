<template>
  <div class="brand-carousel-container">
    <div class="brand-carousel-wrapper">
      <button class="auto-scroll-toggle" @click="toggleAutoScroll" :title="autoScrollEnabled ? 'Desactivar auto-scroll' : 'Activar auto-scroll'">
        <i class="bi" :class="autoScrollEnabled ? 'bi-pause-fill' : 'bi-play-fill'"></i>
      </button>
      <div class="brand-carousel-track" ref="brandTrack">
        <div class="brand-slide" v-for="(slide, index) in slides" :key="index"
          :style="{ backgroundImage: `url(${slide.backgroundImage})` }">
          <div class="slide-overlay"></div>
          <div class="slide-content">
            <h2 class="slide-title">{{ slide.title }}</h2>
            <p class="slide-description">{{ slide.description }}</p>
            <button class="btn btn-outline-light slide-button" @click="navigateToSlideRoute(index)">
              {{ slide.buttonText }}
            </button>
          </div>
        </div>
      </div>
      <button class="brand-control prev" @click="scrollBrand(-1)">
        <span class="carousel-control-prev-icon" aria-hidden="true"></span>
      </button>
      <button class="brand-control next" @click="scrollBrand(1)">
        <span class="carousel-control-next-icon" aria-hidden="true"></span>
      </button>
    </div>
    <div class="brand-pagination">
      <span v-for="(slide, index) in slides" :key="index" :class="{ active: currentBrandIndex === index }" @click="goToBrandSlide(index)"></span>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import ofertas from '@/assets/data/ofertas.json'

const router = useRouter()
const autoScrollEnabled = ref(true)
const autoScrollInterval = ref(null)
const brandTrack = ref(null)
const currentBrandIndex = ref(0)

const slides = ref([
  {
    title: 'Nuestra Historia',
    description: 'Más de 1 mes ofreciendo los mejores productos con calidad garantizada',
    buttonText: 'Conoce más',
    backgroundImage: ofertas[0]?.imagen || '',
    route: '/about'
  },
  {
    title: 'Compromiso con la Calidad',
    description: 'Todos nuestros productos pasan por rigurosos controles de calidad',
    buttonText: 'Nuestros estándares',
    backgroundImage: ofertas[1]?.imagen || '',
    route: '/estandares'
  },
  {
    title: 'Nuestra Ubicación',
    description: 'Visítanos en nuestro centro de eventos estratégicamente ubicado para tu comodidad',
    buttonText: 'Cómo llegar',
    backgroundImage: ofertas[2]?.imagen || '',
    route: '/ubicacion'
  }
])

const clearCarouselInterval = () => {
  if (autoScrollInterval.value) {
    clearInterval(autoScrollInterval.value)
    autoScrollInterval.value = null
  }
}

const scrollToBrandSlide = (index) => {
  if (!brandTrack.value) return
  const numSlides = slides.value.length
  if (numSlides === 0) return

  let newIndex = index
  if (index >= numSlides) {
    newIndex = 0
  } else if (index < 0) {
    newIndex = numSlides - 1
  }

  const slideWidth = 100
  brandTrack.value.style.transition = 'transform 1s ease-out'
  brandTrack.value.style.transform = `translateX(-${newIndex * slideWidth}%)`
  currentBrandIndex.value = newIndex
}

const startAutoScroll = () => {
  clearCarouselInterval()

  if (autoScrollEnabled.value) {
    autoScrollInterval.value = setInterval(() => {
      scrollToBrandSlide(currentBrandIndex.value + 1)
    }, 5000)
  }
}

const navigateToSlideRoute = (index) => {
  const route = slides.value[index].route
  if (route) {
    router.push(route)
  }
}

const toggleAutoScroll = () => {
  autoScrollEnabled.value = !autoScrollEnabled.value
  if (autoScrollEnabled.value) {
    startAutoScroll()
  } else {
    clearCarouselInterval()
  }
}

const scrollBrand = (direction) => {
  const newIndex = currentBrandIndex.value + direction
  scrollToBrandSlide(newIndex)
  if (autoScrollEnabled.value) {
    startAutoScroll()
  }
}

const goToBrandSlide = (index) => {
  scrollToBrandSlide(index)
  if (autoScrollEnabled.value) {
    startAutoScroll()
  }
}

onMounted(() => {
  if (slides.value.length > 0 && autoScrollEnabled.value) {
    startAutoScroll()
  }
})

onUnmounted(() => {
  clearCarouselInterval()
})
</script>

<style lang="scss" scoped>
.auto-scroll-toggle {
  position: absolute;
  top: 20px;
  right: 25px;
  width: 40px;
  height: 40px;
  /* CAMBIADO: Estilo consistente */
  background: rgba(0, 0, 0, 0.3);
  border: none;
  border-radius: 50%;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  z-index: 20;
  transition: all 0.3s ease;
}
.auto-scroll-toggle:hover {
  /* CAMBIADO: Estilo consistente */
  background: rgba(0, 0, 0, 0.5);
  transform: scale(1.1);
}
.auto-scroll-toggle i {
  font-size: 1.2rem;
}

/* Estilos para las flechas laterales */
.brand-control {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 50px;
  height: 50px;
  /* CAMBIADO: Estilo consistente */
  background: rgba(0, 0, 0, 0.3);
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  cursor: pointer;
  transition: all 0.3s ease;
  z-index: 10;
}
.brand-control:hover {
  /* CAMBIADO: Estilo consistente */
  background: rgba(0, 0, 0, 0.5);
}
.brand-control.prev {
  left: 20px;
}
.brand-control.next {
  right: 20px;
}

/* --- AÑADIDO: Definición de los íconos de las flechas --- */
.carousel-control-prev-icon,
.carousel-control-next-icon {
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' fill='%23fff'%3e%3cpath d='M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z'/%3e%3c/svg%3e");
  background-repeat: no-repeat;
  background-size: 50% 50%;
  background-position: center;
  width: 24px;
  height: 24px;
}

.brand-control.next .carousel-control-next-icon {
   transform: rotate(180deg);
}
/* --- FIN DE LA ADICIÓN --- */

.brand-carousel-container {
  position: relative;
  width: 100%;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.brand-carousel-wrapper {
  width: 100%;
  overflow: hidden;
  position: relative;
}

.brand-carousel-track {
  display: flex;
  width: 100%;
  height: 400px;
  will-change: transform;
  transition: transform 0.5s ease-out;
}

.brand-slide {
  flex: 0 0 100%;
  position: relative;
  background-size: cover;
  background-position: center;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.brand-pagination {
  position: absolute;
  bottom: 20px;
  left: 0;
  right: 0;
  display: flex;
  justify-content: center;
  gap: 0.75rem;
  z-index: 10;
}
.brand-pagination span {
  width: 30px;
  height: 3px;
  background-color: rgba(255, 255, 255, 0.5);
  cursor: pointer;
  transition: all 0.3s ease;
}
.brand-pagination span.active {
  background-color: white;
  transform: scaleY(1.5);
}
.brand-pagination span:hover {
  background-color: rgba(255, 255, 255, 0.8);
}

.slide-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
}

.slide-content {
  position: relative;
  z-index: 2;
  text-align: center;
  padding: 2rem;
  max-width: 800px;
}

.slide-title {
  font-size: 2.5rem;
  margin-bottom: 1rem;
  font-weight: 700;
  text-shadow: 1px 1px 3px rgba(0, 0, 0, 0.5);
}

.slide-description {
  font-size: 1.25rem;
  margin-bottom: 2rem;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.5);
}

.slide-button {
  padding: 0.75rem 2rem;
  font-size: 1rem;
  border-width: 2px;
  transition: all 0.3s ease;
  cursor: pointer;
}
.slide-button:hover {
  background-color: rgba(255, 255, 255, 0.1);
  transform: translateY(-2px);
}

@media (max-width: 768px) {
  .brand-carousel-track {
    height: 300px;
  }
  .slide-title {
    font-size: 1.8rem;
  }
  .slide-description {
    font-size: 1rem;
  }
  .brand-pagination {
    gap: 0.5rem;
  }
  .brand-pagination span {
    width: 20px;
    height: 2px;
  }

  .brand-control {
    width: 40px;
    height: 40px;
  }
  .brand-control.prev {
    left: 10px;
  }
  .brand-control.next {
    right: 10px;
  }

  .auto-scroll-toggle {
    width: 35px;
    height: 35px;
    right: 10px;
  }
  .auto-scroll-toggle i {
    font-size: 1rem;
  }
  .brand-pagination {
    right: 60px;
  }
}

.btn-outline-light {
  color: #ffffff;
}

.btn-check:checked+.btn, .btn.active, .btn.show, .btn:first-child:active, :not(.btn-check)+.btn:active {
    color: #1A4456;
}
</style>
