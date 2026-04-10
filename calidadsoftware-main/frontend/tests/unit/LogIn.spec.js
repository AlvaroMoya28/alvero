
/* global jest, beforeAll, afterAll, describe, it, expect */
import { mount, flushPromises } from '@vue/test-utils'
import LogIn from '@/components/LogIn.vue'
import { createTestingPinia } from '@pinia/testing'

// Mock global de vue-router para evitar warning de inyección
jest.mock('vue-router', () => ({
  useRouter: () => ({
    push: jest.fn(),
    replace: jest.fn(),
    currentRoute: { value: { path: '/' } },
    beforeEach: jest.fn(),
    afterEach: jest.fn()
  }),
  useRoute: () => ({
    path: '/',
    params: {},
    query: {}
  })
}))

jest.mock('@/services/UserService', () => ({
  usuarioLogIn: { login: jest.fn() },
  solicitarReseteoPassword: jest.fn(),
  restablecerNuevaPassword: jest.fn()
}))

// Mock de jwt-decode
jest.mock('jwt-decode', () => ({ jwtDecode: jest.fn() }))

// Silenciar warnings y errores de consola para evitar ruido en los tests
beforeAll(() => {
  jest.spyOn(console, 'warn').mockImplementation(() => {})
  jest.spyOn(console, 'error').mockImplementation(() => {})
})
afterAll(() => {
  jest.restoreAllMocks()
})

jest.mock('jwt-decode', () => ({ jwtDecode: jest.fn() }))

const globalStubs = {
  RouterLink: { template: '<a><slot /></a>' }
}

function mountWithProviders (options = {}) {
  return mount(LogIn, {
    global: {
      stubs: globalStubs,
      plugins: [createTestingPinia({ createSpy: jest.fn })]
    },
    ...options
  })
}

