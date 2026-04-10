<template>
  <div class="container-fluid">
    <div class="mt-5 sala">
      <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
  <!-- Título y botón de admin -->
  <div class="d-flex align-items-center gap-3 flex-wrap">
    <h2 class="mb-0 text-primary-custom">{{ props.onlyOfertas ? 'Nuestras Ofertas' : 'Todas Nuestras Salas' }}</h2>
    <div v-if="isAdmin">
      <router-link
        to="/admin/agregar"
        class="btn add-btn d-flex align-items-center"
        style="background-color: #1A4456; color: #fff; font-weight: 600; border-radius: 8px; padding: 10px 18px; font-size: 15px; width: fit-content;"
      >
        <i class="bi bi-plus-lg me-2"></i> Agregar Sala
      </router-link>
    </div>
  </div>

  <!-- Contenedor de filtros y ordenamiento -->
  <div class="d-flex align-items-center flex-wrap gap-3 filters-container">
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

      <!-- Chips de filtros activos - versión compacta -->
      <div v-if="tiposEventoSeleccionados.length > 0" class="d-flex flex-wrap align-items-center gap-2 mb-3">
        <span class="small text-muted">Filtros aplicados:</span>
        <span v-for="tipo in tiposEventoSeleccionados" :key="tipo.idTipoEvento" class="badge bg-primary">
          {{ tipo.nombre }}
          <button @click.stop="removeFilter(tipo)" class="btn-close btn-close-white btn-close-sm ms-1"></button>
        </span>
      </div>
      <div class="d-flex align-items-center gap-2 flex-wrap mb-3 justify-content-between">
        <div class="d-flex align-items-center flex-wrap gap-2">
          <div v-if="searchQuery" class="search-indicator me-3 mb-0">
            Resultados para: "<strong>{{ searchQuery }}</strong>"
            <button @click="clearSearch" class="btn">
              <i class="bi bi-x"></i> Limpiar
            </button>
          </div>
        </div>
      </div>

      <div v-if="isLoading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Cargando...</span>
        </div>
        <p class="mt-2">Cargando salas...</p>
      </div>
      <div v-else-if="error" class="alert alert-danger">
        {{ error }}
      </div>
      <div v-else-if="filteredSalas.length === 0" class="alert alert-info">
        No se encontraron salas que coincidan con la búsqueda.
      </div>

      <div v-else>
        <transition-group name="fade" tag="div" class="salas-grid-container">
          <div
            class="sala-card-wrapper"
            v-for="sala in paginatedSalas"
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
                <span v-if="sala.descuento > 0" class="offer-badge">{{ sala.descuento }}% OFF</span>
              </div>
              <div class="card-body d-flex flex-column">
                <h5 class="card-title text-primary-custom">{{ sala.nombre }}</h5>
                <p class="card-text text-secondary-custom">{{ sala.descripcionCorta }}</p>
                <div class="mt-auto d-flex w-100 align-items-end justify-content-between gap-2" style="min-height:56px;">
                  <div class="price-container-responsive price-row align-items-start justify-content-start flex-grow-1" style="padding-bottom: 6px; max-width: 60%;">
                    <template v-if="sala.descuento > 0">
                      <div class="d-flex flex-column align-items-start w-100" style="gap:0.1rem;">
                        <small class="text-muted original-price mb-0">${{ sala.precioBase }}</small>
                        <strong class="precio-oferta text-danger h5 mb-0 fw-bold">${{ (sala.precioBase * (1 - sala.descuento / 100)).toFixed(2) }}</strong>
                      </div>
                    </template>
                    <template v-else>
                      <div class="d-flex flex-column align-items-start w-100">
                        <strong class="precio h5 mb-0 text-primary-custom fw-bold">${{ sala.precioBase }}</strong>
                      </div>
                    </template>
                  </div>
                  <!-- Cambia este div para asegurar alineación horizontal y buen wrap -->
                  <div class="d-flex align-items-center gap-2 flex-nowrap justify-content-end admin-btn-group">
                    <router-link :to="`/salas/${sala.idSala}`" class="btn btn-secondary align-self-center admin-btn" style="min-width:80px;white-space:nowrap;" @click.stop>
                      Ver Sala
                    </router-link>
                    <router-link
                      :to="`/admin/editar/${sala.idSala}`"
                      class="btn btn-secondary admin-btn"
                      style="min-width:80px;white-space:nowrap;"
                      @click.stop
                      v-if="isAdmin"
                    >
                      Editar
                    </router-link>
                    <button
                      v-if="isAdmin"
                      class="btn btn-eliminar-sala admin-btn"
                      style="min-width:80px;white-space:nowrap;"
                      @click.stop="openDeleteModal(sala)"
                    >
                      Eliminar
                    </button>
                    <!-- Modal de confirmación de eliminación -->
                    <Teleport to="body">
                      <div v-if="showDeleteModal" class="modal-overlay" @click.self="closeDeleteModal">
                        <div class="modal-box modal-error" style="max-width: 450px; border-top-width: 6px;">
                          <button class="close-button" @click="closeDeleteModal" aria-label="Cerrar">
                            <i class="bi bi-x-lg"></i>
                          </button>
                          <i class="modal-icon bi bi-exclamation-triangle-fill" style="color: #dc3545;"></i>
                          <h2 style="margin-top:0;margin-bottom:1rem;font-size:1.5rem;color:#1A4456;">Eliminar Sala</h2>
                          <p style="margin-bottom:1.8rem;font-size:1rem;color:#495057;line-height:1.6;">¿Estás seguro de que quieres eliminar la sala <strong>"{{ salaToDelete?.nombre }}"</strong>? Esta acción es permanente.</p>
                          <div class="modal-actions" style="display:flex;justify-content:center;gap:1rem;">
                            <button @click="closeDeleteModal" class="btn-cancel" style="background-color:#6c757d;color:white;padding:0.75rem 1.5rem;border-radius:8px;font-weight:500;">Cancelar</button>
                            <button @click="confirmDeleteSala" class="btn-confirm-danger" :disabled="deleteLoading" style="background-color:#dc3545;color:white;padding:0.75rem 1.5rem;border-radius:8px;font-weight:500;">
                              <span v-if="deleteLoading" class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                              Eliminar
                            </button>
                          </div>
                        </div>
                      </div>
                    </Teleport>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </transition-group>

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
import { ref, computed, onMounted, watch, defineProps } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getTodasLasSalas, deleteSala } from '@/services/RoomService'
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

