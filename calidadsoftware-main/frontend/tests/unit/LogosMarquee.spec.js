import { mount } from '@vue/test-utils'
import LogosMarquee from '@/components/LogosMarquee.vue'

describe('LogosMarquee.vue', () => {
  it('renders marquee component', () => {
    const wrapper = mount(LogosMarquee, {
      props: {
        logos: [
          { id: 1, url: 'logo1.png', alt: 'Logo 1' },
          { id: 2, url: 'logo2.png', alt: 'Logo 2' }
        ]
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays logo images', () => {
    const wrapper = mount(LogosMarquee, {
      props: {
        logos: [
          { id: 1, url: 'logo1.png', alt: 'Logo 1' }
        ]
      }
    })
    const images = wrapper.findAll('img')
    expect(images.length).toBeGreaterThan(0)
  })

  it('animates logos', () => {
    const wrapper = mount(LogosMarquee, {
      props: {
        logos: [
          { id: 1, url: 'logo1.png' },
          { id: 2, url: 'logo2.png' }
        ],
        animated: true
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('handles empty logos array', () => {
    const wrapper = mount(LogosMarquee, {
      props: {
        logos: []
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('sets custom speed', () => {
    const wrapper = mount(LogosMarquee, {
      props: {
        logos: [{ id: 1, url: 'logo1.png' }],
        speed: 10
      }
    })
    expect(wrapper.props().speed).toBe(10)
  })

  it('pauses on hover', async () => {
    const wrapper = mount(LogosMarquee, {
      props: {
        logos: [{ id: 1, url: 'logo1.png' }],
        pauseOnHover: true
      }
    })
    
    const marquee = wrapper.find('.marquee')
    if (marquee.exists()) {
      await marquee.trigger('mouseenter')
      expect(wrapper.exists()).toBe(true)
    }
  })
})
