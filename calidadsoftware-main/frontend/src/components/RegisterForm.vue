<template>
  <div>
    <div class="background-logo"></div>

    <div class="container py-5">
      <div id="mainCard" class="d-flex justify-content-center align-items-center">
        <form @submit.prevent="Register" class="form-container" style="width: 100%; max-width: 400px">
          <h3 class="text-center mb-4">Registro</h3>

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
              placeholder="Segundo apellido" />
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
            <label class="form-label" for="inputTelefono"> Teléfono </label>
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
            <label class="form-label" for="inputContrasena">
              Contraseña <span class="text-danger">*</span>
            </label>
            <input class="form-control" type="password" id="inputContrasena" v-model="datos.contrasena" required
              placeholder="Mínimo 8 caracteres" />
            <small class="text-danger" v-if="errores.detalles.Contrasena">
              {{ errores.detalles.Contrasena.join(', ') }}
            </small>
            <small class="text-danger" v-else-if="errores.detalles.contrasena">
              {{ errores.detalles.contrasena.join(', ') }}
            </small>
            <small class="text-muted">
              Requisitos: 8+ caracteres, mayúsculas, minúsculas, números y caracteres especiales
            </small>
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

          <!-- Mensaje sobre campos obligatorios -->
          <div class="mb-3 text-muted small">
            <span class="text-danger">*</span> Campos obligatorios
          </div>

          <div v-if="errores.mensaje" class="alert alert-danger mb-3">
            {{ errores.mensaje }}
          </div>

          <div class="d-flex justify-content-between">
            <p class="mb-4">
              Si ya tienes cuenta
              <RouterLink :to="{ name: 'LogIn' }" class="form-action-link">
                inicia sesión aquí
              </RouterLink>
            </p>
          </div>

          <button type="submit" class="btn btn-success" style="width: 100%">
            Registrarse
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { usuarioService } from '@/services/UserService' // Asegúrate que la ruta sea correcta

const router = useRouter()

const datos = reactive({
  idUsuario: '',
  nombre: '',
  apellido1: '',
  apellido2: '',
  email: '',
  telefono: '',
  fechaNacimiento: '',
  contrasena: '',
  confirmarContrasena: '',
  rol: 'CLIENTE' // Rol por defecto para el registro desde este formulario
})

const errores = reactive({
  mensaje: '',
  detalles: {} // Para errores específicos de campos si el backend los devuelve
})

async function Register () {
  try {
    errores.mensaje = ''
    errores.detalles = {}

    // Validación básica de contraseña en frontend
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/
    if (!passwordRegex.test(datos.contrasena)) {
      errores.mensaje = 'La contraseña no cumple con los requisitos'
      errores.detalles.contrasena = [
        'Debe tener al menos 8 caracteres',
        'Debe incluir mayúsculas y minúsculas',
        'Debe incluir números',
        'Debe incluir caracteres especiales'
      ]
      return
    }

    if (errores.detalles.contrasena) {
      delete errores.detalles.contrasena
    }

    if (datos.contrasena !== datos.confirmarContrasena) {
      errores.mensaje = 'Las contraseñas no coinciden'
      errores.detalles.confirmarContrasena = ['Las contraseñas no coinciden']
      return
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
      errores.mensaje = 'Debes tener al menos 18 años para registrarte'
      errores.detalles.fechaNacimiento = ['Debes tener al menos 18 años']
      return
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

    console.log('Enviando datos de registro: ', usuarioData)
    const response = await usuarioService.registrarUsuario(usuarioData) // Asegúrate que este método exista
    console.log('Usuario registrado:', response.data)

    router.push({ name: 'LogIn' })
  } catch (error) {
    console.error('Error en registro:', error)

    if (error.response && error.response.data) {
      // Backend validation errors
      if (error.response.data.errors) {
        errores.detalles = error.response.data.errors

        // Special handling for password errors
        if (error.response.data.errors.Contrasena) {
          errores.mensaje = 'Error en la contraseña: ' +
            error.response.data.errors.Contrasena.join(', ')
        } else {
          errores.mensaje = 'Error de validación'
        }
      } else if (error.response.data.message) {
        errores.mensaje = error.response.data.message
      }
    } else if (error.message) {
      errores.mensaje = error.message
    } else {
      errores.mensaje = 'Ocurrió un error inesperado durante el registro.'
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

.background-logo {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: url("@/assets/eventos.png"); // Asegúrate que la ruta sea correcta
  background-size: cover;
  background-position: center;
  filter: blur(10px);
  z-index: -1;
  opacity: 0.5;
}

#mainCard {
  position: relative;
  z-index: 1;
  // padding: 1px; // No es necesario si container tiene py-5
}

h3 {
  font-family: "Poppins", sans-serif;
  font-weight: bold;
  color: #000000;
}

.text-center {
  color: #143442;
}

.form-label {
  color: #143442;
}

.form-control[data-v-6626deb7] {
  font-family: "Noto Sans", sans-serif;
  font-size: 1rem;
  border-radius: 5px;
  border: 1px solid #ced4da;
  padding: 0.375rem 0.75rem;
  width: 100%;
  color: #143442;
  box-sizing: border-box;
}

.form-container {
  background-color: white;
  width: 100%;
  max-width: 400px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  border-radius: 10px;
  padding: 2rem;
  margin: 2rem auto; // Centra el formulario
}

.form-label {
  font-family: "Poppins", sans-serif;
  display: block; // Asegura que ocupe su línea y el margen inferior funcione
  margin-bottom: 0.5rem; // Espacio entre label e input
  text-align: left;
}

.form-control {
  font-family: 'Noto Sans', sans-serif; // Corregido el nombre de la fuente
  font-size: 1rem;
  border-radius: 5px;
  border: 1px solid #ced4da; // Color de borde estándar
  padding: 0.375rem 0.75rem; // Padding estándar de Bootstrap
  width: 100%;
  box-sizing: border-box; // Importante para que el padding y borde no aumenten el tamaño

  &:focus {
    border-color: #b88b4a; // Color de borde al enfocar
    outline: 0; // Quita el outline por defecto
    box-shadow: 0 0 0 0.2rem rgba(184, 139, 74, 0.25); // Sombra de enfoque
  }
}

// Removidas las clases .form-check ya que no se usan en este template

p {
  font-family: 'Noto Sans', sans-serif; // Corregido
  font-size: 1rem;
}

button {
  // Estilo base para botones si es necesario, pero .btn-success es más específico
  font-family: "Poppins", sans-serif;
  font-size: 1rem;
  font-weight: bold;
  border-radius: 5px;
  transition: all 0.3s ease; // Transición suave
}

.btn-success {
  background-color: #B88B4A;
  color: white;
  width: 100%;
  border: none;
  padding: 0.75rem; // Padding para el botón

  &:hover {
    background-color: #A27035;
  }

  &:focus {
    outline: none;
    // Ajustado el box-shadow para que coincida con el color del botón
    box-shadow: 0 0 0 0.2rem rgba(184, 139, 74, 0.4);
  }
}

.form-action-link {
  color: #b88b4a;
  text-decoration: underline;
  /* Siempre subrayado */
  font-weight: 500;
  /* Opcional: darle un poco más de peso */

  &:hover {
    color: #a27035;
    /* Color hover consistente con .btn-success */
  }
}

.alert.alert-danger {
  text-align: left; // Para que el mensaje de error se alinee a la izquierda
}
</style>