// userStore y isAdmin ya están definidos más abajo

const sortOrder = ref('az')

const precioConDescuento = (sala) => sala.precioBase * (1 - (sala.descuento || 0) / 100)

const filteredSalasSorted = computed(() => {
  const arr = [...filteredSalas.value]
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

const paginatedSalas = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredSalasSorted.value.slice(start, end)
})

const totalPages = computed(() => {
  return Math.ceil(filteredSalasSorted.value.length / itemsPerPage.value)
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

const router = useRouter()
const route = useRoute()
const salas = ref([])
const isLoading = ref(true)
const error = ref(null) // Añadido para manejo de errores
const currentPage = ref(1)
const itemsPerPage = ref(10)
// const maxVisiblePages = ref(5) // Eliminado: ya no se usa
const searchQuery = ref('')
const backendUrl = 'https://localhost:5001'
const userStore = useUserStore()
const isAdmin = computed(() => userStore.isAdmin)
const usuario = computed(() => userStore.getUserFromToken())
const usuarioId = computed(() => usuario.value?.id || usuario.value?.sub)
// Track loading state for each image by sala id
const imageLoading = ref({})

// Permitir filtrar solo ofertas si se pasa la prop onlyOfertas
const props = defineProps({
  onlyOfertas: { type: Boolean, default: false }
})

const filteredSalas = computed(() => {
  let salasFiltradas = salas.value
  if (props.onlyOfertas) {
    salasFiltradas = salasFiltradas.filter(sala => sala.descuento > 0)
  }
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    salasFiltradas = salasFiltradas.filter(sala =>
      sala.nombre.toLowerCase().includes(query) ||
      (sala.descripcionCorta && sala.descripcionCorta.toLowerCase().includes(query)) ||
      sala.precioBase.toString().includes(query))
  }

  if (tiposEventoSeleccionados.value.length > 0) {
    const selectedIds = tiposEventoSeleccionados.value.map(t => t.idTipoEvento)
    salasFiltradas = salasFiltradas.filter(sala =>
      sala.tiposEvento && sala.tiposEvento.some(tipo =>
        selectedIds.includes(tipo.idTipoEvento)
      )
    )
  }

  return salasFiltradas
})

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
    error.value = err.message || 'No se pudieron cargar las salas en este momento.'
  } finally {
    isLoading.value = false
  }
}

