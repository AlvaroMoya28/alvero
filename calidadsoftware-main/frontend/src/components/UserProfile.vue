<template>
  <div class="user-profile-container">
    <div class="profile-sidebar">
      <div class="user-avatar">
        <div class="avatar-placeholder" v-if="name && lastName">
          {{ name.charAt(0) }}{{ lastName.charAt(0) }}
        </div>
        <div class="avatar-placeholder" v-else>
          <i class="bi bi-person"></i>
        </div>
        <h3 v-if="name && lastName">{{ name }} {{ lastName }}</h3>
        <h3 v-else>Cargando...</h3>
      </div>
      <nav class="profile-menu">
        <router-link to="/perfil" class="menu-item" active-class="active">
          <i class="bi bi-person"></i> Perfil
        </router-link>
        <router-link to="/perfil/historial" class="menu-item" active-class="active">
          <i class="bi bi-clock-history"></i> Historial
        </router-link>
        <a href="#" @click.prevent="openPasswordModal" class="menu-item">
          <i class="bi bi-shield-lock"></i> Cambiar contraseña
        </a>
      </nav>
    </div>

    <div class="profile-content">
      <div class="profile-header">
        <h1>Configuración de Perfil</h1>
        <p>Administra tu información personal y preferencias</p>
      </div>

      <div class="profile-section">
        <h2><i class="bi bi-person-badge"></i> Información Personal</h2>
        <div class="form-group">
          <label>Nombre</label>
          <input v-model="name" class="modern-input" placeholder="Tu nombre" :disabled="isLoading">
        </div>
        <div class="form-row">
          <div class="form-group">
            <label>Primer apellido</label>
            <input v-model="lastName" class="modern-input" placeholder="Primer apellido" :disabled="isLoading">
          </div>
          <div class="form-group">
            <label>Segundo apellido</label>
            <input v-model="secondlastName" class="modern-input" placeholder="Segundo apellido" :disabled="isLoading">
          </div>
        </div>
        <button @click="updateNameAndLastName" class="save-btn" :disabled="isLoading">
          <i class="bi bi-check-circle"></i>
          <span v-if="isLoading && loadingAction === 'profileInfo'">Guardando...</span>
          <span v-else>Guardar cambios</span>
        </button>
      </div>

      <div class="profile-section">
        <h2><i class="bi bi-gear"></i> Configuración de Cuenta</h2>
        <div class="setting-item" @click="OpenModalChangeEmail">
          <div class="setting-info">
            <i class="bi bi-envelope"></i>
            <div>
              <h3>Correo electrónico</h3>
              <p>{{ profileDisplayEmail }}</p> </div>
          </div>
          <i class="bi bi-chevron-right"></i>
        </div>

        <div v-if="isOpenChangeEmail" class="modal" @click.self="closeChangeEmailModal">
        <div class="modalContent">
          <h3>Cambia el correo electronico</h3>
           <p id="label">Correo electrónico actual</p>
          <p id="correo-actual">{{ profileDisplayEmail }}</p> <p id="label">* Nuevo Correo Electronico</p>
          <input v-model="editableEmail" type="email" placeholder="Nuevo Correo" :disabled="isLoading">

          <div v-if="modalErrorMessage" class="alert alert-danger mt-2">{{ modalErrorMessage }}</div>
          <div class="actions">
            <button @click="updateEmail" :disabled="isLoading">
              <span v-if="isLoading && loadingAction === 'email'">Guardando...</span>
              <span v-else>Guardar</span>
            </button>
            <button @click="closeChangeEmailModal" :disabled="isLoading">Cancelar</button>
          </div>
        </div>
      </div>

      <div class="setting-item" @click="OpenModalChangeNumber">
          <div class="setting-info">
            <i class="bi bi-telephone"></i>
            <div>
              <h3>Teléfono</h3>
              <p>{{ profileDisplayNumber || 'No especificado' }}</p> </div>
          </div>
          <i class="bi bi-chevron-right"></i>
        </div>

        <div v-if="isOpenChangeNumber" class="modal" @click.self="closeChangeNumberModal">
        <div class="modalContent">
          <h3>Cambiar número de telefono</h3>
          <p id="label">Número telefonico actual</p>
          <p id="correo-actual">{{ profileDisplayNumber || 'No especificado' }}</p> <p id="label">* Numero telefonico nuevo</p>
          <input v-model="editableNumber" type="tel" placeholder="ej. 88888888" :disabled="isLoading">

          <div v-if="modalErrorMessage" class="alert alert-danger mt-2">{{ modalErrorMessage }}</div>
          <div class="actions">
            <button @click="updateNumber" :disabled="isLoading">
               <span v-if="isLoading && loadingAction === 'number'">Guardando...</span>
               <span v-else>Guardar</span>
            </button>
            <button @click="closeChangeNumberModal" :disabled="isLoading">Cancelar</button>
          </div>
        </div>
      </div>

      <div class="setting-item danger" @click="OpenDeleteAccountModal">
          <div class="setting-info">
            <i class="bi bi-trash"></i>
            <div>
              <h3>Eliminar cuenta</h3>
              <p>Esta acción no se puede deshacer</p>
            </div>
          </div>
          <i class="bi bi-chevron-right"></i>
        </div>
      </div>
    </div>

    <div v-if="isDeleteAccountOpen" class="modal" @click.self="closeDeleteAccountModal">
        <div class="modalContent">
          <h3>Eliminar cuenta</h3>
          <p>¿Está seguro que desea eliminar su cuenta permanentemente?</p>
          <div v-if="modalErrorMessage" class="alert alert-danger mt-2">{{ modalErrorMessage }}</div>
          <div class="actions">
             <button @click="CloseDeleteAccountModal" class="btn btn-outline-secondary" :disabled="isLoading">Cancelar</button>
            <button @click="deleteUsuarioAccount" class="btn btn-danger" :disabled="isLoading">
              <span v-if="isLoading && loadingAction === 'deleteAccount'">Eliminando...</span>
              <span v-else>Sí, eliminar cuenta</span>
            </button>
          </div>
        </div>
      </div>

    <div v-if="showPasswordModal" class="modal" @click.self="closePasswordModal">
      <div class="modalContent password-change-modal-content" @click.stop>
        <h3 class="text-center mb-4">Cambiar contraseña</h3>

        <div class="mb-3">
          <label for="currentPassword" class="form-label">* Contraseña actual</label>
          <input id="currentPassword" v-model="currentPassword" type="password"
                 class="form-control"
                 placeholder="Ingresa tu contraseña actual" :disabled="passwordChangeLoading">
        </div>

        <div class="mb-3">
          <label for="newPassword" class="form-label">* Nueva contraseña</label>
          <input id="newPassword" v-model="newPassword" type="password" class="form-control"
                 placeholder="Mínimo 8 caracteres" :disabled="passwordChangeLoading">
        </div>

        <div class="mb-3">
          <label for="confirmPassword" class="form-label">* Confirmar nueva contraseña</label>
          <input id="confirmPassword" v-model="confirmPassword" type="password"
                 class="form-control"
                 placeholder="Repite tu nueva contraseña" :disabled="passwordChangeLoading">
        </div>

        <div v-if="passwordModalSuccessMessage" class="alert alert-success mt-2">
          {{ passwordModalSuccessMessage }}
        </div>
        <div v-if="passwordModalErrorMessage" class="alert alert-danger mt-2">
          {{ passwordModalErrorMessage }}
        </div>

        <div class="d-flex justify-content-between mt-4">
           <button @click="closePasswordModal" class="btn btn-outline-secondary" :disabled="passwordChangeLoading">
            Cancelar
          </button>
          <button @click="submitPasswordChange" class="btn btn-primary" :disabled="passwordChangeLoading"> <span v-if="passwordChangeLoading">Guardando...</span>
            <span v-else>Guardar cambios</span>
          </button>
        </div>
      </div>
    </div>

    <div v-if="showSuccessModal" class="modal" @click.self="closeSuccessModal">
      <div class="modalContent success-modal">
        <i class="bi bi-check-circle-fill"></i>
        <h2>{{ successMessage }}</h2>
        <button @click="closeSuccessModal" class="btn-confirm">Aceptar</button>
      </div>
    </div>

    <div v-if="showErrorModal" class="modal" @click.self="closeErrorModal">
      <div class="modalContent error-modal">
        <i class="bi bi-exclamation-triangle-fill"></i>
        <h2>{{ errorMessage }}</h2>
        <button @click="closeErrorModal" class="btn-confirm">Entendido</button>
      </div>
    </div>

    <div v-if="isLoading && loadingAction !== 'passwordChange'" class="loading-overlay">
      <div class="loading-content">
        <div class="spinner"></div>
        <p>{{ loadingMessage }}</p>
      </div>
    </div>

  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { getUsuarioPorId, updateUsuario, deleteUsuario, changePasswordAuthenticated } from '@/services/UserService'
