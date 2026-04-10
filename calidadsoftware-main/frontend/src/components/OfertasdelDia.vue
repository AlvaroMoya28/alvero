<template>
  <div class="container-fluid">
    <div class="mt-5 sala">
      <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
        <!-- Título -->
        <div class="d-flex align-items-center gap-3 flex-wrap">
          <h2 class="mb-0 text-primary-custom">Ofertas Especiales</h2>
        </div>

        <!-- Contenedor de filtros y ordenamiento (mismo que en salas) -->
        <div class="d-flex align-items-center flex-wrap gap-3 filters-container">
          <!-- Mostrar solo si hay búsqueda -->
          <div v-if="searchQuery" class="search-indicator d-flex align-items-center">
            Resultados para: "<strong>{{ searchQuery }}</strong>"
            <button @click="clearSearch" class="btn btn-sm ms-2">
              <i class="bi bi-x"></i> Limpiar
            </button>
          </div>

          <!-- Selector de ordenamiento -->
          <div class="sort-container d-flex align-items-center">
            <label for="sortPrecio" class="me-2 mb-0">Ordenar por:</label>
            <select id="sortPrecio" v-model="sortOrder" class="form-select">
              <option value="az">Nombre: A-Z</option>
              <option value="za">Nombre: Z-A</option>
              <option value="asc">Precio: menor a mayor</option>
              <option value="desc">Precio: mayor a menor</option>
            </select>
          </div>

          <!-- Filtro de tipos de evento -->
          <div class="dropdown">
            <button class="btn btn-outline-secondary dropdown-toggle" type="button" id="dropdownEventTypes"
                    data-bs-toggle="dropdown" aria-expanded="false">
              <i class="bi bi-funnel"></i> Tipo de evento
              <span v-if="tiposEventoSeleccionados.length > 0" class="badge bg-primary ms-1">
                {{ tiposEventoSeleccionados.length }}
              </span>
            </button>
            <ul class="dropdown-menu dropdown-menu-end p-2" aria-labelledby="dropdownEventTypes" style="min-width: 250px;">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <h6 class="dropdown-header">Filtrar por tipo</h6>
                <button v-if="tiposEventoSeleccionados.length > 0"
                        @click.stop="tiposEventoSeleccionados = []"
                        class="btn btn-sm btn-link p-0">
                  Limpiar
                </button>
              </div>
              <div class="dropdown-item" v-for="tipo in tiposEvento" :key="tipo.idTipoEvento">
                <div class="form-check">
                  <input class="form-check-input" type="checkbox" :id="`check-${tipo.idTipoEvento}`"
                         :value="tipo" v-model="tiposEventoSeleccionados">
                  <label class="form-check-label w-100" :for="`check-${tipo.idTipoEvento}`">
                    {{ tipo.nombre }}
                  </label>
                </div>
              </div>
            </ul>
          </div>
        </div>
      </div>

      <!-- Chips de filtros activos (mismo que en salas) -->
      <div v-if="tiposEventoSeleccionados.length > 0" class="d-flex flex-wrap align-items-center gap-2 mb-3">
        <span class="small text-muted">Filtros aplicados:</span>
        <span v-for="tipo in tiposEventoSeleccionados" :key="tipo.idTipoEvento" class="badge bg-primary">
          {{ tipo.nombre }}
          <button @click.stop="removeFilter(tipo)" class="btn-close btn-close-white btn-close-sm ms-1"></button>
        </span>
      </div>

      <!-- Estados de carga y mensajes (mismo que en salas) -->
      <div v-if="isLoading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Cargando...</span>
        </div>
        <p class="mt-2">Cargando ofertas...</p>
      </div>
      <div v-else-if="error" class="alert alert-danger">
        {{ error }}
      </div>
      <div v-else-if="filteredOfertas.length === 0" class="alert alert-info">
        No se encontraron ofertas que coincidan con la búsqueda.
      </div>

      <!-- Grid de salas (misma estructura que en salas) -->
      <div v-else>
        <transition-group name="fade" tag="div" class="salas-grid-container">
          <div
            class="sala-card-wrapper"
            v-for="sala in paginatedOfertas"
            :key="sala.idSala"
          >
            <div class="card sala-card-fixed fade-card w-100" @click="goToSala(sala.idSala)" style="cursor:pointer;">
              <div class="ratio-container position-relative">
                <div v-if="imageLoading[sala.idSala]" class="image-spinner-overlay">
                  <div class="spinner-border text-primary" role="status" style="width: 2.5rem; height: 2.5rem;">
                    <span class="visually-hidden">Cargando imagen...</span>
                  </div>
                </div>
                <img
                  :src="`${backendUrl}${sala.imagenPrincipal}`"
                  class="card-img-top img-ratio"
                  :alt="'Imagen de ' + sala.nombre"
                  @error="onImageError"
                  @load="onImageLoad(sala.idSala)"
                  :style="imageLoading[sala.idSala] ? 'opacity:0;' : 'opacity:1; transition: opacity 0.3s;'"
                >
                <span class="offer-badge">{{ sala.descuento }}% OFF</span>
              </div>
              <div class="card-body d-flex flex-column">
                <h5 class="card-title text-primary-custom">{{ sala.nombre }}</h5>
                <p class="card-text text-secondary-custom">{{ sala.descripcionCorta }}</p>
                <div class="mt-auto d-flex w-100 align-items-end justify-content-between gap-2" style="min-height:56px;">
                  <div class="price-container-responsive price-row align-items-start justify-content-start flex-grow-1" style="padding-bottom: 6px; max-width: 60%;">
                    <div class="d-flex flex-column align-items-start w-100" style="gap:0.1rem;">
                      <small class="text-muted original-price mb-0">${{ sala.precioBase }}</small>
                      <strong class="precio-oferta text-danger h5 mb-0 fw-bold">${{ (sala.precioBase * (1 - sala.descuento / 100)).toFixed(2) }}</strong>
                    </div>
                  </div>
                  <div class="d-flex align-items-center gap-2 flex-nowrap justify-content-end">
                    <router-link :to="`/salas/${sala.idSala}`" class="btn btn-secondary" style="min-width:80px;white-space:nowrap;" @click.stop>
                      Ver Sala
                    </router-link>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </transition-group>

        <!-- Paginación (misma que en salas) -->
        <nav v-if="totalPages > 1" aria-label="Page navigation">
          <ul class="pagination justify-content-center mt-4">
            <li class="page-item" :class="{ disabled: currentPage === 1 }">
              <button class="page-link" @click="changePage(currentPage - 1)" aria-label="Previous">
                <span aria-hidden="true">&laquo;</span>
              </button>
            </li>
            <li class="page-item" v-for="page in visiblePages" :key="page" :class="{ active: page === currentPage }">
              <button class="page-link" @click="changePage(page)">
                {{ page }}
              </button>
            </li>
            <li class="page-item" :class="{ disabled: currentPage === totalPages }">
              <button class="page-link" @click="changePage(currentPage + 1)" aria-label="Next">
                <span aria-hidden="true">&raquo;</span>
              </button>
            </li>
          </ul>
        </nav>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getTodasLasSalas } from '@/services/RoomService'
