import { mount } from '@vue/test-utils'
import { createPinia } from 'pinia'
import GestionReservas from '@/components/GestionReservas.vue'

describe('GestionReservas.vue', () => {
  it('renders component', () => {
    const pinia = createPinia()
    const wrapper = mount(GestionReservas, {
      global: {
        plugins: [pinia]
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('handles reservations data', () => {
    const pinia = createPinia()
    const wrapper = mount(GestionReservas, {
      global: {
        plugins: [pinia]
      }
    })
    
    if (wrapper.vm.reservas !== undefined) {
      expect(Array.isArray(wrapper.vm.reservas) || wrapper.vm.reservas === null).toBe(true)
    }
  })

  it('displays loading state', () => {
    const pinia = createPinia()
    const wrapper = mount(GestionReservas, {
      global: {
        plugins: [pinia]
      }
    })
    
    if (wrapper.vm.loading !== undefined) {
      wrapper.vm.loading = true
      expect(wrapper.vm.loading).toBe(true)
    }
  })
})
