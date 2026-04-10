<template>
  <!-- TEST: Layout removido para que los tests unitarios funcionen -->
  <div class="container py-5">
    <div id="mainCard" class="d-flex justify-content-center align-items-center">
      <form @submit.prevent="registerAdmin" class="form-container" style="width: 100%; max-width: 400px">
          <h3 class="text-center mb-4">Registro de Administrador</h3>
          <div class="mb-3">
            <label class="form-label" for="inputIdUsuario">
              Nombre de usuario <span class="text-danger">*</span>
            </label>
            <input class="form-control" type="text" id="inputIdUsuario" v-model="datos.idUsuario" required
              placeholder="Nombre de usuario" />
            <small class="text-danger" v-if="errores.detalles.idUsuario">
              {{ errores.detalles.idUsuario[0] }}
            </small>
          </div>

          <div class="mb-3">
            <label class="form-label" for="inputNombre">
              Nombre <span class="text-danger">*</span>
            </label>
            <input class="form-control" type="text" id="inputNombre" v-model="datos.nombre" required
              placeholder="Nombre" />
            <small class="text-danger" v-if="errores.detalles.nombre">
              {{ errores.detalles.nombre[0] }}
            </small>
          </div>

          <div class="mb-3">
            <label class="form-label" for="inputApellido1">
              Primer apellido <span class="text-danger">*</span>
            </label>
            <input class="form-control" type="text" id="inputApellido1" v-model="datos.apellido1" required
              placeholder="Primer apellido" />
            <small class="text-danger" v-if="errores.detalles.apellido1">
              {{ errores.detalles.apellido1[0] }}
            </small>
          </div>

          <div class="mb-3">
            <label class="form-label" for="inputApellido2">Segundo apellido</label>
            <input class="form-control" type="text" id="inputApellido2" v-model="datos.apellido2"
              placeholder="Segundo apellido (opcional)" />
            <small class="text-danger" v-if="errores.detalles.apellido2">
              {{ errores.detalles.apellido2[0] }}
            </small>
          </div>

          <div class="mb-3">
            <label class="form-label" for="inputEmail">
              Email <span class="text-danger">*</span>
            </label>
            <input class="form-control" type="email" id="inputEmail" v-model="datos.email" required
              placeholder="correo@email.com" />
            <small class="text-danger" v-if="errores.detalles.email">
              {{ errores.detalles.email[0] }}
            </small>
          </div>

          <div class="mb-3">
            <label class="form-label" for="inputContrasena">
              Contraseña <span class="text-danger">*</span>
            </label>
            <input class="form-control" type="password" id="inputContrasena" v-model="datos.contrasena" required
              placeholder="Mínimo 8 caracteres" />
            <small class="text-danger" v-if="errores.detalles.contrasena">
              {{ errores.detalles.contrasena[0] }}
            </small>
            <small class="text-muted">Debe contener mayúsculas, minúsculas, números y caracteres especiales</small>
          </div>

          <div class="mb-3">
            <label class="form-label" for="inputConfirmarContrasena">
              Confirmar Contraseña <span class="text-danger">*</span>
            </label>
            <input class="form-control" type="password" id="inputConfirmarContrasena"
              v-model="datos.confirmarContrasena" required placeholder="Repite tu contraseña" />
            <small class="text-danger" v-if="errores.detalles.confirmarContrasena">
              {{ errores.detalles.confirmarContrasena[0] }}
            </small>
          </div>

          <div class="mb-3">
            <label class="form-label" for="inputTelefono">Teléfono</label>
            <input class="form-control" type="tel" id="inputTelefono" v-model="datos.telefono"
              placeholder="Ej: 600123456" />
            <small class="text-danger" v-if="errores.detalles.telefono">
              {{ errores.detalles.telefono[0] }}
            </small>
          </div>

          <div class="mb-3">
            <label class="form-label" for="inputNacimiento">
              Fecha de nacimiento <span class="text-danger">*</span>
            </label>
            <input class="form-control" type="date" id="inputNacimiento" v-model="datos.fechaNacimiento" required />
            <small class="text-danger" v-if="errores.detalles.fechaNacimiento">
              {{ errores.detalles.fechaNacimiento[0] }}
            </small>
          </div>

          <div class="mb-3">
            <label class="form-label" for="inputRol">
              Tipo de Usuario <span class="text-danger">*</span>
            </label>
            <select class="form-select" id="inputRol" v-model="datos.rol" required>
              <option value="" disabled>Seleccione un rol</option>
              <option value="ADMINISTRADOR">Administrador</option>
              <option value="CLIENTE">Cliente</option>
            </select>
            <small class="text-danger" v-if="errores.detalles.rol">
              {{ errores.detalles.rol[0] }}
            </small>
          </div>

          <!-- Mensaje sobre campos obligatorios -->
          <div class="mb-3 text-muted small">
            <span class="text-danger">*</span> Campos obligatorios
          </div>

          <div v-if="errores.mensaje" class="alert alert-danger mb-3">
            {{ errores.mensaje }}
          </div>

          <button type="submit" class="btn btn-success" style="width: 100%">
            Registrar Administrador
          </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { usuarioService } from '@/services/UserService'

