<template>
  <div class="loading-overlay" role="status" aria-live="polite">
    <div class="spinner"></div>
    <div class="label">Procesando…</div>
  </div>
</template>

<script>
export default {
  name: 'LoadingOverlay'
}
</script>

<style scoped>
.loading-overlay {
  position: fixed;
  inset: 0;
  background: rgba(6, 20, 29, 0.55);
  backdrop-filter: blur(2px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}
.spinner {
  width: 54px;
  height: 54px;
  border: 4px solid rgba(255, 255, 255, 0.25);
  border-top-color: #11b6e6;
  border-radius: 50%;
  animation: spin 0.9s linear infinite;
}
.label {
  margin-top: 14px;
  color: #e6f7ff;
  font-weight: 600;
  text-shadow: 0 1px 2px rgba(0,0,0,0.35);
}
@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
