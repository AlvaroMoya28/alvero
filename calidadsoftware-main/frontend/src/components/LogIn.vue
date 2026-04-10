<template>
  <div class="login-view container py-5">
    <section class="hero-doctorpc py-4">
      <div class="row justify-content-center">
        <div class="col-md-7 col-lg-5">
          <div class="card shadow-sm login-card">
            <div class="card-body p-4">
              <h3 class="text-center mb-4 text-dp-primary">Inicio de sesión</h3>

              <form @submit.prevent="handleLogIn">
                <div class="mb-3">
                  <label class="form-label" for="inputId">Nombre de usuario</label>
                  <input class="form-control" type="text" id="inputId" v-model="datos.id" required
                    placeholder="Ingresa tu nombre de usuario" />
                </div>

                <div class="mb-3">
                  <label class="form-label" for="inputPassword">Contraseña</label>
                  <input class="form-control" type="password" id="inputPassword" v-model="datos.password" required
                    placeholder="Ingresa tu contraseña" />

                  <div v-if="errores.mensaje" class="alert alert-danger mt-2">
                    {{ errores.mensaje }}
                  </div>
                </div>

                <button type="submit" class="btn btn-cta w-100" :disabled="isLoadingLogin">
                  <span v-if="isLoadingLogin" class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                  <span v-if="isLoadingLogin"> Verificando...</span>
                  <span v-else>Iniciar Sesión</span>
                </button>

                <div class="text-center mt-3">
                  <a href="#" class="forgot-password" @click.prevent="olvidastePassword">
                    ¿Olvidaste tu contraseña?
                  </a>
                </div>

                <div class="text-center mt-2">
                  <p class="mb-0">
                    Si no tienes cuenta,
                    <RouterLink to="/register" class="register-link">regístrate aquí</RouterLink>
                  </p>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </section>

    <Teleport to="body">
      <div v-if="showRecoveryModal" class="recovery-modal">
        <div class="recovery-content">
          <div class="recovery-header">
            <button class="close-btn" @click="cancelarRecuperacion">&times;</button>
          </div>
          <div v-if="recoveryStep === 1">
            <h4>Recuperar Contraseña</h4>
            <p class="text-text">Ingresa tu correo electrónico registrado para recibir un código de recuperación.</p>
            <form @submit.prevent="enviarSolicitudCodigo">
              <div class="mb-3">
                <input class="form-control" type="email" v-model="recoveryEmail" placeholder="Correo Electrónico"
                  required :disabled="recoveryLoading" />
              </div>
              <button type="submit" style="display: none;"></button>
            </form>
          </div>

          <div v-if="recoveryStep === 2">
            <h4>Ingresar Código y Nueva Contraseña</h4>
            <div v-if="step2UserInstruction" class="alert alert-success mb-3" v-html="step2UserInstruction"></div>
            <form @submit.prevent="cambiarContrasena">
              <div class="mb-3">
                <label for="recoveryCodeInput" class="form-label">Código de Verificación</label>
                <input id="recoveryCodeInput" class="form-control" type="text" v-model="recoveryCode"
                  placeholder="Código de 6 dígitos" required :disabled="recoveryLoading" />
              </div>
              <div class="mb-3">
                <label for="newPasswordInput" class="form-label">Nueva Contraseña</label>
                <input id="newPasswordInput" class="form-control" type="password" v-model="newPasswordRec"
                  placeholder="Nueva Contraseña (mín. 6 caracteres)" required :disabled="recoveryLoading" />
              </div>
              <div class="mb-3">
                <label for="confirmNewPasswordInput" class="form-label">Confirmar Nueva Contraseña</label>
                <input id="confirmNewPasswordInput" class="form-control" type="password"
                  v-model="confirmNewPasswordRec" placeholder="Confirmar Nueva Contraseña" required
                  :disabled="recoveryLoading" />
              </div>
              <button type="submit" style="display: none;"></button>
            </form>
          </div>

          <div v-if="recoveryMessage" class="alert mt-3"
            :class="recoveryMessageType === 'success' ? 'alert-success' : 'alert-danger'" v-html="recoveryMessage">
          </div>

          <div class="d-flex justify-content-between mt-4">
            <button class="btn btn-outline-secondary" @click="cancelarRecuperacion"
              :disabled="recoveryLoading">Cancelar</button>

            <button v-if="recoveryStep === 1" class="btn btn-primary" @click="enviarSolicitudCodigo"
              :disabled="recoveryLoading">
              <span v-if="recoveryLoading">Enviando...</span>
              <span v-else>Enviar Solicitud</span>
            </button>

            <button v-if="recoveryStep === 2" class="btn btn-primary" @click="cambiarContrasena"
              :disabled="recoveryLoading">
              <span v-if="recoveryLoading">Cambiando...</span>
              <span v-else>Cambiar Contraseña</span>
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <Teleport to="body">
      <div v-if="isLoadingLogin" class="loading-overlay">
        <div class="loading-content">
          <div class="spinner"></div>
          <p>{{ loadingLoginMessage }}</p>
        </div>
      </div>
    </Teleport>

    <Teleport to="body">
      <div v-if="showPasswordChangeSuccessModal" class="qb-modal-overlay"
        @click.self="closePasswordChangeSuccessModal">
        <div class="qb-modal-content success-type">
          <i class="bi bi-check-circle-fill"></i>
          <h2>{{ passwordChangeSuccessMessage }}</h2>
          <button @click="closePasswordChangeSuccessModal" class="qb-btn-confirm-success">Aceptar</button>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { usuarioLogIn, solicitarReseteoPassword, restablecerNuevaPassword } from '@/services/UserService'
