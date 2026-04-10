
/* eslint-env jest */
import { mount, flushPromises } from '@vue/test-utils'
import UserManagement from '@/components/UserManagement.vue'
import { createTestingPinia } from '@pinia/testing'
import { useUserStore } from '@/stores/userStore'
import { nextTick } from 'vue'

// Silenciar logs y warnings de consola solo en este archivo
let errorSpy, warnSpy, logSpy;
beforeAll(() => {
  errorSpy = jest.spyOn(console, 'error').mockImplementation((...args) => {
    // Oculta errores de Vue, Jest y logs de servicios
    if (
      typeof args[0] === 'string' &&
      (
        args[0].includes('[Vue warn]') ||
        args[0].includes('Error al') ||
        args[0].includes('Error en') ||
        args[0].includes('Network Error')
      )
    ) {
      // Solo silencia el log, no es necesario return
    }
    // Si quieres ver otros errores, comenta la siguiente línea
    return;
  });
  warnSpy = jest.spyOn(console, 'warn').mockImplementation((...args) => {
    if (
      typeof args[0] === 'string' &&
      (
        args[0].includes('[Vue warn]') ||
        args[0].includes('No match found for location')
      )
    ) {
      return;
    }
    return;
  });
  logSpy = jest.spyOn(console, 'log').mockImplementation((...args) => {
    // Oculta logs de componentes y tests
    if (
      typeof args[0] === 'string' &&
      (
        args[0].includes('<!-- TEST:') ||
        args[0].includes('Request:') ||
        args[0].includes('Usuario registrado:') ||
        args[0].includes('Enviando datos de registro:')
      )
    ) {
      return;
    }
    return;
  });
});
afterAll(() => {
  errorSpy.mockRestore();
  warnSpy.mockRestore();
  logSpy.mockRestore();
});

// Mock global components and router
const globalStubs = {
  'router-link': {
    template: '<a><slot /></a>'
  },
  'router-view': { template: '<div />' },
  AdminLayout: {
    template: '<div><slot /></div>'
  }
}

const globalMocks = {
  $route: { path: '/' },
  $router: { push: () => {} }
}

jest.mock('@/services/UserService', () => ({
  usuarioService: {
    getAllUsers: jest.fn(),
    updateUser: jest.fn()
  }
}))

jest.mock('vue-toastification', () => ({
  useToast: () => ({
    error: jest.fn(),
    warning: jest.fn(),
    success: jest.fn()
  })
}))