describe('LogIn.vue', () => {
  beforeEach(() => {
    jest.clearAllMocks()
  })

  it('renderiza el formulario de login', () => {
    const wrapper = mountWithProviders()
    expect(wrapper.text()).toMatch('Inicio de sesión')
    expect(wrapper.find('input#inputId').exists()).toBe(true)
    expect(wrapper.find('input#inputPassword').exists()).toBe(true)
    expect(wrapper.find('button[type="submit"]').exists()).toBe(true)
  })

  it('muestra error si campos vacíos y se intenta loguear', async () => {
    const wrapper = mountWithProviders()
    await wrapper.find('form').trigger('submit.prevent')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toMatch('ID y contraseña son requeridos')
  })

  it('muestra error si login falla por credenciales', async () => {
    require('@/services/UserService').usuarioLogIn.login.mockRejectedValueOnce(new Error('credenciales'))
    const wrapper = mountWithProviders()
    wrapper.find('input#inputId').setValue('user')
    wrapper.find('input#inputPassword').setValue('pass')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('Usuario o contraseña incorrectos')
  })

  it('redirige a Panel si login es admin', async () => {
    const push = jest.fn()
    jest.spyOn(require('vue-router'), 'useRouter').mockReturnValue({ push })
    require('jwt-decode').jwtDecode.mockReturnValue({ tipoUsuario: 'ADMINISTRADOR' })
    require('@/services/UserService').usuarioLogIn.login.mockResolvedValue({
      token: 'token', usuario: { id: 'admin' }
    })
    const wrapper = mountWithProviders()
    wrapper.find('input#inputId').setValue('admin')
    wrapper.find('input#inputPassword').setValue('adminpass')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(push).toHaveBeenCalledWith({ name: 'Panel' })
    jest.restoreAllMocks()
  })

  it('redirige a / si login es usuario normal', async () => {
    const push = jest.fn()
    jest.spyOn(require('vue-router'), 'useRouter').mockReturnValue({ push })
    require('jwt-decode').jwtDecode.mockReturnValue({ tipoUsuario: 'USUARIO' })
    require('@/services/UserService').usuarioLogIn.login.mockResolvedValue({
      token: 'token', usuario: { id: 'user' }
    })
    const wrapper = mountWithProviders()
    wrapper.find('input#inputId').setValue('user')
    wrapper.find('input#inputPassword').setValue('userpass')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(push).toHaveBeenCalledWith({ path: '/' })
    jest.restoreAllMocks()
  })

  it('abre y cierra el modal de recuperación de contraseña', async () => {
    const wrapper = mountWithProviders()
    expect(wrapper.vm.showRecoveryModal).toBe(false)
    await wrapper.find('.forgot-password').trigger('click')
    expect(wrapper.vm.showRecoveryModal).toBe(true)
    await wrapper.vm.cancelarRecuperacion()
    expect(wrapper.vm.showRecoveryModal).toBe(false)
  })

  it('valida email en recuperación y muestra error si vacío o inválido', async () => {
    const wrapper = mountWithProviders()
    await wrapper.find('.forgot-password').trigger('click')
    wrapper.vm.recoveryEmail = ''
    await wrapper.vm.enviarSolicitudCodigo()
    expect(wrapper.vm.recoveryMessage).toMatch('Por favor ingrese su correo electrónico')
    wrapper.vm.recoveryEmail = 'noemail'
    await wrapper.vm.enviarSolicitudCodigo()
    expect(wrapper.vm.recoveryMessage).toMatch('formato de correo electrónico válido')
  })

  it('muestra mensaje de éxito y pasa a step 2 si recuperación es exitosa', async () => {
    require('@/services/UserService').solicitarReseteoPassword.mockResolvedValueOnce()
    const wrapper = mountWithProviders()
    await wrapper.find('.forgot-password').trigger('click')
    wrapper.vm.recoveryEmail = 'test@mail.com'
    await wrapper.vm.enviarSolicitudCodigo()
    expect(wrapper.vm.recoveryStep).toBe(2)
    expect(wrapper.vm.step2UserInstruction).toMatch('Se ha enviado un código a')
  })

  it('muestra error si recuperación falla', async () => {
    require('@/services/UserService').solicitarReseteoPassword.mockRejectedValueOnce(Object.assign(new Error('fail'), { message: 'fail' }))
    const wrapper = mountWithProviders()
    await wrapper.find('.forgot-password').trigger('click')
    wrapper.vm.recoveryEmail = 'test@mail.com'
    await wrapper.vm.enviarSolicitudCodigo()
    expect(wrapper.vm.recoveryMessageType).toBe('error')
    expect([
      'fail',
      'No se pudo procesar tu solicitud. Inténtalo más tarde.'
    ]).toContain(wrapper.vm.recoveryMessage)
  })

  it('valida campos y muestra error si cambio de contraseña incompleto', async () => {
    const wrapper = mountWithProviders()
    await wrapper.find('.forgot-password').trigger('click')
    wrapper.vm.recoveryStep = 2
    wrapper.vm.recoveryCode = ''
    wrapper.vm.newPasswordRec = ''
    wrapper.vm.confirmNewPasswordRec = ''
    await wrapper.vm.cambiarContrasena()
    expect(wrapper.vm.recoveryMessage).toMatch('Todos los campos son requeridos')
    wrapper.vm.recoveryCode = '123456'
    wrapper.vm.newPasswordRec = '123'
    wrapper.vm.confirmNewPasswordRec = '123'
    await wrapper.vm.cambiarContrasena()
    expect(wrapper.vm.recoveryMessage).toMatch('al menos 6 caracteres')
    wrapper.vm.newPasswordRec = '123456'
    wrapper.vm.confirmNewPasswordRec = '654321'
    await wrapper.vm.cambiarContrasena()
    expect(wrapper.vm.recoveryMessage).toMatch('no coinciden')
  })

  it('muestra modal de éxito si cambio de contraseña es exitoso', async () => {
    require('@/services/UserService').restablecerNuevaPassword.mockResolvedValueOnce()
    const wrapper = mountWithProviders()
    await wrapper.find('.forgot-password').trigger('click')
    wrapper.vm.recoveryStep = 2
    wrapper.vm.recoveryEmail = 'test@mail.com'
    wrapper.vm.recoveryCode = '123456'
    wrapper.vm.newPasswordRec = '123456'
    wrapper.vm.confirmNewPasswordRec = '123456'
    await wrapper.vm.cambiarContrasena()
    expect(wrapper.vm.showPasswordChangeSuccessModal).toBe(true)
    expect(wrapper.vm.passwordChangeSuccessMessage).toMatch('¡Contraseña actualizada exitosamente!')
  })

  it('muestra error si cambio de contraseña falla', async () => {
    require('@/services/UserService').restablecerNuevaPassword.mockRejectedValueOnce(Object.assign(new Error('fail'), { message: 'fail' }))
    const wrapper = mountWithProviders()
    await wrapper.find('.forgot-password').trigger('click')
    wrapper.vm.recoveryStep = 2
    wrapper.vm.recoveryEmail = 'test@mail.com'
    wrapper.vm.recoveryCode = '123456'
    wrapper.vm.newPasswordRec = '123456'
    wrapper.vm.confirmNewPasswordRec = '123456'
    await wrapper.vm.cambiarContrasena()
    expect(wrapper.vm.recoveryMessageType).toBe('error')
    expect([
      'fail',
      'No se pudo cambiar la contraseña. Verifica el código o inténtalo más tarde.'
    ]).toContain(wrapper.vm.recoveryMessage)
  })
})