import { useUserStore } from '@/stores/userStore'

const router = useRouter()
const userStore = useUserStore()

const name = ref('')
const lastName = ref('')
const secondlastName = ref('')
const profileDisplayEmail = ref('')
const profileDisplayNumber = ref('')
const editableEmail = ref('')
const editableNumber = ref('')
const id = ref(userStore.user?.idUsuario || localStorage.getItem('userId') || null)

const isLoading = ref(false)
const loadingMessage = ref('Cargando...')
const loadingAction = ref('')

const isOpenChangeEmail = ref(false)
const isOpenChangeNumber = ref(false)
const isDeleteAccountOpen = ref(false)

// Variables para el modal de éxito general (ya las tienes)
const showSuccessModal = ref(false)
const successMessage = ref('') // Usaremos esta para todos los mensajes de éxito
const modalErrorMessage = ref('') // Para errores en modales de email/teléfono
const showErrorModal = ref(false)
const errorMessage = ref('')

// Estado del modal de cambio de contraseña
const showPasswordModal = ref(false)
const currentPassword = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const passwordModalErrorMessage = ref('') // Específico para errores en este modal
// const passwordModalSuccessMessage = ref(''); // Ya no usaremos esta para el éxito
const passwordChangeLoading = ref(false)

// Función para abrir el modal de éxito general
const openSuccessModal = (message) => {
  successMessage.value = message
  showSuccessModal.value = true
}

