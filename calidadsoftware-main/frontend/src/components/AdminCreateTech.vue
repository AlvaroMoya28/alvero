<template>
  <AdminLayout>
    <div class="content-container">
      <div class="container py-5">
        <div id="mainCard" class="d-flex justify-content-center align-items-center">
          <form @submit.prevent="createTech" class="form-container" style="width: 100%; max-width: 400px">
        <h3 class="text-center mb-4">Crear Técnico</h3>

        <div class="mb-3">
          <label class="form-label" for="inputIdUsuario">Nombre de usuario <span class="text-danger">*</span></label>
          <input id="inputIdUsuario" class="form-control" type="text" v-model="datos.idUsuario" required placeholder="Nombre de usuario" />
          <small class="text-danger" v-if="errores.detalles.idUsuario">{{ errores.detalles.idUsuario[0] }}</small>
        </div>

        <div class="mb-3">
          <label class="form-label" for="inputNombre">Nombre <span class="text-danger">*</span></label>
          <input id="inputNombre" class="form-control" type="text" v-model="datos.nombre" required placeholder="Nombre" />
          <small class="text-danger" v-if="errores.detalles.nombre">{{ errores.detalles.nombre[0] }}</small>
        </div>

        <div class="mb-3">
          <label class="form-label" for="inputApellido1">Primer apellido <span class="text-danger">*</span></label>
          <input id="inputApellido1" class="form-control" type="text" v-model="datos.apellido1" required placeholder="Primer apellido" />
          <small class="text-danger" v-if="errores.detalles.apellido1">{{ errores.detalles.apellido1[0] }}</small>
        </div>

        <div class="mb-3">
          <label class="form-label" for="inputApellido2">Segundo apellido <span class="text-danger">*</span></label>
          <input id="inputApellido2" class="form-control" type="text" v-model="datos.apellido2" required placeholder="Segundo apellido" />
          <small class="text-danger" v-if="errores.detalles.apellido2">{{ errores.detalles.apellido2[0] }}</small>
        </div>

        <div class="mb-3">
          <label class="form-label" for="inputEmail">Email <span class="text-danger">*</span></label>
          <input id="inputEmail" class="form-control" type="email" v-model="datos.email" required placeholder="correo@email.com" />
          <small class="text-danger" v-if="errores.detalles.email">{{ errores.detalles.email[0] }}</small>
        </div>

        <div class="mb-3">
          <label class="form-label" for="inputTelefono">Teléfono</label>
          <input id="inputTelefono" class="form-control" type="tel" v-model="datos.telefono" placeholder="Ej: 600123456" />
          <small class="text-danger" v-if="errores.detalles.telefono">{{ errores.detalles.telefono[0] }}</small>
        </div>

        <div class="mb-3">
          <label class="form-label" for="inputNacimiento">Fecha de nacimiento <span class="text-danger">*</span></label>
          <input id="inputNacimiento" class="form-control" type="date" v-model="datos.fechaNacimiento" required />
          <small class="text-danger" v-if="errores.detalles.fechaNacimiento">{{ errores.detalles.fechaNacimiento[0] }}</small>
        </div>

        <div class="mb-3 text-muted small">
          <span class="text-danger">*</span> Campos obligatorios
        </div>

        <div v-if="errores.mensaje" class="alert alert-danger mb-3">{{ errores.mensaje }}</div>

        <button type="submit" class="btn btn-success" style="width: 100%">Crear Técnico</button>
          </form>
        </div>
      </div>

      <div class="container" v-if="generatedPassword">
      <div class="mt-3 alert alert-info">
        <strong>Contraseña generada:</strong>
        <div class="mt-2 d-flex align-items-center gap-2">
          <code>{{ generatedPassword }}</code>
          <button class="btn btn-sm btn-outline-primary" @click="copyPassword">Copiar</button>
        </div>
        <p class="mt-2 mb-0"><small>Recuerda informar la contraseña al técnico y recomendar cambiarla en el primer login.</small></p>
      </div>
      </div>
    </div>
  </AdminLayout>
</template>

<script setup>
import { reactive, ref } from 'vue'
import AdminLayout from '@/components/AdminLayout.vue'
// import { useRouter } from 'vue-router'
import { usuarioService } from '@/services/UserService'

// const router = useRouter()