import { useUserStore } from '@/stores/userStore'
import { getTiposEvento, getTiposEventoPorSalaId } from '@/services/EventTypeService'

const tiposEvento = ref([])
const tiposEventoSeleccionados = ref([])

async function cargarTiposEvento () {
  try {
    const response = await getTiposEvento()
    tiposEvento.value = response
  } catch (error) {
    console.error('Error al cargar tipos de evento:', error)
  }
}

const sortOrder = ref('az')
const router = useRouter()
const route = useRoute()
const salas = ref([])
const isLoading = ref(true)
const error = ref(null)
const currentPage = ref(1)
const itemsPerPage = ref(8)
const searchQuery = ref('')
const backendUrl = 'https://localhost:5001'
const userStore = useUserStore()
const usuario = computed(() => userStore.getUserFromToken())
const usuarioId = computed(() => usuario.value?.id || usuario.value?.sub)
const imageLoading = ref({})

// Filtrar solo salas con descuento y aplicar otros filtros
const filteredOfertas = computed(() => {
  let ofertasFiltradas = salas.value.filter(sala => sala.descuento > 0)

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    ofertasFiltradas = ofertasFiltradas.filter(sala =>
      sala.nombre.toLowerCase().includes(query) ||
      (sala.descripcionCorta && sala.descripcionCorta.toLowerCase().includes(query)) ||
      sala.precioBase.toString().includes(query))
  }

  if (tiposEventoSeleccionados.value.length > 0) {
    const selectedIds = tiposEventoSeleccionados.value.map(t => t.idTipoEvento)
    ofertasFiltradas = ofertasFiltradas.filter(sala =>
      sala.tiposEvento && sala.tiposEvento.some(tipo =>
        selectedIds.includes(tipo.idTipoEvento)
      ))
  }

  return ofertasFiltradas
})