// Función para abrir modal de error
const openErrorModal = (message) => {
  errorMessage.value = message
  showErrorModal.value = true
}

// Función para cerrar el modal de éxito general
const closeSuccessModal = () => {
  showSuccessModal.value = false
  successMessage.value = '' // Limpiar mensaje al cerrar
}

// Función para cerrar modal de error
const closeErrorModal = () => {
  showErrorModal.value = false
  errorMessage.value = ''
}

onMounted(async () => {
  if (!id.value) {
    console.error('ID de usuario no encontrado. Redirigiendo al login.')
    router.push('/login')
    return
  }
  isLoading.value = true
  loadingMessage.value = 'Cargando perfil...'
  loadingAction.value = 'loadProfile'
  try {
    const userData = await getUsuarioPorId(id.value)
    userStore.setUserData(userData, userStore.token) // Actualizar store
    name.value = userData.nombre || ''
    lastName.value = userData.apellido1 || ''
    secondlastName.value = userData.apellido2 || ''
    profileDisplayEmail.value = userData.email || ''
    profileDisplayNumber.value = userData.telefono || ''
  } catch (error) {
    console.error('Error al obtener datos del usuario en onMounted:', error)
    openErrorModal('Error al cargar el perfil. Intente más tarde.')
  } finally {
    isLoading.value = false
    loadingAction.value = ''
  }
})

const updateNameAndLastName = async () => {
  if (!id.value) return
  isLoading.value = true; loadingMessage.value = 'Actualizando perfil...'; loadingAction.value = 'profileInfo'
  modalErrorMessage.value = '' // Limpiar errores de otros modales si están abiertos
  try {
    const dataToUpdate = {
      nombre: name.value,
      apellido1: lastName.value,
      apellido2: secondlastName.value
    }
    const response = await updateUsuario(id.value, dataToUpdate)
    userStore.setUserData({
      ...userStore.user,
      ...dataToUpdate
    }, response.token || userStore.token)
    openSuccessModal('Información personal actualizada con éxito.')
  } catch (error) {
    console.error('Error al actualizar datos de perfil:', error)
    openSuccessModal('Error al actualizar la información: ' + (error.response?.data?.message || error.message || 'Error desconocido'))
  } finally {
    isLoading.value = false; loadingAction.value = ''
  }
}

