<template>
  <div class="container-nav-bar">
    <nav class="navbar navbar-dark">
      <div class="container-fluid flex-column p-0">
        <div class="top-bar w-100">
          <div class="d-flex justify-content-between align-items-center w-100 p-3">
            <RouterLink to="/" class="navbar-brand mb-0">
              <img src="../assets/logo2.png" alt="Logo DoctorPC" class="navbar-logo" height="48" />
              <span class="brand-title">
                <span class="brand-line-main">DoctorPC</span>
                <span class="brand-sub">Reparación de computadoras</span>
              </span>
            </RouterLink>

            <!-- Mostrar solo el botón de hamburguesa en móvil -->
            <button class="sidemenu__btn d-md-none" @click="navOpen = !navOpen" v-if="!navOpen">
              <span class="top"></span>
              <span class="mid"></span>
              <span class="bottom"></span>
            </button>

            <!-- Barra de búsqueda solo en desktop -->
            <div class="search-container position-relative d-none d-md-block">
              <input class="form-control search-input" type="search" placeholder="Buscar servicios o problemas..." aria-label="Buscar"
                v-model="busqueda" @input="onSearchInput" @keyup.enter="buscar" />
              <button class="search-icon" @click="buscar">
                <i class="bi bi-search"></i>
              </button>
              <ul v-if="showSuggestions && suggestions.length" class="search-suggestions">
                <li v-for="s in suggestions" :key="s.route" @mousedown.prevent="goToRoute(s.route)">
                  <i :class="s.icon"></i>
                  <span v-html="s.label"></span>
                </li>
              </ul>
            </div>
          </div>
        </div>

        <!-- Menú principal solo en desktop -->
        <div class="menu-bar w-100 d-none d-md-block">
          <div class="d-flex justify-content-between align-items-center w-100 px-3 py-2">
            <div class="drop-down-menu">
              <ul class="navbar-nav flex-row">
                <li class="nav-item mx-2">
                  <RouterLink active-class="active" to="/" class="nav-link">Inicio</RouterLink>
                </li>
                <li class="nav-item mx-2">
                  <RouterLink active-class="active" to="/reservar-cita" class="nav-link">Reservar Soporte</RouterLink>
                </li>
                <li class="nav-item mx-2">
                  <RouterLink active-class="active" to="/servicios" class="nav-link">Servicios</RouterLink>
                </li>
                <li class="nav-item mx-2">
                  <RouterLink active-class="active" to="/precios" class="nav-link">Precios</RouterLink>
                </li>
                <li class="nav-item mx-2">
                  <RouterLink active-class="active" to="/contacto" class="nav-link">Contacto</RouterLink>
                </li>

              </ul>
            </div>

            <ul class="navbar-nav flex-row" id="menu-left">
              <li class="nav-item mx-2" v-if="isAdmin">
                <RouterLink active-class="active" to="/admin/dashboard" class="nav-link">
                  <i class="bi bi-speedometer2"></i>
                  <span>Dashboard</span>
                </RouterLink>
              </li>

              <li class="nav-item mx-2" v-if="!isAuthenticated">
                <RouterLink active-class="active" to="/logIn" class="nav-link">
                  <i class="bi bi-person"></i>
                  <span>Iniciar Sesión</span>
                </RouterLink>
              </li>
              <li class="nav-item mx-2" v-if="isTecnico">
                <RouterLink active-class="active" to="/tecnico/mi-horario" class="nav-link">
                  <i class="bi bi-calendar-week"></i>
                  <span>Mi Horario</span>
                </RouterLink>
              </li>
              <li class="nav-item mx-2" v-if="userStore.isAuthenticated">
                <RouterLink active-class="active" to="/perfil" class="nav-link">
                  <i class="bi bi-person-circle"></i>
                  <span>Mi Perfil</span>
                </RouterLink>
              </li>
              <li class="nav-item mx-2" v-if="userStore.isAuthenticated">
                <a href="#" class="nav-link" @click.prevent="showLogoutModal = true">
                  <i class="bi bi-box-arrow-right"></i>
                  <span>Cerrar Sesión</span>
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <!-- Menú hamburguesa para móvil -->
      <transition name="fade">
        <div class="sidemenu-backdrop" v-if="navOpen" @click="navOpen = false"></div>
      </transition>

      <transition name="translateX">
        <nav v-show="navOpen" class="sidemenu__nav">
          <div class="sidemenu__wrapper">
            <div class="sidemenu__header">
              <img src="../assets/logo2.png" alt="Logo" class="sidemenu__logo" />
              <button class="sidemenu__close-btn" @click="navOpen = false">
                <i class="bi bi-x-lg"></i>
              </button>
            </div>

            <!-- Barra de búsqueda en móvil -->
            <div class="search-container-mobile px-3 py-2">
              <div class="position-relative">
                <input class="form-control search-input" type="search" placeholder="Buscar..." aria-label="Buscar"
                  v-model="busqueda" @input="onSearchInput" @keyup.enter="buscarMobile" />
                <button class="search-icon" @click="buscarMobile">
                  <i class="bi bi-search"></i>
                </button>
                <ul v-if="showSuggestions && suggestions.length" class="search-suggestions search-suggestions-mobile">
                  <li v-for="s in suggestions" :key="s.route" @mousedown.prevent="goToRoute(s.route); navOpen = false">
                    <i :class="s.icon"></i>
                    <span v-html="s.label"></span>
                  </li>
                </ul>
              </div>
            </div>

            <ul class="sidemenu__list">
              <li class="sidemenu__item">
                <RouterLink active-class="active" to="/" class="nav-link" @click.native="navOpen = false">
                  <i class="bi bi-house-door"></i>
                  <span>Inicio</span>
                </RouterLink>
              </li>
              <li class="sidemenu__item">
                <RouterLink active-class="active" to="/reservar-cita" class="nav-link" @click.native="navOpen = false">
                  <i class="bi bi-calendar-plus"></i>
                  <span>Reservar Soporte</span>
                </RouterLink>
              </li>
              <li class="sidemenu__item">
                <RouterLink active-class="active" to="/servicios" class="nav-link" @click.native="navOpen = false">
                  <i class="bi bi-gear"></i>
                  <span>Servicios</span>
                </RouterLink>
              </li>
              <li class="sidemenu__item">
                <RouterLink active-class="active" to="/precios" class="nav-link" @click.native="navOpen = false">
                  <i class="bi bi-tag"></i>
                  <span>Precios</span>
                </RouterLink>
              </li>
              <li class="sidemenu__item">
                <RouterLink active-class="active" to="/contacto" class="nav-link" @click.native="navOpen = false">
                  <i class="bi bi-envelope"></i>
                  <span>Contacto</span>
                </RouterLink>
              </li>

              <li class="sidemenu__item" v-if="isAdmin">
                <RouterLink active-class="active" to="/admin/dashboard" class="nav-link" @click.native="navOpen = false">
                  <i class="bi bi-speedometer2"></i>
                  <span>Dashboard</span>
                </RouterLink>
              </li>

              <li class="sidemenu__item" v-if="!isAuthenticated">
                <RouterLink active-class="active" to="/logIn" class="nav-link" @click.native="navOpen = false">
                  <i class="bi bi-person"></i>
                  <span>Iniciar Sesión</span>
                </RouterLink>
              </li>
              <li class="sidemenu__item" v-if="isTecnico">
                <RouterLink active-class="active" to="/tecnico/mi-horario" class="nav-link" @click.native="navOpen = false">
                  <i class="bi bi-calendar-week"></i>
                  <span>Mi Horario</span>
                </RouterLink>
              </li>
              <li class="sidemenu__item" v-if="userStore.isAuthenticated">
                <RouterLink active-class="active" to="/perfil" class="nav-link" @click.native="navOpen = false">
                  <i class="bi bi-person-circle"></i>
                  <span>Mi Perfil</span>
                </RouterLink>
              </li>
              <li class="sidemenu__item" v-if="userStore.isAuthenticated">
                <a href="#" class="nav-link" @click.prevent="showLogoutModal = true; navOpen = false">
                  <i class="bi bi-box-arrow-right"></i>
                  <span>Cerrar Sesión</span>
                </a>
              </li>
            </ul>
          </div>
        </nav>
      </transition>
    </nav>

    <!-- Modal de confirmación de cierre de sesión -->
    <div v-if="showLogoutModal" class="modal-logout-overlay">
      <div class="modal-logout-box">
        <button class="modal-logout-close" @click="showLogoutModal = false" aria-label="Cerrar modal">&times;</button>
        <h3 class="modal-logout-title">¿Cerrar sesión?</h3>
        <p class="modal-logout-message">¿Estás seguro que deseas cerrar sesión?</p>
        <div class="modal-logout-actions modal-logout-actions-separated">
          <button class="btn-logout-cancel" @click="showLogoutModal = false">Cancelar</button>
          <button class="btn-logout-confirm" @click="confirmLogout">Cerrar sesión</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/userStore'

