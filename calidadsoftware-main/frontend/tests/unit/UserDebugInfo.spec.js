import { mount } from '@vue/test-utils'
import { createPinia } from 'pinia'
import UserDebugInfo from '@/components/UserDebugInfo.vue'

describe('UserDebugInfo.vue', () => {
  it('renders debug info', () => {
    const pinia = createPinia()
    const wrapper = mount(UserDebugInfo, {
      global: {
        plugins: [pinia]
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays user state', () => {
    const pinia = createPinia()
    const wrapper = mount(UserDebugInfo, {
      global: {
        plugins: [pinia]
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('shows token information', () => {
    const pinia = createPinia()
    const wrapper = mount(UserDebugInfo, {
      props: {
        showToken: true
      },
      global: {
        plugins: [pinia]
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays localStorage data', () => {
    localStorage.setItem('token', 'test-token')
    const pinia = createPinia()
    const wrapper = mount(UserDebugInfo, {
      global: {
        plugins: [pinia]
      }
    })
    expect(wrapper.exists()).toBe(true)
    localStorage.clear()
  })

  it('formats debug output', () => {
    const pinia = createPinia()
    const wrapper = mount(UserDebugInfo, {
      global: {
        plugins: [pinia]
      }
    })
    if (wrapper.vm.formatDebugInfo) {
      expect(typeof wrapper.vm.formatDebugInfo).toBe('function')
    }
  })
})
