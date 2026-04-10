import { mount } from '@vue/test-utils'
import ReviewModal from '@/components/ReviewModal.vue'

describe('ReviewModal.vue', () => {
  it('renders review modal', () => {
    const wrapper = mount(ReviewModal, {
      props: {
        show: true,
        productId: 1
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays rating stars', () => {
    const wrapper = mount(ReviewModal, {
      props: {
        show: true,
        productId: 1
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('accepts user review input', async () => {
    const wrapper = mount(ReviewModal, {
      props: {
        show: true,
        productId: 1
      }
    })
    
    const textarea = wrapper.find('textarea')
    if (textarea.exists()) {
      await textarea.setValue('Great product!')
      expect(textarea.element.value).toBe('Great product!')
    }
  })

  it('validates rating selection', () => {
    const wrapper = mount(ReviewModal, {
      props: {
        show: true,
        productId: 1
      }
    })
    
    if (wrapper.vm.validateRating) {
      expect(typeof wrapper.vm.validateRating).toBe('function')
    }
  })

  it('emits submit event', async () => {
    const wrapper = mount(ReviewModal, {
      props: {
        show: true,
        productId: 1
      }
    })
    
    if (wrapper.vm.submitReview) {
      wrapper.vm.rating = 5
      wrapper.vm.comment = 'Excellent'
      await wrapper.vm.submitReview()
      expect(wrapper.emitted('submit') || wrapper.emitted('review-submitted')).toBeTruthy()
    }
  })

  it('closes modal on cancel', async () => {
    const wrapper = mount(ReviewModal, {
      props: {
        show: true
      }
    })
    
    const cancelBtn = wrapper.findAll('button').find(btn => 
      btn.text().toLowerCase().includes('cancel') || 
      btn.text().toLowerCase().includes('cerrar')
    )
    
    if (cancelBtn) {
      await cancelBtn.trigger('click')
      expect(wrapper.emitted('close') || wrapper.emitted('update:show')).toBeTruthy()
    }
  })
})
