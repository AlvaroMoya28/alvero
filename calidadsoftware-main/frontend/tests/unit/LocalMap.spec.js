import { mount } from '@vue/test-utils'
import LocalMap from '@/components/LocalMap.vue'

describe('LocalMap.vue', () => {
  it('renders map component', () => {
    const wrapper = mount(LocalMap, {
      props: {
        latitude: 9.9281,
        longitude: -84.0907
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('accepts location props', () => {
    const wrapper = mount(LocalMap, {
      props: {
        latitude: 10.0,
        longitude: -85.0,
        zoom: 15
      }
    })
    expect(wrapper.props().latitude).toBe(10.0)
    expect(wrapper.props().longitude).toBe(-85.0)
  })

  it('renders with default coordinates', () => {
    const wrapper = mount(LocalMap)
    expect(wrapper.exists()).toBe(true)
  })

  it('updates when coordinates change', async () => {
    const wrapper = mount(LocalMap, {
      props: {
        latitude: 10.0,
        longitude: -85.0
      }
    })
    
    await wrapper.setProps({
      latitude: 11.0,
      longitude: -86.0
    })
    
    expect(wrapper.props().latitude).toBe(11.0)
  })
})
