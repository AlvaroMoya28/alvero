import axios from 'axios'

const API_URL = 'https://localhost:5001/api'

export const dashboardService = {
  async getMetrics () {
    try {
      // Obtener todos los usuarios
      const usersResponse = await axios.get(`${API_URL}/usuarios`)
      const allUsers = usersResponse.data

      // Calcular métricas
      const today = new Date()
      today.setHours(0, 0, 0, 0)

      // Usuarios registrados hoy
      const newUsersToday = allUsers.filter(user => {
        const userDate = new Date(user.createdAt)
        return userDate >= today
      }).length

      // Distribución por roles
      const usersByRole = {}
      allUsers.forEach(user => {
        const role = user.tipoUsuario || 'Sin rol'
        usersByRole[role] = (usersByRole[role] || 0) + 1
      })

      // Convertir a formato para el gráfico
      const rolesDistribution = Object.keys(usersByRole).map(role => ({
        role,
        count: usersByRole[role]
      }))

      // Ordenar usuarios por fecha de creación (más recientes primero)
      const sortedUsers = [...allUsers].sort((a, b) =>
        new Date(b.createdAt) - new Date(a.createdAt))

      return {
        metrics: {
          totalUsers: allUsers.length,
          newUsersToday,
          activeEvents: 0, // Ya no hay salas
          usersByRole: rolesDistribution
        },
        recentUsers: sortedUsers.slice(0, 10),
        recentSalas: [] // Ya no hay salas
      }
    } catch (error) {
      console.error('Error fetching dashboard data:', error)
      throw error
    }
  }
}
