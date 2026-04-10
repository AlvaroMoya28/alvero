import { mount, flushPromises } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import { createPinia, setActivePinia } from 'pinia'
import ProductList from '@/components/ProductList.vue'
import * as RoomService from '@/services/RoomService'

jest.mock('@/services/RoomService')

const mockSalas = [
  {
    idSala: 1,
    nombre: 'Sala A',
    descripcionCorta: 'Desc A',
    precioBase: 100,
    descuento: 0,
    imagenPrincipal: '/img1.jpg'
  },
  {
    idSala: 2,
    nombre: 'Sala B',
    descripcionCorta: 'Desc B',
    precioBase: 200,
    descuento: 10,
    imagenPrincipal: '/img2.jpg'
  },
  {
    idSala: 3,
    nombre: 'Sala C',
    descripcionCorta: 'Desc C',
    precioBase: 150,
    descuento: 0,
    imagenPrincipal: '/img3.jpg'
  }
]

describe('ProductList.vue', () => {
  let router
  let pinia

  beforeEach(() => {
    pinia = createPinia()
    setActivePinia(pinia)
    
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/', component: { template: '<div>Home</div>' } },
        { path: '/salas/:id', component: { template: '<div>Sala</div>' } }
      ]
    })

    RoomService.getTodasLasSalas.mockResolvedValue(mockSalas)
  })

  afterEach(() => {
    jest.clearAllMocks()
  })

  it('renders and loads salas on mount', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test Salas'
      }
    })

    await flushPromises()
    
    expect(RoomService.getTodasLasSalas).toHaveBeenCalled()
    expect(wrapper.vm.salas.length).toBe(3)
  })

  it('filters salas without discount when onlyOfertas is false', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.salasRecomendadas.length).toBe(2)
    expect(wrapper.vm.salasRecomendadas.every(s => s.descuento === 0)).toBe(true)
  })

  it('filters salas with discount when onlyOfertas is true', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: true,
        titulo: 'Ofertas'
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.salasRecomendadas.length).toBe(1)
    expect(wrapper.vm.salasRecomendadas.every(s => s.descuento > 0)).toBe(true)
  })

  it('displays custom titulo', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Custom Title'
      }
    })

    await flushPromises()
    
    expect(wrapper.find('h2').text()).toBe('Custom Title')
  })

  it('navigates to sala on click', async () => {
    const pushSpy = jest.spyOn(router, 'push')
    
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      }
    })

    await flushPromises()
    
    wrapper.vm.goToSala(1)
    
    expect(pushSpy).toHaveBeenCalledWith('/salas/1')
  })

  it('handles image load correctly', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      }
    })

    await flushPromises()
    
    wrapper.vm.handleImageLoad(1)
    
    expect(wrapper.vm.imagenCargada[1]).toBe(true)
  })

  it('handles image error with fallback', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      }
    })

    await flushPromises()
    
    const event = { target: { src: '' } }
    wrapper.vm.onImageError(event)
    
    expect(event.target.src).toContain('placeholder')
  })

  it('handles loading state correctly', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      }
    })

    expect(wrapper.vm.isLoading).toBe(true)
    await flushPromises()
    expect(wrapper.vm.isLoading).toBe(false)
  })

  it('handles error state when loading fails', async () => {
    RoomService.getTodasLasSalas.mockRejectedValue(new Error('Failed to load'))
    
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.error).toBeTruthy()
  })

  it('shows no salas message when empty', async () => {
    RoomService.getTodasLasSalas.mockResolvedValue([])
    
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.salasRecomendadas.length).toBe(0)
  })

  it('calculates discounted price correctly', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: true,
        titulo: 'Test'
      }
    })

    await flushPromises()
    
    const salaConDescuento = wrapper.vm.salasRecomendadas[0]
    expect(salaConDescuento.descuento).toBe(10)
    const precioFinal = salaConDescuento.precioBase * (1 - salaConDescuento.descuento / 100)
    expect(precioFinal).toBe(180)
  })

  it('scrolls carousel left', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      }
    })

    await flushPromises()
    
    // Mock the refs and getComputedStyle
    const mockScrollBy = jest.fn()
    wrapper.vm.wrapper = {
      scrollBy: mockScrollBy,
      offsetWidth: 1000,
      scrollWidth: 2000,
      clientWidth: 1000
    }
    
    const mockElement = document.createElement('div')
    mockElement.offsetWidth = 300
    
    wrapper.vm.track = {
      children: [mockElement],
      style: { gap: '20px' }
    }
    
    global.getComputedStyle = jest.fn().mockReturnValue({ gap: '20px' })
    
    wrapper.vm.scroll(-1)
    
    expect(mockScrollBy).toHaveBeenCalled()
  })

  it('scrolls carousel right', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      }
    })

    await flushPromises()
    
    // Mock the refs and getComputedStyle
    const mockScrollBy = jest.fn()
    wrapper.vm.wrapper = {
      scrollBy: mockScrollBy,
      offsetWidth: 1000,
      scrollWidth: 2000,
      clientWidth: 1000
    }
    
    const mockElement = document.createElement('div')
    mockElement.offsetWidth = 300
    
    wrapper.vm.track = {
      children: [mockElement],
      style: { gap: '20px' }
    }
    
    global.getComputedStyle = jest.fn().mockReturnValue({ gap: '20px' })
    
    wrapper.vm.scroll(1)
    
    expect(mockScrollBy).toHaveBeenCalled()
  })

  it('checks if arrows should be shown', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      },
      attachTo: document.body
    })

    await flushPromises()
    
    wrapper.vm.wrapper = {
      scrollWidth: 2000,
      clientWidth: 1000
    }
    wrapper.vm.track = {}
    
    wrapper.vm.checkShowArrows()
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.showArrows).toBe(true)
    wrapper.unmount()
  })

  it('does not show arrows when content fits', async () => {
    const wrapper = mount(ProductList, {
      global: {
        plugins: [router, pinia]
      },
      props: {
        onlyOfertas: false,
        titulo: 'Test'
      },
      attachTo: document.body
    })

    await flushPromises()
    
    wrapper.vm.wrapper = {
      scrollWidth: 1000,
      clientWidth: 1000
    }
    wrapper.vm.track = {}
    
    wrapper.vm.checkShowArrows()
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.showArrows).toBe(false)
    wrapper.unmount()
  })
})
