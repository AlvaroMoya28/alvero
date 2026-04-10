import ReservaService from '@/services/ReservaService'
import axios from 'axios'

jest.mock('axios')

describe('ReservaService', () => {
  beforeEach(() => {
    jest.clearAllMocks()
    localStorage.setItem('token', 'test-token')
  })

  afterEach(() => {
    localStorage.clear()
  })

  it('obtenerReservasPorUsuario fetches user reservations', async () => {
    const mockData = [{ idReserva: 1, estado: 'CONFIRMADA' }]
    axios.get.mockResolvedValue({ data: mockData })
    
    const result = await ReservaService.obtenerReservasPorUsuario('user123')
    
    expect(result).toEqual(mockData)
    expect(axios.get).toHaveBeenCalledWith(
      expect.stringContaining('/reservas/usuario/user123'),
      expect.any(Object)
    )
  })

  it('crearReserva creates a new reservation', async () => {
    const reservaData = { idSala: 1, fechaInicio: '2025-11-26' }
    const mockResponse = { idReserva: 1 }
    axios.post.mockResolvedValue({ data: mockResponse })
    
    const result = await ReservaService.crearReserva(reservaData)
    
    expect(result).toEqual(mockResponse)
    expect(axios.post).toHaveBeenCalled()
  })

  it('confirmarReserva confirms reservation', async () => {
    axios.put.mockResolvedValue({ data: { success: true } })
    
    await ReservaService.confirmarReserva(1)
    
    expect(axios.put).toHaveBeenCalledWith(
      expect.stringContaining('/reservas/1/confirmar'),
      expect.any(Object),
      expect.any(Object)
    )
  })

  it('cancelarReserva cancels reservation', async () => {
    axios.put.mockResolvedValue({ data: { success: true } })
    
    await ReservaService.cancelarReserva(1)
    
    expect(axios.put).toHaveBeenCalled()
  })

  it('obtenerReserva fetches single reservation', async () => {
    const mockData = { idReserva: 1 }
    axios.get.mockResolvedValue({ data: mockData })
    
    const result = await ReservaService.obtenerReserva(1)
    
    expect(result).toEqual(mockData)
  })

  it('obtenerSalasDisponibles fetches available rooms', async () => {
    const mockData = [{ idSala: 1, nombre: 'Sala A' }]
    axios.get.mockResolvedValue({ data: mockData })
    
    const result = await ReservaService.obtenerSalasDisponibles('2025-11-26', '10:00', '12:00')
    
    expect(result).toEqual(mockData)
  })
})