import { useUserStore } from '@/stores/userStore'
import { jwtDecode } from 'jwt-decode'

const router = useRouter()
const userStore = useUserStore()

const datos = reactive({
  id: '',
  password: ''
})

const errores = reactive({ // Para errores en línea
  mensaje: ''
})

// --- Estados para Carga Global ---
const isLoadingLogin = ref(false)
const loadingLoginMessage = ref('Verificando...')
// --- Fin Estados para Carga Global ---

// Estados para el modal de recuperación
const showRecoveryModal = ref(false)
const recoveryStep = ref(1)
const recoveryEmail = ref('')
const recoveryCode = ref('')
const newPasswordRec = ref('')
const confirmNewPasswordRec = ref('')
const recoveryLoading = ref(false)
const recoveryMessage = ref('')
const recoveryMessageType = ref('')
const step2UserInstruction = ref('')

// Estados para el modal de éxito de cambio de contraseña
const showPasswordChangeSuccessModal = ref(false)
const passwordChangeSuccessMessage = ref('')

async function handleLogIn () {
  errores.mensaje = ''
  if (!datos.id || !datos.password) {
    errores.mensaje = 'ID y contraseña son requeridos.'
    return
  }

  isLoadingLogin.value = true
  loadingLoginMessage.value = 'Iniciando sesión...'

  try {
    const response = await usuarioLogIn.login({
      id: datos.id,
      password: datos.password
    })

    const token = response.token
    userStore.setUserData(response.usuario, token)

    // Decodificar token para obtener el tipoUsuario
    const decoded = jwtDecode(token)
    const tipoUsuarioToken = decoded.tipoUsuario || decoded.role || decoded.userRole

    // Redirección basada en el tipo de usuario
    if (tipoUsuarioToken === 'SUPERUSUARIO' || tipoUsuarioToken === 'ADMINISTRADOR') {
      router.push({ name: 'Panel' })
    } else {
      router.push({ path: '/' })
    }
  } catch (error) {
    console.error('Error completo en login:', error)
    if (
      error.message?.toLowerCase().includes('credenciales') ||
      error.message?.toLowerCase().includes('usuario') ||
      error.message?.toLowerCase().includes('contraseña') ||
      error.message?.toLowerCase().includes('password') ||
      error.message?.toLowerCase().includes('not found') ||
      error.message?.toLowerCase().includes('invalid')
    ) {
      errores.mensaje = 'Usuario o contraseña incorrectos'
    } else if (error.message?.toLowerCase().includes('servidor')) {
      errores.mensaje = 'Error del servidor. Por favor, intente más tarde.'
    } else {
      errores.mensaje = 'Error al iniciar sesión. Inténtalo más tarde.'
    }
  } finally {
    isLoadingLogin.value = false
  }
}