const router = useRouter()
const userStore = useUserStore()

const busqueda = ref('')
const showSuggestions = ref(false)
const suggestions = ref([])
const navOpen = ref(false)
const showLogoutModal = ref(false)
const isAuthenticated = computed(() => userStore.isAuthenticated)

const isAdmin = computed(() => {
  const user = userStore.user
  if (user && user.tipoUsuario) {
    return user.tipoUsuario === 'SUPERUSUARIO' || user.tipoUsuario === 'ADMINISTRADOR'
  }
  return false
})

const isTecnico = computed(() => {
  const user = userStore.user
  if (user && user.tipoUsuario) {
    return user.tipoUsuario === 'TECNICO'
  }
  return false
})

const KEYWORD_MAP = [
  { keywords: ['cita', 'citas', 'reserva', 'reservas', 'soporte'], route: '/reservar-cita', label: 'Reservar cita', icon: 'bi bi-calendar-plus' },
  { keywords: ['servicio', 'servicios', 'arreglo', 'reparación'], route: '/servicios', label: 'Servicios', icon: 'bi bi-gear' },
  { keywords: ['precio', 'precios', 'costo'], route: '/precios', label: 'Precios', icon: 'bi bi-tag' },
  { keywords: ['contacto', 'contactanos', 'contactar'], route: '/contacto', label: 'Contacto', icon: 'bi bi-envelope' },
  { keywords: ['diagnostico', 'diagnóstico'], route: '/diagnostico', label: 'Diagnóstico', icon: 'bi bi-stethoscope' },
  { keywords: ['hardware', 'reparación hardware', 'reparacion hardware'], route: '/reparacion-hardware', label: 'Reparación de hardware', icon: 'bi bi-cpu' },
  { keywords: ['datos', 'recuperación', 'recuperacion'], route: '/recuperacion-datos', label: 'Recuperación de datos', icon: 'bi bi-hdd' },
  { keywords: ['perfil', 'usuario'], route: '/perfil', label: 'Mi perfil', icon: 'bi bi-person-circle' },
  { keywords: ['login', 'iniciar sesión', 'sesion'], route: '/logIn', label: 'Iniciar sesión', icon: 'bi bi-person' },
  { keywords: ['admin', 'dashboard'], route: '/admin/dashboard', label: 'Panel administrador', icon: 'bi bi-speedometer2' }
]

