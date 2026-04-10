<template>
  <div class="tech-layout">
    <aside class="sidebar">
      <div class="sidebar-header">
        <h3>Panel Técnico</h3>
      </div>

      <nav class="sidebar-nav">
        <router-link to="/tecnico/dashboard" class="nav-item">
          <i class="bi bi-speedometer2"></i>
          <span>Dashboard</span>
        </router-link>
        <router-link to="/tecnico/mi-horario" class="nav-item">
          <i class="bi bi-calendar-week"></i>
          <span>Mi Horario</span>
        </router-link>
      </nav>

      <div class="sidebar-footer">
        <button @click="cerrarSesion" class="btn-logout">
          <i class="bi bi-box-arrow-right"></i>
          <span>Cerrar Sesión</span>
        </button>
      </div>
    </aside>

    <main class="main-content">
      <slot />
    </main>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/userStore'

const router = useRouter()
const userStore = useUserStore()

const cerrarSesion = () => {
  userStore.logout()
  router.push('/logIn')
}
</script>

<style scoped>
.tech-layout {
  display: flex;
  min-height: 100vh;
  background: #f5f7fa;
}

.sidebar {
  width: 260px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  display: flex;
  flex-direction: column;
  position: fixed;
  height: 100vh;
  left: 0;
  top: 0;
  z-index: 1000;
}

.sidebar-header {
  padding: 24px 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.sidebar-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.sidebar-nav {
  flex: 1;
  padding: 20px 0;
  overflow-y: auto;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 20px;
  color: rgba(255, 255, 255, 0.85);
  text-decoration: none;
  transition: all 0.3s ease;
  border-left: 3px solid transparent;
}

.nav-item:hover {
  background: rgba(255, 255, 255, 0.1);
  color: white;
}

.nav-item.router-link-active {
  background: rgba(255, 255, 255, 0.15);
  border-left-color: white;
  color: white;
}

.nav-item i {
  font-size: 1.25rem;
  width: 24px;
  text-align: center;
}

.nav-item span {
  font-weight: 500;
}

.sidebar-footer {
  padding: 20px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.btn-logout {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 12px;
  background: rgba(255, 255, 255, 0.1);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 0.95rem;
  font-weight: 500;
}

.btn-logout:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.btn-logout i {
  font-size: 1.1rem;
}

.main-content {
  margin-left: 260px;
  flex: 1;
  padding: 24px;
  min-height: 100vh;
}

@media (max-width: 768px) {
  .sidebar {
    width: 100%;
    height: auto;
    position: relative;
  }

  .sidebar-nav {
    display: flex;
    padding: 0;
  }

  .nav-item {
    flex: 1;
    justify-content: center;
    border-left: none;
    border-bottom: 3px solid transparent;
  }

  .nav-item.router-link-active {
    border-left-color: transparent;
    border-bottom-color: white;
  }

  .nav-item span {
    display: none;
  }

  .main-content {
    margin-left: 0;
    padding: 16px;
  }
}
</style>
