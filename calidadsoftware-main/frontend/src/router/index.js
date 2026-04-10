import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import RegisterView from '@/views/RegisterView.vue'
import LogInView from '@/views/LogInView.vue'
import NotFoundView from '@/views/NotFoundView.vue'
import UserProfileView from '@/views/UserProfileView.vue'
import RegisterUserView from '@/views/RegisterUserView.vue'
import ContactView from '@/views/ContactView.vue'
import ReservarView from '@/views/ReservarView.vue'
import ServiciosView from '@/views/ServiciosView.vue'
import PreciosView from '@/views/PreciosView.vue'
import { useUserStore } from '@/stores/userStore'
import DashBoardView from '@/views/DashBoardView.vue'
import UserManagement from '@/components/UserManagement.vue'
import AboutView from '@/views/AboutView.vue'
import GestionReservasView from '@/views/GestionReservasView.vue'
import AdminCreateTech from '@/components/AdminCreateTech.vue'
import AdminTechSchedulesView from '@/views/AdminTechSchedulesView.vue'
import BookAppointmentView from '@/views/BookAppointmentView.vue'
import TechAppointmentsView from '@/views/TechAppointmentsView.vue'
import TechScheduleView from '@/views/TechScheduleView.vue'
import TechDashboardView from '@/views/TechDashboardView.vue'
import TermsView from '@/views/TermsView.vue'
import RefundsView from '@/views/RefundsView.vue'
import PrivacyView from '@/views/PrivacyView.vue'
import CookiesView from '@/views/CookiesView.vue'
import WarrantyRequestView from '@/views/WarrantyRequestView.vue'
import DiagnosisView from '@/views/DiagnosisView.vue'
import HardwareRepairView from '@/views/HardwareRepairView.vue'
import DataRecoveryView from '@/views/DataRecoveryView.vue'

const router = createRouter({
  history: createWebHistory('/'),
  routes: [
    {
      path: '/',
      name: 'Inicio',
      component: HomeView
    },
    {
      path: '/reservar',
      name: 'Reservar',
      component: ReservarView
    },
    {
      path: '/servicios',
      name: 'Servicios',
      component: ServiciosView
    },
    {
      path: '/precios',
      name: 'Precios',
      component: PreciosView
    },
    {
      path: '/register',
      name: 'Register',
      component: RegisterView
    },
    {
      path: '/perfil',
      name: 'Perfil',
      component: UserProfileView
    },
    {
      path: '/logIn',
      name: 'LogIn',
      component: LogInView
    },
    {
      path: '/admin/dashboard',
      name: 'Panel',
      component: DashBoardView
    },
    {
      path: '/admin/panel/registerUser',
      name: 'RegisterUser', // It's good practice to name all routes
      component: RegisterUserView
    },
    {
      path: '/contactanos',
      name: 'contactanos',
      component: ContactView
    },
    {
      path: '/contacto',
      name: 'Contacto',
      component: ContactView
    },
    {
      path: '/terminos',
      name: 'Terminos',
      component: TermsView
    },
    {
      path: '/reembolsos',
      name: 'Reembolsos',
      component: RefundsView
    },
    {
      path: '/privacidad',
      name: 'Privacidad',
      component: PrivacyView
    },
    {
      path: '/cookies',
      name: 'Cookies',
      component: CookiesView
    },
    {
      path: '/garantia',
      name: 'Garantia',
      component: WarrantyRequestView
    },
    {
      path: '/diagnostico',
      name: 'Diagnostico',
      component: DiagnosisView
    },
    {
      path: '/reparacion-hardware',
      name: 'ReparacionHardware',
      component: HardwareRepairView
    },
    {
      path: '/recuperacion-datos',
      name: 'RecuperacionDatos',
      component: DataRecoveryView
    },
    {
      path: '/about',
      name: 'About',
      component: AboutView
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: NotFoundView
    },
    {
      path: '/admin/dashboard',
      name: 'dashboard',
      component: DashBoardView
    },
    {
      path: '/admin/create-tech',
      name: 'AdminCreateTech',
      component: AdminCreateTech,
      meta: { requiresAuth: true }
    },
    {
      path: '/admin/dashboard/users',
      name: 'UsersManagement',
      component: UserManagement
    },
    {
      path: '/admin/reservas',
      name: 'GestionReservas',
      component: GestionReservasView
    },
    {
      path: '/admin/horarios-tecnicos',
      name: 'AdminTechSchedules',
      component: AdminTechSchedulesView
    },
    {
      path: '/reservar-cita',
      name: 'BookAppointment',
      component: BookAppointmentView
    },
    {
      path: '/tecnico/mis-citas',
      name: 'TechAppointments',
      component: TechAppointmentsView,
      meta: { requiresAuth: true, role: 'TECNICO' }
    },
    {
      path: '/tecnico/mi-horario',
      name: 'TechSchedule',
      component: TechScheduleView,
      meta: { requiresAuth: true, role: 'TECNICO' }
    },
    {
      path: '/tecnico/dashboard',
      name: 'TechDashboard',
      component: TechDashboardView,
      meta: { requiresAuth: true, role: 'TECNICO' }
    }
  ],

  /**
   * Controls the scroll position when navigating between routes.
   * @param {object} to - The target Route Object being navigated to.
   * @param {object} from - The current Route Object being navigated away from.
   * @param {object|null} savedPosition - If this is a popstate navigation (triggered by the
   * browser's back/forward buttons), savedPosition will be the scroll position saved by the browser.
   * @returns {object|false|Promise} - An object with `top` and `left` properties, or `false` to prevent scrolling.
   * Can also return a Promise that resolves to the scroll position.
   */
  scrollBehavior (to, from, savedPosition) {
    // If a saved position exists (e.g., when using browser back/forward buttons),
    // return that position.
    if (savedPosition) {
      return savedPosition
    } else {
      // Otherwise, always scroll to the top for new navigations.
      return { top: 0, left: 0 }
    }
  }
})

router.beforeEach((to, from, next) => {
  const userStore = useUserStore()

  // Rutas que requieren autenticación (solo admin y técnicos)
  const adminRequired = to.path.startsWith('/admin')
  const tecnicoRequired = to.path.startsWith('/tecnico')

  // Si es ruta de admin o técnico y no está autenticado, redirigir a login
  if ((adminRequired || tecnicoRequired) && !userStore.isAuthenticated) {
    return next('/logIn')
  }

  // Si es ruta de admin pero no tiene permisos de admin, redirigir a home
  if (adminRequired && !['ADMINISTRADOR', 'SUPERUSUARIO'].includes(userStore.user?.tipoUsuario)) {
    return next('/')
  }

  // Si es ruta de técnico pero no tiene permisos de técnico, redirigir a home
  if (tecnicoRequired && userStore.user?.tipoUsuario !== 'TECNICO') {
    return next('/')
  }

  next()
})

export default router
