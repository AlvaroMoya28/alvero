


/* global jest, beforeAll, afterAll, describe, it, expect, beforeEach */
import { mount, flushPromises } from '@vue/test-utils'
import NavBar from '@/components/NavBar.vue'
import { createTestingPinia } from '@pinia/testing'
import { useUserStore } from '@/stores/userStore'
import { createRouter, createMemoryHistory } from 'vue-router'

// Silenciar warnings y errores de consola para evitar ruido en los tests
let errorSpy, warnSpy
beforeAll(() => {
  warnSpy = jest.spyOn(console, 'warn').mockImplementation(() => {})
  errorSpy = jest.spyOn(console, 'error').mockImplementation(() => {})
})
afterAll(() => {
  warnSpy.mockRestore()
  errorSpy.mockRestore()
})

jest.setTimeout(15000) // evitar timeout en tests lentos

const Dummy = { template: '<div>dummy</div>' }

describe('NavBar.vue', () => {
  let wrapper
  let userStore
  let router

  beforeEach(async () => {
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/', name: 'home', component: Dummy },
        { path: '/salas', name: 'salas', component: Dummy },
        { path: '/ofertas', name: 'ofertas', component: Dummy },
        { path: '/ubicacion', name: 'ubicacion', component: Dummy },
        { path: '/admin', name: 'admin', component: Dummy },
        { path: '/admin/dashboard', name: 'admin-dashboard', component: Dummy },
        { path: '/editar-salas', name: 'editar-salas', component: Dummy },
        { path: '/logIn', name: 'logIn', component: Dummy },
        { path: '/perfil', name: 'perfil', component: Dummy },
        { path: '/mis-reservas', name: 'mis-reservas', component: Dummy },
        { path: '/reservas', name: 'reservas', component: Dummy },
      ],
    })
    router.push('/')
    await router.isReady()

    const pinia = createTestingPinia({ stubActions: false })
    wrapper = mount(NavBar, {
      global: {
        plugins: [pinia, router],
      },
    })

    userStore = useUserStore()
  })

  it('renderiza el componente con elementos básicos', () => {
    expect(wrapper.find('.navbar-brand').exists()).toBe(true)
  })

  it('muestra menú lateral al hacer toggle y lo oculta', async () => {
    expect(wrapper.vm.navOpen).toBe(false)
    await wrapper.find('.sidemenu__btn').trigger('click')
    expect(wrapper.vm.navOpen).toBe(true)

    const backdrop = wrapper.find('.sidemenu-backdrop')
    await backdrop.trigger('click')
    expect(wrapper.vm.navOpen).toBe(false)
  })

  it('muestra enlaces solo para admin cuando isAdmin es true', async () => {
    userStore.user = { tipoUsuario: 'ADMINISTRADOR' }
    userStore.isAuthenticated = true
    await flushPromises()

    expect(wrapper.vm.isAdmin).toBe(true)
    expect(wrapper.text()).toContain('Dashboard')
  })

  it('muestra botones de login o perfil según autenticación', async () => {
    userStore.isAuthenticated = false
    await flushPromises()
    expect(wrapper.text()).toContain('Iniciar Sesión')

    userStore.isAuthenticated = true
    userStore.user = { tipoUsuario: 'USUARIO' }
    await flushPromises()
    expect(wrapper.text()).toContain('Mi Perfil')
  })

  it('abre y cierra modal de logout', async () => {
    // Simula usuario autenticado para que aparezca el enlace "Cerrar Sesión"
    userStore.isAuthenticated = true
    userStore.user = { tipoUsuario: 'USUARIO' }
    await flushPromises()

    // Vuelve a montar para que tome los cambios del store
    await wrapper.vm.$nextTick()
    
    const logoutLink = wrapper.findAll('a.nav-link').filter(link => link.text().includes('Cerrar Sesión'))[0]
    expect(logoutLink).toBeTruthy() // para confirmar que existe

    await logoutLink.trigger('click')
    expect(wrapper.vm.showLogoutModal).toBe(true)

    // Botón cancelar
    await wrapper.find('button.btn-logout-cancel').trigger('click')
    expect(wrapper.vm.showLogoutModal).toBe(false)
  })

  it('confirma logout y llama a userStore.logout y router.push', async () => {
    userStore.logout = jest.fn().mockResolvedValue()
    const routerPushSpy = jest.spyOn(router, 'push')

    wrapper.vm.showLogoutModal = true
    await wrapper.vm.confirmLogout()

    expect(userStore.logout).toHaveBeenCalled()
    expect(routerPushSpy).toHaveBeenCalledWith('/')
    expect(wrapper.vm.showLogoutModal).toBe(false)
  })
})
