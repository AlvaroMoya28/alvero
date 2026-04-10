<template>
  <section class="testimonials container">
    <h2 class="title">Lo que dicen nuestros clientes</h2>
    <div class="cards">
      <article class="t-card">
        <div class="stars">
          <i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i>
        </div>
        <p>“Me salvaron el semestre. Entregaron mi laptop en un día con todo funcionando.”</p>
        <div class="meta">
          <span class="avatar">EA</span>
          <div>
            <strong>Esteban A.</strong>
            <small>Universitario</small>
          </div>
        </div>
      </article>
      <article class="t-card">
        <div class="stars">
          <i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i>
        </div>
        <p>“Diagnóstico claro y garantía real. Muy profesionales, súper recomendados.”</p>
        <div class="meta">
          <span class="avatar">LM</span>
          <div>
            <strong>Laura M.</strong>
            <small>Diseñadora</small>
          </div>
        </div>
      </article>
      <article class="t-card">
        <div class="stars">
          <i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i><i class="fa-solid fa-star"></i>
        </div>
        <p>“Retiro a domicilio y entrega puntual. Excelente calidad-precio.”</p>
        <div class="meta">
          <span class="avatar">JR</span>
          <div>
            <strong>José R.</strong>
            <small>Emprendedor</small>
          </div>
        </div>
      </article>
    </div>
  </section>
</template>

<script setup>
</script>

<style scoped>
.title { color: var(--dp-darkest); margin-bottom: 1rem; }
.cards { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; }
.t-card { background: #fff; border: 1px solid rgba(3,33,63,0.06); border-radius: 12px; padding: 1rem; box-shadow: 0 8px 20px rgba(3,33,63,0.06); }
.stars { color: var(--dp-accent); margin-bottom: .5rem; }
.t-card p { color: #0b2136; opacity: .9; margin: 0 0 .75rem; }
.meta { display: flex; align-items: center; gap: .6rem; }
.avatar { width: 36px; height: 36px; border-radius: 999px; display: grid; place-items: center; background: rgba(11,91,215,.12); color: var(--dp-primary); font-weight: 700; }

@media (max-width: 992px) {
  .cards { grid-template-columns: 1fr; }
}
</style>