function resetRecoveryForm () {
  recoveryStep.value = 1
  recoveryEmail.value = ''
  recoveryCode.value = ''
  newPasswordRec.value = ''
  confirmNewPasswordRec.value = ''
  recoveryMessage.value = ''
  recoveryMessageType.value = ''
  step2UserInstruction.value = ''
  recoveryLoading.value = false
}

function olvidastePassword () {
  resetRecoveryForm()
  showRecoveryModal.value = true
}

function cancelarRecuperacion () {
  showRecoveryModal.value = false
  resetRecoveryForm()
}

const openPasswordChangeSuccessModal = (message) => {
  passwordChangeSuccessMessage.value = message
  showPasswordChangeSuccessModal.value = true
}

const closePasswordChangeSuccessModal = () => {
  showPasswordChangeSuccessModal.value = false
  passwordChangeSuccessMessage.value = ''
}

async function enviarSolicitudCodigo () {
  if (!recoveryEmail.value) {
    recoveryMessage.value = 'Por favor ingrese su correo electrónico.'
    recoveryMessageType.value = 'error'
    return
  }
  const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailPattern.test(recoveryEmail.value)) {
    recoveryMessage.value = 'Por favor ingrese un formato de correo electrónico válido.'
    recoveryMessageType.value = 'error'
    return
  }

  recoveryLoading.value = true
  recoveryMessage.value = ''
  recoveryMessageType.value = ''
  step2UserInstruction.value = ''

  try {
    await solicitarReseteoPassword({ email: recoveryEmail.value })
    step2UserInstruction.value = `Se ha enviado un código a <strong>${recoveryEmail.value}</strong>. Revisa tu correo (incluyendo spam) e ingrésalo a continuación.`
    recoveryStep.value = 2
  } catch (error) {
    console.error('Error al enviar solicitud de código:', error)
    recoveryMessage.value = error.message || 'No se pudo procesar tu solicitud. Inténtalo más tarde.'
    recoveryMessageType.value = 'error'
  } finally {
    recoveryLoading.value = false
  }
}

async function cambiarContrasena () {
  if (!recoveryCode.value || !newPasswordRec.value || !confirmNewPasswordRec.value) {
    recoveryMessage.value = 'Todos los campos son requeridos para cambiar la contraseña.'
    recoveryMessageType.value = 'error'
    return
  }
  if (newPasswordRec.value.length < 6) { // Usando newPasswordRec
    recoveryMessage.value = 'La nueva contraseña debe tener al menos 6 caracteres.'
    recoveryMessageType.value = 'error'
    return
  }
  if (newPasswordRec.value !== confirmNewPasswordRec.value) { // Usando newPasswordRec y confirmNewPasswordRec
    recoveryMessage.value = 'Las contraseñas no coinciden.'
    recoveryMessageType.value = 'error'
    return
  }

  step2UserInstruction.value = ''
  recoveryLoading.value = true
  recoveryMessage.value = ''
  recoveryMessageType.value = ''

  try {
    const resetData = {
      email: recoveryEmail.value,
      code: recoveryCode.value,
      newPassword: newPasswordRec.value // Usando newPasswordRec
    }
    await restablecerNuevaPassword(resetData)

    // Limpiar y cerrar el modal de recuperación
    recoveryMessage.value = ''
    recoveryMessageType.value = ''
    step2UserInstruction.value = ''
    cancelarRecuperacion() // Cierra el modal de recuperación de contraseña

    // Mostrar el nuevo modal de éxito
    openPasswordChangeSuccessModal('¡Contraseña actualizada exitosamente! Ya puedes iniciar sesión.')
  } catch (error) {
    console.error('Error al cambiar contraseña:', error)
    recoveryMessage.value = error.message || 'No se pudo cambiar la contraseña. Verifica el código o inténtalo más tarde.'
    recoveryMessageType.value = 'error'
  } finally {
    recoveryLoading.value = false
  }
}
</script>

<style lang="scss" scoped>
.login-view { padding-bottom: 2rem; }

