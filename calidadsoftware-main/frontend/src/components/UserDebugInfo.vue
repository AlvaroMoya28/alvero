<template>
  <div class="user-debug-info" v-if="showDebug">
    <div class="debug-header">
      <h6>Debug Info</h6>
      <button @click="toggleDebug" class="btn-close-debug">×</button>
    </div>
    <div class="debug-content">
      <div class="debug-item">
        <strong>Token:</strong>
        <span :class="tokenPresent ? 'text-success' : 'text-danger'">
          {{ tokenPresent ? 'Presente' : 'Ausente' }}
        </span>
      </div>
      <div class="debug-item" v-if="user">
        <strong>Usuario:</strong> {{ user.nombre || user.email || 'N/A' }}
      </div>
      <div class="debug-item" v-if="user">
        <strong>Rol:</strong>
        <span class="badge" :class="getRoleBadgeClass(user.rol)">
          {{ user.rol }}
        </span>
      </div>
      <div class="debug-item" v-if="user">
        <strong>ID Usuario:</strong> {{ user.id || 'N/A' }}
      </div>
      <div class="debug-item" v-if="tokenExpiry">
        <strong>Token expira:</strong>
        <span :class="isTokenExpiringSoon ? 'text-warning' : 'text-success'">
          {{ tokenExpiry }}
        </span>
      </div>
      <button @click="refreshToken" class="btn btn-sm btn-primary mt-2">
        Recargar Info
      </button>
    </div>
  </div>
  <button @click="toggleDebug" class="btn-debug-toggle" v-else>
    <i class="bi bi-bug"></i>
  </button>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { jwtDecode } from 'jwt-decode'

const showDebug = ref(false)
const tokenPresent = ref(false)
const user = ref(null)
const tokenExpiry = ref(null)
const isTokenExpiringSoon = ref(false)

const toggleDebug = () => {
  showDebug.value = !showDebug.value
  if (showDebug.value) {
    refreshToken()
  }
}

const getRoleBadgeClass = (rol) => {
  const roleColors = {
    ADMINISTRADOR: 'bg-danger',
    SUPERUSUARIO: 'bg-purple',
    TECNICO: 'bg-info',
    CLIENTE: 'bg-secondary'
  }
  return roleColors[rol] || 'bg-secondary'
}

const refreshToken = () => {
  const token = localStorage.getItem('token')
  tokenPresent.value = !!token

  if (token) {
    try {
      const decoded = jwtDecode(token)
      console.log('Token decodificado:', decoded)

      // Extraer información del token
      user.value = {
        id: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
            decoded.nameid ||
            decoded.sub,
        nombre: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
                decoded.name,
        email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ||
               decoded.email,
        rol: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
             decoded.role
      }

      // Calcular expiración
      if (decoded.exp) {
        const expiryDate = new Date(decoded.exp * 1000)
        tokenExpiry.value = expiryDate.toLocaleString('es-ES')

        const now = Date.now()
        const timeUntilExpiry = decoded.exp * 1000 - now
        const minutesUntilExpiry = timeUntilExpiry / 1000 / 60

        isTokenExpiringSoon.value = minutesUntilExpiry < 30
      }
    } catch (error) {
      console.error('Error decodificando token:', error)
      user.value = null
      tokenExpiry.value = null
    }
  } else {
    user.value = null
    tokenExpiry.value = null
  }
}

onMounted(() => {
  refreshToken()
})
</script>

<style scoped>
.user-debug-info {
  position: fixed;
  bottom: 20px;
  right: 20px;
  background: white;
  border: 2px solid #667eea;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 10000;
  min-width: 300px;
  max-width: 400px;
}

.debug-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  padding-bottom: 8px;
  border-bottom: 1px solid #e9ecef;
}

.debug-header h6 {
  margin: 0;
  color: #667eea;
  font-weight: 600;
}

.btn-close-debug {
  background: none;
  border: none;
  font-size: 24px;
  color: #6c757d;
  cursor: pointer;
  padding: 0;
  line-height: 1;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-close-debug:hover {
  color: #dc3545;
}

.debug-content {
  font-size: 0.9rem;
}

.debug-item {
  margin-bottom: 8px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 8px;
}

.debug-item strong {
  color: #495057;
  white-space: nowrap;
}

.text-success {
  color: #198754;
  font-weight: 500;
}

.text-danger {
  color: #dc3545;
  font-weight: 500;
}

.text-warning {
  color: #ffc107;
  font-weight: 500;
}

.badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: 500;
  color: white;
}

.bg-danger {
  background-color: #dc3545;
}

.bg-purple {
  background-color: #6f42c1;
}

.bg-info {
  background-color: #0dcaf0;
}

.bg-secondary {
  background-color: #6c757d;
}

.btn-debug-toggle {
  position: fixed;
  bottom: 20px;
  right: 20px;
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background: #667eea;
  color: white;
  border: none;
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  z-index: 10000;
  transition: all 0.3s ease;
}

.btn-debug-toggle:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 16px rgba(102, 126, 234, 0.4);
}

.btn-sm {
  padding: 6px 12px;
  font-size: 0.875rem;
  border-radius: 6px;
  border: none;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: #0d6efd;
  color: white;
}

.btn-primary:hover {
  background: #0b5ed7;
}

.mt-2 {
  margin-top: 8px;
}
</style>
