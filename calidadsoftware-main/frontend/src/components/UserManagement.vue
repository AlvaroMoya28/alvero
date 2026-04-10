<template>
  <AdminLayout>
    <div class="content-container">
      <div class="users-management">
        <div class="header">
          <h1 class="page-title">Gestión de Usuarios</h1>
          <div class="search-filter">
            <input v-model="searchQuery" type="text" placeholder="Buscar usuarios..." class="search-input">
            <select v-model="roleFilter" class="role-filter">
              <option value="">Todos los roles</option>
              <option v-for="role in availableRoles" :key="role" :value="role">{{ role }}</option>
            </select>
            <select v-model="statusFilter" class="status-filter">
              <option value="ACTIVO">Activos</option>
              <option value="INACTIVO">Inactivos</option>
              <option value="">Todos</option>
            </select>
          </div>
        </div>

        <div class="users-table-container">
          <table class="users-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Nombre</th>
                <th>Email</th>
                <th>Teléfono</th>
                <th>Rol</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="user in filteredUsers" :key="user.idUsuario">
                <td>{{ user.idUsuario }}</td>
                <td>{{ user.nombre }} {{ user.apellido1 }} {{ user.apellido2 }}</td>
                <td>{{ user.email }}</td>
                <td>{{ user.telefono || 'N/A' }}</td>
                <td>{{ user.tipoUsuario }}</td>
                <td>
                  <span :class="['status-badge', user.estado.toLowerCase()]">
                    {{ user.estado }}
                  </span>
                </td>
                <td class="actions">
                  <button
                    @click="showEditModal(user)"
                    class="action-btn"
                    title="Editar usuario"
                    :disabled="!canEditUser(user)"
                  >
                    <i class="bi bi-pencil-square"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>

          <div v-if="loading" class="loading-spinner">
            <i class="bi bi-arrow-repeat"></i> Cargando usuarios...
          </div>
          <div v-if="!loading && users.length === 0" class="no-users">
            No se encontraron usuarios
          </div>
        </div>

        <!-- Modal de edición -->
        <div v-if="showModal" class="modal-overlay">
          <div class="modal-content">
            <div class="modal-header">
              <h2>Editar Usuario</h2>
              <button @click="closeModal" class="close-btn">&times;</button>
            </div>
            <div class="modal-body">
              <form @submit.prevent="saveUserChanges">
                <div class="form-group">
                  <label>Nombre</label>
                  <input v-model="editingUser.nombre" type="text" required>
                </div>
                <div class="form-group">
                  <label>Apellido 1</label>
                  <input v-model="editingUser.apellido1" type="text" required>
                </div>
                <div class="form-group">
                  <label>Apellido 2</label>
                  <input v-model="editingUser.apellido2" type="text">
                </div>
                <div class="form-group">
                  <label>Email</label>
                  <input v-model="editingUser.email" type="email" required>
                </div>
                <div class="form-group">
                  <label>Teléfono</label>
                  <input v-model="editingUser.telefono" type="tel">
                </div>
                <div class="form-group" v-if="canEditRole(editingUser)">
                  <label>Rol</label>
                  <select v-model="editingUser.tipoUsuario" class="role-select">
                    <option v-for="role in availableRoles" :key="role" :value="role">{{ role }}</option>
                  </select>
                </div>
                <div class="form-actions">
                  <button type="button" @click="closeModal" class="btn-cancel">Cancelar</button>
                  <button type="submit" class="btn-save">Guardar Cambios</button>
                  <button
                    type="button"
                    @click="deactivateUser"
                    class="btn-delete"
                    v-if="editingUser.estado === 'ACTIVO'"
                  >
                    Eliminar Usuario
                  </button>
                  <button
                    type="button"
                    @click="reactivateUser"
                    class="btn-reactivate"
                    v-if="editingUser.estado === 'INACTIVO'"
                  >
                    Reactivar Usuario
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
        <!-- Modal de confirmación (similar al de tu perfil) -->
        <div v-if="showSuccessModal" class="modal" @click.self="closeSuccessModal">
          <div class="modalContent success-modal">
            <i class="bi bi-check-circle-fill"></i>
            <h2>{{ successMessage }}</h2>
            <button @click="closeSuccessModal" class="btn-confirm">Aceptar</button>
          </div>
        </div>

        <!-- Loading overlay (opcional, similar al de tu perfil) -->
        <div v-if="isLoading" class="loading-overlay">
          <div class="loading-content">
            <div class="spinner"></div>
            <p>{{ loadingMessage }}</p>
          </div>
        </div>
      </div>
    </div>
  </AdminLayout>
</template>

<script setup>
import AdminLayout from '@/components/AdminLayout.vue'
import { ref, computed, onMounted } from 'vue'
import { usuarioService } from '@/services/UserService'
import { useToast } from 'vue-toastification'
import { useUserStore } from '@/stores/userStore'

const toast = useToast()
const userStore = useUserStore()

