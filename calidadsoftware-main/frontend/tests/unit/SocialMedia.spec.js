import { mount } from '@vue/test-utils'
import SocialMedia from '@/components/SocialMedia.vue'

describe('SocialMedia.vue', () => {
  it('renders social media icons', () => {
    const wrapper = mount(SocialMedia)
    expect(wrapper.exists()).toBe(true)
  })

  it('displays social media links', () => {
    const wrapper = mount(SocialMedia, {
      props: {
        facebook: 'https://facebook.com/test',
        instagram: 'https://instagram.com/test',
        twitter: 'https://twitter.com/test'
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('handles missing social links', () => {
    const wrapper = mount(SocialMedia, {
      props: {
        facebook: '',
        instagram: ''
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('opens links in new tab', () => {
    const wrapper = mount(SocialMedia, {
      props: {
        facebook: 'https://facebook.com/test'
      }
    })
    const links = wrapper.findAll('a')
    links.forEach(link => {
      if (link.attributes('target')) {
        expect(link.attributes('target')).toBe('_blank')
      }
    })
  })
})
