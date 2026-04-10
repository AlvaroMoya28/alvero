import { mount, flushPromises } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import { createPinia, setActivePinia } from 'pinia'
import ProductCard from '@/components/ProductCard.vue'
import { useUserStore } from '@/stores/userStore'
import * as RoomService from '@/services/RoomService'
import * as EventTypeService from '@/services/EventTypeService'

// Mock services
jest.mock('@/services/RoomService')
jest.mock('@/services/EventTypeService')

const mockSalas = [
  {
    idSala: 1,
    nombre: 'Sala A',
    descripcionCorta: 'Descripción A',
    precioBase: 100,
    descuento: 10,
    imagenPrincipal: '/img1.jpg'
  },
  {
    idSala: 2,
    nombre: 'Sala B',
    descripcionCorta: 'Descripción B',
    precioBase: 200,
    descuento: 0,
    imagenPrincipal: '/img2.jpg'
  },
  {
    idSala: 3,
    nombre: 'Sala C',
    descripcionCorta: 'Descripción C',
    precioBase: 150,
    descuento: 20,
    imagenPrincipal: '/img3.jpg'
  }
]

const mockTiposEvento = [
  { idTipoEvento: 1, nombre: 'Conferencia' },
  { idTipoEvento: 2, nombre: 'Fiesta' }
]

