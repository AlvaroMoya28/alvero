<template>
  <div class="admin-dashboard">
    <!-- Barra lateral -->
    <aside class="sidebar">
      <div class="sidebar-header">
        <h2>Admin Panel</h2>
      </div>
        <nav class="sidebar-nav">
          <router-link to="/admin/dashboard" class="nav-item" :class="{ active: $route.path === '/admin/dashboard' }">
            <i class="bi bi-speedometer2"></i>
            <span>Dashboard</span>
          </router-link>
          <router-link to="/admin/dashboard/users" class="nav-item" :class="{ active: $route.path === '/admin/dashboard/users' }">
            <i class="bi bi-people"></i>
            <span>Usuarios</span>
          </router-link>
          <router-link to="/admin/panel/registerUser" class="nav-item" :class="{ active: $route.path === '/admin/panel/registerUser' }">
            <i class="bi bi-person-plus"></i>
            <span>Registrar Administrador</span>
          </router-link>
          <router-link v-if="isAdmin" to="/admin/create-tech" class="nav-item" :class="{ active: $route.path === '/admin/create-tech' }">
            <i class="bi bi-person-badge"></i>
            <span>Registrar Técnico</span>
          </router-link>
          <router-link to="/admin/horarios-tecnicos" class="nav-item" :class="{ active: $route.path === '/admin/horarios-tecnicos' }">
            <i class="bi bi-calendar-week"></i>
            <span>Horarios Técnicos</span>
          </router-link>
        </nav>
    </aside>

    <!-- Contenido principal -->
    <main class="main-content">
      <!-- Contenido específico de cada vista -->
      <slot></slot>
    </main>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useUserStore } from '@/stores/userStore'

const userStore = useUserStore()

const isAdmin = computed(() => {
  const tipo = userStore.user?.tipoUsuario
  return tipo === 'ADMINISTRADOR' || tipo === 'SUPERUSUARIO'
})

</script>

<style scoped>
.admin-dashboard {
  display: flex;
  min-height: 100vh;
  background-color: var(--dp-bg);
}

.sidebar {
  width: 250px;
  background-color: var(--dp-darkest);
  color: white;
  display: flex;
  flex-direction: column;
  padding: 1.5rem 0;
  box-shadow: 2px 0 10px rgba(0, 0, 0, 0.1);
}

.sidebar-header {
  padding: 0 1.5rem 1.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);

  h2 {
    margin: 0;
    font-size: 1.3rem;
    font-weight: 600;
  }
}

.sidebar-nav {
  flex: 1;
  padding: 1.5rem 0;
}

.nav-item {
  display: flex;
  align-items: center;
  padding: 0.8rem 1.5rem;
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  transition: all 0.2s ease;
  font-size: 0.95rem;

  i {
    margin-right: 0.8rem;
    font-size: 1.1rem;
  }

  &:hover {
    background-color: rgba(255, 255, 255, 0.1);
    color: white;
  }

  &.active {
    background-color: rgba(255, 255, 255, 0.2);
    color: white;
    border-left: 3px solid var(--dp-accent);
  }
}

.sidebar-footer {
  padding: 1.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.btn-logout {
  display: flex;
  align-items: center;
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.8);
  cursor: pointer;
  font-size: 0.9rem;
  transition: color 0.2s ease;

  i {
    margin-right: 0.5rem;
  }

  &:hover {
    color: white;
  }
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.top-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  background-color: white;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.05);
}

.search-bar {
  display: flex;
  align-items: center;
  background-color: var(--dp-bg);
  border-radius: 8px;
  padding: 0.5rem 1rem;
  width: 300px;

  i {
    color: #6c757d;
    margin-right: 0.5rem;
  }

  input {
    border: none;
    background: none;
    outline: none;
    width: 100%;
    font-size: 0.9rem;
  }
}

.user-info {
  display: flex;
  align-items: center;
  gap: 1rem;

  .username {
    font-weight: 500;
    color: #333;
  }

  .avatar {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background-color: #e9ecef;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #6c757d;

    i {
      font-size: 1.5rem;
    }
  }
}

