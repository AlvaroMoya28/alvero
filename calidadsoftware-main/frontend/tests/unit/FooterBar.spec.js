/* global jest, beforeAll, afterAll, describe, it, expect, beforeEach */
/* global jest, beforeAll, afterAll, describe, it, expect, beforeEach */
import { mount } from '@vue/test-utils'
import FooterBar from '@/components/FooterBar.vue'
import { createRouter, createWebHistory } from 'vue-router'
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

describe('FooterBar.vue', () => {
  let wrapper
  let router

  beforeEach(async () => {
    router = createRouter({
      history: createWebHistory(),
      routes: [
        { path: '/salas', name: 'Salas', component: { template: '<div>Salas</div>' } },
        { path: '/ofertas', name: 'Ofertas', component: { template: '<div>Ofertas</div>' } },
        { path: '/contactanos', name: 'Contactanos', component: { template: '<div>Contactanos</div>' } },
        { path: '/ubicacion', name: 'Ubicacion', component: { template: '<div>Ubicacion</div>' } },
        { path: '/about', name: 'About', component: { template: '<div>About</div>' } },
        { path: '/estandares', name: 'Estandares', component: { template: '<div>Estandares</div>' } }
      ]
    })
    wrapper = mount(FooterBar, {
      global: {
        plugins: [router]
      }
    })
    await router.isReady()
  })

  it('renderiza secciones y enlaces correctamente', () => {
    expect(wrapper.text()).toContain('Políticas')
    expect(wrapper.text()).toContain('Ayuda')
    expect(wrapper.text()).toContain('Servicios')
    expect(wrapper.findAll('a').length).toBeGreaterThan(5)
  })

  it('contiene los enlaces correctos', () => {
    const links = wrapper.findAll('a')
    const hrefs = links.map(a => a.attributes('href'))
    expect(hrefs).toContain('/terminos')
    expect(hrefs).toContain('/contacto')
    expect(hrefs).toContain('/reservar')
  })

  it('muestra el logo y copyright', () => {
    expect(wrapper.text()).toContain('DoctorPC')
    expect(wrapper.text()).toContain('Todos los derechos reservados')
  })
})