const normalize = (s) => s.toLowerCase().normalize('NFD').replace(/\p{Diacritic}/gu, '')

const computeSuggestions = (query) => {
  const q = normalize(query)
  const matches = []
  for (const entry of KEYWORD_MAP) {
    if (entry.keywords.some(k => normalize(k).includes(q) || q.includes(normalize(k)))) {
      matches.push({ route: entry.route, label: highlightLabel(entry.label, query), icon: entry.icon })
    }
  }
  return matches.slice(0, 5)
}

const highlightLabel = (label, query) => {
  const q = normalize(query)
  if (!q) return label
  const idx = normalize(label).indexOf(q)
  if (idx === -1) return label
  const start = label.slice(0, idx)
  const mid = label.slice(idx, idx + query.length)
  const end = label.slice(idx + query.length)
  return `${start}<strong>${mid}</strong>${end}`
}

const onSearchInput = () => {
  const q = busqueda.value.trim()
  if (!q) {
    showSuggestions.value = false
    suggestions.value = []
    return
  }
  suggestions.value = computeSuggestions(q)
  showSuggestions.value = suggestions.value.length > 0
}

const goToRoute = (routePath) => {
  showSuggestions.value = false
  suggestions.value = []
  busqueda.value = ''
  router.push(routePath)
}

