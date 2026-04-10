import { mount } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import AboutView from '@/views/AboutView.vue'

describe('AboutView.vue', () => {
  let router

  beforeEach(() => {
    router = createRouter({
      history: createMemoryHistory(),
      routes: [{ path: '/', component: { template: '<div>Home</div>' } }]
    })
  })

  it('renders about view', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.find('.container').exists()).toBe(true)
  })

  it('displays main title', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.text()).toContain('Sobre Nosotros')
  })

  it('displays who we are section', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.text()).toContain('¿Quiénes somos?')
  })

  it('displays team members section', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.text()).toContain('Integrantes del Grupo')
  })

  it('displays all team members', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.text()).toContain('Alejandro Solórzano')
    expect(wrapper.text()).toContain('Álvaro Moya')
    expect(wrapper.text()).toContain('Antony Picado')
    expect(wrapper.text()).toContain('Kenneth Osorio')
  })

  it('displays team member emails', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.text()).toContain('alejandro.solorzanobaudrit@ucr.ac.cr')
    expect(wrapper.text()).toContain('alvaro.moyaarrieta@ucr.ac.cr')
  })

  it('displays history section', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.text()).toContain('Nuestra Historia')
  })

  it('displays values section', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.text()).toContain('Nuestros Valores')
  })

  it('displays all company values', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.text()).toContain('Innovación')
    expect(wrapper.text()).toContain('Calidad')
    expect(wrapper.text()).toContain('Trabajo en equipo')
    expect(wrapper.text()).toContain('Compromiso')
    expect(wrapper.text()).toContain('Empatía')
  })

  it('displays commitment section', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.text()).toContain('Nuestro Compromiso')
  })

  it('has multiple about sections', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    const sections = wrapper.findAll('.about-section')
    expect(sections.length).toBeGreaterThan(3)
  })

  it('displays person icons for team members', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    const icons = wrapper.findAll('.bi-person-circle')
    expect(icons.length).toBe(4)
  })

  it('has styled team list', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.find('.team-list').exists()).toBe(true)
  })

  it('has styled values list', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    expect(wrapper.find('.about-values').exists()).toBe(true)
  })

  it('displays email spans for contact info', () => {
    const wrapper = mount(AboutView, {
      global: { plugins: [router] }
    })
    const emails = wrapper.findAll('.email')
    expect(emails.length).toBe(4)
  })
})
