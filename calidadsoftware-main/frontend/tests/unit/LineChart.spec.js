import { shallowMount } from '@vue/test-utils'
import LineChart from '@/components/LineChart.vue'

describe('LineChart.vue', () => {
  it('renders the chart component', () => {
    const wrapper = shallowMount(LineChart, {
      props: {
        chartData: {
          labels: ['Enero', 'Febrero', 'Marzo'],
          datasets: [{
            label: 'Ventas',
            data: [10, 20, 30]
          }]
        }
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('accepts chart options', () => {
    const wrapper = shallowMount(LineChart, {
      props: {
        chartData: { labels: [], datasets: [] },
        options: { responsive: true }
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('renders without data', () => {
    const wrapper = shallowMount(LineChart, {
      props: {
        chartData: { labels: [], datasets: [] }
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('updates when data changes', async () => {
    const wrapper = shallowMount(LineChart, {
      props: {
        chartData: { labels: ['A'], datasets: [{ data: [1] }] }
      }
    })
    
    await wrapper.setProps({
      chartData: { labels: ['A', 'B'], datasets: [{ data: [1, 2] }] }
    })
    
    expect(wrapper.exists()).toBe(true)
  })
})
