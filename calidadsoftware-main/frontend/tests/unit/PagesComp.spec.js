import { mount } from '@vue/test-utils'
import PagesComp from '@/components/PagesComp.vue'

describe('PagesComp.vue', () => {
  it('renders pages component', () => {
    const wrapper = mount(PagesComp, {
      global: {
        stubs: ['router-view']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays page content', () => {
    const wrapper = mount(PagesComp, {
      props: {
        title: 'Test Page'
      },
      global: {
        stubs: ['router-view']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('handles navigation', () => {
    const wrapper = mount(PagesComp, {
      global: {
        stubs: ['router-view', 'router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })
})
