import { mount, flushPromises } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import { createPinia, setActivePinia } from 'pinia'
import OfertasdelDia from '@/components/OfertasdelDia.vue'
import { useUserStore } from '@/stores/userStore'
import * as RoomService from '@/services/RoomService'
import * as EventTypeService from '@/services/EventTypeService'

jest.mock('@/services/RoomService')
jest.mock('@/services/EventTypeService')

const mockSalas = [
  {
    idSala: 1,
    nombre: 'Oferta A',
    descripcionCorta: 'Desc A',
    precioBase: 100,
    descuento: 20,
    imagenPrincipal: '/img1.jpg'
  },
  {
    idSala: 2,
    nombre: 'Oferta B',
    descripcionCorta: 'Desc B',
    precioBase: 200,
    descuento: 15,
    imagenPrincipal: '/img2.jpg'
  },
  {
    idSala: 3,
    nombre: 'Sala Sin Oferta',
    descripcionCorta: 'Sin descuento',
    precioBase: 150,
    descuento: 0,
    imagenPrincipal: '/img3.jpg'
  }
]

const mockTiposEvento = [
  { idTipoEvento: 1, nombre: 'Conferencia' },
  { idTipoEvento: 2, nombre: 'Fiesta' }
]

describe('OfertasdelDia.vue', () => {
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
    EventTypeService.getTiposEvento.mockResolvedValue(mockTiposEvento)
    EventTypeService.getTiposEventoPorSalaId.mockResolvedValue([mockTiposEvento[0]])
  })

  afterEach(() => {
    jest.clearAllMocks()
  })

  it('renders and loads ofertas on mount', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    expect(RoomService.getTodasLasSalas).toHaveBeenCalled()
    expect(wrapper.vm.salas.length).toBe(3)
  })

  it('filters only salas with descuento > 0', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.filteredOfertas.length).toBe(2)
    expect(wrapper.vm.filteredOfertas.every(s => s.descuento > 0)).toBe(true)
  })

  it('sorts ofertas by price ascending', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.sortOrder = 'asc'
    await wrapper.vm.$nextTick()
    
    const sorted = wrapper.vm.filteredOfertasSorted
    expect(sorted[0].idSala).toBe(1) // 80 (100 - 20%)
    expect(sorted[1].idSala).toBe(2) // 170 (200 - 15%)
  })

  it('sorts ofertas by price descending', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.sortOrder = 'desc'
    await wrapper.vm.$nextTick()
    
    const sorted = wrapper.vm.filteredOfertasSorted
    expect(sorted[0].idSala).toBe(2)
  })

  it('sorts ofertas by name A-Z', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.sortOrder = 'az'
    await wrapper.vm.$nextTick()
    
    const sorted = wrapper.vm.filteredOfertasSorted
    expect(sorted[0].nombre).toBe('Oferta A')
    expect(sorted[1].nombre).toBe('Oferta B')
  })

  it('sorts ofertas by name Z-A', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.sortOrder = 'za'
    await wrapper.vm.$nextTick()
    
    const sorted = wrapper.vm.filteredOfertasSorted
    expect(sorted[0].nombre).toBe('Oferta B')
  })

  it('filters ofertas by search query', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.searchQuery = 'Oferta A'
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.filteredOfertas.length).toBe(1)
    expect(wrapper.vm.filteredOfertas[0].nombre).toBe('Oferta A')
  })

  it('filters ofertas by event type', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.tiposEventoSeleccionados = [mockTiposEvento[0]]
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.filteredOfertas.length).toBe(2)
  })

  it('removes filter correctly', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.tiposEventoSeleccionados = [mockTiposEvento[0], mockTiposEvento[1]]
    wrapper.vm.removeFilter(mockTiposEvento[0])
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.tiposEventoSeleccionados.length).toBe(1)
  })

  it('paginates ofertas correctly', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.itemsPerPage = 1
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.paginatedOfertas.length).toBe(1)
    expect(wrapper.vm.totalPages).toBe(2)
  })

  it('changes page correctly', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.itemsPerPage = 1
    wrapper.vm.changePage(2)
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.currentPage).toBe(2)
  })

  it('does not change page if out of bounds', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.changePage(100)
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.currentPage).toBe(1)
  })

  it('calculates visible pages correctly', async () => {
    const manyOfertas = Array.from({ length: 20 }, (_, i) => ({
      idSala: i + 1,
      nombre: `Oferta ${i + 1}`,
      descripcionCorta: 'Desc',
      precioBase: 100,
      descuento: 10,
      imagenPrincipal: '/img.jpg'
    }))
    
    RoomService.getTodasLasSalas.mockResolvedValue(manyOfertas)
    
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.itemsPerPage = 5
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.visiblePages.length).toBeLessThanOrEqual(5)
  })

  it('navigates to sala on click', async () => {
    const pushSpy = jest.spyOn(router, 'push')
    
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.goToSala(1)
    
    expect(pushSpy).toHaveBeenCalledWith('/salas/1')
  })

  it('handles image load correctly', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.imageLoading[1] = true
    wrapper.vm.onImageLoad(1)
    
    expect(wrapper.vm.imageLoading[1]).toBe(false)
  })

  it('handles image error with fallback', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    const event = { target: { src: '' } }
    wrapper.vm.onImageError(event)
    
    expect(event.target.src).toContain('placeholder')
  })

  it('clears search correctly', async () => {
    const pushSpy = jest.spyOn(router, 'push')
    
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    wrapper.vm.searchQuery = 'test'
    wrapper.vm.clearSearch()
    
    expect(pushSpy).toHaveBeenCalled()
  })

  it('calculates precio con descuento correctly', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    const sala = { precioBase: 100, descuento: 20 }
    const result = wrapper.vm.precioConDescuento(sala)
    
    expect(result).toBe(80)
  })

  it('handles loading state correctly', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    expect(wrapper.vm.isLoading).toBe(true)
    await flushPromises()
    expect(wrapper.vm.isLoading).toBe(false)
  })

  it('handles error state when loading fails', async () => {
    RoomService.getTodasLasSalas.mockRejectedValue(new Error('Failed to load'))
    
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.error).toBeTruthy()
  })

  it('watches route query changes', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    await router.push({ path: '/', query: { q: 'test' } })
    await flushPromises()
    
    expect(wrapper.vm.searchQuery).toBe('test')
  })

  it('loads tipos de evento on mount', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    expect(EventTypeService.getTiposEvento).toHaveBeenCalled()
    expect(wrapper.vm.tiposEvento.length).toBe(2)
  })

  it('loads tipos de evento for each sala', async () => {
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    expect(EventTypeService.getTiposEventoPorSalaId).toHaveBeenCalledTimes(3)
  })

  it('handles error when loading tipos de evento for sala', async () => {
    EventTypeService.getTiposEventoPorSalaId.mockRejectedValue(new Error('Failed'))
    
    const wrapper = mount(OfertasdelDia, {
      global: {
        plugins: [router, pinia]
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.salas[0].tiposEvento).toEqual([])
  })
})