.top-bar {
  background-color: white;
  padding: 15px 25px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.content-container {
  flex: 1;
  padding: 2rem;
  overflow-y: auto;
}

.page-title {
  font-size: 1.8rem;
  color: var(--dp-primary);
  margin-bottom: 0.5rem;
}

.page-subtitle {
  color: #6c757d;
  margin-bottom: 2rem;
  font-size: 1rem;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.metric-card {
  background-color: white;
  border-radius: 10px;
  padding: 1.5rem;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
  display: flex;
  justify-content: space-between;
  transition: transform 0.3s ease;

  &:hover {
    transform: translateY(-5px);
  }
}

.metric-info {
  h3 {
    font-size: 0.95rem;
    color: #6c757d;
    margin: 0 0 0.5rem;
    font-weight: 500;
  }

  .metric-value {
    font-size: 1.8rem;
    font-weight: 600;
    color: var(--dp-primary);
    margin: 0.5rem 0;
  }

  .metric-change {
    font-size: 0.8rem;
    margin: 0;

    &.positive {
      color: var(--dp-accent);
    }

    &.negative {
      color: #dc3545;
    }

    i {
      margin-right: 0.3rem;
    }
  }
}

.metric-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 50px;
  height: 50px;
  background-color: rgba(11, 91, 215, 0.1);
  border-radius: 50%;
  color: var(--dp-primary);
  font-size: 1.5rem;
}

.charts-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.tables-grid {
  display: grid;
  gap: 1.5rem;
}

.chart-card, .table-card {
  background-color: white;
  border-radius: 10px;
  padding: 1.5rem;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
}

.chart-card {
  h3 {
    font-size: 1rem;
    color: var(--dp-dark);
    margin-top: 0;
    margin-bottom: 1.5rem;
  }
}

.chart-placeholder {
  height: 300px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #6c757d;
  background-color: #f8f9fa;
  border-radius: 5px;
}

.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;

  h3 {
    font-size: 1rem;
    color: #1A4456;
    margin: 0;
  }

  .view-all {
    font-size: 0.85rem;
    color: var(--dp-primary);
    text-decoration: none;
    transition: color 0.2s ease;

    &:hover {
      color: var(--dp-dark);
      text-decoration: underline;
    }
  }
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;

  th {
    text-align: left;
    padding: 0.75rem 1rem;
    background-color: #f8f9fa;
    color: #495057;
    font-weight: 600;
    border-bottom: 1px solid #e9ecef;
  }

  td {
    padding: 0.75rem 1rem;
    border-bottom: 1px solid #e9ecef;
    color: #495057;
  }

  tr:last-child td {
    border-bottom: none;
  }

  tr:hover td {
    background-color: #f8f9fa;
  }
}

.role-badge {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;

  &.admin {
    background-color: #d4edda;
    color: #155724;
  }

  &.user {
    background-color: #e2e3e5;
    color: #383d41;
  }

  &.moderator {
    background-color: #fff3cd;
    color: #856404;
  }
}

.table-action {
  background: none;
  border: none;
  color: #6c757d;
  cursor: pointer;
  margin-right: 0.5rem;
  font-size: 1rem;
  transition: color 0.2s ease;

  &:hover {
    color: var(--dp-primary);
  }
}

.log-details {
  max-width: 200px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

@media (max-width: 1200px) {
  .charts-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .sidebar {
    width: 70px;
    padding: 1rem 0;
  }

  .sidebar-header h2,
  .nav-item span,
  .btn-logout span {
    display: none;
  }

  .nav-item {
    justify-content: center;
    padding: 0.8rem 0;

    i {
      margin-right: 0;
      font-size: 1.3rem;
    }
  }

  .sidebar-footer {
    padding: 1rem;
  }

  .top-bar {
    padding: 1rem;
  }

  .search-bar {
    width: auto;
    input {
      display: none;
    }
  }

  .content-container {
    padding: 1rem;
  }

  .metrics-grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 576px) {
  .metrics-grid {
    grid-template-columns: 1fr;
  }
}
</style>