const buscar = () => {
  const q = busqueda.value.trim()
  if (!q) {
    showSuggestions.value = false
    return
  }
  const s = computeSuggestions(q)
  if (s.length) {
    goToRoute(s[0].route)
  } else {
    showSuggestions.value = false
  }
}

const buscarMobile = () => {
  buscar()
  navOpen.value = false
}

const logout = async () => {
  try {
    await userStore.logout()
    router.push('/')
  } catch (error) {
    console.error('Error al cerrar sesión:', error)
  }
}

const confirmLogout = async () => {
  showLogoutModal.value = false
  await logout()
}
</script>

<style scoped lang="scss">
.search-container-mobile {
  padding: 1rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);

  .search-input {
    width: 100%;
    padding-right: 40px;
    border-radius: 20px;
    border: 1px solid rgba(255, 255, 255, 0.2);
    background-color: rgba(255, 255, 255, 0.1);
    color: white;

    &::placeholder {
      color: rgba(255, 255, 255, 0.6);
    }
  }

  .search-icon {
    position: absolute;
    right: 10px;
    top: 50%;
    transform: translateY(-50%);
    background: transparent;
    border: none;
    color: rgba(255, 255, 255, 0.6);
    cursor: pointer;
  }
}

.sidemenu__item .nav-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1.5rem;

  i {
    width: 24px;
    text-align: center;
    font-size: 1.1rem;
    letter-spacing: 12px;
  }
}

/* Ocultar menú desktop en móvil */
@media (max-width: 767.98px) {
  .menu-bar {
    display: none;
  }

  .sidemenu__nav {
    width: 85%;
    max-width: 320px;
  }
}

/* Mostrar menú desktop en pantallas grandes */
@media (min-width: 768px) {
  .sidemenu__btn, .sidemenu__nav {
    display: none !important;
  }
}

