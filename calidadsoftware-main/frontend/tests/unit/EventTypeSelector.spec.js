import { mount, flushPromises } from '@vue/test-utils'
import EventTypeSelector from '@/components/EventTypeSelector.vue'
import * as EventTypeService from '@/services/EventTypeService'

jest.mock('@/services/EventTypeService')

const mockTiposEvento = [
  { idTipoEvento: 1, nombre: 'Conferencia', icono: 'bi-briefcase' },
  { idTipoEvento: 2, nombre: 'Fiesta', icono: 'bi-gift' },
  { idTipoEvento: 3, nombre: 'Reunión', icono: 'bi-people' }
]

describe('EventTypeSelector.vue', () => {
  beforeEach(() => {
    EventTypeService.getTiposEvento.mockResolvedValue(mockTiposEvento)
  })

  afterEach(() => {
    jest.clearAllMocks()
  })

  it('renders component correctly', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    expect(wrapper.exists()).toBe(true)
    expect(wrapper.find('.event-type-selector').exists()).toBe(true)
  })

  it('loads tipos de evento on mount', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    expect(EventTypeService.getTiposEvento).toHaveBeenCalled()
    expect(wrapper.vm.tiposEventoDisponibles.length).toBe(3)
  })

  it('displays all tipos de evento as tags', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    const tags = wrapper.findAll('.tag')
    expect(tags.length).toBe(3)
    expect(tags[0].text()).toContain('Conferencia')
    expect(tags[1].text()).toContain('Fiesta')
    expect(tags[2].text()).toContain('Reunión')
  })

  it('marks selected tipos as selected', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [1, 2],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.isSelected(1)).toBe(true)
    expect(wrapper.vm.isSelected(2)).toBe(true)
    expect(wrapper.vm.isSelected(3)).toBe(false)
  })

  it('toggles selection when tag is clicked', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    wrapper.vm.toggleSelection(1)
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.selectedIds).toContain(1)
  })

  it('emits update:modelValue when selection changes', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    wrapper.vm.toggleSelection(1)
    await wrapper.vm.$nextTick()
    
    expect(wrapper.emitted('update:modelValue')).toBeTruthy()
    expect(wrapper.emitted('update:modelValue')[0][0]).toContain(1)
  })

  it('deselects when clicking selected tag', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [1],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    wrapper.vm.toggleSelection(1)
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.selectedIds).not.toContain(1)
  })

  it('validates when required and no selection', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: true
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.error).toBe('Debe seleccionar al menos un tipo de evento')
  })

  it('does not show error when required and has selection', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [1],
        name: 'tiposEvento',
        required: true
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.error).toBe('')
  })

  it('does not validate when not required', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    expect(wrapper.vm.error).toBe('')
  })

  it('emits validation event', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: true
      }
    })

    await flushPromises()
    
    expect(wrapper.emitted('validation')).toBeTruthy()
    expect(wrapper.emitted('validation')[0][0]).toBe(false)
  })

  it('watches modelValue prop changes', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [1],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    await wrapper.setProps({ modelValue: [1, 2] })
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.selectedIds).toContain(1)
    expect(wrapper.vm.selectedIds).toContain(2)
  })

  it('displays icons when available', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    const icons = wrapper.findAll('.tag i')
    expect(icons.length).toBe(3)
    expect(icons[0].classes()).toContain('bi-briefcase')
  })

  it('handles error when loading tipos de evento fails', async () => {
    const consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation()
    EventTypeService.getTiposEvento.mockRejectedValue(new Error('Failed to load'))
    
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [],
        name: 'tiposEvento',
        required: false
      }
    })

    await flushPromises()
    
    expect(consoleErrorSpy).toHaveBeenCalled()
    expect(wrapper.vm.tiposEventoDisponibles.length).toBe(0)
    
    consoleErrorSpy.mockRestore()
  })

  it('creates hidden input with selected ids', async () => {
    const wrapper = mount(EventTypeSelector, {
      props: {
        modelValue: [1, 2],
        name: 'customName',
        required: false
      }
    })

    await flushPromises()
    
    const hiddenInput = wrapper.find('input[type="hidden"]')
    expect(hiddenInput.exists()).toBe(true)
    expect(hiddenInput.attributes('name')).toBe('customName')
    expect(hiddenInput.attributes('value')).toBe('1,2')
  })
})
