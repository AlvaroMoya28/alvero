<template>
  <section class="info-wrapper">
    <div class="container">
      <div class="info-card">
        <h1 class="info-title">Diagnóstico</h1>
        <p class="lead">Evaluación completa de tu equipo para identificar fallas.</p>
        <ul class="info-list">
          <li>Pruebas de hardware y software.</li>
          <li>Informe técnico con tiempos estimados.</li>
          <li>Recomendaciones de mejora y mantenimiento.</li>
        </ul>
      </div>
    </div>
  </section>
</template>

<script setup>
</script>

<style scoped>
.info-wrapper { padding: 2rem 1rem; background: var(--dp-bg); }
.container { max-width: 980px; margin: 0 auto; }
.info-card { background: #fff; border-radius: 12px; padding: 1.5rem; box-shadow: 0 6px 16px rgba(3,33,63,0.06); }
.info-title { color: var(--dp-primary); margin-bottom: .75rem; }
.lead { color: #0b2136; opacity: .85; margin-bottom: 1rem; }
.info-list { margin: 0; padding-left: 1.1rem; color: #0b2136; }
.info-list li { margin: .35rem 0; }
</style>