.modal-logout-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(8px);
  z-index: 2100;
  display: flex;
  align-items: center;
  justify-content: center;
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from {
    transform: translateY(30px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-logout-box {
  background: linear-gradient(135deg, #ffffff 0%, #f8f9fa 100%);
  border-radius: 20px;
  box-shadow: 0 20px 60px rgba(26, 68, 86, 0.3);
  min-width: 380px;
  max-width: 90vw;
  padding: 2.5rem 2rem 2rem 2rem;
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: stretch;
  animation: slideUp 0.3s ease;
  border: 2px solid #e9ecef;
}

.modal-logout-close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  font-size: 1.8rem;
  background: #f8f9fa;
  border: none;
  color: #495057;
  cursor: pointer;
  transition: all 0.2s;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
}

.modal-logout-close:hover {
  background: #e9ecef;
  color: #1A4456;
  transform: rotate(90deg);
}

.modal-logout-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1A4456;
  margin-bottom: 0.75rem;
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.modal-logout-title::before {
  content: '⚠️';
  font-size: 1.8rem;
}

.modal-logout-message {
  color: #495057;
  font-size: 1.05rem;
  margin-bottom: 2rem;
  text-align: center;
  line-height: 1.6;
}

.modal-logout-actions {
  display: flex;
  gap: 0.75rem;
}

.modal-logout-actions-separated {
  flex-direction: row;
  justify-content: space-between;
}

.btn-logout-cancel {
  background: #fff;
  border: 2px solid #B88B4A;
  color: #B88B4A;
  padding: 0.8rem 1.5rem;
  border-radius: 12px;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
  cursor: pointer;
  flex: 1;
  box-shadow: 0 2px 8px rgba(184, 139, 74, 0.1);
}

.btn-logout-cancel:hover {
  background: #B88B4A;
  color: #fff;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(184, 139, 74, 0.3);
}

.btn-logout-confirm {
  background: linear-gradient(135deg, #1A4456 0%, #0d2a35 100%);
  color: #fff;
  border: none;
  border-radius: 12px;
  padding: 0.8rem 1.5rem;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
  cursor: pointer;
  flex: 1;
  box-shadow: 0 4px 12px rgba(26, 68, 86, 0.3);
}

.btn-logout-confirm:hover {
  background: linear-gradient(135deg, #B88B4A 0%, #966f38 100%);
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(184, 139, 74, 0.4);
}

@media (max-width: 480px) {
  .modal-logout-box {
    min-width: 90%;
    padding: 2rem 1.5rem;
  }

  .modal-logout-title {
    font-size: 1.3rem;
  }

  .modal-logout-message {
    font-size: 1rem;
  }

  .modal-logout-actions-separated {
    flex-direction: column;
  }

  .btn-logout-cancel,
  .btn-logout-confirm {
    width: 100%;
  }
}
.container-nav-bar {
  position: sticky;
  top: 0;
  z-index: 990;
}

.navbar {
  padding: 0;
}

  .top-bar {
  background-color: var(--dp-primary);
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
  padding: 8px 32px; /* more vertical space to feel wider */
}

.menu-bar {
  background-color: var(--dp-dark);
  padding-left: 26px;
  padding-right: 26px;
}

.menu-bar > .d-flex {
  min-height: 50px;
}

.drop-down-menu {
  @media (max-width: 768px) {
    display: none;
  }
}

.search-container {
  width: 300px;
  position: relative;
}

.search-input {
  padding-right: 40px;
  border-radius: 20px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  background-color: rgba(255, 255, 255, 0.1);
  color: white;
  transition: all 0.3s ease;

  &:focus {
    background-color: rgba(255, 255, 255, 0.2);
    box-shadow: 0 0 0 0.25rem rgba(26, 68, 86, 0.25);
    border-color: #1A4456;
    outline: none;
  }

  &::placeholder {
    color: rgba(255, 255, 255, 0.6);
  }
}

.search-icon {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  background: transparent;
  border: none;
  color: rgba(255, 255, 255, 0.6);
  cursor: pointer;
  transition: color 0.2s;

  &:hover {
    color: #1A4456;
  }
}

.search-suggestions {
  position: absolute;
  top: calc(100% + 6px);
  left: 0;
  right: 0;
  background: var(--dp-darkest);
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 12px;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.25);
  list-style: none;
  margin: 0;
  padding: 6px;
  z-index: 1200;

  li {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 8px 10px;
    color: rgba(255, 255, 255, 0.9);
    border-radius: 8px;
    cursor: pointer;

    i { font-size: 1.1rem; width: 22px; text-align: center; }

    &:hover {
      background: rgba(255, 255, 255, 0.08);
    }
  }
}

.search-suggestions-mobile {
  position: relative;
  top: 6px;
}

.navbar-nav .nav-link {
  color: rgba(255, 255, 255, 1);
  font-size: 1rem;
  padding: 0.4rem 0.8rem;
  display: flex;
  align-items: center;
  gap: 0.3rem;
  transition: color 0.2s, background-color 0.2s;
  text-decoration: none;
  border-radius: 20px;

  &:hover:not(.active) {
    color: rgba(255, 255, 255, 0.8) !important;
    background-color: rgba(255, 255, 255, 0.1);
  }

  &.active {
    background-color: #1A4456;
    color: white !important;
    font-weight: 500;

    &:hover {
        background-color: #1A4456;
        color: white !important;
    }
  }
}

.navbar-brand {
  padding: 5px 0;
  display: flex;
  align-items: center;
  text-decoration: none;

  .navbar-logo {
    height: 42px; /* larger logo desktop */
    width: auto;
    transition: opacity 0.3s ease;

    &:hover {
      opacity: 0.9;
    }
  }

  @media (max-width: 768px) {
    .navbar-logo {
      height: 36px; /* slightly larger on mobile too */
    }
  }

  .brand-title {
    margin-left: 10px;
    display: inline-flex;
    flex-direction: column; /* stack lines */
    line-height: 1.1;
  }

  .brand-line-main {
    font-size: 1.35rem;
    font-weight: 700;
    color: #fff;
  }

  .brand-sub {
    font-size: 0.95rem;
    font-weight: 600;
    color: rgba(255, 255, 255, 0.9);
  }
}

#sidemenu-container {
  min-height: 50px;
  display: flex;
  align-items: center;
  position: relative;
}

.sidemenu__btn {
  display: none;
  width: 50px;
  height: 50px;
  background: transparent;
  border: none;
  position: relative;
  z-index: 1000;
  appearance: none;
  cursor: pointer;
  outline: none;

  @media (max-width: 768px) {
    display: block;
  }

  span {
    display: block;
    width: 20px;
    height: 2px;
    margin: auto;
    background: white;
    position: absolute;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    transition: all 0.4s ease;

    &.top {
      transform: translateY(-6px);
    }
    &.mid {
      transform: translateY(0px);
    }
    &.bottom {
      transform: translateY(6px);
    }
  }
}

.sidemenu-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 998;
}

.sidemenu__nav {
  width: 280px;
  height: 100vh;
  background: var(--dp-dark);
  position: fixed;
  top: 0;
  left: 0;
  z-index: 999;
  box-shadow: 3px 0px 15px rgba(0, 0, 0, 0.2);
  display: flex;
  flex-direction: column;

  @media (min-width: 769px) {
    display: none !important;
  }
}

.sidemenu__wrapper {
  display: flex;
  flex-direction: column;
  height: 100%;
  overflow-y: auto;
}

.sidemenu__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
  background-color: var(--dp-darkest);

  .sidemenu__logo {
    height: 28px;
    width: auto;
  }
}

