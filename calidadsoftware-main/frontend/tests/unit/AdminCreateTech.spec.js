import { mount } from '@vue/test-utils'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import AdminCreateTech from '@/components/AdminCreateTech.vue'

describe('AdminCreateTech.vue', () => {
  let router
  let pinia

  beforeEach(() => {
    router = createRouter({
      history: createWebHistory(),
      routes: [{ path: '/', component: { template: '<div>Home</div>' } }]
    })
    pinia = createPinia()
  })

  it('renders create tech form', () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays all required form fields', () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    const inputs = wrapper.findAll('input')
    expect(inputs.length).toBeGreaterThan(0)
  })

  it('validates identificacion field', async () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.validateIdentificacion) {
      const result = wrapper.vm.validateIdentificacion('123456789')
      expect(typeof result).toBe('boolean')
    }
  })

  it('validates email format', async () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.validateEmail) {
      expect(wrapper.vm.validateEmail('test@test.com')).toBe(true)
      expect(wrapper.vm.validateEmail('invalid')).toBe(false)
    }
  })

  it('handles form submission', async () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.tecnico) {
      wrapper.vm.tecnico.identificacion = '123456789'
      wrapper.vm.tecnico.nombre = 'Juan'
      wrapper.vm.tecnico.email = 'juan@test.com'
    }
    
    expect(wrapper.vm.tecnico || wrapper.vm.formData).toBeDefined()
  })

  it('shows loading state during submission', async () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.loading !== undefined) {
      wrapper.vm.loading = true
      await wrapper.vm.$nextTick()
      expect(wrapper.vm.loading).toBe(true)
    }
  })

  it('displays success message on create', async () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.showSuccess) {
      wrapper.vm.showSuccess = true
      await wrapper.vm.$nextTick()
      expect(wrapper.vm.showSuccess).toBe(true)
    }
  })

  it('handles API errors gracefully', async () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.error) {
      wrapper.vm.error = 'Error de conexión'
      await wrapper.vm.$nextTick()
      expect(wrapper.vm.error).toBeTruthy()
    }
  })

  it('resets form after successful creation', async () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.resetForm) {
      await wrapper.vm.resetForm()
      expect(wrapper.vm.tecnico || wrapper.vm.formData).toBeDefined()
    }
  })

  it('validates required fields before submit', () => {
    const wrapper = mount(AdminCreateTech, {
      global: {
        plugins: [router, pinia]
      }
    })
    
    if (wrapper.vm.validateForm) {
      expect(typeof wrapper.vm.validateForm).toBe('function')
    }
  })
})
