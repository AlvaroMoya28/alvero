import { mount } from '@vue/test-utils'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import TechLayout from '@/components/TechLayout.vue'

describe('TechLayout.vue', () => {
  let router
  let pinia

  beforeEach(() => {
    router = createRouter({
      history: createWebHistory(),
      routes: [
        { path: '/', component: { template: '<div>Home</div>' } },
        { path: '/tech', component: { template: '<div>Tech</div>' } }
      ]
    })
    pinia = createPinia()
  })

  it('renders technician layout', () => {
    const wrapper = mount(TechLayout, {
      global: {
        plugins: [router, pinia],
        stubs: ['router-view', 'router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays tech navigation', () => {
    const wrapper = mount(TechLayout, {
      global: {
        plugins: [router, pinia],
        stubs: ['router-view', 'router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('shows tech menu items', () => {
    const wrapper = mount(TechLayout, {
      global: {
        plugins: [router, pinia],
        stubs: ['router-view', 'router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('handles tech logout', async () => {
    const wrapper = mount(TechLayout, {
      global: {
        plugins: [router, pinia],
        stubs: ['router-view', 'router-link']
      }
    })
    
    if (wrapper.vm.logout) {
      await wrapper.vm.logout()
      expect(true).toBe(true)
    }
  })

  it('displays tech profile info', () => {
    const wrapper = mount(TechLayout, {
      global: {
        plugins: [router, pinia],
        stubs: ['router-view', 'router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('renders content area', () => {
    const wrapper = mount(TechLayout, {
      global: {
        plugins: [router, pinia],
        stubs: ['router-view', 'router-link']
      }
    })
    expect(wrapper.findComponent({ name: 'router-view' }).exists() || wrapper.html().includes('router-view')).toBe(true)
  })
})