describe('UserManagement.vue', () => {
  let wrapper
  let userStore
  // Para máxima cobertura, el usuario actual es superusuario por defecto
  const usersMock = [
    {
      idUsuario: 1,
      nombre: 'Juan',
      apellido1: 'Pérez',
      apellido2: 'Gómez',
      email: 'juan@example.com',
      telefono: '123456789',
      tipoUsuario: 'ADMINISTRADOR',
      estado: 'ACTIVO'
    },
    {
      idUsuario: 2,
      nombre: 'Ana',
      apellido1: 'López',
      apellido2: '',
      email: 'ana@example.com',
      telefono: '',
      tipoUsuario: 'CLIENTE',
      estado: 'INACTIVO'
    },
    {
      idUsuario: 3,
      nombre: 'Super',
      apellido1: 'User',
      apellido2: '',
      email: 'super@example.com',
      telefono: '',
      tipoUsuario: 'SUPERUSUARIO',
      estado: 'ACTIVO'
    }
  ]

  beforeEach(async () => {
    const { usuarioService } = require('@/services/UserService')
    usuarioService.getAllUsers.mockResolvedValue(usersMock)
    usuarioService.updateUser.mockResolvedValue({ ...usersMock[0], nombre: 'Juanito' })

    wrapper = mount(UserManagement, {
      global: {
        plugins: [createTestingPinia({
          initialState: {
            userStore: {
              user: { idUsuario: 99, tipoUsuario: 'SUPERUSUARIO' }
            }
          },
          createSpy: jest.fn
        })],
        stubs: globalStubs,
        mocks: globalMocks
      }
    })
    userStore = useUserStore()
    // Asegura que user siempre esté definido
    if (!userStore.user) {
      userStore.user = { idUsuario: 99, tipoUsuario: 'SUPERUSUARIO' }
    }
    await flushPromises()
  })

  it('renderiza la tabla de usuarios y filtros', async () => {
    // Mostrar todos los usuarios: quitar filtro de estado
    await wrapper.find('.status-filter').setValue('')
    await nextTick()
    expect(wrapper.find('.users-table').exists()).toBe(true)
    expect(wrapper.findAll('tbody tr')).toHaveLength(usersMock.length)
    expect(wrapper.find('.search-input').exists()).toBe(true)
    expect(wrapper.find('.role-filter').exists()).toBe(true)
    expect(wrapper.find('.status-filter').exists()).toBe(true)
  })

  it('filtra usuarios por búsqueda', async () => {
    // Mostrar todos los usuarios antes de buscar
    await wrapper.find('.status-filter').setValue('')
    await nextTick()
    await wrapper.find('.search-input').setValue('ana')
    await nextTick()
    // Solo Ana debe aparecer
    const rows = wrapper.findAll('tbody tr')
    expect(rows).toHaveLength(1)
    expect(wrapper.html()).toContain('ana@example.com')
  })

  it('filtra usuarios por rol', async () => {
    // Mostrar todos los usuarios antes de filtrar
    await wrapper.find('.status-filter').setValue('')
    await nextTick()
    await wrapper.find('.role-filter').setValue('CLIENTE')
    await nextTick()
    const rows = wrapper.findAll('tbody tr')
    expect(rows).toHaveLength(1)
    expect(wrapper.html()).toContain('ana@example.com')
  })

  it('filtra usuarios por estado', async () => {
    await wrapper.find('.status-filter').setValue('INACTIVO')
    await nextTick()
    const rows = wrapper.findAll('tbody tr')
    expect(rows).toHaveLength(1)
    expect(wrapper.html()).toContain('INACTIVO')
  })

  it('muestra mensaje si no hay usuarios', async () => {
    const { usuarioService } = require('@/services/UserService')
    usuarioService.getAllUsers.mockResolvedValue([])
    const emptyWrapper = mount(UserManagement, {
      global: {
        plugins: [createTestingPinia({
          initialState: { userStore: { user: { idUsuario: 99, tipoUsuario: 'SUPERUSUARIO' } } },
          createSpy: jest.fn
        })],
        stubs: globalStubs,
        mocks: globalMocks
      }
    })
    await flushPromises()
    expect(emptyWrapper.find('.no-users').exists()).toBe(true)
  })

  it('abre el modal de edición y edita usuario', async () => {
    // Busca el primer usuario editable (no el superusuario ni el propio user)
    const btns = wrapper.findAll('.action-btn')
    await btns[0].trigger('click')
    await nextTick()
    expect(wrapper.find('.modal-content').exists()).toBe(true)
    await wrapper.find('input[type="text"]').setValue('Juanito')
    await wrapper.find('.btn-save').trigger('submit')
    await flushPromises()
    expect(wrapper.find('.modal').exists()).toBe(true)
    expect(wrapper.html()).toContain('Usuario actualizado correctamente')
  })

  it('desactiva usuario', async () => {
    const btns = wrapper.findAll('.action-btn')
    await btns[0].trigger('click')
    await nextTick()
    await wrapper.find('.btn-delete').trigger('click')
    await flushPromises()
    expect(wrapper.find('.modal').exists()).toBe(true)
    expect(wrapper.html()).toContain('desactivado correctamente')
  })

  it('reactiva usuario', async () => {
    // Mostrar todos los usuarios antes de buscar el botón de Ana
    await wrapper.find('.status-filter').setValue('')
    await nextTick()
    const btns = wrapper.findAll('.action-btn')
    // El segundo usuario es el inactivo (Ana)
    await btns[1].trigger('click')
    await nextTick()
    await wrapper.find('.btn-reactivate').trigger('click')
    await flushPromises()
    expect(wrapper.find('.modal').exists()).toBe(true)
    expect(wrapper.html()).toContain('reactivado correctamente')
  })

  it('no permite editar superusuario si no eres superusuario', async () => {
    // Cambia el tipo de usuario actual a ADMINISTRADOR y vuelve a montar el componente
    const { usuarioService } = require('@/services/UserService')
    usuarioService.getAllUsers.mockResolvedValue(usersMock)
    const localWrapper = mount(UserManagement, {
      global: {
        plugins: [createTestingPinia({
          initialState: {
            userStore: {
              user: { idUsuario: 99, tipoUsuario: 'ADMINISTRADOR' }
            }
          },
          createSpy: jest.fn
        })],
        stubs: globalStubs,
        mocks: globalMocks
      }
    })
    await flushPromises()
    await localWrapper.find('.status-filter').setValue('')
    await nextTick()
    // Buscar el botón de acción correspondiente al superusuario (por email)
    const rows = localWrapper.findAll('tbody tr')
    let superBtn = null
    for (const row of rows) {
      if (row.html().includes('super@example.com')) {
        superBtn = row.find('.action-btn')
        break
      }
    }
    expect(superBtn).not.toBeNull()
    expect(superBtn.attributes('disabled')).toBeDefined()
  })

  it('no permite editarte a ti mismo', async () => {
    // Cambia el idUsuario del usuario actual a 1 y vuelve a montar el componente
    const { usuarioService } = require('@/services/UserService')
    usuarioService.getAllUsers.mockResolvedValue(usersMock)
    const localWrapper = mount(UserManagement, {
      global: {
        plugins: [createTestingPinia({
          initialState: {
            userStore: {
              user: { idUsuario: 1, tipoUsuario: 'SUPERUSUARIO' }
            }
          },
          createSpy: jest.fn
        })],
        stubs: globalStubs,
        mocks: globalMocks
      }
    })
    await flushPromises()
    await localWrapper.find('.status-filter').setValue('')
    await nextTick()
    // Buscar el botón de acción correspondiente al usuario actual (por email)
    const rows = localWrapper.findAll('tbody tr')
    let selfBtn = null
    for (const row of rows) {
      if (row.html().includes('juan@example.com')) {
        selfBtn = row.find('.action-btn')
        break
      }
    }
    expect(selfBtn).not.toBeNull()
    expect(selfBtn.attributes('disabled')).toBeDefined()
  })

  it('muestra error si falla la carga de usuarios', async () => {
    const { usuarioService } = require('@/services/UserService')
    usuarioService.getAllUsers.mockRejectedValue(new Error('fail'))
    const errorWrapper = mount(UserManagement, {
      global: {
        plugins: [createTestingPinia({
          initialState: { userStore: { user: { idUsuario: 99, tipoUsuario: 'SUPERUSUARIO' } } },
          createSpy: jest.fn
        })],
        stubs: globalStubs,
        mocks: globalMocks
      }
    })
    await flushPromises()
    // Si hay error, loading se pone en false y users.length === 0, así que debe mostrar el mensaje de no usuarios
    expect(errorWrapper.find('.no-users').exists()).toBe(true)
  })

  it('muestra error si falla la actualización', async () => {
    const { usuarioService } = require('@/services/UserService')
    usuarioService.updateUser.mockRejectedValue(new Error('fail'))
    await wrapper.findAll('.action-btn')[0].trigger('click')
    await nextTick()
    await wrapper.find('.btn-save').trigger('submit')
    await flushPromises()
    // El modal de éxito no debe mostrarse
    expect(wrapper.find('.modal').exists()).toBe(false)
  })
})
