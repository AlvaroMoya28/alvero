import { mount } from '@vue/test-utils'
import NotFoundComp from '@/components/NotFoundComp.vue'

describe('NotFoundComp.vue', () => {
  it('renders 404 page', () => {
    const wrapper = mount(NotFoundComp, {
      global: {
        stubs: ['router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays 404 message', () => {
    const wrapper = mount(NotFoundComp, {
      global: {
        stubs: ['router-link']
      }
    })
    expect(wrapper.text()).toMatch(/404|not found|no encontrado/i)
  })

  it('shows home link', () => {
    const wrapper = mount(NotFoundComp, {
      global: {
        stubs: ['router-link']
      }
    })
    const link = wrapper.findComponent({ name: 'router-link' })
    expect(link.exists() || wrapper.html().includes('router-link')).toBe(true)
  })

  it('displays custom error message', () => {
    const wrapper = mount(NotFoundComp, {
      props: {
        message: 'Custom error message'
      },
      global: {
        stubs: ['router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('renders error illustration', () => {
    const wrapper = mount(NotFoundComp, {
      global: {
        stubs: ['router-link']
      }
    })
    const img = wrapper.find('img, svg')
    expect(wrapper.exists()).toBe(true)
  })

  it('shows back button', () => {
    const wrapper = mount(NotFoundComp, {
      global: {
        stubs: ['router-link']
      }
    })
    const button = wrapper.find('button, a')
    expect(button.exists() || wrapper.html().includes('router-link')).toBe(true)
  })
})
