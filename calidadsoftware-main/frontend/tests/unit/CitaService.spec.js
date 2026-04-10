import CitaService from '@/services/CitaService'
import axios from 'axios'

jest.mock('axios')

describe('CitaService', () => {
  const API_URL = process.env.VUE_APP_API_URL || 'https://localhost:7167/api'

  beforeEach(() => {
    jest.clearAllMocks()
    localStorage.clear()
  })

  describe('obtenerDisponibilidadGeneral', () => {
    it('returns availability data', async () => {
      const mockData = [
        {
          fecha: '2025-11-26',
          horaInicio: '08:00',
          tecnicosDisponibles: 2
        }
      ]

      axios.get.mockResolvedValue({ data: mockData })

      const result = await CitaService.obtenerDisponibilidadGeneral('2025-11-26', '2025-11-30')

      expect(axios.get).toHaveBeenCalledWith(
        `${API_URL}/citas/disponibilidad-general`,
        { params: { fechaDesde: '2025-11-26', fechaHasta: '2025-11-30' } }
      )
      expect(result).toEqual(mockData)
    })

    it('throws error on failed request', async () => {
      axios.get.mockRejectedValue(new Error('Network error'))

      await expect(
        CitaService.obtenerDisponibilidadGeneral('2025-11-26', '2025-11-30')
      ).rejects.toThrow('Network error')
    })
  })

  describe('obtenerHorariosDisponibles', () => {
    it('returns available schedules for technician', async () => {
      const mockData = [
        {
          idHorario: 1,
          fecha: '2025-11-26',
          horaInicio: '08:00',
          horaFin: '09:00',
          disponibleReal: true
        }
      ]

      axios.get.mockResolvedValue({ data: mockData })

      const result = await CitaService.obtenerHorariosDisponibles('TEC001', '2025-11-26', '2025-11-30')

      expect(axios.get).toHaveBeenCalledWith(
        `${API_URL}/citas/horarios-disponibles/TEC001`,
        { params: { fechaDesde: '2025-11-26', fechaHasta: '2025-11-30' } }
      )
      expect(result).toEqual(mockData)
    })
  })

  describe('obtenerMisCitasTecnico', () => {
    it('returns technician appointments', async () => {
      const mockData = [
        {
          idCita: 1,
          nombreCliente: 'Juan Pérez',
          fechaCita: '2025-11-26',
          estado: 'PENDIENTE'
        }
      ]

      axios.get.mockResolvedValue({ data: mockData })

      const result = await CitaService.obtenerMisCitasTecnico('2025-11-26', '2025-11-30')

      expect(axios.get).toHaveBeenCalledWith(
        `${API_URL}/citas/tecnico/mis-citas`,
        { params: { fechaDesde: '2025-11-26', fechaHasta: '2025-11-30' } }
      )
      expect(result).toEqual(mockData)
    })
  })

  describe('obtenerCitasTecnico', () => {
    it('returns appointments for specific technician', async () => {
      const mockData = [
        {
          idCita: 2,
          nombreCliente: 'María López',
          fechaCita: '2025-11-27',
          estado: 'CONFIRMADA'
        }
      ]

      axios.get.mockResolvedValue({ data: mockData })

      const result = await CitaService.obtenerCitasTecnico('TEC001', '2025-11-26', '2025-11-30')

      expect(axios.get).toHaveBeenCalledWith(
        `${API_URL}/citas/tecnico/TEC001`,
        { params: { fechaDesde: '2025-11-26', fechaHasta: '2025-11-30' } }
      )
      expect(result).toEqual(mockData)
    })
  })

  describe('obtenerMisCitas', () => {
    it('returns client appointments', async () => {
      const mockData = [
        {
          idCita: 3,
          fechaCita: '2025-11-28',
          horaInicio: '10:00',
          estado: 'PENDIENTE'
        }
      ]

      axios.get.mockResolvedValue({ data: mockData })

      const result = await CitaService.obtenerMisCitas()

      expect(axios.get).toHaveBeenCalledWith(`${API_URL}/citas/mis-citas`)
      expect(result).toEqual(mockData)
    })
  })

  describe('crearCita', () => {
    it('creates new appointment successfully', async () => {
      const citaData = {
        nombreCliente: 'Pedro Ruiz',
        emailCliente: 'pedro@test.com',
        fechaCita: '2025-11-30',
        horaInicio: '14:00',
        idUsuarioTecnico: 'TEC001'
      }

      const mockResponse = { idCita: 10, ...citaData, estado: 'PENDIENTE' }
      axios.post.mockResolvedValue({ data: mockResponse })

      const result = await CitaService.crearCita(citaData)

      expect(axios.post).toHaveBeenCalledWith(`${API_URL}/citas`, citaData)
      expect(result).toEqual(mockResponse)
    })

    it('throws error on validation failure', async () => {
      const citaData = { nombreCliente: '' }
      axios.post.mockRejectedValue(new Error('Validation error'))

      await expect(CitaService.crearCita(citaData)).rejects.toThrow('Validation error')
    })
  })

  describe('confirmarCita', () => {
    it('confirms appointment with valid token', async () => {
      const token = 'test-jwt-token'
      localStorage.setItem('token', token)

      const mockResponse = { success: true }
      axios.put.mockResolvedValue({ data: mockResponse })

      const result = await CitaService.confirmarCita(1)

      expect(axios.put).toHaveBeenCalledWith(
        `${API_URL}/citas/1/confirmar`,
        {},
        { headers: { Authorization: `Bearer ${token}` } }
      )
      expect(result).toEqual(mockResponse)
    })
  })

  describe('completarCita', () => {
    it('completes appointment successfully', async () => {
      const token = 'test-jwt-token'
      localStorage.setItem('token', token)

      const mockResponse = { success: true }
      axios.put.mockResolvedValue({ data: mockResponse })

      const result = await CitaService.completarCita(1)

      expect(axios.put).toHaveBeenCalledWith(
        `${API_URL}/citas/1/completar`,
        {},
        { headers: { Authorization: `Bearer ${token}` } }
      )
      expect(result).toEqual(mockResponse)
    })
  })

  describe('cancelarCita', () => {
    it('cancels appointment successfully', async () => {
      const token = 'test-jwt-token'
      localStorage.setItem('token', token)

      const mockResponse = { success: true }
      axios.delete.mockResolvedValue({ data: mockResponse })

      const result = await CitaService.cancelarCita(1)

      expect(axios.delete).toHaveBeenCalledWith(
        `${API_URL}/citas/1`,
        { headers: { Authorization: `Bearer ${token}` } }
      )
      expect(result).toEqual(mockResponse)
    })

    it('handles cancellation error', async () => {
      const token = 'test-jwt-token'
      localStorage.setItem('token', token)

      axios.delete.mockRejectedValue(new Error('Cannot cancel'))

      await expect(CitaService.cancelarCita(1)).rejects.toThrow('Cannot cancel')
    })
  })

  describe('bloquearHorario', () => {
    it('blocks time slot successfully', async () => {
      const mockResponse = { success: true }
      axios.post.mockResolvedValue({ data: mockResponse })

      const result = await CitaService.bloquearHorario('2025-11-26', '08:00', 'Reunión')

      expect(axios.post).toHaveBeenCalledWith(`${API_URL}/citas/horarios/bloquear`, {
        fecha: '2025-11-26',
        horaInicio: '08:00',
        motivoBloqueo: 'Reunión'
      })
      expect(result).toEqual(mockResponse)
    })
  })

  describe('desbloquearHorario', () => {
    it('unblocks time slot successfully', async () => {
      const mockResponse = { success: true }
      axios.post.mockResolvedValue({ data: mockResponse })

      const result = await CitaService.desbloquearHorario('2025-11-26', '08:00')

      expect(axios.post).toHaveBeenCalledWith(`${API_URL}/citas/horarios/desbloquear`, {
        fecha: '2025-11-26',
        horaInicio: '08:00'
      })
      expect(result).toEqual(mockResponse)
    })
  })

  describe('generarHorariosSemana', () => {
    it('generates week schedules successfully', async () => {
      const mockResponse = { success: true }
      axios.post.mockResolvedValue({ data: mockResponse })

      const result = await CitaService.generarHorariosSemana('TEC001', '2025-11-26')

      expect(axios.post).toHaveBeenCalledWith(`${API_URL}/citas/horarios/generar`, {
        idTecnico: 'TEC001',
        fechaInicio: '2025-11-26'
      })
      expect(result).toEqual(mockResponse)
    })
  })

  describe('obtenerHorariosTecnico', () => {
    it('retrieves technician schedules', async () => {
      const mockData = [
        {
          idHorario: 1,
          fecha: '2025-11-26',
          horaInicio: '08:00',
          disponible: true
        }
      ]

      axios.get.mockResolvedValue({ data: mockData })

      const result = await CitaService.obtenerHorariosTecnico('TEC001', '2025-11-26', '2025-11-30')

      expect(axios.get).toHaveBeenCalledWith(
        `${API_URL}/citas/tecnico/TEC001/horarios`,
        { params: { fechaDesde: '2025-11-26', fechaHasta: '2025-11-30' } }
      )
      expect(result).toEqual(mockData)
    })
  })
})