const OpenModalChangeEmail = () => {
  editableEmail.value = profileDisplayEmail.value
  modalErrorMessage.value = ''
  isOpenChangeEmail.value = true
}
const closeChangeEmailModal = () => { isOpenChangeEmail.value = false }
const updateEmail = async () => {
  if (!id.value) return
  if (!editableEmail.value) {
    modalErrorMessage.value = 'El nuevo correo no puede estar vacío.'; return
  }
  isLoading.value = true; loadingMessage.value = 'Actualizando correo...'; loadingAction.value = 'email'
  modalErrorMessage.value = ''
  try {
    await updateUsuario(id.value, { email: editableEmail.value })
    profileDisplayEmail.value = editableEmail.value
    userStore.setUserData({ ...userStore.user, email: editableEmail.value }, userStore.token)
    closeChangeEmailModal() // Cerrar modal de edición
    openSuccessModal('Correo electrónico actualizado con éxito.') // Abrir modal de éxito general
  } catch (error) {
    console.error('Error al actualizar email:', error)
    modalErrorMessage.value = error.response?.data?.message || error.message || 'Error al actualizar el correo.'
  } finally {
    isLoading.value = false; loadingAction.value = ''
  }
}

const OpenModalChangeNumber = () => {
  editableNumber.value = profileDisplayNumber.value
  modalErrorMessage.value = ''
  isOpenChangeNumber.value = true
}
const closeChangeNumberModal = () => { isOpenChangeNumber.value = false }
const updateNumber = async () => {
  if (!id.value) return
  isLoading.value = true; loadingMessage.value = 'Actualizando teléfono...'; loadingAction.value = 'number'
  modalErrorMessage.value = ''
  try {
    await updateUsuario(id.value, { telefono: editableNumber.value })
    profileDisplayNumber.value = editableNumber.value
    userStore.setUserData({ ...userStore.user, telefono: editableNumber.value }, userStore.token)
    closeChangeNumberModal() // Cerrar modal de edición
    openSuccessModal('Número de teléfono actualizado con éxito.') // Abrir modal de éxito general
  } catch (error) {
    console.error('Error al actualizar teléfono:', error)
    modalErrorMessage.value = error.response?.data?.message || error.message || 'Error al actualizar el teléfono.'
  } finally {
    isLoading.value = false; loadingAction.value = ''
  }
}

const OpenDeleteAccountModal = () => {
  modalErrorMessage.value = ''
  isDeleteAccountOpen.value = true
}
const CloseDeleteAccountModal = () => { isDeleteAccountOpen.value = false }
const deleteUsuarioAccount = async () => {
  if (!id.value) return
  isLoading.value = true; loadingMessage.value = 'Eliminando cuenta...'; loadingAction.value = 'deleteAccount'
  modalErrorMessage.value = ''
  try {
    await deleteUsuario(id.value)
    CloseDeleteAccountModal()
    openSuccessModal('Cuenta eliminada con éxito. Serás redirigido.')
    userStore.logout()
    setTimeout(() => {
      router.push('/login')
    }, 2500)
  } catch (error) {
    console.error('Error al eliminar cuenta:', error)
    modalErrorMessage.value = error.response?.data?.message || error.message || 'Error al eliminar la cuenta.'
  } finally {
    isLoading.value = false; loadingAction.value = ''
  }
}

// --- Lógica para el Modal de Cambio de Contraseña ---
const openPasswordModal = () => {
  resetPasswordForm()
  showPasswordModal.value = true
}

const closePasswordModal = () => {
  showPasswordModal.value = false
  // Opcional: resetPasswordForm(); // Si se quiere que se limpie siempre al cerrar por "Cancelar"
}

const resetPasswordForm = () => {
  currentPassword.value = ''
  newPassword.value = ''
  confirmPassword.value = ''
  passwordModalErrorMessage.value = '' // Usaremos este para errores DENTRO del modal de contraseña
  // passwordModalSuccessMessage.value = ''; // Ya no se usa, usamos el general 'successMessage'
  passwordChangeLoading.value = false
}

