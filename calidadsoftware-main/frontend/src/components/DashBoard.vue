<template>
  <div class="admin-dashboard">
   <AdminLayout>

    <!-- Main Content -->
    <div class="main-content">

      <!-- Dashboard Content -->
      <div class="content-container">
        <h1 class="page-title">Panel de Control</h1>
        <p class="page-subtitle">Resumen de actividad y estadísticas</p>

        <!-- Métricas rápidas -->
        <div class="metrics-grid">
          <div class="metric-card">
            <div class="metric-info">
              <h3>Usuarios totales</h3>
              <p class="metric-value">{{ metrics.totalUsers }}</p>
              <p class="metric-change positive" v-if="metrics.newUsersToday > 0">
                <i class="bi bi-arrow-up"></i> {{ metrics.newUsersToday }} hoy
              </p>
              <p class="metric-change neutral" v-else>
                <i class="bi bi-dash"></i> 0 hoy
              </p>
            </div>
            <div class="metric-icon">
              <i class="bi bi-people-fill"></i>
            </div>
          </div>
        </div>

        <!-- Gráficos -->
        <div class="charts-grid">
          <div class="chart-card">
            <h3>Distribución de usuarios</h3>
            <DoughnutChart v-if="!loading && metrics.usersByRole.length > 0" :chartData="rolesChartData" />
            <div v-else class="chart-placeholder">
              {{ loading ? 'Cargando...' : 'No hay datos disponibles' }}
            </div>
          </div>
        </div>

        <!-- Tablas -->
        <div class="tables-grid">
          <div class="table-card">
            <div class="table-header">
              <h3>Últimos usuarios registrados</h3>
              <router-link to="/admin/dashboard/users" class="view-all">Ver todos</router-link>
            </div>
            <table class="admin-table">
              <thead>
                <tr>
                  <th>Nombre</th>
                  <th>Email</th>
                  <th>Rol</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="user in recentUsers" :key="user.idUnico">
                  <td>{{ user.nombre }}</td>
                  <td>{{ user.email }}</td>
                  <td>
                    <span class="role-badge" :class="user.tipoUsuario?.toLowerCase() || 'user'">
                      {{ user.tipoUsuario || 'Usuario' }}
                    </span>
                  </td>
                </tr>
                <tr v-if="recentUsers.length === 0 && !loading">
                  <td colspan="3" class="text-center">No hay usuarios registrados</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
    </AdminLayout>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import DoughnutChart from '@/components/DoughnutChart.vue'
import { dashboardService } from '@/services/DashboardService'
import AdminLayout from './AdminLayout.vue'

// Datos del dashboard
const metrics = ref({
  totalUsers: 0,
  newUsersToday: 0,
  activeEvents: 0,
  usersByRole: []
})

const recentUsers = ref([])
const loading = ref(true)

// Paleta DoctorPC desde CSS variables
const getCssVar = (name) => getComputedStyle(document.documentElement).getPropertyValue(name).trim()
const hexToRgba = (hex, alpha = 1) => {
  const h = hex.replace('#', '')
  const bigint = parseInt(h.length === 3 ? h.split('').map(c => c + c).join('') : h, 16)
  const r = (bigint >> 16) & 255
  const g = (bigint >> 8) & 255
  const b = bigint & 255
  return `rgba(${r}, ${g}, ${b}, ${alpha})`
}
const palette = ref({
  primary: '#0b5bd7',
  accent: '#00c853',
  dark: '#052e5d'
})

// Datos para el gráfico de roles
const rolesChartData = computed(() => ({
  labels: metrics.value.usersByRole.map(r => r.role),
  datasets: [{
    data: metrics.value.usersByRole.map(r => r.count),
    backgroundColor: [
      palette.value.primary,
      palette.value.accent,
      hexToRgba(palette.value.primary, 0.6),
      hexToRgba(palette.value.accent, 0.6),
      palette.value.dark
    ],
    borderWidth: 1
  }]
}))

// Cargar datos al montar el componente
onMounted(async () => {
  // sincronizar paleta desde CSS variables
  const p = getCssVar('--dp-primary') || palette.value.primary
  const a = getCssVar('--dp-accent') || palette.value.accent
  const d = getCssVar('--dp-dark') || palette.value.dark
  palette.value = { primary: p, accent: a, dark: d }
  try {
    loading.value = true
    const data = await dashboardService.getMetrics()

    metrics.value = {
      totalUsers: data.metrics.totalUsers || 0,
      newUsersToday: data.metrics.newUsersToday || 0,
      activeEvents: data.metrics.activeEvents || 0,
      usersByRole: data.metrics.usersByRole || []
    }

    recentUsers.value = data.recentUsers || []
  } catch (err) {
    console.error('Error loading dashboard data:', err)
    // Mostrar valores por defecto en caso de error
    metrics.value = {
      totalUsers: 0,
      newUsersToday: 0,
      activeEvents: 0,
      usersByRole: []
    }
    recentUsers.value = []
  } finally {
    loading.value = false
  }
})
</script>

<style lang="scss" scoped>
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

.charts-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  margin-bottom: 30px;
}

.chart-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
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

.chart-card,
.table-card {
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

  .tables-grid {
    grid-template-columns: 1fr;
  }

  .admin-table {
    font-size: 0.8rem;

    th,
    td {
      padding: 0.5rem;
    }
  }
}
</style>
