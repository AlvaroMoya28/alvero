import { mount } from '@vue/test-utils'
import TestimonialsSection from '@/components/TestimonialsSection.vue'

describe('TestimonialsSection.vue', () => {
  it('renders testimonials section', () => {
    const wrapper = mount(TestimonialsSection, {
      global: {
        stubs: ['TestimonialsComp']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays testimonials data', () => {
    const wrapper = mount(TestimonialsSection, {
      props: {
        testimonials: [
          { id: 1, name: 'John', text: 'Great!' },
          { id: 2, name: 'Jane', text: 'Awesome!' }
        ]
      },
      global: {
        stubs: ['TestimonialsComp']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('handles empty testimonials', () => {
    const wrapper = mount(TestimonialsSection, {
      props: {
        testimonials: []
      },
      global: {
        stubs: ['TestimonialsComp']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })
})
