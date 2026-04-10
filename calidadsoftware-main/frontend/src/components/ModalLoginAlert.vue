<template>
  <div class="modal-backdrop">
    <div class="modal-container">
      <div class="modal-content p-4">
        <h3 class="text-center mb-3">Debes iniciar sesión</h3>
        <p class="text-center text-muted mb-4">Para realizar una reserva, primero debes iniciar sesión.</p>
        <button class="btn btn-primary-custom w-100 mb-2" @click="irALogin">Iniciar sesión</button>
        <button class="btn btn-outline-custom w-100" @click="$emit('cerrar')">Cancelar</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

const irALogin = () => {
  router.push('/logIn')
}
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.modal-container {
  background: #fff;
  border-radius: 8px;
  padding: 1.5rem;
  max-width: 400px;
  width: 100%;
}

/* Botón Iniciar Sesión */
.btn-primary-custom {
  background-color: #163A49;
  color: #fff;
  border: none;
  padding: 0.75rem;
  border-radius: 6px;
  transition: background-color 0.3s ease;
}

.btn-primary-custom:hover {
  background-color: #122F3B;
  color: white;
}

/* Botón Cancelar */
.btn-outline-custom {
  background-color: #fff;
  border: 1px solid #1A4456;
  color: #1A4456;
  padding: 0.75rem;
  border-radius: 6px;
  transition: background-color 0.3s ease;
}

.btn-outline-custom:hover {
  background-color: #f7f7f7;
}
</style>
