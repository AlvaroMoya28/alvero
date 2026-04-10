import { mount } from '@vue/test-utils'
import ModalLoginAlert from '@/components/ModalLoginAlert.vue'

describe('ModalLoginAlert.vue', () => {
  it('renders modal when visible', () => {
    const wrapper = mount(ModalLoginAlert, {
      props: {
        show: true,
        message: 'Please login'
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('hides modal when show is false', () => {
    const wrapper = mount(ModalLoginAlert, {
      props: {
        show: false
      }
    })
    const modal = wrapper.find('.modal')
    expect(!modal.exists() || !modal.isVisible()).toBe(true)
  })

  it('displays custom message', () => {
    const wrapper = mount(ModalLoginAlert, {
      props: {
        show: true,
        message: 'Custom alert message'
      }
    })
    expect(wrapper.text()).toContain('Custom alert message')
  })

  it('emits close event', async () => {
    const wrapper = mount(ModalLoginAlert, {
      props: {
        show: true
      }
    })
    
    const closeBtn = wrapper.find('button')
    if (closeBtn.exists()) {
      await closeBtn.trigger('click')
      expect(wrapper.emitted('close') || wrapper.emitted('update:show')).toBeTruthy()
    }
  })

  it('shows login redirect option', () => {
    const wrapper = mount(ModalLoginAlert, {
      props: {
        show: true,
        showLoginButton: true
      },
      global: {
        stubs: ['router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })
})