// Estados para el modal de éxito
const showSuccessModal = ref(false)
const successMessage = ref('')
const isLoading = ref(false)
const loadingMessage = ref('')

// Función para abrir el modal de éxito
const openSuccessModal = (message) => {
  successMessage.value = message
  showSuccessModal.value = true
}

// Función para cerrar el modal de éxito
const closeSuccessModal = () => {
  showSuccessModal.value = false
  successMessage.value = ''
}

// Datos
const users = ref([])
const loading = ref(true)
const searchQuery = ref('')
const roleFilter = ref('')
const showModal = ref(false)
const editingUser = ref(null)
const statusFilter = ref('ACTIVO')

// Roles disponibles (agregado 'TECNICO' para que se muestren los técnicos)
const availableRoles = ref(['SUPERUSUARIO', 'ADMINISTRADOR', 'TECNICO', 'CLIENTE'])

// Obtener usuarios al montar el componente
onMounted(async () => {
  try {
    loading.value = true
    const response = await usuarioService.getAllUsers()
    users.value = response
  } catch (error) {
    toast.error('Error al cargar usuarios: ' + error.message)
  } finally {
    loading.value = false
  }
})

const deactivateUser = async () => {
  try {
    if (!canEditUser(editingUser.value)) {
      toast.warning('No tienes permisos para eliminar este usuario')
      return
    }

    isLoading.value = true
    loadingMessage.value = 'Desactivando usuario...'

    // eslint-disable-next-line
    const response = await usuarioService.updateUser(editingUser.value.idUsuario, {
      estado: 'INACTIVO'
    })
    // Actualizar localmente el estado en la lista de usuarios
    const index = users.value.findIndex(u => u.idUsuario === editingUser.value.idUsuario)
    if (index !== -1) {
      users.value[index].estado = 'INACTIVO'
    }

    openSuccessModal(`Usuario ${editingUser.value.nombre} desactivado correctamente`)
    closeModal()
  } catch (error) {
    toast.error('Error al desactivar usuario: ' + error.message)
  } finally {
    isLoading.value = false
  }
}

const reactivateUser = async () => {
  try {
    if (!canEditUser(editingUser.value)) {
      toast.warning('No tienes permisos para reactivar este usuario')
      return
    }

    isLoading.value = true
    loadingMessage.value = 'Reactivando usuario...'

    // eslint-disable-next-line
    const response = await usuarioService.updateUser(editingUser.value.idUsuario, {
      estado: 'ACTIVO'
    })
    // Actualizar localmente el estado en la lista
    const index = users.value.findIndex(u => u.idUsuario === editingUser.value.idUsuario)
    if (index !== -1) {
      users.value[index].estado = 'ACTIVO'
    }

    openSuccessModal(`Usuario ${editingUser.value.nombre} reactivado correctamente`)
    closeModal()
  } catch (error) {
    toast.error('Error al reactivar usuario: ' + error.message)
  } finally {
    isLoading.value = false
  }
}

// Verificar si el usuario actual puede editar a otro usuario
const canEditUser = (user) => {
  const currentUser = userStore.user
  if (!currentUser) return false
  // Solo superusuarios pueden editar otros superusuarios
  if (user.tipoUsuario === 'SUPERUSUARIO' && currentUser.tipoUsuario !== 'SUPERUSUARIO') {
    return false
  }
  // No puedes editarte a ti mismo desde aquí (debería hacerse en un perfil)
  return user.idUsuario !== currentUser.idUsuario
}

// Verificar si el usuario actual puede editar el rol de otro usuario
const canEditRole = (user) => {
  const currentUser = userStore.user
  if (!currentUser) return false
  // Solo superusuarios pueden cambiar roles
  if (currentUser.tipoUsuario !== 'SUPERUSUARIO') {
    return false
  }
  // No puedes cambiar tu propio rol
  return user.idUsuario !== currentUser.idUsuario
}

// Filtrar usuarios basado en búsqueda y filtro de rol
const filteredUsers = computed(() => {
  const currentUser = userStore.user

  return users.value.filter(user => {
    const matchesSearch =
      user.nombre.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      user.email.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      user.idUsuario.toString().includes(searchQuery.value)

    const matchesRole = roleFilter.value ? user.tipoUsuario === roleFilter.value : true
    const matchesStatus = statusFilter.value ? user.estado === statusFilter.value : true

    const canSeeUser =
      !currentUser || currentUser.tipoUsuario === 'SUPERUSUARIO' ||
      user.tipoUsuario !== 'SUPERUSUARIO'

    return matchesSearch && matchesRole && matchesStatus && canSeeUser
  })
})

// Mostrar modal de edición
const showEditModal = (user) => {
  if (!canEditUser(user)) {
    toast.warning('No tienes permisos para editar este usuario')
    return
  }
  editingUser.value = { ...user }
  showModal.value = true
}

