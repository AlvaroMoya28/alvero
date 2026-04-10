import { mount } from '@vue/test-utils'
import ReservaCard from '@/components/ReservaCard.vue'

describe('ReservaCard.vue', () => {
  it('renders reservation card', () => {
    const wrapper = mount(ReservaCard, {
      props: {
        reserva: {
          idReserva: 1,
          nombreSala: 'Sala A',
          fechaInicio: '2025-11-26',
          estado: 'CONFIRMADA'
        }
      }
    })
    
    expect(wrapper.exists()).toBe(true)
  })

  it('displays reservation details', () => {
    const wrapper = mount(ReservaCard, {
      props: {
        reserva: {
          idReserva: 1,
          nombreSala: 'Sala A',
          fechaInicio: '2025-11-26',
          horaInicio: '10:00',
          horaFin: '12:00',
          estado: 'CONFIRMADA'
        }
      }
    })
    
    expect(wrapper.text()).toContain('Sala A')
  })

  it('shows estado badge', () => {
    const wrapper = mount(ReservaCard, {
      props: {
        reserva: {
          idReserva: 1,
          estado: 'PENDIENTE'
        }
      }
    })
    
    expect(wrapper.html()).toContain('PENDIENTE')
  })
})
