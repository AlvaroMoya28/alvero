import axios from 'axios'

const API_URL = process.env.VUE_APP_API_URL || 'https://localhost:7167/api'

export default {
  // Obtener disponibilidad agregada (todos los técnicos)
  async obtenerDisponibilidadGeneral (fechaDesde, fechaHasta) {
    const response = await axios.get(`${API_URL}/citas/disponibilidad-general`, {
      params: { fechaDesde, fechaHasta }
    })
    return response.data
  },

  // Obtener horarios disponibles de un técnico
  async obtenerHorariosDisponibles (idTecnico, fechaDesde, fechaHasta) {
    const response = await axios.get(`${API_URL}/citas/horarios-disponibles/${idTecnico}`, {
      params: { fechaDesde, fechaHasta }
    })
    return response.data
  },

  // Obtener citas del técnico logueado
  async obtenerMisCitasTecnico (fechaDesde = null, fechaHasta = null) {
    const response = await axios.get(`${API_URL}/citas/tecnico/mis-citas`, {
      params: { fechaDesde, fechaHasta }
    })
    return response.data
  },

  // Obtener citas de un técnico específico (admin)
  async obtenerCitasTecnico (idTecnico, fechaDesde = null, fechaHasta = null) {
    const response = await axios.get(`${API_URL}/citas/tecnico/${idTecnico}`, {
      params: { fechaDesde, fechaHasta }
    })
    return response.data
  },

  // Obtener citas del cliente logueado
  async obtenerMisCitas () {
    const response = await axios.get(`${API_URL}/citas/mis-citas`)
    return response.data
  },

  // Crear una nueva cita
  async crearCita (citaData) {
    const response = await axios.post(`${API_URL}/citas`, citaData)
    return response.data
  },

  // Confirmar cita (técnico/admin)
  async confirmarCita (idCita) {
    const token = localStorage.getItem('token')
    console.log('=== CONFIRMAR CITA ===')
    console.log('ID Cita:', idCita)
    console.log('Token presente:', !!token)
    console.log('Token:', token?.substring(0, 50) + '...')
    console.log('URL:', `${API_URL}/citas/${idCita}/confirmar`)

    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]))
        console.log('Payload del token COMPLETO:', payload)
        console.log('Todas las propiedades del token:', Object.keys(payload))
        console.log('Rol (ClaimTypes.Role):', payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'])
        console.log('Rol (role):', payload.role)
        console.log('TipoUsuario:', payload.tipoUsuario)
      } catch (e) {
        console.error('Error decodificando token:', e)
      }
    }

    const response = await axios.put(`${API_URL}/citas/${idCita}/confirmar`, {}, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
    return response.data
  },

  // Completar cita (técnico/admin)
  async completarCita (idCita) {
    const token = localStorage.getItem('token')
    const response = await axios.put(`${API_URL}/citas/${idCita}/completar`, {}, {
      headers: { Authorization: `Bearer ${token}` }
    })
    return response.data
  },

  // Cancelar cita
  async cancelarCita (idCita) {
    const token = localStorage.getItem('token')
    const response = await axios.delete(`${API_URL}/citas/${idCita}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
    return response.data
  },

  // Bloquear horario (técnico)
  async bloquearHorario (fecha, horaInicio, motivoBloqueo) {
    const response = await axios.post(`${API_URL}/citas/horarios/bloquear`, {
      fecha,
      horaInicio,
      motivoBloqueo
    })
    return response.data
  },

  // Desbloquear horario (técnico)
  async desbloquearHorario (fecha, horaInicio) {
    const response = await axios.post(`${API_URL}/citas/horarios/desbloquear`, {
      fecha,
      horaInicio
    })
    return response.data
  },

  // Generar horarios semana (admin)
  async generarHorariosSemana (idTecnico, fechaInicio) {
    const response = await axios.post(`${API_URL}/citas/horarios/generar`, {
      idTecnico,
      fechaInicio
    })
    return response.data
  },

  // Obtener horarios de técnico (admin/técnico)
  async obtenerHorariosTecnico (idTecnico, fechaDesde, fechaHasta) {
    const response = await axios.get(`${API_URL}/citas/tecnico/${idTecnico}/horarios`, {
      params: { fechaDesde, fechaHasta }
    })
    return response.data
  }
}
