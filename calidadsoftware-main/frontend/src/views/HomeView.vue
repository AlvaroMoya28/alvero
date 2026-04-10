<template>
  <div class="home-view">
    <!-- HERO -->
    <section class="landing-hero">
      <div class="container hero-grid">
        <div class="hero-copy">
          <span class="badge-soft">Soporte integral · 24h</span>
          <h1>
            Pon tu PC en manos de expertos
            <span class="highlight">rápido y sin complicaciones</span>
          </h1>
          <p class="subtitle">
            Diagnóstico sin costo, retiro a domicilio y garantía real. Reparamos
            hardware y software con repuestos certificados.
          </p>
          <div class="cta-row">
            <router-link to="/reservar" class="btn btn-cta btn-lg">Reservar cita</router-link>
            <router-link to="/servicios" class="btn btn-outline-primary btn-lg">Ver servicios</router-link>
          </div>
          <ul class="trust-list">
            <li>
              <i class="fa-solid fa-star"></i>
              4.9/5 satisfacción
            </li>
            <li>
              <i class="fa-solid fa-clock-rotate-left"></i>
              24h promedio
            </li>
            <li>
              <i class="fa-solid fa-shield-check"></i>
              90 días de garantía
            </li>
          </ul>
        </div>
        <div class="hero-visual">
          <div class="device-card">
            <div class="screen-glow"></div>
            <div class="screen"></div>
            <div class="base"></div>
          </div>
        </div>
      </div>
      <div class="bg-blob blob-1"></div>
      <div class="bg-blob blob-2"></div>
    </section>

    <!-- FEATURES GRID -->
    <section class="features container">
      <div class="grid">
        <div class="feature-card">
          <div class="icon"><i class="fa-solid fa-screwdriver-wrench"></i></div>
          <h3>Reparación experta</h3>
          <p>Especialistas en laptops y desktops. Piezas originales y pruebas de calidad.</p>
        </div>
        <div class="feature-card">
          <div class="icon"><i class="fa-solid fa-house-circle-check"></i></div>
          <h3>Retiro a domicilio</h3>
          <p>Vamos por tu equipo y lo devolvemos reparado. Cobertura en tu zona.</p>
        </div>
        <div class="feature-card">
          <div class="icon"><i class="fa-solid fa-coins"></i></div>
          <h3>Diagnóstico gratis</h3>
          <p>Evalúamos el problema sin costo. Presupuesto claro antes de avanzar.</p>
        </div>
        <div class="feature-card">
          <div class="icon"><i class="fa-solid fa-shield-halved"></i></div>
          <h3>Garantía real</h3>
          <p>Respaldo por 90 días y soporte post-servicio para tu tranquilidad.</p>
        </div>
      </div>
    </section>

    <!-- STEPS -->
    <section class="steps container">
      <h2>¿Cómo funciona?</h2>
      <div class="steps-row">
        <div class="step">
          <span class="step-num">1</span>
          <h4>Agenda en línea</h4>
          <p>Elige fecha y cuéntanos el problema.</p>
        </div>
        <div class="step">
          <span class="step-num">2</span>
          <h4>Diagnóstico</h4>
          <p>Confirmamos la falla y enviamos presupuesto.</p>
        </div>
        <div class="step">
          <span class="step-num">3</span>
          <h4>Reparación y entrega</h4>
          <p>Servicio ágil, pruebas y garantía.</p>
        </div>
      </div>
    </section>

    <!-- LOGOS -->
    <LogosMarquee />

    <!-- CTA STRIP -->
    <section class="cta-strip">
      <div class="container cta-inner">
        <div>
          <h3>¿Listo para recuperar tu equipo?</h3>
          <p>Agenda ahora y recibe prioridad de atención.</p>
        </div>
        <router-link to="/reservar" class="btn btn-cta btn-lg">Agendar ahora</router-link>
      </div>
    </section>

    <!-- Testimonios -->
    <Testimonials />

    <!-- Opcional: carrusel de marcas si existe -->
    <CarouselBrand v-if="$options.components && $options.components.CarouselBrand" />
  </div>
</template>

<script setup>
import CarouselBrand from '@/components/CarruselMarca.vue'
import Testimonials from '@/components/TestimonialsComp.vue'
import LogosMarquee from '@/components/LogosMarquee.vue'
</script>

<style scoped>
.home-view { display: flex; flex-direction: column; gap: 2.5rem; }

