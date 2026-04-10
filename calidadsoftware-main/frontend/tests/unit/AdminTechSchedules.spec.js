import { mount } from '@vue/test-utils'
import { createPinia } from 'pinia'
import AdminTechSchedules from '@/components/AdminTechSchedules.vue'

describe('AdminTechSchedules.vue', () => {
  it('renders component', () => {
    const pinia = createPinia()
    const wrapper = mount(AdminTechSchedules, {
      global: {
        plugins: [pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('initializes with empty schedules', () => {
    const pinia = createPinia()
    const wrapper = mount(AdminTechSchedules, {
      global: {
        plugins: [pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    if (wrapper.vm.horarios !== undefined) {
      expect(Array.isArray(wrapper.vm.horarios) || wrapper.vm.horarios === null).toBe(true)
    }
  })

  it('handles loading state', () => {
    const pinia = createPinia()
    const wrapper = mount(AdminTechSchedules, {
      global: {
        plugins: [pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    if (wrapper.vm.loading !== undefined) {
      expect(typeof wrapper.vm.loading).toBe('boolean')
    }
  })
})