async function submitPasswordChange () {
  passwordModalErrorMessage.value = '' // Limpiar mensajes de error previos del modal de contraseña
  // successMessage.value = ''; // No es necesario limpiar el general aquí

  // 1. Validaciones del Frontend
  if (!currentPassword.value || !newPassword.value || !confirmPassword.value) {
    passwordModalErrorMessage.value = 'Todos los campos son obligatorios.'
    return
  }
  if (newPassword.value !== confirmPassword.value) {
    passwordModalErrorMessage.value = 'Las nuevas contraseñas no coinciden.'
    return
  }
  if (newPassword.value.length < 8) {
    passwordModalErrorMessage.value = 'La nueva contraseña debe tener al menos 8 caracteres.'
    return
  }
  if (newPassword.value === currentPassword.value) {
    passwordModalErrorMessage.value = 'La nueva contraseña no puede ser igual a la contraseña actual.'
    return
  }

  passwordChangeLoading.value = true

  try {
    const passwordData = {
      currentPassword: currentPassword.value,
      newPassword: newPassword.value
    }

    const response = await changePasswordAuthenticated(passwordData)

    resetPasswordForm() // Limpiar el formulario del modal de cambio de contraseña
    closePasswordModal() // Cerrar el modal de cambio de contraseña

    // ABRIR EL MODAL DE ÉXITO GENERAL
    openSuccessModal(response.message || 'Contraseña actualizada exitosamente.')
  } catch (error) {
    // Mostrar error DENTRO del modal de cambio de contraseña
    passwordModalErrorMessage.value = error.message || 'Ocurrió un error al cambiar la contraseña.'
    console.error('Error al cambiar contraseña en UserProfile.vue:', error)
  } finally {
    passwordChangeLoading.value = false
  }
}
</script>

<style lang="scss" scoped>
.error-modal {
  text-align: center;
  padding: 2.5rem !important;

  i.bi-exclamation-triangle-fill {
    font-size: 4rem;
    color: #dc3545; /* Rojo para errores */
    margin-bottom: 1.5rem;
  }

  h2 {
    color: #dc3545; /* Rojo para errores */
    margin-bottom: 2rem;
    font-size: 1.5rem;
    line-height: 1.4;
  }

  .btn-confirm {
    background-color: #dc3545; /* Rojo para errores */
    color: white;
    border: none;
    padding: 0.8rem 2rem;
    border-radius: 8px;
    font-size: 1rem;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.3s ease;

    &:hover {
      background-color: #c82333; /* Rojo más oscuro al hacer hover */
    }
  }
}