const datos = reactive({
  idUsuario: '',
  nombre: '',
  apellido1: '',
  apellido2: '',
  email: '',
  telefono: '',
  fechaNacimiento: ''
})

const errores = reactive({ mensaje: '', detalles: {} })
const generatedPassword = ref('')

async function createTech () {
  try {
    errores.mensaje = ''
    errores.detalles = {}

    // Validaciones frontend
    if (!datos.apellido2) throw new Error('El segundo apellido es obligatorio para generar la contraseña')
    if (!datos.fechaNacimiento) throw new Error('La fecha de nacimiento es obligatoria')

    // Validar edad >= 18
    const fechaNacimiento = new Date(datos.fechaNacimiento)
    const hoy = new Date()
    let edad = hoy.getFullYear() - fechaNacimiento.getFullYear()
    const mes = hoy.getMonth() - fechaNacimiento.getMonth()
    if (mes < 0 || (mes === 0 && hoy.getDate() < fechaNacimiento.getDate())) edad--
    if (edad < 18) throw new Error('El técnico debe ser mayor de 18 años')

    // Generar contraseña
    const year = fechaNacimiento.getFullYear()
    const senha = datos.apellido2.toLowerCase() + year
    generatedPassword.value = senha

    const payload = {
      idUsuario: datos.idUsuario,
      nombre: datos.nombre,
      apellido1: datos.apellido1,
      apellido2: datos.apellido2 || null,
      email: datos.email,
      telefono: datos.telefono || null,
      fechaNacimiento: datos.fechaNacimiento,
      contrasena: senha,
      confirmarContrasena: senha,
      rol: 'TECNICO'
    }

    const response = await usuarioService.registrarTecnico(payload)
    console.log('Técnico creado', response.data)
    // Mostrar contraseña (se queda en variable)
    errores.mensaje = ''
  } catch (error) {
    if (error.response?.data?.errors) {
      errores.detalles = error.response.data.errors
      errores.mensaje = 'Por favor corrige los errores en el formulario'
    } else if (error.response?.data?.message) {
      errores.mensaje = error.response.data.message
    } else {
      errores.mensaje = error.message
    }
    setTimeout(() => {
      const firstError = document.querySelector('.text-danger')
      if (firstError) firstError.scrollIntoView({ behavior: 'smooth', block: 'center' })
    }, 100)
  }
}

function copyPassword () {
  try {
    navigator.clipboard.writeText(generatedPassword.value)
  } catch (e) {
    console.warn('No se pudo copiar')
  }
}
</script>

<style lang="scss" scoped>
.form-control:required:invalid { border-left: 3px solid #dc3545; }
/* Sin fondo borroso; mantener limpio estilo DoctorPC */
#mainCard { position: relative; z-index: 1; padding: 0px; }
h3 { font-family: 'Poppins', sans-serif; font-weight: bold; color: var(--dp-primary); }
.form-container { background-color: white; width: 100%; max-width: 400px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); border-radius: 10px; padding: 2rem; margin: 2rem auto; }
.form-label { font-family: 'Poppins', sans-serif; margin-bottom: 0.5rem; display: block; color: #0b2136; }
.form-control { font-family: 'Noto Sans', sans-serif; font-size: 1rem; border-radius: 5px; border: 1px solid #ced4da; padding: 0.375rem 0.75rem; width: 100%; &:focus { border-color: var(--dp-primary); outline: 0; box-shadow: 0 0 0 0.2rem rgba(11,91,215,0.25); } }
.form-select { @extend .form-control; appearance: none; background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%23052e5d' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3cpolyline points='6 9 12 15 18 9'%3e%3c/polyline%3e%3c/svg%3e"); background-repeat: no-repeat; background-position: right 0.75rem center; background-size: 1rem; color: var(--dp-dark); }
p { font-family: 'Noto Sans', sans-serif; font-size: 1rem; }
.btn-success { background-color: var(--dp-accent); color: white; width: 100%; border: none; padding: 0.75rem; font-family: 'Poppins', sans-serif; font-weight: bold; border-radius: 5px; transition: all 0.3s ease; &:hover { background-color: var(--dp-dark); } &:focus { outline: none; box-shadow: 0 0 0 0.2rem rgba(5,46,93,0.25); } }
.text-primary { color: var(--dp-primary); text-decoration: none; font-weight: 500; &:hover { text-decoration: underline; color: var(--dp-dark); } }
</style>