const precioConDescuento = (sala) => sala.precioBase * (1 - (sala.descuento || 0) / 100)

const filteredOfertasSorted = computed(() => {
  const arr = [...filteredOfertas.value]
  if (sortOrder.value === 'asc') {
    arr.sort((a, b) => precioConDescuento(a) - precioConDescuento(b))
  } else if (sortOrder.value === 'desc') {
    arr.sort((a, b) => precioConDescuento(b) - precioConDescuento(a))
  } else if (sortOrder.value === 'az') {
    arr.sort((a, b) => a.nombre.localeCompare(b.nombre, 'es', { sensitivity: 'base' }))
  } else if (sortOrder.value === 'za') {
    arr.sort((a, b) => b.nombre.localeCompare(a.nombre, 'es', { sensitivity: 'base' }))
  }
  return arr
})

const paginatedOfertas = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredOfertasSorted.value.slice(start, end)
})

const totalPages = computed(() => {
  return Math.ceil(filteredOfertasSorted.value.length / itemsPerPage.value)
})

const maxVisiblePages = ref(5)
const visiblePages = computed(() => {
  const pages = []
  let startPage = Math.max(1, currentPage.value - Math.floor(maxVisiblePages.value / 2))
  const endPage = Math.min(totalPages.value, startPage + maxVisiblePages.value - 1)

  if (endPage - startPage + 1 < maxVisiblePages.value) {
    startPage = Math.max(1, endPage - maxVisiblePages.value + 1)
  }

  for (let i = startPage; i <= endPage; i++) {
    pages.push(i)
  }

  return pages
})

const changePage = (page) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

// Función para remover un filtro individual
const removeFilter = (tipo) => {
  tiposEventoSeleccionados.value = tiposEventoSeleccionados.value.filter(t => t.idTipoEvento !== tipo.idTipoEvento)
}

async function cargarSalas () {
  isLoading.value = true
  error.value = null
  try {
    const response = await getTodasLasSalas()
    salas.value = await Promise.all(response.map(async sala => {
      try {
        const tipos = await getTiposEventoPorSalaId(sala.idSala)
        return { ...sala, tiposEvento: tipos }
      } catch (error) {
        console.error(`Error al cargar tipos de evento para sala ${sala.idSala}:`, error)
        return { ...sala, tiposEvento: [] }
      }
    }))
    // Set all images as loading initially
    imageLoading.value = {}
    for (const sala of salas.value) {
      imageLoading.value[sala.idSala] = true
    }
  } catch (err) {
    console.error('Error al cargar salas:', err)
    error.value = err.message || 'No se pudieron cargar las ofertas en este momento.'
  } finally {
    isLoading.value = false
  }
}

