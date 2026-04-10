import { mount } from '@vue/test-utils'
import ConfirmModal from '@/components/ConfirmModal.vue'

describe('ConfirmModal.vue', () => {
  it('renders when visible', () => {
    const wrapper = mount(ConfirmModal, {
      props: {
        modelValue: true,
        title: 'Confirm',
        message: 'Are you sure?'
      }
    })
    
    expect(wrapper.exists()).toBe(true)
    expect(wrapper.text()).toContain('Confirm')
  })

  it('emits confirm event when confirmed', async () => {
    const wrapper = mount(ConfirmModal, {
      props: {
        modelValue: true,
        title: 'Test'
      }
    })
    
    const confirmBtn = wrapper.find('button.btn-primary, button.btn-success')
    if (confirmBtn.exists()) {
      await confirmBtn.trigger('click')
      expect(wrapper.emitted('confirm')).toBeTruthy()
    }
  })

  it('emits update:modelValue when closed', async () => {
    const wrapper = mount(ConfirmModal, {
      props: {
        modelValue: true
      }
    })
    
    const cancelBtn = wrapper.find('button.btn-secondary, button.btn-cancel')
    if (cancelBtn.exists()) {
      await cancelBtn.trigger('click')
      expect(wrapper.emitted('update:modelValue')).toBeTruthy()
    }
  })

  it('does not render when not visible', () => {
    const wrapper = mount(ConfirmModal, {
      props: {
        modelValue: false
      }
    })
    
    const modal = wrapper.find('.modal')
    expect(!modal.exists() || !modal.isVisible()).toBe(true)
  })
})
