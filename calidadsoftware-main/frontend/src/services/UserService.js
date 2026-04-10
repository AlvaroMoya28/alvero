import axios from 'axios'
import { useUserStore } from '@/stores/userStore'
import router from '@/router'
import { jwtDecode } from 'jwt-decode'

const API_URL = 'https://localhost:5001/api/usuarios'

// Configuración del interceptor para añadir token a las peticiones
axios.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  error => {
    return Promise.reject(error)
  }
)

// Interceptor para manejar errores de autenticación
axios.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      // Solo forzar redirección en secciones protegidas (/admin, /tecnico)
      const currentPath = router.currentRoute.value?.path || ''
      const isProtected = currentPath.startsWith('/admin') || currentPath.startsWith('/tecnico')

      if (isProtected) {
        const userStore = useUserStore()
        userStore.logout()
        router.push('/login')
      }
      // En rutas públicas, dejamos que el componente maneje el error sin redirigir
    }
    return Promise.reject(error)
  }
)

export async function solicitarReseteoPassword (emailData) {
  try {
    // El backend para forgot-password espera un objeto con la propiedad "email"
    const response = await axios.post(`${API_URL}/forgot-password`, emailData)
    return response.data // O simplemente response si no espera un cuerpo específico
  } catch (error) {
    // Incluso si el backend siempre devuelve OK por seguridad,
    // podrían ocurrir errores de red o del servidor.
    console.error('Error en solicitarReseteoPassword:', error)
    throw new Error(error.response?.data?.message || 'Error al solicitar el reseteo de contraseña.')
  }
}

export async function restablecerNuevaPassword (resetData) {
  // resetData debe ser un objeto como: { email: "string", code: "string", newPassword: "string" }
  try {
    const response = await axios.post(`${API_URL}/reset-password`, resetData)
    return response.data // El backend envía un mensaje de éxito
  } catch (error) {
    console.error('Error en restablecerNuevaPassword:', error)
    // El backend puede devolver errores si el código es inválido/expirado o si hay problemas al actualizar.
    throw new Error(error.response?.data?.message || 'Error al restablecer la contraseña.')
  }
}

export async function getUsuarioPorId (id) {
  const response = await axios.get(`${API_URL}/${id}`, {
    params: {
      fields: 'nombre,apellido1,apellido2,email,telefono' // Campos que necesitas
    }
  })
  return response.data
}

export const usuarioService = {
  async getAllUsers () {
    try {
      const response = await axios.get(API_URL)
      return response.data
    } catch (error) {
      console.error('Error fetching users:', error)
      throw error
    }
  },

  async getTecnicosPublicos () {
    try {
      const response = await axios.get(`${API_URL}/tecnicos-publicos`)
      return response.data
    } catch (error) {
      console.error('Error fetching public technicians:', error)
      throw error
    }
  },

  async updateUser (id, updateData) {
    try {
      const response = await axios.put(`${API_URL}/${id}`, updateData)
      return response.data
    } catch (error) {
      console.error('Error updating user:', error)
      throw error
    }
  },

  async deleteUser (id) {
    try {
      const response = await axios.delete(`${API_URL}/${id}`)
      return response.data
    } catch (error) {
      console.error('Error deleting user:', error)
      throw error
    }
  },

  async registrarUsuario (usuarioData) {
    try {
      // Validación básica en el frontend antes de enviar
      if (usuarioData.contrasena.length < 8) {
        throw new Error('La contraseña debe tener al menos 8 caracteres')
      }

      // Limpiar datos opcionales
      const dataToSend = {
        ...usuarioData,
        apellido2: usuarioData.apellido2 || null,
        telefono: usuarioData.telefono || null
      }

      const response = await axios.post(`${API_URL}/registro`, dataToSend, {
        validateStatus: function (status) {
          return status < 500 // Para manejar errores 400 y 409
        }
      })

      // Si la respuesta es exitosa pero no tiene datos (por si acaso)
      if (!response.data) {
        throw new Error('No se recibió respuesta del servidor')
      }

      return response.data
    } catch (error) {
      if (error.response) {
        const { data, status } = error.response

        // Manejo específico de errores de validación (400)
        if (status === 400) {
          // Construir mensajes detallados
          let errorMessage = 'Error en el registro: '

          // Si hay errores específicos de contraseña
          if (data.errors?.Contrasena) {
            const passwordErrors = data.errors.Contrasena

            if (passwordErrors.some(e => e.includes('mayúsculas'))) {
              errorMessage += 'La contraseña debe contener al menos una mayúscula. '
            }
            if (passwordErrors.some(e => e.includes('minúsculas'))) {
              errorMessage += 'La contraseña debe contener al menos una minúscula. '
            }
            if (passwordErrors.some(e => e.includes('números'))) {
              errorMessage += 'La contraseña debe contener al menos un número. '
            }
            if (passwordErrors.some(e => e.includes('caracteres especiales'))) {
              errorMessage += 'La contraseña debe contener al menos un carácter especial. '
            }
          }

          // Si hay otros errores
          if (data.errors) {
            for (const key in data.errors) {
              if (key !== 'Contrasena') {
                errorMessage += data.errors[key].join(' ') + ' '
              }
            }
          }

          // Si hay un mensaje general
          if (data.message && !errorMessage.includes(data.message)) {
            errorMessage += data.message
          }

          throw new Error(errorMessage.trim() || 'Error de validación en los datos')
        }

        // Manejo de conflictos (409 - usuario/email ya existe)
        if (status === 409) {
          throw new Error(data.message || 'El nombre de usuario o email ya está registrado')
        }

        // Otros errores del servidor
        throw new Error(data.message || `Error del servidor (${status})`)
      }

      // Si es un error de red o validación del frontend
      throw error
    }
  },

  async registrarTecnico (usuarioData) {
    try {
      // No aplicamos la validación de contraseña frontend aquí: backend validará el patrón
      const dataToSend = {
        ...usuarioData,
        apellido2: usuarioData.apellido2 || null,
        telefono: usuarioData.telefono || null
      }

      const response = await axios.post(`${API_URL}/registro-tecnico`, dataToSend, {
        validateStatus: function (status) {
          return status < 500
        }
      })

      if (!response.data) {
        throw new Error('No se recibió respuesta del servidor')
      }

      return response.data
    } catch (error) {
      if (error.response) {
        const { data, status } = error.response
        if (status === 400) {
          throw new Error(data.message || 'Error de validación en los datos')
        }
        if (status === 409) {
          throw new Error(data.message || 'El nombre de usuario o email ya está registrado')
        }
        throw new Error(data.message || `Error del servidor (${status})`)
      }
      throw error
    }
  }
}

