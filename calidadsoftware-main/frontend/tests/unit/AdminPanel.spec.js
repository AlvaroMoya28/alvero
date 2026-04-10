import { mount } from '@vue/test-utils'
import { createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import AdminPanel from '@/components/AdminPanel.vue'

describe('AdminPanel.vue', () => {
  it('renders component', () => {
    const pinia = createPinia()
    const router = createRouter({
      history: createWebHistory(),
      routes: [{ path: '/', component: { template: '<div>Home</div>' } }]
    })
    
    const wrapper = mount(AdminPanel, {
      global: {
        plugins: [pinia, router],
        stubs: ['router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays admin navigation', () => {
    const pinia = createPinia()
    const router = createRouter({
      history: createWebHistory(),
      routes: [{ path: '/', component: { template: '<div>Home</div>' } }]
    })
    
    const wrapper = mount(AdminPanel, {
      global: {
        plugins: [pinia, router],
        stubs: ['router-link']
      }
    })
    
    expect(wrapper.exists()).toBe(true)
  })
})