// Observar cambios en la ruta para actualizar la búsqueda
watch(() => route.query.q, (newQuery) => {
  searchQuery.value = newQuery || ''
  currentPage.value = 1 // Resetear a la primera página al buscar
}, { immediate: true })

// Watch para inicializar el estado de carga de imagen de las salas visibles
watch(paginatedOfertas, (visibles) => {
  for (const sala of visibles) {
    if (imageLoading.value[sala.idSala] === undefined) {
      imageLoading.value[sala.idSala] = true
    }
  }
}, { immediate: true })

onMounted(async () => {
  await Promise.all([cargarSalas(), cargarTiposEvento()])
})

// Eliminado: carga de favoritos

const clearSearch = () => {
  router.push({ path: route.path, query: {} })
}

// Función para manejar errores al cargar imágenes
const onImageError = (event) => {
  event.target.src = 'https://via.placeholder.com/400x300.png?text=Imagen+no+disponible'
}

// Imagen cargada correctamente
const onImageLoad = (idSala) => {
  imageLoading.value[idSala] = false
}

// Navegar a la sala
const goToSala = (idSala) => {
  router.push(`/salas/${idSala}`)
}
</script>

<style lang="scss" scoped>
.salas-grid-container {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: 1.5rem;
  justify-content: flex-start;
  width: 100%;
}

.sala-card-wrapper {
  display: flex;
  justify-content: flex-start;
  min-width: 320px;
  max-width: 100%;
}

.sala-card-fixed {
  width: 100%;
  max-width: 400px;
  min-width: 320px;
  height: 540px;
  background: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  margin: 0 auto;

  &:hover {
    transform: translateY(-5px);
    box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
  }
}

.filters-container {
  background: #f8f9fa;
  padding: 0.75rem;
  border-radius: 0.5rem;
  gap: 1rem;

  @media (max-width: 992px) {
    width: 100%;
    margin-top: 1rem;
  }

  .search-indicator {
    background: white;
    padding: 0.5rem 0.75rem;
    border-radius: 0.375rem;
    font-size: 0.9rem;
    border: 1px solid #dee2e6;

    button {
      padding: 0.25rem 0.5rem;
      font-size: 0.8rem;
    }
  }

  .sort-container {
    label {
      white-space: nowrap;
      font-size: 0.9rem;
      color: #495057;
    }

    select {
      min-width: 180px;
      font-size: 0.9rem;
    }
  }

  .dropdown {
    .dropdown-toggle {
      display: flex;
      align-items: center;
      padding: 0.5rem 0.75rem;
      font-size: 0.9rem;

      .badge {
        font-size: 0.65rem;
      }
    }
  }
}

.dropdown {
  .dropdown-toggle {
    display: flex;
    align-items: center;
    padding: 0.375rem 0.75rem;

    .badge {
      font-size: 0.65rem;
    }
  }

  .dropdown-menu {
    max-height: 300px;
    overflow-y: auto;

    .dropdown-item {
      padding: 0.25rem 0.5rem;

      .form-check-label {
        cursor: pointer;
      }
    }
  }
}

/* Chips de filtros activos compactos */
.badge {
  display: inline-flex;
  align-items: center;
  font-weight: normal;
  padding: 0.25rem 0.5rem;

  .btn-close {
    font-size: 0.5rem;
    opacity: 0.7;

    &:hover {
      opacity: 1;
    }
  }
}

/* Ajustes responsive */
@media (max-width: 768px) {
  .d-flex.justify-content-between {
    flex-direction: column;
    align-items: flex-start !important;
    gap: 1rem;
  }

  .filters-container {
    width: 100%;
    flex-direction: column;
    align-items: stretch;
    gap: 0.5rem;

    .sort-container {
      width: 100%;

      select {
        width: 100%;
      }
    }

    .dropdown {
      margin-left: 0;
      width: 100%;

      .dropdown-toggle {
        width: 100%;
        justify-content: center;
      }
    }
  }
}

