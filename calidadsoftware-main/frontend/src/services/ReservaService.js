import axios from 'axios'

const API_URL = 'https://localhost:5001/api/reservas'
const API_URL_CANCELAR = 'https://localhost:5001/api/Reservas'

export async function getReservaById (reservaId) {
  try {
    const response = await axios.get(`${API_URL}/${reservaId}`)
    return response.data
  } catch (error) {
    console.error('Error al obtener la reserva:', error)
    return null
  }
}

// Crear una nueva reserva
export async function createReserva (reservaData) {
  try {
    const response = await axios.post(API_URL, reservaData)
    return response.data
  } catch (error) {
    if (error.response && error.response.data) {
      // Intenta extraer un mensaje legible del backend
      const data = error.response.data
      const message =
        typeof data === 'string'
          ? data
          : data.message || data.error || JSON.stringify(data)
      throw new Error(message)
    }
    throw new Error('Error al crear la reserva')
  }
}

export async function cancelarReservaById (reservaId) {
  try {
    const response = await axios.put(`${API_URL_CANCELAR}/cancelar/${reservaId}`)
    return response.data
  } catch (error) {
    if (error.response && error.response.data) {
      const data = error.response.data
      const message =
        typeof data === 'string'
          ? data
          : data.message || data.error || JSON.stringify(data)
      throw new Error(message)
    }
    throw new Error('Error al cancelar la reserva')
  }
}