.sidemenu__close-btn {
  background: transparent;
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
  padding: 0.5rem;
  line-height: 1;
  transition: color 0.2s ease;

  &:hover {
    color: rgba(255, 255, 255, 0.7);
  }
}

.sidemenu__list {
  list-style: none;
  padding: 1rem 0;
  margin: 0;
  flex-grow: 1;
}

.sidemenu__item {
  a.nav-link {
    text-decoration: none;
    line-height: 1.6em;
    font-size: 1.1em;
    padding: 0.75rem 1.5rem;
    display: block;
    color: rgba(255, 255, 255, 0.9);
    transition: background-color 0.3s ease, color 0.3s ease;
    border-radius: 0;
    margin: 0;

    &:hover:not(.active) {
      background: rgba(255, 255, 255, 0.08);
      color: white;
    }
     &.active {
      background-color: var(--dp-accent);
      color: white !important;
      font-weight: 600;
      border-radius: 4px;
      margin: 0.25rem 1rem;
      padding: 0.75rem 1rem;

      &:hover {
        background-color: color-mix(in srgb, var(--dp-accent) 80%, black 20%);
        color: white !important;
      }
     }
  }
}

.translateX-enter-active,
.translateX-leave-active {
  transition: transform 0.35s cubic-bezier(0.5, 0, 0.5, 1);
}
/* Asegurar que el panel se deslice por encima del botón hamburguesa al cerrarse */
.sidemenu__nav.translateX-leave-active {
  z-index: 1001; /* Ligeramente superior al z-index del botón hamburguesa (1000) */
}

.translateX-enter-from,
.translateX-leave-to {
  transform: translateX(-100%);
}

.translateX-enter-to,
.translateX-leave-from {
  transform: translateX(0);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.35s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
.fade-enter-to,
.fade-leave-from {
  opacity: 1;
}

.modal-backdrop-custom {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.4);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-custom {
  background: #fff;
  border-radius: 10px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.18);
  min-width: 320px;
  max-width: 90vw;
  padding: 1.5rem 1.5rem 1rem 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: stretch;
}
.modal-header-custom {
  font-weight: 600;
  font-size: 1.2rem;
  margin-bottom: 0.5rem;
}
.modal-body-custom {
  margin-bottom: 1rem;
}

.modal-footer-custom {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;

  .btn {
    flex: 1;
    padding: 0.75rem;
    border-radius: 4px;
    font-weight: 500;
    transition: background-color 0.3s ease;
  }
  .btn-secondary {
    background-color: rgba(255, 255, 255, 0.1);
    color: #1A4456;
    border: 1px solid #1A4456;
    &:hover {
      background-color: #f8f9fa;
      color: #122e3a;
      border-color: #122e3a;
    }
  }
  .btn-danger {
    background-color: #dc3545;
    color: white;
    &:hover {
      background-color: #c82333;
    }
  }
}

@import url("https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.0/font/bootstrap-icons.css");
</style>