export async function updateUsuario (id, data) {
  const response = await axios.put(`${API_URL}/${id}`, data)
  return response.data
}

export const usuarioLogIn = {
  async login (usuarioData) {
    try {
      const response = await axios.post(`${API_URL}/login`, usuarioData)

      // Verifica la estructura de la respuesta
      if (!response.data?.token) {
        throw new Error('La respuesta del servidor no tiene el formato esperado')
      }

      const token = response.data.token
      // Guardar el token en localStorage
      localStorage.setItem('token', token)

      const decodedToken = jwtDecode(token)

      return {
        token,
        usuario: {
          idUsuario: decodedToken.idUsuario,
          nombre: decodedToken.nombre,
          apellido1: decodedToken.apellido1,
          apellido2: decodedToken.apellido2,
          email: decodedToken.email,
          telefono: decodedToken.telefono,
          tipoUsuario: decodedToken.tipoUsuario,
          estado: decodedToken.estado
        }
      }
    } catch (error) {
      console.error('Error en login:', {
        message: error.message,
        response: error.response?.data,
        stack: error.stack
      })

      let errorMessage = 'Error al iniciar sesión'

      if (error.response) {
        // Si el backend devuelve un mensaje de error
        if (error.response.data && error.response.data.message) {
          errorMessage = error.response.data.message
        } else if (error.response.status === 401) {
          errorMessage = 'Credenciales inválidas'
        } else if (error.response.status === 500) {
          errorMessage = 'Error interno del servidor'
        }
      } else if (error.request) {
        errorMessage = 'No se recibió respuesta del servidor'
      } else if (error.message) {
        errorMessage = error.message
      }

      throw new Error(errorMessage)
    }
  }
}

export async function deleteUsuario (id) {
  try {
    const response = await axios.delete(`${API_URL}/${id}`)
    return response.data
  } catch (error) {
    if (error.response) {
      throw new Error(error.response.data.message || 'Error al eliminar usuario')
    }
    throw new Error('Error de conexión con el servidor')
  }
}

/**
 * Cambia la contraseña de un usuario autenticado.
 * El ID del usuario se infiere del token JWT en el backend.
 * @param {object} passwordData - Objeto con { currentPassword: "actual", newPassword: "nueva" }.
 */
export async function changePasswordAuthenticated (passwordData) {
  try {
    // Llama al endpoint del backend que no requiere el ID en la URL explícitamente.
    // El backend usará el token para identificar al usuario.
    const response = await axios.post(`${API_URL}/change-password`, passwordData)
    return response.data // El backend debería devolver un objeto como { message: "..." }
  } catch (error) {
    console.error('Error en el servicio changePasswordAuthenticated:', error.response?.data || error.message)
    // Lanza el mensaje de error del backend si está disponible, sino uno genérico.
    throw new Error(error.response?.data?.message || 'Error al intentar cambiar la contraseña.')
  }
}

export async function getUserReservas (userId) {
  try {
    const response = await axios.get(`${API_URL}/${userId}/reservas`)
    return response.data
  } catch (error) {
    console.error('Error al obtener reservas del usuario:', error)
    return []
  }
}

export async function payReserva (reservaId) {
  try {
    const response = await axios.post(`${API_URL}/reservas/${reservaId}/pagar`)
    return response.data
  } catch (error) {
    console.error('Error al pagar la reserva:', error)
    throw error
  }
}
