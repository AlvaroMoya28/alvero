<template>
  <div class="app-shell">
    <header class="header-bar">
      <div class="brand">
        <span class="brand-mark">V&amp;A</span>
        <div>
          <p class="brand-label">Verónica & Álvaro</p>
          <p class="brand-subtitle">Un recorrido por nuestro primer año</p>
        </div>
      </div>

      <button class="header-toggle" :class="{ open: menuOpen }" :aria-expanded="menuOpen" @click="menuOpen = !menuOpen" aria-label="Abrir menú">
        <span></span>
        <span></span>
        <span></span>
      </button>

      <nav class="header-nav" :class="{ open: menuOpen }">
        <a href="#home" @click="menuOpen = false">Inicio</a>
        <a href="#story" @click="menuOpen = false">Historia</a>
        <a href="#gallery" @click="menuOpen = false">Galería</a>
        <a href="#garden" @click="menuOpen = false">Jardín</a>
        <a href="#quiz" @click="menuOpen = false">Juego</a>
        <a href="#stats" @click="menuOpen = false">Datos</a>
        <a href="#letter" @click="menuOpen = false">Carta</a>
      </nav>
    </header>

    <router-view />

    <footer class="footer-bar">
      <div>
        <p>Hecho con amor por Álvaro 💙</p>
        <p>Desde 10 de abril hasta siempre</p>
      </div>
      <p class="footer-credit">Si llegaste hasta aquí… te amo más todavía.</p>
    </footer>
  </div>
</template>

<script setup>
import { ref } from 'vue';
const menuOpen = ref(false);
</script>
