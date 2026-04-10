<template>
  <div class="tech-dashboard">
    <h2>Dashboard del Técnico</h2>

    <div class="stats-grid">
      <div class="stat-card pendiente">
        <div class="stat-icon">
          <i class="bi bi-clock-history"></i>
        </div>
        <div class="stat-content">
          <h3>{{ stats.pendientes }}</h3>
          <p>Citas Pendientes</p>
        </div>
      </div>

      <div class="stat-card confirmada">
        <div class="stat-icon">
          <i class="bi bi-check-circle"></i>
        </div>
        <div class="stat-content">
          <h3>{{ stats.confirmadas }}</h3>
          <p>Citas Confirmadas</p>
        </div>
      </div>

      <div class="stat-card completada">
        <div class="stat-icon">
          <i class="bi bi-check-all"></i>
        </div>
        <div class="stat-content">
          <h3>{{ stats.completadas }}</h3>
          <p>Citas Completadas</p>
        </div>
      </div>

      <div class="stat-card total">
        <div class="stat-icon">
          <i class="bi bi-calendar-month"></i>
        </div>
        <div class="stat-content">
          <h3>{{ stats.totalMes }}</h3>
          <p>Total del Mes</p>
        </div>
      </div>
    </div>

    <div class="quick-actions">
      <h3>Acciones Rápidas</h3>
      <div class="action-buttons">
        <router-link to="/tecnico/mi-horario" class="action-btn">
          <i class="bi bi-calendar-week"></i>
          <span>Ver Mi Horario</span>
        </router-link>
      </div>
    </div>

    <div v-if="proximasCitas.length > 0" class="proximas-citas">
      <h3>Próximas Citas</h3>
      <div class="citas-list">
        <div v-for="cita in proximasCitas" :key="cita.idCita" class="cita-item">
          <div class="cita-icon">
            <i class="bi bi-calendar-event"></i>
          </div>
          <div class="cita-info">
            <div class="cita-cliente">{{ cita.nombreCliente }}</div>
            <div class="cita-fecha">
              {{ formatearFecha(cita.fechaCita) }} - {{ cita.horaInicio }}
            </div>
          </div>
          <div class="cita-estado">
            <span :class="`badge badge-${cita.estado.toLowerCase()}`">
              {{ cita.estado }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import CitaService from '@/services/CitaService'

const stats = ref({
  pendientes: 0,
  confirmadas: 0,
  completadas: 0,
  totalMes: 0
})

const proximasCitas = ref([])

const cargarEstadisticas = async () => {
  try {
    const now = new Date()
    const primerDiaMes = new Date(now.getFullYear(), now.getMonth(), 1)
    const ultimoDiaMes = new Date(now.getFullYear(), now.getMonth() + 1, 0)

    const fechaDesde = primerDiaMes.toISOString().split('T')[0]
    const fechaHasta = ultimoDiaMes.toISOString().split('T')[0]

    const citas = await CitaService.obtenerMisCitasTecnico(fechaDesde, fechaHasta)

    stats.value.pendientes = citas.filter(c => c.estado === 'PENDIENTE').length
    stats.value.confirmadas = citas.filter(c => c.estado === 'CONFIRMADA').length
    stats.value.completadas = citas.filter(c => c.estado === 'COMPLETADA').length
    stats.value.totalMes = citas.length

    // Obtener próximas 5 citas
    const hoy = new Date()
    hoy.setHours(0, 0, 0, 0)
    proximasCitas.value = citas
      .filter(c => new Date(c.fechaCita) >= hoy && (c.estado === 'PENDIENTE' || c.estado === 'CONFIRMADA'))
      .sort((a, b) => new Date(a.fechaCita) - new Date(b.fechaCita))
      .slice(0, 5)
  } catch (error) {
    console.error('Error cargando estadísticas:', error)
  }
}

const formatearFecha = (fecha) => {
  const date = new Date(fecha)
  return date.toLocaleDateString('es-ES', {
    weekday: 'short',
    day: '2-digit',
    month: 'short'
  })
}

onMounted(() => {
  cargarEstadisticas()
})
</script>

<style scoped>
.tech-dashboard {
  padding: 24px;
  max-width: 1200px;
  margin: 0 auto;
}

h2 {
  font-size: 2rem;
  color: #2c3e50;
  margin-bottom: 32px;
  font-weight: 600;
}

h3 {
  font-size: 1.5rem;
  color: #2c3e50;
  margin-bottom: 20px;
  font-weight: 600;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 24px;
  margin-bottom: 40px;
}

.stat-card {
  background: white;
  border-radius: 12px;
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s, box-shadow 0.2s;
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 28px;
  color: white;
}

.stat-card.pendiente .stat-icon {
  background: linear-gradient(135deg, #ffc107 0%, #ff9800 100%);
}

.stat-card.confirmada .stat-icon {
  background: linear-gradient(135deg, #0d6efd 0%, #0b5ed7 100%);
}

.stat-card.completada .stat-icon {
  background: linear-gradient(135deg, #198754 0%, #157347 100%);
}

.stat-card.total .stat-icon {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stat-content h3 {
  font-size: 2.5rem;
  font-weight: 700;
  margin: 0;
  color: #2c3e50;
}

.stat-content p {
  margin: 4px 0 0 0;
  color: #6c757d;
  font-size: 0.95rem;
}

.quick-actions {
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 40px;
}

.action-buttons {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
}

.action-btn {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px 24px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  text-decoration: none;
  border-radius: 8px;
  font-weight: 500;
  transition: transform 0.2s, box-shadow 0.2s;
}

.action-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.action-btn i {
  font-size: 1.5rem;
}

.proximas-citas {
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.citas-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.cita-item {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
  transition: background 0.2s;
}

.cita-item:hover {
  background: #e9ecef;
}

.cita-icon {
  width: 48px;
  height: 48px;
  border-radius: 8px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.5rem;
}

.cita-info {
  flex: 1;
}

.cita-cliente {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 4px;
}

.cita-fecha {
  font-size: 0.9rem;
  color: #6c757d;
}

.cita-estado {
  margin-left: auto;
}

.badge {
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 500;
}

.badge-pendiente {
  background: #ffc107;
  color: #000;
}

.badge-confirmada {
  background: #0d6efd;
  color: white;
}

.badge-completada {
  background: #198754;
  color: white;
}

@media (max-width: 768px) {
  .tech-dashboard {
    padding: 16px;
  }

  h2 {
    font-size: 1.5rem;
    margin-bottom: 24px;
  }

  h3 {
    font-size: 1.25rem;
  }

  .stats-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }

  .stat-content h3 {
    font-size: 2rem;
  }

  .cita-item {
    flex-wrap: wrap;
  }

  .cita-estado {
    margin-left: 0;
    width: 100%;
  }
}
</style>
