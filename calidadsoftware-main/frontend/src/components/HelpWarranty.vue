<template>
  <section class="info-wrapper">
    <div class="container">
      <div class="info-card">
        <h1 class="info-title">Solicitar garantía</h1>
        <p class="lead">Ingresa tu número de orden y detalles del problema.</p>
        <form class="warranty-form" @submit.prevent>
          <div class="row">
            <div class="col">
              <label class="form-label">Número de orden</label>
              <input class="form-control" type="text" placeholder="DOC-000123" required />
            </div>
            <div class="col">
              <label class="form-label">Fecha de compra</label>
              <input class="form-control" type="date" required />
            </div>
          </div>
          <div class="mt-3">
            <label class="form-label">Descripción del problema</label>
            <textarea class="form-control" rows="4" placeholder="Cuéntanos qué sucede" required></textarea>
          </div>
          <div class="mt-3">
            <button class="btn btn-primary">Enviar solicitud</button>
          </div>
        </form>
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
.form-label { color: #0b2136; font-weight: 600; }
.form-control:focus { border-color: var(--dp-primary); box-shadow: 0 0 0 .2rem rgba(11,91,215,.15); }
.btn-primary { background: var(--dp-primary); border-color: var(--dp-primary); }
.btn-primary:hover { background: var(--dp-dark); border-color: var(--dp-dark); }
.row { display: flex; gap: 1rem; }
.col { flex: 1; }
</style>