.offer-badge {
  position: absolute;
  top: 12px;
  left: 12px;
  background-color: #dc3545;
  color: white;
  padding: 0.35rem 0.7rem;
  border-radius: 6px;
  font-size: 0.95rem;
  font-weight: bold;
  z-index: 3;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}

.precio-oferta {
  color: #dc3545;
  font-size: 1.25rem;
  font-weight: bold;
}

.descuento-oferta {
  color: #dc3545;
  margin-left: 0.5rem;
  font-weight: bold;
}

.original-price {
  color: #6c757d;
  text-decoration: line-through;
  font-size: 0.9em;
}

.price-container-responsive {
  min-width: 0;
  flex: 1 1 0%;
  display: flex;
  flex-direction: row !important;
  align-items: flex-start;
  justify-content: flex-start;
  gap: 0.4rem;
}

.price-container {
  display: flex;
  flex-direction: column;
}

.fade-enter-active {
  transition: opacity 0.6s cubic-bezier(0.4, 0, 0.2, 1);
}

.fade-leave-active {
  transition: none;
}

.fade-enter-from {
  opacity: 0;
}

.fade-enter-to,
.fade-leave-from,
.fade-leave-to {
  opacity: 1;
}

.fade-card {
  opacity: 1;
}

.image-spinner-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.7);
  z-index: 2;
}

.search-indicator {
  .btn.btn-sm {
    font-family: inherit; /* Misma fuente */
    padding: 0.25rem 0.5rem;
    border-radius: 20px;
    background-color: #f8f9fa;
    border: 1px solid #dee2e6;
    color: #6c757d;
    font-size: 0.85rem;
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    transition: all 0.2s ease;

    &:hover {
      background-color: #e9ecef;
      color: #1a4456;
    }

    .bi.bi-x {
      font-size: 1rem;
    }
  }
}

.d-flex.flex-wrap.gap-2.mb-3 {
  gap: 0.75rem !important;
  align-items: center;

  .small.text-muted {
    font-family: inherit; /* Usa la misma fuente que el resto */
    font-size: 0.9rem;
    color: #6c757d;
  }

  .badge.bg-primary {
    background-color: #1a4456 !important;
    border: none;
    border-radius: 20px;
    padding: 0.5rem 0.75rem;
    font-family: inherit; /* Misma fuente */
    font-weight: 500;
    font-size: 0.85rem;
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    transition: all 0.2s ease;

    &:hover {
      transform: translateY(-1px);
      box-shadow: 0 4px 6px rgba(0,0,0,0.15);
    }

    .btn-close {
      opacity: 0.8;
      font-size: 0.65rem;
      padding: 0.15rem;
      transition: opacity 0.2s ease;

      &:hover {
        opacity: 1;
      }
    }
  }
}

.dropdown-header {
  .btn.btn-sm.btn-link {
    font-family: inherit; /* Misma fuente */
    color: #6c757d;
    text-decoration: none;
    font-size: 0.85rem;
    padding: 0.25rem 0.5rem;
    border-radius: 4px;

    &:hover {
      color: #1a4456;
      background-color: #f8f9fa;
    }
  }
}

.ratio-container {
  position: relative;
  width: 100%;
  padding-top: 75%;
  overflow: hidden;
  background-color: #f8f9fa;
}

.img-ratio {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
}

.card {
  transition: all 0.3s ease;

  &:hover {
    transform: translateY(-5px);
    box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
  }
}

.card-title {
  color: #1a4456;
}

.card-text {
  color: #1a4456;
}

.mt-5 {
  color: #1a4456;
}

.sala {
  color: #1A4456;
  padding: 0 38px;

  @media (max-width: 768px) {
    padding: 0 15px;
  }
}