.login-card { border: 1px solid rgba(0,0,0,0.06); border-radius: 12px; }

h3 {
  font-family: 'Poppins', sans-serif;
  font-weight: 700;
  color: var(--dp-primary);
}

.form-label { font-weight: 600; color: #0b2136; }
.form-control { font-size: 1rem; border-radius: 8px; }

.h4,
h4 { font-size: calc(1.275rem + .3vw); color: #0b2136; }

button { font-family: 'Poppins', sans-serif; font-size: 1rem; font-weight: 600; border-radius: 8px; transition: all 0.2s ease; }

.spinner-border-sm {
  width: 1em;
  height: 1em;
  border-width: .2em;
  vertical-align: -0.125em;
}

.btn:focus,
.btn:active,
.btn-primary:focus { box-shadow: none !important; outline: none !important; }

.forgot-password { font-size: 0.9rem; font-weight: 500; text-decoration: underline; cursor: pointer; color: rgba(0,0,0,0.6); }
.forgot-password:hover { color: rgba(0,0,0,0.75); }
.register-link { color: var(--dp-primary); text-decoration: underline; font-weight: 600; }
.register-link:hover { color: var(--dp-dark); }

.recovery-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1050;
  padding: 2rem 1rem;
  box-sizing: border-box;
}

.recovery-content {
  background-color: white;
  padding: 2rem;
  border-radius: 10px;
  width: 100%;
  max-width: 450px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  text-align: center;
  max-height: 100%;
  overflow-y: auto;
}

:deep(.recovery-content .register-link) {
  color: #b88b4a;
  text-decoration: underline;
  font-weight: 500;

  &:hover {
    color: #a27035;
  }
}

.recovery-content p {
  margin-bottom: 1.5rem;
}

.recovery-content .form-label {
  text-align: left;
  display: block;
  margin-bottom: .5rem;
  font-weight: 500;
}

.alert {
  padding: 0.75rem 1.25rem;
  margin-top: 1rem;
  border: 1px solid transparent;
  border-radius: 0.25rem;
  text-align: left;
}

.alert-success {
  color: #155724;
  background-color: #d4edda;
  border-color: #c3e6cb;
}

.alert-danger {
  color: #721c24;
  background-color: #f8d7da;
  border-color: #f5c6cb;
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
}

.loading-content {
  background-color: white;
  border-radius: 12px;
  padding: 2rem;
  text-align: center;
  max-width: 300px;
  width: 90%;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);

  p {
    margin-top: 1rem;
    color: var(--dp-dark);
    font-weight: 500;
    font-size: 1rem;
  }
}

.spinner {
  width: 50px;
  height: 50px;
  border: 5px solid #f3f3f3;
  border-top: 5px solid var(--dp-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

/* Estilos para el nuevo modal de éxito (inspirados en UserProfile.vue) */
.qb-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  /* Consistente con recovery-modal */
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1055;
  /* Ligeramente por encima del recovery-modal si es necesario */
}

.qb-modal-content {
  background-color: white;
  border-radius: 12px;
  width: 90%;
  max-width: 500px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.15);
}

.qb-modal-content.success-type {
  padding: 2.5rem;
  text-align: center;

  i.bi-check-circle-fill {
    font-size: 4rem;
    color: var(--dp-accent);
    /* Color de éxito estándar */
    margin-bottom: 1.5rem;
  }

  h2 {
    color: var(--dp-dark);
    /* Color de título consistente */
    margin-bottom: 2rem;
    font-size: 1.5rem;
    line-height: 1.4;
  }
}

.qb-btn-confirm-success {
  background-color: var(--dp-primary);
  /* Botón primario oscuro */
  color: white;
  border: none;
  padding: 0.8rem 2rem;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;

  &:hover {
    background-color: var(--dp-dark);
    /* Un poco más claro al hacer hover */
  }
}

.recovery-header { display: flex; justify-content: flex-end; }
.close-btn { background: transparent; border: none; font-size: 1.5rem; font-weight: bold; cursor: pointer; color: #333; margin-bottom: 0.5rem; }
.text-text { color: #0b2136; }
</style>