/* HERO */
.landing-hero {
  position: relative;
  padding: 4.5rem 0 3.5rem;
  background: linear-gradient(180deg, rgba(11,91,215,0.08), rgba(3,33,63,0.06));
  overflow: hidden;
}
.hero-grid { display: grid; grid-template-columns: 1.1fr .9fr; gap: 2rem; align-items: center; }
.hero-copy h1 { color: var(--dp-darkest); font-weight: 800; line-height: 1.1; margin: .5rem 0 1rem; }
.hero-copy .highlight { display: block; color: var(--dp-primary); }
.hero-copy .subtitle { color: #0b2136; opacity: .85; max-width: 52ch; }
.badge-soft { display: inline-block; background: rgba(11,91,215,.12); color: var(--dp-primary); padding: .35rem .6rem; border-radius: 999px; font-weight: 600; font-size: .85rem; }
.cta-row { display: flex; gap: .75rem; margin-top: 1rem; }
.trust-list { display: flex; gap: 1.25rem; margin-top: 1.25rem; padding: 0; list-style: none; color: #0b2136; opacity: .85; }
.trust-list i { color: var(--dp-accent); margin-right: .35rem; }

.hero-visual { display: flex; justify-content: center; }
.device-card { position: relative; width: 360px; height: 260px; background: #0e2d57; border-radius: 16px; box-shadow: 0 20px 40px rgba(3,33,63,.25), 0 8px 20px rgba(3,33,63,.15); overflow: hidden; }
.screen-glow { position: absolute; inset: -20%; background: radial-gradient( circle at 30% 30%, rgba(0,200,83,0.35), transparent 45%), radial-gradient( circle at 75% 20%, rgba(11,91,215,0.35), transparent 40%);}
.screen { position: absolute; inset: 18px 18px 60px; background: linear-gradient(180deg, #0b5bd7 0%, #073a8d 100%); border-radius: 10px; box-shadow: inset 0 0 24px rgba(255,255,255,.08); }
.base { position: absolute; left: 18px; right: 18px; bottom: 18px; height: 28px; border-radius: 8px; background: #091f3c; box-shadow: inset 0 1px 0 rgba(255,255,255,0.06); }

.bg-blob { position: absolute; border-radius: 999px; filter: blur(40px); opacity: .35; }
.blob-1 { width: 360px; height: 360px; background: var(--dp-primary); top: -60px; right: -80px; animation: float 9s ease-in-out infinite; }
.blob-2 { width: 300px; height: 300px; background: var(--dp-accent); bottom: -80px; left: -60px; opacity: .28; animation: float 11s ease-in-out infinite reverse; }

@keyframes float { 0%,100% { transform: translateY(0) } 50% { transform: translateY(14px) } }

/* FEATURES */
.features .grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 1rem; }
.feature-card { background: rgba(255,255,255,0.9); border-radius: 14px; padding: 1.1rem; box-shadow: 0 10px 28px rgba(3,33,63,0.08); border: 1px solid rgba(3,33,63,0.06); transition: transform .18s ease, box-shadow .18s ease, border-color .18s ease; }
.feature-card:hover { transform: translateY(-3px); box-shadow: 0 18px 36px rgba(3,33,63,0.12); }
.feature-card .icon { width: 44px; height: 44px; border-radius: 10px; display: grid; place-items: center; background: rgba(11,91,215,.12); color: var(--dp-primary); margin-bottom: .5rem; }
.feature-card h3 { font-size: 1.05rem; margin: .25rem 0 .35rem; color: var(--dp-darkest); }
.feature-card p { color: #0b2136; opacity: .85; margin: 0; }

/* STEPS */
.steps h2 { color: var(--dp-darkest); margin-bottom: .8rem; }
.steps-row { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; }
.step { background: #fff; border: 1px solid rgba(3,33,63,0.06); border-radius: 12px; padding: 1rem; box-shadow: 0 8px 20px rgba(3,33,63,0.06); }
.step-num { width: 28px; height: 28px; display: inline-grid; place-items: center; background: var(--dp-primary); color: #fff; border-radius: 999px; font-weight: 700; margin-bottom: .4rem; }
.step h4 { margin: 0 0 .25rem; color: var(--dp-darkest); }
.step p { margin: 0; color: #0b2136; opacity: .85; }

/* CTA STRIP */
.cta-strip { background: var(--dp-darkest); color: #fff; padding: 1.6rem 0; }
.cta-inner { display: flex; align-items: center; justify-content: space-between; gap: 1rem; }
.cta-inner h3 { margin: 0 0 .2rem; }
.cta-inner p { margin: 0; opacity: .9; }

/* Utilities */
.container { max-width: 1140px; margin: 0 auto; padding: 0 1rem; }

@media (max-width: 992px) {
  .hero-grid { grid-template-columns: 1fr; }
  .hero-visual { order: -1; margin-bottom: 1rem; }
  .features .grid { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 576px) {
  .features .grid { grid-template-columns: 1fr; }
  .cta-inner { flex-direction: column; text-align: center; }
}
</style>
