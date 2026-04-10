/* global jest, beforeAll, afterAll, describe, it, expect */
// Silenciar warnings y errores de consola para evitar ruido en los tests
beforeAll(() => {
  jest.spyOn(console, 'warn').mockImplementation(() => {})
  jest.spyOn(console, 'error').mockImplementation(() => {})
})
afterAll(() => {
  jest.restoreAllMocks()
})
import { shallowMount } from '@vue/test-utils'
import UserProfile from '@/components/UserProfile.vue'

// Mock servicios
jest.mock('@/services/UserService', () => ({
  getUsuarioPorId: jest.fn(() => Promise.resolve({
    nombre: 'Tony',
    apellido1: 'Picado',
    apellido2: 'Alvarado',
    email: 'tony@test.com',
    telefono: '88888888'
  })),
  updateUsuario: jest.fn(() => Promise.resolve({ token: 'nuevo-token' })),
  deleteUsuario: jest.fn(),
  changePasswordAuthenticated: jest.fn(() => Promise.resolve({ message: 'Contraseña actualizada' }))
}))

jest.mock('@/stores/userStore', () => ({
  useUserStore: () => ({
    user: { idUsuario: 1 },
    setUserData: jest.fn(),
    logout: jest.fn(),
    token: 'fake-token'
  })
}))

jest.mock('vue-router', () => ({
  useRouter: () => ({
    push: jest.fn()
  })
}))

