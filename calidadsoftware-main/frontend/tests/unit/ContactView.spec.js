import { mount } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import ContactView from '@/views/ContactView.vue'

describe('ContactView.vue', () => {
  let router

  beforeEach(() => {
    router = createRouter({
      history: createMemoryHistory(),
      routes: [{ path: '/', component: { template: '<div>Home</div>' } }]
    })
  })

  it('renders contact view', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.find('.chas').exists()).toBe(true)
  })

  it('displays contact information section', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.text()).toContain('Maneras de contacto')
  })

  it('displays address information', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.text()).toContain('WWQX+567, San José, San Pedro, Costa Rica')
  })

  it('displays phone number', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.text()).toContain('(506) 8451-2502')
  })

  it('displays email address', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.text()).toContain('aaak7.eventos@gmail.com')
  })

  it('displays operating hours', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.text()).toContain('Lun - Vie')
    expect(wrapper.text()).toContain('9am - 5pm')
  })

  it('renders contact form', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.find('form').exists()).toBe(true)
  })

  it('has fullname input field', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    const input = wrapper.find('#fullname')
    expect(input.exists()).toBe(true)
    expect(input.attributes('type')).toBe('text')
    expect(input.attributes('required')).toBeDefined()
  })

  it('has email input field with icon', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    const input = wrapper.find('#email')
    expect(input.exists()).toBe(true)
    expect(input.attributes('type')).toBe('email')
    expect(input.attributes('required')).toBeDefined()
  })

  it('has phone input field', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    const input = wrapper.find('#phone')
    expect(input.exists()).toBe(true)
    expect(input.attributes('type')).toBe('number')
  })

  it('has subject input field', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    const input = wrapper.find('#subject')
    expect(input.exists()).toBe(true)
    expect(input.attributes('required')).toBeDefined()
  })

  it('has message textarea field', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    const textarea = wrapper.find('#message')
    expect(textarea.exists()).toBe(true)
    expect(textarea.element.tagName).toBe('TEXTAREA')
    expect(textarea.attributes('required')).toBeDefined()
  })

  it('has submit button', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    const button = wrapper.find('button[type="reset"]')
    expect(button.exists()).toBe(true)
    expect(button.text()).toContain('Enviar mensaje')
  })

  it('displays background logo', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.find('.background-logo').exists()).toBe(true)
  })

  it('has proper form structure with row and columns', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.findAll('.row').length).toBeGreaterThan(0)
    expect(wrapper.findAll('.col-12').length).toBeGreaterThan(0)
  })

  it('displays all required field indicators', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    const requiredIndicators = wrapper.findAll('.text-danger')
    expect(requiredIndicators.length).toBeGreaterThan(0)
  })

  it('has input groups for email and phone', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    const inputGroups = wrapper.findAll('.input-group')
    expect(inputGroups.length).toBeGreaterThan(1)
  })

  it('displays contact icons', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    const svgs = wrapper.findAll('svg')
    expect(svgs.length).toBeGreaterThan(5)
  })

  it('has proper card styling classes', () => {
    const wrapper = mount(ContactView, {
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.find('.card').exists()).toBe(true)
    expect(wrapper.find('.card-body').exists()).toBe(true)
  })
})
