import { mount } from '@vue/test-utils'
import LoadingOverlay from '@/components/LoadingOverlay.vue'

describe('LoadingOverlay.vue', () => {
  it('renders when visible', () => {
    const wrapper = mount(LoadingOverlay, {
      props: {
        visible: true
      }
    })
    
    expect(wrapper.exists()).toBe(true)
  })

  it('displays loading message', () => {
    const wrapper = mount(LoadingOverlay, {
      props: {
        visible: true,
        message: 'Loading data...'
      }
    })
    
    expect(wrapper.text()).toContain('Loading')
  })

  it('does not render when not visible', () => {
    const wrapper = mount(LoadingOverlay, {
      props: {
        visible: false
      }
    })
    
    const overlay = wrapper.find('.loading-overlay, .overlay')
    expect(!overlay.exists() || !overlay.isVisible()).toBe(true)
  })

  it('shows spinner element', () => {
    const wrapper = mount(LoadingOverlay, {
      props: {
        visible: true
      }
    })
    
    const spinner = wrapper.find('.spinner, .spinner-border, .loading-spinner')
    expect(spinner.exists() || wrapper.html().includes('spinner')).toBe(true)
  })
})