// Observar cambios en la ruta para actualizar la búsqueda
watch(() => route.query.q, (newQuery) => {
  searchQuery.value = newQuery || ''
  currentPage.value = 1 // Resetear a la primera página al buscar
}, { immediate: true })

// Calcular salas paginadas: ahora se pagina sobre el array ya ordenado
// (definido más abajo junto con la paginación)

// Watch para inicializar el estado de carga de imagen de las salas visibles
watch(paginatedSalas, (visibles) => {
  for (const sala of visibles) {
    if (imageLoading.value[sala.idSala] === undefined) {
      imageLoading.value[sala.idSala] = true
    }
  }
}, { immediate: true })

// Calcular total de páginas
// const totalPages = computed(() => Math.ceil(filteredSalas.value.length / itemsPerPage.value)) // Eliminado: ya no se usa

// Paginación eliminada: visiblePages y changePage removidos

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

// Modal de confirmación para eliminar sala
const showDeleteModal = ref(false)
const salaToDelete = ref(null)
const deleteLoading = ref(false)

const openDeleteModal = (sala) => {
  salaToDelete.value = sala
  showDeleteModal.value = true
}
const closeDeleteModal = () => {
  showDeleteModal.value = false
  salaToDelete.value = null
  deleteLoading.value = false
}
const confirmDeleteSala = async () => {
  if (!salaToDelete.value) return
  deleteLoading.value = true
  try {
    await deleteSala(salaToDelete.value.idSala)
    salas.value = salas.value.filter(s => s.idSala !== salaToDelete.value.idSala)
    closeDeleteModal()
    // Aquí podrías mostrar una notificación de éxito si tienes un sistema de notificaciones global
  } catch (err) {
    closeDeleteModal()
    // Aquí podrías mostrar una notificación de error si tienes un sistema de notificaciones global
  }
}
</script>

<style lang="scss" scoped>
.salas-grid-container {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: 1.5rem;
  justify-content: flex-start; // Alinea a la izquierda
  width: 100%;
}