const router = useRouter()

const datos = reactive({
  idUsuario: '',
  nombre: '',
  apellido1: '',
  apellido2: '',
  email: '',
  contrasena: '',
  confirmarContrasena: '',
  telefono: '',
  fechaNacimiento: '',
  rol: ''
})

const errores = reactive({
  mensaje: '',
  detalles: {}
})

async function registerAdmin () {
  try {
    errores.mensaje = ''
    errores.detalles = {}

    if (datos.contrasena !== datos.confirmarContrasena) {
      throw new Error('Las contraseñas no coinciden')
    }

    // Validar fecha de nacimiento
    const fechaNacimiento = new Date(datos.fechaNacimiento)
    const hoy = new Date()
    let edad = hoy.getFullYear() - fechaNacimiento.getFullYear()
    const mes = hoy.getMonth() - fechaNacimiento.getMonth()

    if (mes < 0 || (mes === 0 && hoy.getDate() < fechaNacimiento.getDate())) {
      edad--
    }

    if (edad < 18) {
      throw new Error('Debes tener al menos 18 años para registrarte')
    }

    const usuarioData = {
      idUsuario: datos.idUsuario,
      nombre: datos.nombre,
      apellido1: datos.apellido1,
      apellido2: datos.apellido2 || null,
      email: datos.email,
      telefono: datos.telefono || null,
      fechaNacimiento: datos.fechaNacimiento,
      contrasena: datos.contrasena,
      confirmarContrasena: datos.confirmarContrasena,
      rol: datos.rol
    }

    console.log('Request:', usuarioData)

    const response = await usuarioService.registrarUsuario(usuarioData)
    console.log('Usuario registrado:', response.data)

    router.push({ name: 'Panel' })
  } catch (error) {
    if (error.response?.data?.errors) {
      // Errores de validación del backend
      errores.detalles = error.response.data.errors
      errores.mensaje = 'Por favor corrige los errores en el formulario'
    } else if (error.response?.data?.message) {
      // Mensaje de error general del backend
      errores.mensaje = error.response.data.message
    } else {
      // Error de red o validación del frontend
      errores.mensaje = error.message
    }

    // Scroll al primer error
    setTimeout(() => {
      const firstError = document.querySelector('.text-danger')
      if (firstError) {
        firstError.scrollIntoView({ behavior: 'smooth', block: 'center' })
      }
    }, 100)
  }
}
</script>

<style lang="scss" scoped>
.form-control:required:invalid {
  border-left: 3px solid #dc3545;
}

/* Limpieza de fondo para mantener estilo limpio tipo homepage */

#mainCard {
  position: relative;
  z-index: 1;
  padding: 0px;
}

h3 {
  font-family: 'Poppins', sans-serif;
  font-weight: bold;
  color: var(--dp-primary);
}

.form-container {
  background-color: white;
  width: 100%;
  max-width: 400px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  border-radius: 10px;
  padding: 2rem;
  margin: 2rem auto;
}

.form-label {
  font-family: 'Poppins', sans-serif;
  margin-bottom: 0.5rem;
  display: block;
}

.form-control {
  font-family: 'Noto Sans', sans-serif;
  font-size: 1rem;
  border-radius: 5px;
  border: 1px solid #ced4da;
  padding: 0.375rem 0.75rem;
  width: 100%;

  &:focus {
    border-color: var(--dp-primary);
    outline: 0;
    box-shadow: 0 0 0 0.2rem rgba(11, 91, 215, 0.25);
  }
}

.text-center {
  color: var(--dp-dark);
}

.form-label {
  color: #0b2136;
}

.form-control[data-v-3a08e23e] {
  font-family: "Noto Sans", sans-serif;
  font-size: 1rem;
  border-radius: 5px;
  border: 1px solid #ced4da;
  padding: 0.375rem 0.75rem;
  width: 100%;
  color: #143442;
  box-sizing: border-box;
}

.form-select {
  @extend .form-control;
  appearance: none;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%231A4456' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3cpolyline points='6 9 12 15 18 9'%3e%3c/polyline%3e%3c/svg%3e");
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  background-size: 1rem;
  color: var(--dp-dark);

  option {
    padding: 0.5rem;
  }

  option:checked {
    background-color: var(--dp-primary);
    color: white;
  }
}

p {
  font-family: 'Noto Sans', sans-serif;
  font-size: 1rem;
}

.btn-success {
  background-color: var(--dp-accent);
  color: white;
  width: 100%;
  border: none;
  padding: 0.75rem;
  font-family: 'Poppins', sans-serif;
  font-weight: bold;
  border-radius: 5px;
  transition: all 0.3s ease;

  &:hover {
    background-color: var(--dp-dark);
  }

  &:focus {
    outline: none;
    box-shadow: 0 0 0 0.2rem rgba(5, 46, 93, 0.25);
  }
}

.text-primary {
  color: var(--dp-primary);
  text-decoration: none;
  font-weight: 500;

  &:hover {
    text-decoration: underline;
    color: var(--dp-dark);
  }
}
</style>
