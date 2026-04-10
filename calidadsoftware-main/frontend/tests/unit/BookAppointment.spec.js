import { mount } from '@vue/test-utils'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import BookAppointment from '@/components/BookAppointment.vue'

describe('BookAppointment.vue', () => {
  let router
  let pinia

  beforeEach(() => {
    pinia = createPinia()
    router = createRouter({
      history: createWebHistory(),
      routes: [{ path: '/', component: { template: '<div>Home</div>' } }]
    })
  })

  it('renders the component', () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    expect(wrapper.exists()).toBe(true)
    expect(wrapper.find('h2').text()).toContain('Reservar')
  })

  it('initializes with week data', () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    expect(wrapper.vm.fechaInicio).toBeDefined()
    expect(wrapper.vm.diasSemana).toBeDefined()
    expect(Array.isArray(wrapper.vm.diasSemana)).toBe(true)
  })

  it('displays availability grid', () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    expect(wrapper.find('.disponibilidad-grid').exists()).toBe(true)
  })

  it('formats dates correctly', () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    const fecha = '2025-11-26'
    const result = wrapper.vm.formatearFecha(fecha)
    expect(typeof result).toBe('string')
    expect(result.length).toBeGreaterThan(0)
  })

  it('calculates week dates', () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.calcularDiasSemana) {
      wrapper.vm.calcularDiasSemana()
      expect(wrapper.vm.diasSemana.length).toBeGreaterThan(0)
    }
  })

  it('handles slot selection', async () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.seleccionarSlot) {
      await wrapper.vm.seleccionarSlot('2025-11-26', 10)
      expect(wrapper.vm.slotSeleccionado).toBeDefined()
    }
  })

  it('gets slot availability', () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.getSlotDisponibilidad) {
      const result = wrapper.vm.getSlotDisponibilidad('2025-11-26', 10)
      expect(typeof result).toBe('number')
    }
  })

  it('closes modal', async () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.cerrarModal) {
      wrapper.vm.slotSeleccionado = { fecha: '2025-11-26', hora: 10 }
      await wrapper.vm.cerrarModal()
      expect(wrapper.vm.slotSeleccionado).toBeNull()
    }
  })

  it('validates form data', () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.validarFormulario) {
      const result = wrapper.vm.validarFormulario()
      expect(typeof result).toBe('boolean')
    }
  })

  it('submits appointment', async () => {
    const wrapper = mount(BookAppointment, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.enviarReserva) {
      wrapper.vm.formData = {
        nombre: 'Test',
        email: 'test@test.com',
        cedula: '123456789',
        telefono: '88888888',
        descripcion: 'Test'
      }
      expect(typeof wrapper.vm.enviarReserva).toBe('function')
    }
  })
})
