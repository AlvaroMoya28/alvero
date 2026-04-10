const globalMountOptions = {
  attachTo: document.body,
  global: {
    mocks: {
      $route: { path: '/' }
    },
    stubs: ['router-link', 'router-view']
  }
}

/* global jest, beforeAll, afterAll, describe, it, expect, beforeEach */
// Mock de AdminLayout
jest.mock('@/components/AdminLayout.vue', () => ({
  default: { template: '<div><slot></slot></div>' }
}))

// Mock de vue-router
let mockPush;
jest.mock('vue-router', () => ({
  useRouter: () => {
    if (!mockPush) mockPush = jest.fn();
    return { push: mockPush };
  },
}));

// Mock de usuarioService
const mockRegistrarUsuario = jest.fn();
jest.mock('@/services/UserService', () => ({
  usuarioService: { registrarUsuario: (...args) => mockRegistrarUsuario(...args) }
}));

import { mount, flushPromises } from '@vue/test-utils'
import RegisterUser from '../../src/components/RegisterUser.vue'


// Un solo beforeAll para limpiar logs y mockear scrollIntoView
beforeAll(() => {
  Element.prototype.scrollIntoView = jest.fn()
  jest.spyOn(console, 'warn').mockImplementation(() => {})
  jest.spyOn(console, 'error').mockImplementation(() => {})
  jest.spyOn(console, 'log').mockImplementation((...args) => {
    if (
      typeof args[0] === 'string' &&
      (
        args[0].includes('<!-- TEST:') ||
        args[0].includes('Request:') ||
        args[0].includes('Usuario registrado:') ||
        args[0].includes('Enviando datos de registro:')
      )
    ) {
      // No hace nada, oculta el log
    }
  })
})
afterAll(() => {
  jest.restoreAllMocks()
})

describe('RegisterUser.vue', () => {
  beforeEach(() => {
    jest.clearAllMocks()
    mockPush = jest.fn() // Reinicializa mockPush antes de cada test
  })

  it('renderiza todos los campos obligatorios', () => {
    const wrapper = mount(RegisterUser, globalMountOptions)
    // Log para depuración: ver qué se renderiza realmente
    // eslint-disable-next-line no-console
    console.log(wrapper.html())
    expect(wrapper.find('input#inputIdUsuario').exists()).toBe(true)
    expect(wrapper.find('input#inputNombre').exists()).toBe(true)
    expect(wrapper.find('input#inputApellido1').exists()).toBe(true)
    expect(wrapper.find('input#inputEmail').exists()).toBe(true)
    expect(wrapper.find('input#inputContrasena').exists()).toBe(true)
    expect(wrapper.find('input#inputConfirmarContrasena').exists()).toBe(true)
    expect(wrapper.find('input#inputNacimiento').exists()).toBe(true)
    expect(wrapper.find('select#inputRol').exists()).toBe(true)
  })

  it('muestra error si las contraseñas no coinciden', async () => {
    const wrapper = mount(RegisterUser, globalMountOptions)
    await wrapper.find('input#inputIdUsuario').setValue('admin1')
    await wrapper.find('input#inputNombre').setValue('Admin')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('admin@mail.com')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password2!')
    await wrapper.find('input#inputNacimiento').setValue('2000-01-01')
    await wrapper.find('select#inputRol').setValue('ADMINISTRADOR')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('Las contraseñas no coinciden')
  })

  it('muestra error si el usuario es menor de 18 años', async () => {
    const wrapper = mount(RegisterUser, globalMountOptions)
    await wrapper.find('input#inputIdUsuario').setValue('admin1')
    await wrapper.find('input#inputNombre').setValue('Admin')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('admin@mail.com')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password1!')
    await wrapper.find('input#inputNacimiento').setValue('2020-01-01')
    await wrapper.find('select#inputRol').setValue('ADMINISTRADOR')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('Debes tener al menos 18 años')
  })

  it('llama a usuarioService.registrarUsuario y redirige si el registro es exitoso', async () => {
    mockRegistrarUsuario.mockResolvedValueOnce({ data: { idUsuario: 'admin1' } })
    const wrapper = mount(RegisterUser, globalMountOptions)
    await wrapper.find('input#inputIdUsuario').setValue('admin1')
    await wrapper.find('input#inputNombre').setValue('Admin')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('admin@mail.com')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password1!')
    await wrapper.find('input#inputNacimiento').setValue('2000-01-01')
    await wrapper.find('select#inputRol').setValue('ADMINISTRADOR')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(mockRegistrarUsuario).toHaveBeenCalled()
    expect(mockPush).toHaveBeenCalledWith({ name: 'Panel' })
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
    const wrapper = mount(RegisterUser, globalMountOptions)
    await wrapper.find('input#inputIdUsuario').setValue('admin1')
    await wrapper.find('input#inputNombre').setValue('Admin')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('admin@mail.com')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password1!')
    await wrapper.find('input#inputNacimiento').setValue('2000-01-01')
    await wrapper.find('select#inputRol').setValue('ADMINISTRADOR')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('Por favor corrige los errores en el formulario')
    expect(wrapper.text()).toMatch('El email ya está en uso')
  })

  it('muestra mensaje de error inesperado si el backend no responde', async () => {
    mockRegistrarUsuario.mockRejectedValueOnce(new Error('Network Error'))
    const wrapper = mount(RegisterUser, globalMountOptions)
    await wrapper.find('input#inputIdUsuario').setValue('admin1')
    await wrapper.find('input#inputNombre').setValue('Admin')
    await wrapper.find('input#inputApellido1').setValue('Apellido')
    await wrapper.find('input#inputEmail').setValue('admin@mail.com')
    await wrapper.find('input#inputContrasena').setValue('Password1!')
    await wrapper.find('input#inputConfirmarContrasena').setValue('Password1!')
    await wrapper.find('input#inputNacimiento').setValue('2000-01-01')
    await wrapper.find('select#inputRol').setValue('ADMINISTRADOR')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()
    expect(wrapper.text()).toMatch('Network Error')
  })
})