describe('ProductCard.vue', () => {
  let router
  let pinia

  beforeEach(() => {
    pinia = createPinia()
    setActivePinia(pinia)
    
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/', component: { template: '<div>Home</div>' } },
        { path: '/salas/:id', component: { template: '<div>Sala</div>' } },
        { path: '/admin/agregar', component: { template: '<div>Agregar</div>' } },
        { path: '/admin/editar/:id', component: { template: '<div>Editar</div>' } }
      ]
    })

    RoomService.getTodasLasSalas.mockResolvedValue(mockSalas)
    RoomService.deleteSala.mockResolvedValue({})
    EventTypeService.getTiposEvento.mockResolvedValue(mockTiposEvento)
    EventTypeService.getTiposEventoPorSalaId.mockResolvedValue([mockTiposEvento[0]])
  })

  afterEach(() => {
    jest.clearAllMocks()
  })

  it('renders and loads salas on mount', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    expect(RoomService.getTodasLasSalas).toHaveBeenCalled()
    expect(wrapper.vm.salas.length).toBe(3)
  })

  it('filters salas when onlyOfertas is true', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: true }
    })

    await flushPromises()
    
    // Solo salas con descuento > 0
    expect(wrapper.vm.filteredSalas.length).toBe(2)
    expect(wrapper.vm.filteredSalas.every(s => s.descuento > 0)).toBe(true)
  })

  it('sorts salas by price ascending', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.sortOrder = 'asc'
    await wrapper.vm.$nextTick()
    
    const sorted = wrapper.vm.filteredSalasSorted
    expect(sorted[0].idSala).toBe(1) // Sala A con descuento: 90
    expect(sorted[2].idSala).toBe(2) // Sala B sin descuento: 200
  })

  it('sorts salas by price descending', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.sortOrder = 'desc'
    await wrapper.vm.$nextTick()
    
    const sorted = wrapper.vm.filteredSalasSorted
    expect(sorted[0].idSala).toBe(2) // Sala B: 200
  })

  it('sorts salas by name A-Z', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.sortOrder = 'az'
    await wrapper.vm.$nextTick()
    
    const sorted = wrapper.vm.filteredSalasSorted
    expect(sorted[0].nombre).toBe('Sala A')
    expect(sorted[1].nombre).toBe('Sala B')
    expect(sorted[2].nombre).toBe('Sala C')
  })

  it('sorts salas by name Z-A', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.sortOrder = 'za'
    await wrapper.vm.$nextTick()
    
    const sorted = wrapper.vm.filteredSalasSorted
    expect(sorted[0].nombre).toBe('Sala C')
    expect(sorted[2].nombre).toBe('Sala A')
  })

  it('filters salas by search query', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.searchQuery = 'Sala B'
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.filteredSalas.length).toBe(1)
    expect(wrapper.vm.filteredSalas[0].nombre).toBe('Sala B')
  })

  it('filters salas by event type', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.tiposEventoSeleccionados = [mockTiposEvento[0]]
    await wrapper.vm.$nextTick()
    
    // Todas las salas tienen el tipo de evento 1 mockeado
    expect(wrapper.vm.filteredSalas.length).toBe(3)
  })

  it('removes filter correctly', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.tiposEventoSeleccionados = [mockTiposEvento[0], mockTiposEvento[1]]
    wrapper.vm.removeFilter(mockTiposEvento[0])
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.tiposEventoSeleccionados.length).toBe(1)
    expect(wrapper.vm.tiposEventoSeleccionados[0].idTipoEvento).toBe(2)
  })

  it('paginates salas correctly', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.itemsPerPage = 2
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.paginatedSalas.length).toBe(2)
    expect(wrapper.vm.totalPages).toBe(2)
  })

  it('changes page correctly', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.itemsPerPage = 2
    wrapper.vm.changePage(2)
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.currentPage).toBe(2)
  })

  it('does not change page if out of bounds', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.changePage(100)
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.currentPage).toBe(1)
  })

  it('calculates visible pages correctly', async () => {
    // Mock 50 salas to create many pages
    const manySalas = Array.from({ length: 50 }, (_, i) => ({
      idSala: i + 1,
      nombre: `Sala ${i + 1}`,
      descripcionCorta: 'Desc',
      precioBase: 100,
      descuento: 0,
      imagenPrincipal: '/img.jpg'
    }))
    
    RoomService.getTodasLasSalas.mockResolvedValue(manySalas)
    
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.itemsPerPage = 5
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.visiblePages.length).toBeLessThanOrEqual(5)
  })

  it('navigates to sala on click', async () => {
    const pushSpy = jest.spyOn(router, 'push')
    
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.goToSala(1)
    
    expect(pushSpy).toHaveBeenCalledWith('/salas/1')
  })

  it('handles image load correctly', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.imageLoading[1] = true
    wrapper.vm.onImageLoad(1)
    
    expect(wrapper.vm.imageLoading[1]).toBe(false)
  })

  it('handles image error with fallback', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    const event = { target: { src: '' } }
    wrapper.vm.onImageError(event)
    
    expect(event.target.src).toContain('placeholder')
  })

  it('opens delete modal', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    const sala = wrapper.vm.salas[0]
    wrapper.vm.openDeleteModal(sala)
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.showDeleteModal).toBe(true)
    expect(wrapper.vm.salaToDelete).toBe(sala)
  })

  it('closes delete modal', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.showDeleteModal = true
    wrapper.vm.salaToDelete = wrapper.vm.salas[0]
    wrapper.vm.closeDeleteModal()
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.showDeleteModal).toBe(false)
    expect(wrapper.vm.salaToDelete).toBe(null)
  })

  it('confirms delete sala successfully', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    const initialLength = wrapper.vm.salas.length
    wrapper.vm.salaToDelete = wrapper.vm.salas[0]
    await wrapper.vm.confirmDeleteSala()
    await flushPromises()
    
    expect(RoomService.deleteSala).toHaveBeenCalledWith(1)
    expect(wrapper.vm.salas.length).toBe(initialLength - 1)
    expect(wrapper.vm.showDeleteModal).toBe(false)
  })

  it('handles delete error gracefully', async () => {
    RoomService.deleteSala.mockRejectedValue(new Error('Delete failed'))
    
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.salaToDelete = wrapper.vm.salas[0]
    await wrapper.vm.confirmDeleteSala()
    await flushPromises()
    
    expect(wrapper.vm.showDeleteModal).toBe(false)
  })

  it('clears search correctly', async () => {
    const pushSpy = jest.spyOn(router, 'push')
    
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    wrapper.vm.searchQuery = 'test'
    wrapper.vm.clearSearch()
    
    expect(pushSpy).toHaveBeenCalled()
  })

  it('shows admin buttons when user is admin', async () => {
    const userStore = useUserStore()
    userStore.user = { role: 'Admin' }
    
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia],
        stubs: {
          Teleport: true
        }
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    expect(wrapper.vm.isAdmin).toBe(true)
  })

  it('calculates precio con descuento correctly', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    const sala = { precioBase: 100, descuento: 10 }
    const result = wrapper.vm.precioConDescuento(sala)
    
    expect(result).toBe(90)
  })

  it('handles loading state correctly', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    expect(wrapper.vm.isLoading).toBe(true)
    await flushPromises()
    expect(wrapper.vm.isLoading).toBe(false)
  })

  it('handles error state when loading fails', async () => {
    RoomService.getTodasLasSalas.mockRejectedValue(new Error('Failed to load'))
    
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    expect(wrapper.vm.error).toBeTruthy()
  })

  it('watches route query changes', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    await router.push({ path: '/', query: { q: 'test' } })
    await flushPromises()
    
    expect(wrapper.vm.searchQuery).toBe('test')
  })

  it('loads tipos de evento on mount', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    expect(EventTypeService.getTiposEvento).toHaveBeenCalled()
    expect(wrapper.vm.tiposEvento.length).toBe(2)
  })

  it('loads tipos de evento for each sala', async () => {
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    expect(EventTypeService.getTiposEventoPorSalaId).toHaveBeenCalledTimes(3)
    expect(wrapper.vm.salas[0].tiposEvento).toBeDefined()
  })

  it('handles error when loading tipos de evento for sala', async () => {
    EventTypeService.getTiposEventoPorSalaId.mockRejectedValue(new Error('Failed'))
    
    const wrapper = mount(ProductCard, {
      global: {
        plugins: [router, pinia]
      },
      props: { onlyOfertas: false }
    })

    await flushPromises()
    
    expect(wrapper.vm.salas[0].tiposEvento).toEqual([])
  })
})