// Cerrar modal
const closeModal = () => {
  showModal.value = false
}

// Guardar cambios del usuario
const saveUserChanges = async () => {
  try {
    isLoading.value = true
    loadingMessage.value = 'Guardando cambios...'

    // Preparar datos para enviar (excluir campos que no se deben actualizar)
    const updateData = {
      nombre: editingUser.value.nombre,
      apellido1: editingUser.value.apellido1,
      apellido2: editingUser.value.apellido2,
      email: editingUser.value.email,
      telefono: editingUser.value.telefono
    }

    // Solo incluir el rol si el usuario tiene permisos para cambiarlo
    if (canEditRole(editingUser.value)) {
      updateData.tipoUsuario = editingUser.value.tipoUsuario
    }

    const response = await usuarioService.updateUser(editingUser.value.idUsuario, updateData)

    // Actualizar la lista de usuarios
    const index = users.value.findIndex(u => u.idUsuario === editingUser.value.idUsuario)
    if (index !== -1) {
      users.value[index] = { ...users.value[index], ...response }
    }

    openSuccessModal('Usuario actualizado correctamente')
    closeModal()
  } catch (error) {
    toast.error('Error al actualizar usuario: ' + error.message)
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.btn-reactivate {
  background-color: var(--dp-accent);
  color: white;
  padding: 0.5rem 1rem;
  margin-left: 0.5rem;
  border: none;
  cursor: pointer;
  border-radius: 4px;
}

.btn-delete {
  background-color: #e74c3c;
  color: white;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  margin-left: auto;
}

.btn-delete:hover {
  background-color: #c0392b;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modalContent {
  background-color: white;
  padding: 2rem;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.success-modal {
  text-align: center;
  padding: 2rem;
}

.success-modal i {
  font-size: 3rem;
  color: var(--dp-accent);
  margin-bottom: 1rem;
}

.success-modal h2 {
  margin-bottom: 1.5rem;
  color: #333;
}

.btn-confirm {
  background-color: var(--dp-primary);
  color: white;
  border: none;
  padding: 0.5rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.3s;
}

.btn-confirm:hover {
  background-color: var(--dp-dark);
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(255, 255, 255, 0.8);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.loading-content {
  text-align: center;
}

.spinner {
  border: 4px solid rgba(0, 0, 0, 0.1);
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border-left-color: var(--dp-primary);
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.role-select:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
  opacity: 0.7;
}

.action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.action-btn:disabled:hover {
  background-color: transparent;
  color: var(--dp-primary);
}

.content-container {
  padding: 20px;
  flex: 1;
  overflow-y: auto;
}

.users-management {
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  padding: 32px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-title {
  font-size: 24px;
  color: var(--dp-primary);
  margin: 0;
}

.search-filter {
  display: flex;
  gap: 10px;
}

.search-input, .role-filter {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.search-input, .status-filter {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.users-management {
  padding: 20px;
  background-color: var(--dp-bg);
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.search-filter {
  display: flex;
  gap: 10px;
}

.search-input, .role-filter {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.users-table-container {
  overflow-x: auto;
}

.users-table {
  width: 100%;
  border-collapse: collapse;
  background-color: white;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.users-table th, .users-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

.users-table th {
  background-color: var(--dp-primary);
  color: white;
  font-weight: 500;
}

.users-table tr:hover {
  background-color: #f1f1f1;
}

.role-select {
  padding: 6px 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: white;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  text-transform: uppercase;
}

.status-badge.activo {
  background-color: #e6f7e6;
  color: #2e7d32;
}

.status-badge.inactivo {
  background-color: #ffebee;
  color: #c62828;
}

.actions {
  display: flex;
  gap: 8px;
}

.action-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: var(--dp-primary);
  font-size: 16px;
  padding: 5px;
  border-radius: 4px;
  transition: all 0.2s;
}

.action-btn:hover {
  background-color: rgba(11,91,215,0.08);
  color: var(--dp-dark);
}

.loading-spinner, .no-users {
  text-align: center;
  padding: 20px;
  color: #666;
}

/* Modal styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background-color: white;
  border-radius: 8px;
  width: 500px;
  max-width: 90%;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.modal-header {
  padding: 16px 20px;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h2 {
  margin: 0;
  font-size: 20px;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #666;
}

.modal-body {
  padding: 20px;
}

.form-group {
  margin-bottom: 16px;
}

.form-group label {
  display: block;
  margin-bottom: 6px;
  font-weight: 500;
}

.form-group input {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

.btn-cancel, .btn-save {
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
}

.btn-cancel {
  background-color: #f5f5f5;
  color: #333;
  border: 1px solid #ddd;
}

.btn-save {
  background-color: #1A4456;
  color: white;
  border: none;
}

.h1, h1 {
    font-size: 1.8rem;
    margin-bottom: 0.5rem;
    color: #1A4456;
}

.btn-save:hover {
  background-color: #0d2e3d;
}
</style>
