<template>
  <section class="marquee-wrap">
    <div class="container">
      <div class="marquee" :style="{ '--speed': speed + 's' }">
        <div class="track">
          <span class="pill">Dell</span>
          <span class="pill">HP</span>
          <span class="pill">Lenovo</span>
          <span class="pill">Acer</span>
          <span class="pill">ASUS</span>
          <span class="pill">MSI</span>
          <span class="pill">Apple</span>
          <span class="pill">Samsung</span>
          <!-- duplicated for seamless loop -->
          <span class="pill">Dell</span>
          <span class="pill">HP</span>
          <span class="pill">Lenovo</span>
          <span class="pill">Acer</span>
          <span class="pill">ASUS</span>
          <span class="pill">MSI</span>
          <span class="pill">Apple</span>
          <span class="pill">Samsung</span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
const speed = 22
</script>

<style scoped>
.marquee-wrap { padding: .5rem 0 0.2rem; }
.marquee { overflow: hidden; position: relative; mask-image: linear-gradient(90deg, transparent, #000 10%, #000 90%, transparent); }
.track { display: inline-flex; gap: .6rem; white-space: nowrap; animation: scroll var(--speed, 20s) linear infinite; }
.pill { background: rgba(11,91,215,.12); color: var(--dp-primary); border: 1px solid rgba(11,91,215,.2); padding: .45rem .75rem; border-radius: 999px; font-weight: 600; font-size: .95rem; }

@keyframes scroll { from { transform: translateX(0) } to { transform: translateX(-50%) } }
</style>