.user-profile-container {
  display: flex;
  min-height: 100vh;
  background-color: var(--dp-bg);
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.profile-sidebar {
  width: 280px;
  background: linear-gradient(135deg, var(--dp-dark) 0%, var(--dp-darkest) 100%);
  color: white;
  padding: 2rem 1.5rem;
  position: sticky;
  top: 139px; /* Ajusta esto según la altura de tu Navbar si tienes uno fijo */
  height: calc(100vh - 139px); /* Ajusta esto según la altura de tu Navbar */
}

.user-avatar {
  text-align: center;
  margin-bottom: 2rem;
  padding-bottom: 2rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  h3 {
    margin-top: 0.5rem;
    font-size: 1.1rem;
    font-weight: 500;
  }
}

.avatar-placeholder {
  width: 80px;
  height: 80px;
  background-color: var(--dp-primary);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  font-weight: bold;
  margin: 0 auto 1rem;
  color: white;
   i.bi-person {
    font-size: 2.5rem;
  }
}

.profile-menu {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.menu-item {
  padding: 0.8rem 1rem;
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 0.8rem;
  transition: all 0.3s ease;
  i {
    font-size: 1.1rem;
    min-width: 20px; /* Para alinear el texto si algunos items no tienen icono */
  }

  &:hover {
    background-color: rgba(255, 255, 255, 0.1);
    color: white;
  }

  &.active { // Vue Router active class
    background-color: rgba(11, 91, 215, 0.2);
    color: white;
    font-weight: 600;
  }
}

.profile-content {
  flex: 1;
  padding: 2rem 3rem;
  max-width: 900px; // Un poco más de ancho para el contenido
  margin: 0 auto; // Centrar el contenido si hay espacio extra
}

.profile-header {
  margin-bottom: 2.5rem;
  h1 {
    font-size: 2rem;
    color: var(--dp-primary);
    margin-bottom: 0.5rem;
  }
  p {
    color: #6c757d;
    font-size: 1rem;
  }
}

.profile-section {
  background-color: white;
  border-radius: 12px;
  padding: 1.5rem 2rem;
  margin-bottom: 2rem; // Aumentar separación entre secciones
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08); // Sombra más suave
  h2 {
    font-size: 1.3rem;
    color: var(--dp-dark);
    margin-bottom: 1.8rem; // Más espacio
    display: flex;
    align-items: center;
    gap: 0.75rem; // Más espacio con el icono
    padding-bottom: 0.75rem; // Línea sutil debajo del título de sección
    border-bottom: 1px solid #eee;
    i {
      color: var(--dp-accent);
      font-size: 1.5rem; // Icono un poco más grande
    }
  }
}

.form-group {
  margin-bottom: 1.5rem; // Más espacio
  label {
    display: block;
    margin-bottom: 0.6rem; // Más espacio
    color: #495057;
    font-weight: 500;
    font-size: 0.95rem; // Ligeramente más pequeño
  }
}

.form-row {
  display: flex;
  gap: 1.5rem;
  .form-group {
    flex: 1;
  }
}

.modern-input {
  width: 100%;
  padding: 0.8rem 1rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  font-size: 1rem;
  transition: all 0.3s ease;
  box-sizing: border-box;
  &:focus {
    outline: none;
    border-color: var(--dp-primary); // Color de acento al enfocar
    box-shadow: 0 0 0 3px rgba(11, 91, 215, 0.15);
  }
}
.modern-input:disabled {
  background-color: #f8f9fa;
  cursor: not-allowed;
}

.save-btn {
  background-color: var(--dp-primary);
  color: white;
  border: none;
  padding: 0.8rem 1.5rem;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  display: inline-flex; // Para que el spinner se alinee bien
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 1rem;
  &:hover {
    background-color: var(--dp-dark);
    transform: translateY(-2px);
  }
   &:disabled {
    background-color: rgba(11, 91, 215, 0.5);
    cursor: not-allowed;
    transform: none;
  }
}

.setting-item {
  padding: 1.2rem 0.5rem; // Un poco menos de padding horizontal
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  transition: all 0.2s ease-in-out; // Transición más rápida
  border-radius: 4px; // Bordes redondeados sutiles para el hover
  &:last-child {
    border-bottom: none;
  }
  &:hover {
    background-color: #f8f9fa; // Un fondo muy sutil al hacer hover
  }
  &.danger {
    .setting-info i, .setting-info h3 {
      color: #dc3545 !important;
    }
    &:hover {
      background-color: rgba(220, 53, 69, 0.05); // Fondo rojo sutil al hacer hover
    }
  }
  .bi-chevron-right {
    color: #adb5bd; // Chevron más sutil
    transition: transform 0.2s ease-in-out;
  }
  &:hover .bi-chevron-right {
    transform: translateX(3px); // Mover ligeramente el chevron al hacer hover
  }
}

.setting-info {
  display: flex;
  align-items: center;
  gap: 1rem;
  i {
    font-size: 1.3rem;
    color: var(--dp-primary); // Iconos consistentes con el tema
    min-width: 24px; // Para alineación
    text-align: center;
  }
  h3 {
    font-size: 1rem;
    color: #343a40;
    margin-bottom: 0.2rem;
    font-weight: 500; // Ligeramente menos bold
  }
  p {
    font-size: 0.9rem;
    color: #6c757d;
    margin: 0; // Quitar margen para mejor control
  }
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.65);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1050;
  padding: 1rem;
  box-sizing: border-box;
}

