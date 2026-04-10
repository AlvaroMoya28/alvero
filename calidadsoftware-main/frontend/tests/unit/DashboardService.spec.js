/* global describe, it, expect, beforeEach, jest */
import axios from 'axios'

jest.mock('axios')

let dashboardService

describe('dashboardService', () => {
  beforeEach(() => {
    jest.clearAllMocks()
    axios.get.mockReset()
    // Importar dinámicamente después de mockear axios
    dashboardService = require('@/services/DashboardService.js').dashboardService
  })

  it('getMetrics retorna métricas correctamente', async () => {
    // Mock de respuesta de usuarios
    axios.get.mockResolvedValueOnce({
      data: [
        { idUnico: '1', tipoUsuario: 'ADMINISTRADOR', createdAt: new Date().toISOString() },
        { idUnico: '2', tipoUsuario: 'CLIENTE', createdAt: new Date().toISOString() }
      ]
    })
    
    const result = await dashboardService.getMetrics()
    expect(axios.get).toHaveBeenCalledWith('https://localhost:5001/api/usuarios')
    expect(result.metrics.totalUsers).toBe(2)
  })

  it('getMetrics propaga errores de axios', async () => {
    axios.get.mockRejectedValueOnce(new Error('Network Error'))
    await expect(dashboardService.getMetrics()).rejects.toThrow()
  })
})
