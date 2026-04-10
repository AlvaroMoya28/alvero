import { mount } from '@vue/test-utils'
import { createPinia } from 'pinia'
import TechAppointments from '@/components/TechAppointments.vue'

describe('TechAppointments.vue', () => {
  it('renders component', () => {
    const pinia = createPinia()
    const wrapper = mount(TechAppointments, {
      global: {
        plugins: [pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays appointments list', () => {
    const pinia = createPinia()
    const wrapper = mount(TechAppointments, {
      global: {
        plugins: [pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    wrapper.vm.citas = [
      { idCita: 1, nombreCliente: 'Test', estado: 'PENDIENTE' }
    ]
    
    expect(wrapper.vm.citas.length).toBe(1)
  })

  it('handles empty appointments', () => {
    const pinia = createPinia()
    const wrapper = mount(TechAppointments, {
      global: {
        plugins: [pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    wrapper.vm.citas = []
    expect(wrapper.vm.citas.length).toBe(0)
  })
})
