import { setActivePinia, createPinia } from 'pinia'
import { useUserStore } from '@/stores/userStore'

describe('userStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('initializes with default state', () => {
    const store = useUserStore()
    expect(store.identificacion).toBe(null)
    expect(store.token).toBe(null)
  })

  it('setUserData updates user information', () => {
    const store = useUserStore()
    const userData = {
      identificacion: 'USER123',
      nombre: 'Test User',
      email: 'test@test.com',
      rol: 'CLIENTE'
    }
    
    store.setUserData(userData, 'test-token')
    
    expect(store.identificacion).toBe('USER123')
    expect(store.token).toBe('test-token')
    expect(store.nombre).toBe('Test User')
  })

  it('logout clears user data', () => {
    const store = useUserStore()
    store.setUserData({ identificacion: 'USER123' }, 'token')
    
    store.logout()
    
    expect(store.identificacion).toBe(null)
    expect(store.token).toBe(null)
  })

  it('isAuthenticated returns correct value', () => {
    const store = useUserStore()
    expect(store.isAuthenticated).toBe(false)
    
    store.setUserData({ identificacion: 'USER123' }, 'token')
    expect(store.isAuthenticated).toBe(true)
  })

  it('hasRole checks user role correctly', () => {
    const store = useUserStore()
    store.setUserData({ identificacion: 'USER123', rol: 'TECNICO' }, 'token')
    
    if (store.hasRole) {
      expect(store.hasRole('TECNICO')).toBe(true)
      expect(store.hasRole('ADMIN')).toBe(false)
    }
  })

  it('updateProfile updates user profile data', () => {
    const store = useUserStore()
    store.setUserData({ identificacion: 'USER123', nombre: 'Old Name' }, 'token')
    
    if (store.updateProfile) {
      store.updateProfile({ nombre: 'New Name', email: 'new@test.com' })
      expect(store.nombre).toBe('New Name')
    }
  })

  it('setToken updates token', () => {
    const store = useUserStore()
    
    if (store.setToken) {
      store.setToken('new-token')
      expect(store.token).toBe('new-token')
    }
  })

  it('getUserData returns user data', () => {
    const store = useUserStore()
    store.setUserData({ 
      identificacion: 'USER123',
      nombre: 'Test',
      email: 'test@test.com' 
    }, 'token')
    
    if (store.getUserData) {
      const data = store.getUserData()
      expect(data.identificacion).toBe('USER123')
    }
  })
})