describe('UserProfile.vue', () => {
  it('se monta correctamente y renderiza los nombres', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: {
        components: {
          'router-link': {
            template: '<a><slot /></a>'
          }
        }
      }
    })
    await wrapper.vm.$nextTick()
    await new Promise(r => setTimeout(r, 0)) // Espera a que onMounted termine
    expect(wrapper.exists()).toBe(true)
    expect(wrapper.text()).toContain('Configuración de Perfil')
    expect(wrapper.text()).toContain('Tony Picado')
    expect(wrapper.text()).toContain('tony@test.com')
    expect(wrapper.text()).toContain('88888888')
  })

  it('renderiza avatar con iniciales', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: {
        components: { 'router-link': { template: '<a><slot /></a>' } }
      }
    })
    await wrapper.vm.$nextTick()
    await new Promise(resolve => setTimeout(resolve, 0))
    expect(wrapper.find('.avatar-placeholder').text()).toContain('TP')
    // No se puede testear el fallback sin refactor, porque name/lastName son refs y no props
  })

  it('abre el modal de cambio de correo al hacer clic', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: {
        components: { 'router-link': { template: '<a><slot /></a>' } }
      }
    })
    await wrapper.vm.$nextTick()
    await new Promise(r => setTimeout(r, 0))
    await wrapper.vm.OpenModalChangeEmail()
    expect(wrapper.vm.isOpenChangeEmail).toBe(true)
    expect(wrapper.vm.editableEmail).toBe('tony@test.com')
  })

  it('abre el modal de cambio de teléfono al hacer clic', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: {
        components: { 'router-link': { template: '<a><slot /></a>' } }
      }
    })
    await wrapper.vm.$nextTick()
    await new Promise(r => setTimeout(r, 0))
    await wrapper.vm.OpenModalChangeNumber()
    expect(wrapper.vm.isOpenChangeNumber).toBe(true)
    expect(wrapper.vm.editableNumber).toBe('88888888')
  })

  it('abre y cierra correctamente el modal de contraseña', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: {
        components: { 'router-link': { template: '<a><slot /></a>' } }
      }
    })
    wrapper.vm.openPasswordModal()
    expect(wrapper.vm.showPasswordModal).toBe(true)
    wrapper.vm.closePasswordModal()
    expect(wrapper.vm.showPasswordModal).toBe(false)
  })

  it('guarda cambios de nombre y apellidos (éxito)', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: { components: { 'router-link': { template: '<a><slot /></a>' } } }
    })
    wrapper.vm.name = 'Nuevo'
    wrapper.vm.lastName = 'Apellido'
    wrapper.vm.secondlastName = 'Segundo'
    await wrapper.vm.updateNameAndLastName()
    expect(wrapper.vm.successMessage).toContain('actualizada')
    expect(wrapper.vm.showSuccessModal).toBe(true)
  })

  it('guarda cambios de email (éxito)', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: { components: { 'router-link': { template: '<a><slot /></a>' } } }
    })
    wrapper.vm.editableEmail = 'nuevo@test.com'
    await wrapper.vm.updateEmail()
    expect(wrapper.vm.profileDisplayEmail).toBe('nuevo@test.com')
    expect(wrapper.vm.showSuccessModal).toBe(true)
  })

  it('no permite email vacío', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: { components: { 'router-link': { template: '<a><slot /></a>' } } }
    })
    wrapper.vm.editableEmail = ''
    await wrapper.vm.updateEmail()
    expect(wrapper.vm.modalErrorMessage).toBe('El nuevo correo no puede estar vacío.')
  })

  it('guarda cambios de teléfono (éxito)', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: { components: { 'router-link': { template: '<a><slot /></a>' } } }
    })
    wrapper.vm.editableNumber = '99999999'
    await wrapper.vm.updateNumber()
    expect(wrapper.vm.profileDisplayNumber).toBe('99999999')
    expect(wrapper.vm.showSuccessModal).toBe(true)
  })

  it('elimina cuenta (éxito)', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: { components: { 'router-link': { template: '<a><slot /></a>' } } }
    })
    await wrapper.vm.deleteUsuarioAccount()
    expect(wrapper.vm.showSuccessModal).toBe(true)
    expect(wrapper.vm.successMessage).toContain('eliminada')
  })

  it('cierra modales de éxito y error', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: { components: { 'router-link': { template: '<a><slot /></a>' } } }
    })
    wrapper.vm.openSuccessModal('ok')
    expect(wrapper.vm.showSuccessModal).toBe(true)
    wrapper.vm.closeSuccessModal()
    expect(wrapper.vm.showSuccessModal).toBe(false)
    wrapper.vm.openErrorModal('err')
    expect(wrapper.vm.showErrorModal).toBe(true)
    wrapper.vm.closeErrorModal()
    expect(wrapper.vm.showErrorModal).toBe(false)
  })

  it('valida cambio de contraseña (errores de frontend)', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: { components: { 'router-link': { template: '<a><slot /></a>' } } }
    })
    // Todos vacíos
    await wrapper.vm.submitPasswordChange()
    expect(wrapper.vm.passwordModalErrorMessage).toBe('Todos los campos son obligatorios.')
    // No coinciden
    wrapper.vm.currentPassword = '12345678'
    wrapper.vm.newPassword = '123456789'
    wrapper.vm.confirmPassword = 'diferente'
    await wrapper.vm.submitPasswordChange()
    expect(wrapper.vm.passwordModalErrorMessage).toBe('Las nuevas contraseñas no coinciden.')
    // Menos de 8
    wrapper.vm.newPassword = 'short'
    wrapper.vm.confirmPassword = 'short'
    await wrapper.vm.submitPasswordChange()
    expect(wrapper.vm.passwordModalErrorMessage).toBe('La nueva contraseña debe tener al menos 8 caracteres.')
    // Igual a actual
    wrapper.vm.newPassword = '12345678'
    wrapper.vm.confirmPassword = '12345678'
    await wrapper.vm.submitPasswordChange()
    expect(wrapper.vm.passwordModalErrorMessage).toBe('La nueva contraseña no puede ser igual a la contraseña actual.')
  })

  it('cambia contraseña correctamente (éxito)', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: { components: { 'router-link': { template: '<a><slot /></a>' } } }
    })
    wrapper.vm.currentPassword = 'oldpassword'
    wrapper.vm.newPassword = 'newpassword123'
    wrapper.vm.confirmPassword = 'newpassword123'
    await wrapper.vm.submitPasswordChange()
    expect(wrapper.vm.showSuccessModal).toBe(true)
    expect(wrapper.vm.successMessage).toContain('Contraseña actualizada')
  })

  it('renderiza modales condicionales', async () => {
    const wrapper = shallowMount(UserProfile, {
      global: { components: { 'router-link': { template: '<a><slot /></a>' } } }
    })
    wrapper.vm.showSuccessModal = true
    wrapper.vm.showErrorModal = true
    wrapper.vm.showPasswordModal = true
    wrapper.vm.isOpenChangeEmail = true
    wrapper.vm.isOpenChangeNumber = true
    wrapper.vm.isDeleteAccountOpen = true
    await wrapper.vm.$nextTick()
    expect(wrapper.findAll('.modal').length).toBeGreaterThan(0)
  })
})
