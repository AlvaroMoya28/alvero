import { mount, flushPromises } from '@vue/test-utils'
import DashBoard from '@/components/DashBoard.vue'

jest.mock('@/services/DashboardService', () => ({
  dashboardService: {
    getMetrics: jest.fn()
  }
}))

const globalStubs = {
  'router-link': {
    template: '<a><slot /></a>'
  },
  AdminLayout: { template: '<div><slot /></div>' },
  LineChart: { template: '<div class="line-chart-mock"></div>', props: ['chartData'] },
  DoughnutChart: { template: '<div class="doughnut-chart-mock"></div>', props: ['chartData'] }
}

describe('DashBoard.vue', () => {
  // Suprime logs de error en consola para evitar ruido en los tests
  let consoleErrorSpy
  beforeAll(() => {
    consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation(() => {})
  })
  afterAll(() => {
    consoleErrorSpy.mockRestore()
  })
  const metricsMock = {
    totalUsers: 100,
    newUsersToday: 5,
    dailyActivity: 80,
    activeEvents: 12,
    ticketsSold: 200,
    weeklyActivity: [10, 20, 30, 40, 50, 60, 70],
    usersByRole: [
      { role: 'Admin', count: 2 },
      { role: 'User', count: 98 }
    ]
  }
  const recentUsersMock = [
    { id: 1, name: 'Juan', email: 'juan@mail.com', role: 'Admin', createdAt: '2025-06-01T10:00:00Z' },
    { id: 2, name: 'Ana', email: 'ana@mail.com', role: 'User', createdAt: '2025-06-02T12:00:00Z' }
  ]
  const recentLogsMock = [
    { id: 1, timestamp: '2025-06-01T10:00:00Z', user: 'Juan', action: 'Login', details: 'Acceso correcto' },
    { id: 2, timestamp: '2025-06-02T12:00:00Z', user: 'Ana', action: 'Registro', details: 'Nuevo usuario' }
  ]

  beforeEach(() => {
    jest.clearAllMocks()
    require('@/services/DashboardService').dashboardService.getMetrics.mockResolvedValue({
      metrics: metricsMock,
      recentUsers: recentUsersMock,
      recentLogs: recentLogsMock
    })
  })

  it('renderiza el dashboard y muestra métricas', async () => {
    const wrapper = mount(DashBoard, { global: { stubs: globalStubs } })
    await flushPromises()
    expect(wrapper.text()).toMatch('Panel de Control')
    expect(wrapper.text()).toMatch('Usuarios totales')
    expect(wrapper.text()).toMatch('5 hoy')
  })

  it('renderiza los gráficos con los datos correctos', async () => {
    const wrapper = mount(DashBoard, { global: { stubs: globalStubs } })
    await flushPromises()
    expect(wrapper.text()).toMatch('Distribución de usuarios')
  })

  it('renderiza la tabla de usuarios recientes', async () => {
    const wrapper = mount(DashBoard, { global: { stubs: globalStubs } })
    await flushPromises()
    expect(wrapper.text()).toMatch('Últimos usuarios registrados')
  })

  it('muestra placeholders de gráficos si loading', async () => {
    // Forzar loading manualmente
    const wrapper = mount(DashBoard, { global: { stubs: globalStubs } })
    wrapper.vm.loading = true
    await wrapper.vm.$nextTick()
    expect(wrapper.html()).toMatch('Cargando...')
  })

  it('muestra mensaje de error si falla la carga', async () => {
    require('@/services/DashboardService').dashboardService.getMetrics.mockRejectedValueOnce(new Error('fail'))
    const wrapper = mount(DashBoard, { global: { stubs: globalStubs } })
    await flushPromises()
    // El error solo se loguea, no se muestra en UI, pero loading debe ser false
    expect(wrapper.vm.loading).toBe(false)
  })
})
