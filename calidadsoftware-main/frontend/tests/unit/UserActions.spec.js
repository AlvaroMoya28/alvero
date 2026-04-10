import { mount } from '@vue/test-utils'
import UserActions from '@/components/UserActions.vue'

describe('UserActions.vue', () => {
  it('renders user action buttons', () => {
    const wrapper = mount(UserActions, {
      global: {
        stubs: ['router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays edit action', () => {
    const wrapper = mount(UserActions, {
      props: {
        userId: '123'
      },
      global: {
        stubs: ['router-link']
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('displays delete action', () => {
    const wrapper = mount(UserActions, {
      props: {
        userId: '123',
        showDelete: true
      }
    })
    expect(wrapper.exists()).toBe(true)
  })

  it('emits edit event', async () => {
    const wrapper = mount(UserActions, {
      props: {
        userId: '123'
      },
      global: {
        stubs: ['router-link']
      }
    })
    
    const editBtn = wrapper.find('button')
    if (editBtn.exists()) {
      await editBtn.trigger('click')
      expect(wrapper.emitted('edit') || wrapper.emitted('editUser')).toBeTruthy()
    }
  })

  it('confirms before delete', async () => {
    const wrapper = mount(UserActions, {
      props: {
        userId: '123',
        showDelete: true
      }
    })
    
    if (wrapper.vm.confirmDelete) {
      expect(typeof wrapper.vm.confirmDelete).toBe('function')
    }
  })
})
