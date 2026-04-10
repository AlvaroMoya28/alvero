/* global jest, beforeAll, afterAll, describe, it, expect, beforeEach */
import { mount, flushPromises } from '@vue/test-utils'
import RegisterForm from '@/components/RegisterForm.vue'
import { createTestingPinia } from '@pinia/testing'

// Mock de vue-router
jest.mock('vue-router', () => ({
  useRouter: () => ({ push: jest.fn() }),
  RouterLink: { template: '<a><slot /></a>' }
}))


// Mock de usuarioService
const mockRegistrarUsuario = jest.fn();
jest.mock('@/services/UserService', () => ({
  usuarioService: { registrarUsuario: (...args) => mockRegistrarUsuario(...args) }
}));

// Silenciar warnings y errores de consola
beforeAll(() => {
  jest.spyOn(console, 'warn').mockImplementation(() => {})
  jest.spyOn(console, 'error').mockImplementation(() => {})
})
afterAll(() => {
  jest.restoreAllMocks()
})

describe('RegisterForm.vue', () => {
  beforeEach(() => {
    jest.clearAllMocks()
  })

  it('renderiza todos los campos obligatorios', () => {
    const wrapper = mount(RegisterForm, {
      global: {
        plugins: [createTestingPinia({ createSpy: jest.fn })],
        stubs: { RouterLink: true }
      }
    })
    expect(wrapper.find('input#inputIdUsuario').exists()).toBe(true)
    expect(wrapper.find('input#inputNombre').exists()).toBe(true)
    expect(wrapper.find('input#inputApellido1').exists()).toBe(true)
    expect(wrapper.find('input#inputEmail').exists()).toBe(true)
    expect(wrapper.find('input#inputNacimiento').exists()).toBe(true)
    expect(wrapper.find('input#inputContrasena').exists()).toBe(true)
    expect(wrapper.find('input#inputConfirmarContrasena').exists()).toBe(true)
  })

  it('muestra error si la contraseña no cumple requisitos', async () => {
    const wrapper = mount(RegisterForm, {
      global: {
        plugins: [createTestingPinia({ createSpy: jest.fn })],
        stubs: { RouterLink: true }
      }
    })
    await wrapper.find('input#inputIdUsuario').setValue('usuario1')
    await wrapper.find('input#inputNombre').setValue('Nombre')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('test@mail.com')
    await wrapper.find('input#inputNacimiento').setValue('2000-01-01')
    await wrapper.find('input#inputContrasena').setValue('123') // No cumple
    await wrapper.find('input#inputConfirmarContrasena').setValue('123')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('La contraseña no cumple con los requisitos')
    expect(wrapper.text()).toMatch('Debe tener al menos 8 caracteres')
  })

  it('muestra error si las contraseñas no coinciden', async () => {
    const wrapper = mount(RegisterForm, {
      global: {
        plugins: [createTestingPinia({ createSpy: jest.fn })],
        stubs: { RouterLink: true }
      }
    })
    await wrapper.find('input#inputIdUsuario').setValue('usuario1')
    await wrapper.find('input#inputNombre').setValue('Nombre')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('test@mail.com')
    await wrapper.find('input#inputNacimiento').setValue('2000-01-01')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password2!')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('Las contraseñas no coinciden')
  })

  it('muestra error si el usuario es menor de 18 años', async () => {
    const wrapper = mount(RegisterForm, {
      global: {
        plugins: [createTestingPinia({ createSpy: jest.fn })],
        stubs: { RouterLink: true }
      }
    })
    await wrapper.find('input#inputIdUsuario').setValue('usuario1')
    await wrapper.find('input#inputNombre').setValue('Nombre')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('test@mail.com')
    await wrapper.find('input#inputNacimiento').setValue('2020-01-01')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password1!')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('Debes tener al menos 18 años')
  })

  it('llama a usuarioService.registrarUsuario y redirige si el registro es exitoso', async () => {
    mockRegistrarUsuario.mockResolvedValueOnce({ data: { idUsuario: 'usuario1' } })
    const push = jest.fn()
    jest.spyOn(require('vue-router'), 'useRouter').mockReturnValue({ push })
    const wrapper = mount(RegisterForm, {
      global: {
        plugins: [createTestingPinia({ createSpy: jest.fn })],
        stubs: { RouterLink: true }
      }
    })
    await wrapper.find('input#inputIdUsuario').setValue('usuario1')
    await wrapper.find('input#inputNombre').setValue('Nombre')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('test@mail.com')
    await wrapper.find('input#inputNacimiento').setValue('2000-01-01')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password1!')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(mockRegistrarUsuario).toHaveBeenCalled()
    expect(push).toHaveBeenCalledWith({ name: 'LogIn' })
  })

  it('muestra errores de backend si el registro falla', async () => {
    mockRegistrarUsuario.mockRejectedValueOnce({
      response: {
        data: {
          errors: { email: ['El email ya está en uso'] },
          message: 'Error de validación'
        }
      }
    })
    const wrapper = mount(RegisterForm, {
      global: {
        plugins: [createTestingPinia({ createSpy: jest.fn })],
        stubs: { RouterLink: true }
      }
    })
    await wrapper.find('input#inputIdUsuario').setValue('usuario1')
    await wrapper.find('input#inputNombre').setValue('Nombre')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('test@mail.com')
    await wrapper.find('input#inputNacimiento').setValue('2000-01-01')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password1!')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('Error de validación')
    expect(wrapper.text()).toMatch('El email ya está en uso')
  })

  it('muestra mensaje de error inesperado si el backend no responde', async () => {
    mockRegistrarUsuario.mockRejectedValueOnce(new Error('Network Error'))
    const wrapper = mount(RegisterForm, {
      global: {
        plugins: [createTestingPinia({ createSpy: jest.fn })],
        stubs: { RouterLink: true }
      }
    })
    await wrapper.find('input#inputIdUsuario').setValue('usuario1')
    await wrapper.find('input#inputNombre').setValue('Nombre')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('test@mail.com')
    await wrapper.find('input#inputNacimiento').setValue('2000-01-01')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password1!')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('Network Error')
  })
})
