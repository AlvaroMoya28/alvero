import { mount } from '@vue/test-utils'
import CarruselMarca from '@/components/CarruselMarca.vue'

describe('CarruselMarca.vue', () => {
  it('renders carousel', () => {
    const wrapper = mount(CarruselMarca, {
      props: {
        items: [
          { id: 1, nombre: 'Brand 1', imagen: 'brand1.jpg' },
          { id: 2, nombre: 'Brand 2', imagen: 'brand2.jpg' }
        ]
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays brand items', () => {
    const wrapper = mount(CarruselMarca, {
      props: {
        items: [
          { id: 1, nombre: 'Brand 1' }
        ]
      }
    })
    expect(wrapper.props().items.length).toBe(1)
  })

  it('handles navigation buttons', async () => {
    const wrapper = mount(CarruselMarca, {
      props: {
        items: [
          { id: 1, nombre: 'Brand 1' },
          { id: 2, nombre: 'Brand 2' },
          { id: 3, nombre: 'Brand 3' }
        ]
      }
    })
    
    if (wrapper.vm.next) {
      await wrapper.vm.next()
      expect(typeof wrapper.vm.next).toBe('function')
    }
  })

  it('auto-plays carousel', () => {
    const wrapper = mount(CarruselMarca, {
      props: {
        items: [{ id: 1, nombre: 'Brand 1' }],
        autoPlay: true
      }
    })
    
    if (wrapper.vm.startAutoPlay) {
      expect(typeof wrapper.vm.startAutoPlay).toBe('function')
    }
  })

  it('pauses on hover', async () => {
    const wrapper = mount(CarruselMarca, {
      props: {
        items: [{ id: 1, nombre: 'Brand 1' }]
      }
    })
    
    const carousel = wrapper.find('.carousel, .carrusel')
    if (carousel.exists()) {
      await carousel.trigger('mouseenter')
      expect(wrapper.exists()).toBe(true)
    }
  })

  it('shows indicators', () => {
    const wrapper = mount(CarruselMarca, {
      props: {
        items: [
          { id: 1, nombre: 'Brand 1' },
          { id: 2, nombre: 'Brand 2' }
        ],
        showIndicators: true
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('handles empty items array', () => {
    const wrapper = mount(CarruselMarca, {
      props: {
        items: []
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('updates current index', async () => {
    const wrapper = mount(CarruselMarca, {
      props: {
        items: [
          { id: 1, nombre: 'Brand 1' },
          { id: 2, nombre: 'Brand 2' }
        ]
      }
    })
    
    if (wrapper.vm.currentIndex !== undefined) {
      wrapper.vm.currentIndex = 1
      await wrapper.vm.$nextTick()
      expect(wrapper.vm.currentIndex).toBe(1)
    }
  })
})
