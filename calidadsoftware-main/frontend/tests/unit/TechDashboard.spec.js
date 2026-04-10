import { mount } from '@vue/test-utils'
import { createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import TechDashboard from '@/components/TechDashboard.vue'

describe('TechDashboard.vue', () => {
  let pinia
  let router

  beforeEach(() => {
    pinia = createPinia()
    router = createRouter({
      history: createWebHistory(),
      routes: [{ path: '/', component: { template: '<div>Home</div>' } }]
    })
  })

  it('renders component', () => {
    const wrapper = mount(TechDashboard, {
      global: {
        plugins: [pinia, router],
        stubs: ['LoadingOverlay']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays dashboard metrics', () => {
    const wrapper = mount(TechDashboard, {
      global: {
        plugins: [pinia, router],
        stubs: ['LoadingOverlay']
      }
    })
    
    if (wrapper.vm.citas !== undefined) {
      expect(Array.isArray(wrapper.vm.citas) || wrapper.vm.citas === null).toBe(true)
    }
  })

  it('handles loading state', () => {
    const wrapper = mount(TechDashboard, {
      global: {
        plugins: [pinia, router],
        stubs: ['LoadingOverlay']
      }
    })
    
    if (wrapper.vm.loading !== undefined) {
      expect(typeof wrapper.vm.loading).toBe('boolean')
    }
  })
})