.sala-card-wrapper {
  display: flex;
  justify-content: flex-start; // Alinea cada tarjeta a la izquierda
  min-width: 320px;
  max-width: 100%;
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

.dropdown-toggle .badge.bg-primary {
  background-color: #1a4456 !important;
}

.filter-chip {
  background-color: #1a4456 !important;
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
    gap: 0.75rem;

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

.filter-container {
  display: flex;
  align-items: center;
  min-width: 250px;
}

.event-type-filter {
  width: 250px;

  .multiselect {
    min-height: 38px;
    border: 1px solid #ced4da;
    border-radius: 0.375rem;

    &-tags {
      padding: 0.375rem 2.5rem 0.375rem 0.75rem;
      min-height: 38px;
    }

    &-placeholder {
      color: #6c757d;
      padding-top: 0.375rem;
      padding-bottom: 0.375rem;
    }

    &-tags-wrap {
      display: flex;
      flex-wrap: wrap;
      gap: 0.25rem;
    }

    &-tag {
      background: #1A4456;
      color: white;
      padding: 0.15rem 0.5rem;
      margin: 0;
      font-size: 0.8rem;

      &-remove {
        color: white;
        &:hover {
          background: transparent;
          color: #f8f9fa;
        }
      }
    }

    &-single {
      padding-top: 0.375rem;
      padding-bottom: 0.375rem;
    }
  }
}

.clear-filter-btn {
  height: 38px;
  width: 38px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
}

/* Chips de filtros activos */
.filter-chip {
  display: inline-flex;
  align-items: center;
  background-color: #1A4456;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.875rem;

  .btn-close {
    filter: invert(1);
    opacity: 0.8;
    font-size: 0.6rem;

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
  }
}

/* --- MODAL GENÉRICO (Consistente con EditRoomView, RoomInfoAdmin, etc.) --- */

/* --- MODAL DE CONFIRMACIÓN DE ELIMINAR SALA (estilo EditRoomView.vue) --- */
/* --- MODAL GENÉRICO (Consistente con EditRoomView, RoomInfoAdmin, etc.) --- */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(30, 44, 70, 0.35);
  backdrop-filter: blur(2px);
  z-index: 2100;
  display: flex;
  align-items: center;
  justify-content: center;
}
.modal-box {
  position: relative;
  background: white;
  padding: 2rem;
  border-radius: 12px;
  width: 100%;
  max-width: 450px;
  box-shadow: 0 5px 20px rgba(0,0,0,0.2);
  text-align: center;
  border-top: 6px solid #dc3545;
}
.modal-error {
  border-top-color: #dc3545;
}
.modal-icon {
  font-size: 3.5rem;
  margin-bottom: 1rem;
  display: block;
  color: #dc3545;
}
.close-button {
  position: absolute;
  top: 12px;
  right: 16px;
  background: none;
  border: none;
  font-size: 1.25rem;
  color: #6c757d;
  cursor: pointer;
}
.close-button:hover {
  color: #000;
}
.modal-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
}
.btn-cancel {
  background-color: #6c757d;
  color: white;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  font-weight: 500;
  border: none;
}
.btn-cancel:hover {
  background-color: #5a6268;
}
.btn-confirm-danger {
  background-color: #dc3545;
  color: white;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  font-weight: 500;
  border: none;
}
.btn-confirm-danger:hover {
  background-color: #c82333;
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
.fade-enter-to, .fade-leave-from, .fade-leave-to {
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
  background: rgba(255,255,255,0.7);
  z-index: 2;
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

  /*
   * El hover debe ir anidado dentro de la clase, no al tope del archivo.
   * Se elimina el selector '&:hover' fuera de contexto para evitar el error de compilación.
   */
}

select[multiple] {
  height: auto;
  min-height: 38px;
  padding: 6px 12px;
}

select[multiple] option {
  padding: 4px 8px;
}

/* Para indicar cuando hay filtros aplicados */
.filter-active {
  position: relative;
}

.filter-active::after {
  content: "";
  position: absolute;
  top: -2px;
  right: -2px;
  width: 8px;
  height: 8px;
  background-color: #dc3545;
  border-radius: 50%;
}

.card-title {
  color: #1A4456;
}

.card-text {
  color: #1A4456;
}

.mt-5 {
  color: #1A4456;
}

.precio {
  color: #1A4456;
}

.sala {
  color: #1A4456;
  padding: 0 38px;

  @media (max-width: 768px) {
    padding: 0 15px;
  }
}

.sala-carousel-wrapper {
  width: 100%;
  overflow-x: auto;
  scroll-behavior: smooth;
  -ms-overflow-style: none;
  scrollbar-width: none;

  &::-webkit-scrollbar {
    display: none;
  }
}

.sala-carousel-track {
  display: flex;
  gap: 20px;
  padding: 10px 0;
}

/* --- TAMAÑO FIJO DE LAS TARJETAS (ajusta aquí para probar diferentes tamaños) --- */
// --- TAMAÑO FIJO DE LAS TARJETAS (ajusta aquí para probar diferentes tamaños) ---
.sala-card-fixed {
  width: 100%;
  max-width: 400px; // antes: 450px
  min-width: 400px;
  height: 540px;
  min-height: 400px;
  max-height: 540px;
  /* Puedes modificar width/height aquí para probar otros tamaños de tarjeta. Ubicación: línea ~587, clase .sala-card-fixed */
  background: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  margin: 0 auto;

  /*
   * Para responsividad:
   * - width: 100% permite que la tarjeta se adapte al ancho de la columna bootstrap.
   * - max-width: 450px asegura que nunca se pase de ese tamaño.
   * - min-width: 260px evita que se colapse demasiado en pantallas pequeñas.
   * - margin: 0 auto centra la tarjeta en su columna.
   *
   * Puedes ajustar max-width/min-width según el diseño deseado.
   */

  /* Puedes modificar width/height arriba para probar otros tamaños */

/*
 * Se eliminó un selector '&:hover' inválido a nivel raíz (línea ~204) para evitar error de compilación SCSS.
 */
}

/*
 * Se eliminó otro selector '&:hover' inválido a nivel raíz (línea ~203) para evitar error de compilación SCSS.
 */

.carousel-control {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 40px;
  height: 40px;
  background: #1A4456;
  border: none;
  border-radius: 50%;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
  z-index: 10;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0.8;

  &:hover {
    opacity: 1;
    background: #f8f9fa;
  }

  &.prev {
    left: 0;
  }

  &.next {
    right: 0;
  }

  &-prev-icon,
  &-next-icon {
    background-size: 100% 100%;
    width: 20px;
    height: 20px;
  }
}

.btn {
  --bs-btn-color: #fff;
  --bs-btn-bg: #1A4456;
  --bs-btn-border-color: #1A4456;
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

.originalprice-and-button {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.original-price {
  color: #6c757d;
  text-decoration: line-through;
  font-size: 1.0em;
}

.pagination {
  .page-item {
    margin: 0 2px;

    &.active .page-link {
      background-color: #1A4456;
      border-color: #1A4456;
      color: white;
    }

    .page-link {
      color: #1A4456;
      border: 1px solid #dee2e6;

      &:hover {
        background-color: #f8f9fa;
        color: #122e3a;
      }

      &:focus {
        box-shadow: 0 0 0 0.25rem rgba(26, 68, 86, 0.25);
      }
    }

    &.disabled .page-link {
      color: #6c757d;
      pointer-events: none;
    }
  }
}

/* Ajustes responsive para la paginación */
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

.page-item.active .page-link {
  z-index: 3;
  color: #fff;
  background-color: #1A4456;
  border-color: #1A4456;
}

.page-link {
  position: relative;
  display: block;
  padding: var(--bs-pagination-padding-y) var(--bs-pagination-padding-x);
  font-size: var(--bs-pagination-font-size);
  color: #1A4456;
  text-decoration: none;
  background-color: var(--bs-pagination-bg);
  border: var(--bs-pagination-border-width) solid var(--bs-pagination-border-color);
  transition: color .15s ease-in-out, background-color .15s ease-in-out, border-color .15s ease-in-out, box-shadow .15s ease-in-out;
}
@media (max-width: 758px) {
  .card {
    min-width: 100%;
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
/* Badge y estilos de oferta para las cards de salas con descuento */
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
  letter-spacing: 0.5px;
}

.precio-oferta {
  color: #1a4456;
  font-size: 1.25rem;
  font-weight: bold;
  line-height: 1.2;
}
.precio {
  font-weight: bold;
}

.original-price {
  color: #6c757d;
  text-decoration: line-through;
  font-size: 0.9em;
}

.price-container {
  display: flex;
  flex-direction: column !important;
  gap: 0.2rem;
}

/* Responsive price container for ProductCard */
// Alineación a la izquierda para el precio en la tarjeta
.price-container-responsive,
.price-container {
  min-width: 0;
  flex: 1 1 0%;
  display: flex;
  flex-direction: row !important;
  align-items: flex-start;
  justify-content: flex-start;
  gap: 0.4rem;
}
.price-row {
  flex-wrap: wrap;
}
@media (max-width: 768px) {
  .price-container-responsive,
  .price-container {
    flex-direction: row !important;
    flex-wrap: wrap;
    align-items: center;
    gap: 0.3rem;
    min-width: 0;
    max-width: 100%;
  }
  .precio-oferta,
  .descuento-oferta {
    font-size: 0.95rem;
  }
  .original-price {
    font-size: 0.85em;
  }
}

/* Botón eliminar sala color personalizado */
.btn-eliminar-sala {
  background-color: #bb2d3b !important;
  border-color: #bb2d3b !important;
  color: #fff !important;
}
.btn-eliminar-sala:hover, .btn-eliminar-sala:focus {
  background-color: #a12632 !important;
  border-color: #a12632 !important;
  color: #fff !important;
}

.admin-btn-group {
  flex-wrap: wrap;
  gap: 0.5rem;
  min-width: 120px;
  max-width: 40%;
  justify-content: flex-end;
}
.admin-btn {
  margin-bottom: 0 !important;
  padding: 0.4rem 0.7rem !important;
  font-size: 0.97rem !important;
  min-width: 80px !important;
  white-space: nowrap;
  flex: 0 1 auto;
}
@media (max-width: 1200px) {
  .admin-btn-group {
    max-width: 60%;
    gap: 0.3rem;
  }
  .admin-btn {
    font-size: 0.93rem !important;
    padding: 0.35rem 0.5rem !important;
    min-width: 70px !important;
  }
}
@media (max-width: 768px) {
  .admin-btn-group {
    max-width: 100%;
    flex-wrap: wrap;
    gap: 0.3rem;
  }
  .admin-btn {
    font-size: 0.9rem !important;
    padding: 0.3rem 0.4rem !important;
    min-width: 65px !important;
  }
}
</style>