.text-primary-custom {
  color: #1A4456;
}

.text-secondary-custom {
  color: #6c757d;
}

.sala-card-fixed {
  width: 100%;
  max-width: 400px;
  min-width: 320px;
  height: 540px;
  background: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  margin: 0 auto;

  &:hover {
    transform: translateY(-5px);
    box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
  }
}

.badge.bg-primary {
  display: inline-flex;
  align-items: center;
  padding: 0.4rem 0.75rem;
  font-size: 0.85rem;
  font-weight: 500;
  background-color: #1A4456 !important;

  .btn-close {
    font-size: 0.5rem;
    margin-left: 0.25rem;
  }
}

/* Transiciones y efectos */
.fade-enter-active {
  transition: opacity 0.6s cubic-bezier(0.4, 0, 0.2, 1);
}
.fade-leave-active {
  transition: none;
}
.fade-enter-from {
  opacity: 0;
}
.fade-enter-to, .fade-leave-from, .fade-leave-to {
  opacity: 1;
}

.image-spinner-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255,255,255,0.7);
  z-index: 2;
}

/* Botones y paginación */
.btn {
  --bs-btn-color: #fff;
  --bs-btn-bg: #1A4456;
  --bs-btn-border-color: #1A4456;
  --bs-btn-hover-color: #fff;
  --bs-btn-hover-bg: #122e3a;
  --bs-btn-hover-border-color: #122e3a;
  box-shadow: none;
}

.btn-heart {
  background: transparent;
  border: none;
  padding: 0;
  cursor: pointer;
  transition: transform 0.2s ease;
  outline: none;
  box-shadow: none !important;

  i {
    color: #dc3545 !important;
  }
}

.dropdown-toggle .badge.bg-primary {
  background-color: #1a4456 !important;
}

.filter-chip {
  background-color: #1a4456 !important;
}

.btn {
  --bs-btn-color: #fff;
  --bs-btn-bg: #1a4456;
  --bs-btn-border-color: #1a4456;
  --bs-btn-hover-color: #fff;
  --bs-btn-hover-bg: #122e3a;
  --bs-btn-hover-border-color: #122e3a;
  box-shadow: none;
}

.btn:hover {
  background-color: #122e3a;
  border-color: #122e3a;
  color: #fff;
}

.pagination {
  .page-item {
    &.active .page-link {
      background-color: #1A4456;
      border-color: #1A4456;
    }

    .page-link {
      color: #1A4456;
    }
  }
}

.btn-heart {
  background: transparent;
  border: none;
  padding: 0;
  margin-right: 0.5rem;
  cursor: pointer;
  transition: transform 0.2s ease;
  font-size: 1.25rem;
  outline: none;
  box-shadow: none !important;
}

.btn-heart:focus {
  outline: none !important;
  box-shadow: none !important;
}

.btn-heart i {
  color: #1A4456 !important;
}

/* Responsive */
@media (max-width: 768px) {
  .d-flex.justify-content-between {
    flex-direction: column;
    align-items: flex-start !important;
    gap: 1rem;
  }

  .filters-container {
    width: 100%;
    flex-direction: column;
    align-items: stretch;
    gap: 0.5rem;

    .sort-container {
      width: 100%;

      select {
        width: 100%;
      }
    }
  }
}

@media (max-width: 768px) {
  .sala-card-fixed {
    min-width: 100%;
    max-width: 100%;
    height: auto;
  }

  .filters-container {
    flex-direction: column;

    .sort-container {
      flex-direction: column;
      align-items: flex-start;

      select {
        width: 100%;
      }
    }
  }
}

@media (max-width: 576px) {
  .pagination {
    flex-wrap: wrap;

    .page-item {
      margin: 2px;
    }
  }

  .offer-badge {
    font-size: 0.75rem;
    padding: 0.25rem 0.5rem;
  }
}
</style>