.modalContent {
  background-color: white;
  padding: 2rem; // Padding uniforme
  border-radius: 10px;
  width: 100%;
  max-width: 480px; // Ancho máximo ligeramente ajustado
  box-shadow: 0 5px 25px rgba(0, 0, 0, 0.15); // Sombra más suave
  text-align: left;
  max-height: calc(100vh - 4rem); // Evitar que sea más alto que la pantalla
  overflow-y: auto; // Scroll si el contenido es mucho

  h3 { // Estilo para los h3 dentro de los modales (como el de cambiar contraseña)
    font-family: 'Poppins', sans-serif;
    font-weight: 600; // Ligeramente menos bold que h1
    color: var(--dp-primary); // Color principal
    margin-top: 0;
    margin-bottom: 1.5rem;
    text-align: center;
    font-size: 1.6rem; // Tamaño ajustado
  }
   p#indicacion {
    font-size: 0.85rem;
    color: #6c757d;
    text-align: center;
    margin-bottom: 1.5rem;
  }
  p#label {
      display: block;
      text-align: left;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #343a40; // Color de label más oscuro
      font-size: 0.9rem;
  }
  p#correo-actual {
    font-weight: normal;
    text-align: left;
    margin-bottom: 1rem;
    padding: 0.75rem 1rem; // Buen padding
    background-color: #f1f1f1; // Fondo para destacar que no es editable
    border-radius: 6px;
    font-size: 0.95rem;
    color: #495057;
    word-break: break-all; // Para emails largos
  }

  input[type="email"], input[type="tel"] { // Estilo para inputs en modales de email/teléfono
    padding: 0.8rem 1rem;
    border: 1px solid #ced4da;
    border-radius: 6px;
    width: 100%;
    box-sizing: border-box;
    font-size: 1rem;
    margin-bottom: 1.2rem; // Espacio debajo del input
     &:focus {
      outline: none;
      border-color: var(--dp-primary);
      box-shadow: 0 0 0 0.2rem rgba(11, 91, 215, 0.2);
    }
  }
  input[type="email"]:disabled, input[type="tel"]:disabled {
      background-color: #e9ecef;
      cursor: not-allowed;
  }

  .actions {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
    margin-top: 1.5rem; // Espacio consistente

    button {
      padding: 0.7rem 1.5rem;
      border-radius: 8px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.3s ease;
      border: none;
      font-size: 0.95rem;

      // Usar clases custom para evitar conflictos y mayor control
      &.btn-primary-custom { // Para el botón principal del modal (Guardar, Cambiar)
        background-color: var(--dp-primary);
        color: white;
        &:hover { background-color: var(--dp-dark); }
        &:disabled { background-color: #526f7c !important; color: #c0c9d0 !important; cursor: not-allowed; }
      }
      &.btn-secondary-custom { // Para el botón de cancelar
        background-color: #6c757d;
        color: white;
        &:hover { background-color: #5a6268; }
        &:disabled { background-color: #9fa6ac !important; color: #e0e3e6 !important; cursor: not-allowed; }
      }
      &.btn-danger-custom { // Para el botón de eliminar
        background-color: #dc3545;
        color: white;
        &:hover { background-color: #c82333; }
        &:disabled { background-color: #e48089 !important; color: #f9d9dc !important; cursor: not-allowed; }
      }
    }
  }
}

.password-change-modal-content { // Contenedor específico del modal de cambio de contraseña
  .mb-3 { margin-bottom: 1.2rem !important; } // Más espacio
  .mt-2 { margin-top: 1rem !important; } // Para mensajes de error/éxito
  .mt-4 { margin-top: 2rem !important; } // Más espacio antes de los botones
  .text-center { text-align: center !important; }
  .d-flex { display: flex !important; }
  .justify-content-between { justify-content: space-between !important; }

  .form-label { // Labels dentro del modal de cambio de contraseña
    font-family: 'Poppins', sans-serif;
    display: block;
    text-align: left;
    margin-bottom: 0.5rem;
    font-weight: 500;
    color: #343a40;
    font-size: 0.9rem;
  }

  .form-control { // Inputs dentro del modal de cambio de contraseña
    font-family: 'Noto Sans', sans-serif;
    font-size: 1rem;
    padding: 0.8rem 1rem;
    border: 1px solid #ced4da;
    border-radius: 6px; // Bordes consistentes
    width: 100%;
    box-sizing: border-box;
    transition: border-color .15s ease-in-out, box-shadow .15s ease-in-out;

    &:focus {
      color: #495057;
      background-color: #fff;
      border-color: #b88b4a; // Color de acento
      outline: 0;
      box-shadow: 0 0 0 0.2rem rgba(184, 139, 74, 0.2); // Sombra de acento
    }
    &::placeholder {
      color: #6c757d;
      opacity: 1;
    }
     &:disabled {
      background-color: #e9ecef;
      cursor: not-allowed;
    }
  }

  .alert { // Mensajes de error/éxito dentro del modal
    width: 100%;
    padding: 0.8rem 1.25rem;
    border-radius: 6px;
    text-align: center;
    font-size: 0.95rem;
    border-width: 1px;
    border-style: solid;
  }
  .alert-danger {
    color: #721c24;
    background-color: #f8d7da;
    border-color: #f5c6cb;
  }
   .alert-success { // Ya definido abajo, pero por si acaso
    color: #155724;
    background-color: #d4edda;
    border-color: #c3e6cb;
  }

  .btn { // Estilos base para botones del modal (Bootstrap-like)
    display: inline-block;
    font-family: 'Poppins', sans-serif;
    font-weight: 500; // Menos bold que los botones principales
    font-size: 0.95rem;
    text-align: center;
    vertical-align: middle;
    cursor: pointer;
    user-select: none;
    background-color: transparent;
    border: 1px solid transparent;
    padding: 0.7rem 1.2rem; // Padding ajustado
    border-radius: 6px;
    transition: all 0.2s ease-in-out;
  }

  .btn-primary { // Botón principal (Guardar Cambios) del modal de contraseña
    background-color: var(--dp-primary); // Color primario
    color: white;
    border-color: var(--dp-primary);
    &:hover {
      background-color: var(--dp-dark);
      border-color: var(--dp-dark);
    }
    &:focus {
      outline: none;
      box-shadow: 0 0 0 0.2rem rgba(5, 46, 93, 0.3);
    }
    &:disabled {
      background-color: #526f7c !important;
      border-color: #526f7c !important;
      color: #c0c9d0 !important;
      cursor: not-allowed;
    }
  }

  .btn-outline-secondary { // Botón de cancelar del modal de contraseña
    color: #6c757d;
    border-color: #6c757d;
    background-color: transparent;
    &:hover {
      color: #fff;
      background-color: #6c757d;
      border-color: #6c757d;
    }
     &:focus {
      outline: none;
      box-shadow: 0 0 0 0.2rem rgba(108, 117, 125, 0.5);
    }
     &:disabled {
      color: #9fa6ac;
      border-color: #9fa6ac;
      background-color: transparent;
      cursor: not-allowed;
    }
  }
}

.success-modal { // Para el modal de éxito general
  text-align: center;
  padding: 2.5rem !important;
  i.bi-check-circle-fill {
    font-size: 4rem;
    color: var(--dp-accent);
    margin-bottom: 1.5rem;
  }
  h2 {
    color: var(--dp-dark);
    margin-bottom: 2rem;
    font-size: 1.5rem;
    line-height: 1.4;
  }
  .btn-confirm {
    background-color: var(--dp-primary);
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
    }
  }
}

@media (max-width: 768px) {
  .user-profile-container {
    flex-direction: column;
  }
  .profile-sidebar {
    width: 100%;
    height: auto;
    position: relative;
    top: 0;
    padding: 1.5rem;
    border-right: none; // Quitar borde en móvil
    border-bottom: 1px solid #dee2e6; // Añadir borde inferior
  }
  .profile-content {
    padding: 1.5rem;
    max-width: 100%; // Ocupar todo el ancho
  }
  .form-row {
    flex-direction: column;
    gap: 0; // Quitar gap si los form-group ya tienen margin-bottom
  }

  // Ajustar botones en modales para móvil
  .modalContent .actions {
    flex-direction: column-reverse; // Botón principal arriba
    gap: 0.75rem;
    button {
      width: 100%; // Botones ocupan todo el ancho
    }
  }

  .password-change-modal-content {
    .d-flex.justify-content-between { // Para los botones del modal de cambio de contraseña
      flex-direction: column-reverse;
      gap: 0.75rem;
      button {
        width: 100%;
      }
    }
  }
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(255, 255, 255, 0.8); // Fondo más claro para el loading
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000; // Muy alto para estar por encima de todo
}

.loading-content {
  background-color: transparent; // Sin fondo para el contenedor del spinner
  text-align: center;
  p {
    margin-top: 1rem;
    color: var(--dp-dark);
    font-weight: 500;
    font-size: 1.1rem; // Un poco más grande
    text-shadow: 0 0 5px white; // Sombra para legibilidad si el fondo es complejo
  }
}

.spinner {
  width: 60px; // Spinner más grande
  height: 60px;
  border: 6px solid rgba(11, 91, 215, 0.2); // Borde base del spinner más sutil
  border-top: 6px solid var(--dp-primary); // Color de acento para la parte que gira
  border-radius: 50%;
  animation: spin 0.8s linear infinite; // Más rápido
  margin: 0 auto;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

// Remover el spinner ::after de .save-btn si se usa el overlay global
// Si se prefiere el spinner en el botón, mantener estos estilos y ajustar el :disabled
.save-btn:disabled {
  opacity: 0.8;
  // Quitar el spinner ::after si se usa el overlay global
}
</style>
