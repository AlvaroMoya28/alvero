<template>
  <section class="carousel-panel" id="gallery">
    <div class="carousel-header">
      <div>
        <small>Galería</small>
        <h2>Momentos que florecen</h2>
      </div>
      <div class="carousel-controls">
        <button class="icon-button" @click="prevSlide" aria-label="Slide anterior">←</button>
        <button class="icon-button" @click="nextSlide" aria-label="Siguiente slide">→</button>
      </div>
    </div>

    <article class="carousel-card">
      <img :src="current.image" :alt="current.title" />
      <div class="carousel-copy">
        <span>{{ current.subtitle }}</span>
        <h3>{{ current.title }}</h3>
        <p>{{ current.description }}</p>
      </div>
    </article>

    <div class="carousel-dots">
      <button
        v-for="(slide, index) in slides"
        :key="slide.title"
        :class="{ active: index === currentIndex }"
        @click="setSlide(index)"
        aria-label="Ir a la diapositiva"
      ></button>
    </div>
  </section>
</template>

<script>
import { computed, ref, onMounted, onBeforeUnmount } from 'vue';

export default {
  name: 'PhotoCarousel',
  setup() {
    const slides = [
      {
        title: 'Nuestro primer paseo juntos',
        subtitle: 'Un recuerdo lleno de esperanza',
        description: 'Cada paso a tu lado se siente como si el mundo estuviera hecho para nosotros.',
        image: 'https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=900&q=80'
      },
      {
        title: 'Un jardín de sueños',
        subtitle: 'Amor y naturaleza',
        description: 'Tú, con tu pasión por la tierra, transformas cada día en algo hermoso.',
        image: 'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=900&q=80'
      },
      {
        title: 'Atardecer en calma',
        subtitle: 'Promesas suaves',
        description: 'Nuestro primer año juntos es solo el preludio de un futuro más brillante.',
        image: 'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=900&q=80'
      }
    ];

    const currentIndex = ref(0);
    const intervalId = ref(null);

    const nextSlide = () => {
      currentIndex.value = (currentIndex.value + 1) % slides.length;
    };

    const prevSlide = () => {
      currentIndex.value = (currentIndex.value - 1 + slides.length) % slides.length;
    };

    const setSlide = (index) => {
      currentIndex.value = index;
    };

    const current = computed(() => slides[currentIndex.value]);

    onMounted(() => {
      intervalId.value = window.setInterval(nextSlide, 6000);
    });

    onBeforeUnmount(() => {
      if (intervalId.value) {
        window.clearInterval(intervalId.value);
      }
    });

    return {
      slides,
      currentIndex,
      current,
      nextSlide,
      prevSlide,
      setSlide
    };
  }
};
</script>

<style scoped>
.carousel-panel {
  margin-top: 32px;
}

.carousel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 18px;
}

.carousel-header small {
  display: block;
  color: #7cb2d1;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.18em;
}

.carousel-header h2 {
  margin: 0;
  font-size: 1.7rem;
}

.carousel-card {
  border-radius: 32px;
  overflow: hidden;
  display: grid;
  grid-template-columns: 1fr 1fr;
  align-items: center;
  gap: 24px;
  background: white;
  border: 1px solid rgba(142, 201, 224, 0.22);
  box-shadow: 0 24px 60px rgba(20, 50, 90, 0.08);
}

.carousel-card img {
  width: 100%;
  min-height: 320px;
  object-fit: cover;
}

.carousel-copy {
  padding: 28px;
}

.carousel-copy span {
  display: block;
  margin-bottom: 12px;
  font-weight: 700;
  color: #7cb2d1;
}

.carousel-copy h3 {
  margin: 0 0 16px;
  font-size: 1.6rem;
}

.carousel-copy p {
  margin: 0;
  color: #556778;
  line-height: 1.8;
}

.carousel-dots {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-top: 18px;
}

.icon-button {
  width: 46px;
  height: 46px;
  border-radius: 50%;
  border: 1px solid rgba(140, 201, 224, 0.35);
  background: rgba(255, 255, 255, 0.9);
  cursor: pointer;
  font-size: 1.1rem;
}

.carousel-dots button {
  width: 14px;
  height: 14px;
  border: 0;
  border-radius: 50%;
  background: rgba(140, 201, 224, 0.35);
  cursor: pointer;
}

.carousel-dots button.active {
  background: #8cc9e0;
}

@media (max-width: 860px) {
  .carousel-card {
    grid-template-columns: 1fr;
  }
}
</style>
