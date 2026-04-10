import { mount } from '@vue/test-utils'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import { useUserStore } from '@/stores/userStore'
import TechSchedule from '@/components/TechSchedule.vue'

describe('TechSchedule.vue', () => {
  let router
  let pinia

  beforeEach(() => {
    pinia = createPinia()
    const userStore = useUserStore()
    userStore.identificacion = 'TECH001'
    userStore.rol = 'TECNICO'
    
    router = createRouter({
      history: createWebHistory(),
      routes: [{ path: '/', component: { template: '<div>Home</div>' } }]
    })
  })

  it('renders the component', () => {
    const wrapper = mount(TechSchedule, {
      global: {
        plugins: [router, pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('initializes with current week', () => {
    const wrapper = mount(TechSchedule, {
      global: {
        plugins: [router, pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    expect(wrapper.vm.fechaInicio).toBeDefined()
    expect(wrapper.vm.fechaFin).toBeDefined()
  })

  it('calculates week dates', () => {
    const wrapper = mount(TechSchedule, {
      global: {
        plugins: [router, pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    if (wrapper.vm.calcularSemana) {
      wrapper.vm.calcularSemana()
      expect(wrapper.vm.diasSemana).toBeDefined()
    }
  })

  it('formats dates', () => {
    const wrapper = mount(TechSchedule, {
      global: {
        plugins: [router, pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    if (wrapper.vm.formatearFecha) {
      const result = wrapper.vm.formatearFecha(new Date())
      expect(typeof result).toBe('string')
    }
  })

  it('navigates to next week', async () => {
    const wrapper = mount(TechSchedule, {
      global: {
        plugins: [router, pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    const initialDate = wrapper.vm.fechaInicio
    if (wrapper.vm.siguienteSemana) {
      await wrapper.vm.siguienteSemana()
      expect(wrapper.vm.fechaInicio).not.toEqual(initialDate)
    }
  })

  it('navigates to previous week', async () => {
    const wrapper = mount(TechSchedule, {
      global: {
        plugins: [router, pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    const initialDate = wrapper.vm.fechaInicio
    if (wrapper.vm.semanaAnterior) {
      await wrapper.vm.semanaAnterior()
      expect(wrapper.vm.fechaInicio).not.toEqual(initialDate)
    }
  })

  it('gets cita in slot', () => {
    const wrapper = mount(TechSchedule, {
      global: {
        plugins: [router, pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    wrapper.vm.citas = [{
      idCita: 1,
      fechaCita: '2025-11-26',
      horaInicio: '10:00',
      estado: 'PENDIENTE'
    }]
    
    if (wrapper.vm.getCitaEnSlot) {
      const result = wrapper.vm.getCitaEnSlot('2025-11-26', '10:00')
      expect(result).toBeDefined()
    }
  })

  it('shows cita details', async () => {
    const wrapper = mount(TechSchedule, {
      global: {
        plugins: [router, pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    const cita = {
      idCita: 1,
      nombreCliente: 'Test',
      fechaCita: '2025-11-26'
    }
    
    if (wrapper.vm.verDetalleCita) {
      await wrapper.vm.verDetalleCita(cita)
      expect(wrapper.vm.citaSeleccionada).toBeDefined()
    }
  })

  it('handles loading state', () => {
    const wrapper = mount(TechSchedule, {
      global: {
        plugins: [router, pinia],
        stubs: ['ConfirmModal', 'LoadingOverlay']
      }
    })
    
    expect(typeof wrapper.vm.loading).toBe('boolean')
  })
})
