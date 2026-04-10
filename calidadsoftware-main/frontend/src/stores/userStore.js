import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { jwtDecode } from 'jwt-decode'

export const useUserStore = defineStore('user', () => {
  // Inicializa el token directamente desde localStorage al definir el ref.
  // Esto simplifica un poco la inicialización, pero la lógica de validación se hará después.
  const token = ref(localStorage.getItem('token'))

  const _userInternal = ref(null) // Un ref interno si necesitas almacenar más datos que los del token.

  // Función auxiliar para verificar si un token ha expirado
  const isTokenExpired = (tokenString) => {
    if (!tokenString) return true // Si no hay token, se considera expirado/inválido
    try {
      const decoded = jwtDecode(tokenString)
      const currentTime = Date.now() / 1000 // jwtDecode exp está en segundos
      return decoded.exp < currentTime
    } catch (error) {
      console.error('Error decodificando el token o token inválido:', error)
      return true // Si hay error al decodificar, se considera expirado/inválido
    }
  }

  // La autenticación se basa en que haya un token Y que no esté expirado.
  const isAuthenticated = computed(() => {
    return !!token.value && !isTokenExpired(token.value)
  })

  // El usuario actual se obtiene decodificando el token, solo si está autenticado.
  const currentUser = computed(() => {
    if (isAuthenticated.value && token.value) { // Usa el isAuthenticated ya validado
      try {
        return jwtDecode(token.value)
      } catch (e) {
        console.error('Fallo al decodificar token en currentUser, esto no debería ocurrir si isAuthenticated es true:', e)
        // Si llega aquí, algo raro pasó, mejor limpiar
        logout() // Llama a la acción de logout del store
        return null
      }
    }
    return null // No autenticado o sin token
  })

  const setUserData = (userDataFromApi, authToken) => {
    if (authToken) {
      if (isTokenExpired(authToken)) {
        console.warn('Se intentó establecer un token expirado. Se procederá a desloguear.')
        logout() // Limpiar sesión si el token nuevo ya está expirado
        return
      }
      token.value = authToken
      localStorage.setItem('token', authToken)

      if (userDataFromApi) {
        _userInternal.value = userDataFromApi
        // Opcional: guardar este userDataFromApi en localStorage si es necesario persistirlo
        // localStorage.setItem('userDetails', JSON.stringify(userDataFromApi));
      } else if (token.value) {
        // Si no se pasan datos de usuario pero sí un token, decodificar para _userInternal
        // _userInternal.value = jwtDecode(token.value);
        // Sin embargo, currentUser ya hace esto, así que _userInternal puede ser opcional
        // o usarse para datos que *no* están en el token.
      }
    } else {
      // Si no se provee authToken, es un error o un intento de limpiar.
      // La función logout es más apropiada para limpiar.
      console.warn('setUserData fue llamado sin authToken.')
    }
  }

  const logout = () => {
    token.value = null
    _userInternal.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('userDetails') // Si se guardan detalles adicionales
    console.log('Usuario deslogueado, token y datos limpiados.')
  }

  // Función de inicialización para verificar el token al cargar el store.
  const initializeStore = () => {
    const storedToken = localStorage.getItem('token')
    if (storedToken) {
      if (isTokenExpired(storedToken)) {
        console.log('Token almacenado ha expirado. Limpiando sesión.')
        logout() // Llama a logout para limpiar el token expirado y el estado
      } else {
        token.value = storedToken // El token es válido, establecerlo en el estado reactivo
      }
    } else {
      // No hay token, asegúrate que el estado esté limpio (aunque los refs ya inician en null)
      if (token.value) token.value = null
      if (_userInternal.value) _userInternal.value = null
    }
  }

  // Llamar a la inicialización cuando el store se define por primera vez.
  initializeStore()

  // El método getUserFromToken es redundante si currentUser hace lo mismo.
  // Se deja por si se usa en algún lado, pero es mejor usar currentUser.
  const getUserFromToken = () => {
    return currentUser.value
  }

  const isAdmin = computed(() => {
    const user = currentUser.value
    return user?.tipoUsuario === 'ADMINISTRADOR' || user?.tipoUsuario === 'SUPERUSUARIO'
  })

  return {
    user: currentUser,
    token, // El string del token
    isAuthenticated, // El booleano que indica si está autenticado y el token no ha expirado
    isAdmin,
    setUserData,
    logout,
    initialize: initializeStore, // Exponer para reinicializar si es necesario
    getUserFromToken
  }
})
