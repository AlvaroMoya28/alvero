<template>
  <div>
    <div class="background-logo"></div>

    <div id="mainCard" class="d-flex justify-content-center align-items-center min-vh-100">
      <div class="form-container" style="width: 100%; max-width: 500px">
        <h3 class="text-center mb-4">Panel de Administración</h3>

        <div class="d-grid gap-3 mb-4">
          <button class="btn btn-admin btn-tall" @click="registrarAdmin">Registrar Administrador</button>
          <button class="btn btn-admin btn-tall" @click="verLogs">Visualizar Logs</button>
          <button class="btn btn-admin btn-tall" @click="gestionarRoles">Gestión de Roles</button>
        </div>

        <div class="logout-container">
          <button class="btn btn-sm btn-logout" @click="cerrarSesion">Cerrar Sesión</button>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

function registrarAdmin () {
  router.push('/admin/panel/registerUser')
}

function verLogs () {
  alert('Mostrando logs del sistema...')
}

function gestionarRoles () {
  alert('Accediendo a gestión de roles...')
}

function cerrarSesion () {
  router.push('/login')
}
</script>

<style lang="scss" scoped>
.background-logo {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: url("@/assets/eventos.png");
  background-size: cover;
  background-position: center;
  filter: blur(10px);
  z-index: -1;
  opacity: 0.5;
}

#mainCard {
  position: relative;
  z-index: 1;
}

.form-container {
  background-color: white;
  border-radius: 10px;
  padding: 2rem;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  position: relative;
  min-height: 400px;
}

h3 {
  font-family: "Poppins", sans-serif;
  font-weight: bold;
  color: #1A4456;
}

.btn-tall {
  font-family: "Poppins", sans-serif;
  font-size: 1rem;
  font-weight: bold;
  border-radius: 5px;
  transition: all 0.3s ease;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid transparent;
}

/* Botón principal dorado que cambia a azul */
.btn-admin {
  background-color: #B88B4A;
  color: white;

  &:hover,
  &:active,
  &:focus {
    background-color: #1A4456;
    color: white;
  }

  &:focus {
    box-shadow: 0 0 0 0.2rem rgba(26, 68, 86, 0.4);
  }
}

.logout-container {
  position: absolute;
  bottom: 20px;
  left: 0;
  right: 0;
  display: flex;
  justify-content: center;
}

/* Botón de logout con colores invertidos */
.btn-logout {
  font-family: "Poppins", sans-serif;
  font-weight: bold;
  padding: 0.25rem 1.5rem;
  font-size: 0.875rem;
  background-color: #1A4456;
  color: white;
  border: 2px solid #1A4456;
  border-radius: 20px;

  &:hover {
    background-color: #B88B4A;
    border-color: #B88B4A;
  }
}

/* Eliminar outline al hacer focus */
button:focus {
  outline: none;
}
</style>
