import { shallowMount } from '@vue/test-utils'
import DoughnutChart from '@/components/DoughnutChart.vue'

describe('DoughnutChart.vue', () => {
  it('renders the chart', () => {
    const wrapper = shallowMount(DoughnutChart, {
      props: {
        chartData: {
          labels: ['Red', 'Blue'],
          datasets: [{ data: [10, 20] }]
        }
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('accepts custom options', () => {
    const wrapper = shallowMount(DoughnutChart, {
      props: {
        chartData: { labels: [], datasets: [] },
        options: { cutout: '70%' }
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('handles empty data', () => {
    const wrapper = shallowMount(DoughnutChart, {
      props: {
        chartData: { labels: [], datasets: [] }
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('updates on prop change', async () => {
    const wrapper = shallowMount(DoughnutChart, {
      props: {
        chartData: { labels: ['A'], datasets: [{ data: [10] }] }
      }
    })
    
    await wrapper.setProps({
      chartData: { labels: ['A', 'B'], datasets: [{ data: [10, 20] }] }
    })
    
    expect(wrapper.props().chartData.labels.length).toBe(2)
  })
})
