<template>
  <section class="music-panel">
    <div class="music-card">
      <div>
        <small>Sonido suave</small>
        <h2>Música para nuestro aniversario</h2>
        <p>Un acompañamiento tranquilo que hace la página aún más especial.</p>
      </div>
      <div class="music-actions">
        <button class="btn-music" @click="togglePlay">
          {{ playing ? 'Pausar música' : 'Reproducir música' }}
        </button>
        <span class="music-state">{{ playing ? 'Sonando...' : 'En espera' }}</span>
      </div>
    </div>
    <audio ref="audioRef" :src="trackUrl" @ended="playing = false" preload="auto" />
  </section>
</template>

<script>
import { ref } from 'vue';

export default {
  name: 'MusicPlayer',
  setup() {
    const trackUrl = 'https://cdn.pixabay.com/audio/2023/06/26/audio_6985a861a5.mp3';
    const playing = ref(false);
    const audioRef = ref(null);

    const togglePlay = () => {
      if (!audioRef.value) {
        return;
      }

      const audio = audioRef.value;
      if (playing.value) {
        audio.pause();
        playing.value = false;
      } else {
        audio.play().then(() => {
          playing.value = true;
        }).catch(() => {
          playing.value = false;
        });
      }
    };

    return {
      trackUrl,
      playing,
      audioRef,
      togglePlay
    };
  }
};
</script>

<style scoped>
.music-panel {
  margin-bottom: 28px;
}

.music-card {
  display: grid;
  grid-template-columns: 1fr auto;
  align-items: center;
  gap: 22px;
  padding: 24px 28px;
  border-radius: 32px;
  background: rgba(255, 255, 255, 0.96);
  border: 1px solid rgba(140, 201, 224, 0.26);
  box-shadow: 0 18px 42px rgba(20, 50, 90, 0.08);
}

.music-card small {
  display: block;
  margin-bottom: 6px;
  color: #7cb2d1;
  font-weight: 700;
  letter-spacing: 0.15em;
  text-transform: uppercase;
}

.music-card h2 {
  margin: 0 0 10px;
  font-size: 1.5rem;
}

.music-card p {
  margin: 0;
  color: #556778;
}

.music-actions {
  display: grid;
  gap: 12px;
  text-align: right;
}

.btn-music {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 160px;
  min-height: 48px;
  border-radius: 999px;
  border: none;
  background: linear-gradient(135deg, #8cc9e0, #f9d8e0);
  color: white;
  font-weight: 700;
  cursor: pointer;
}

.music-state {
  color: #7b9cae;
  font-size: 0.95rem;
}

@media (max-width: 740px) {
  .music-card {
    grid-template-columns: 1fr;
    text-align: left;
  }
  .music-actions {
    text-align: left;
  }
}
</style>
