<template>
  <div class = "layout">
    <NavBar/>
    <router-view/>
    <FooterBar/>

    <Teleport to="body">
      <div v-if="isLoadingPage" class="loading-overlay-app">
        <div class="loading-content-app">
          <div class="spinner-app"></div>
          <p>{{ loadingPageMessage }}</p>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import NavBar from './components/NavBar.vue'
import FooterBar from './components/FooterBar.vue'

const isLoadingPage = ref(false)
const loadingPageMessage = ref('Cargando...')

const router = useRouter()

router.beforeEach((to, from, next) => {
  isLoadingPage.value = true
  // Usar el nombre de la ruta si está definido, sino el path.
  // También se puede personalizar el texto como se prefiera.
  let routeName = to.name || to.path.substring(1) || 'inicio' // Quita el '/' inicial y usa 'inicio' para la raíz
  // Capitalizar la primera letra del nombre de la ruta para mejor presentación
  routeName = routeName.charAt(0).toUpperCase() + routeName.slice(1)

  loadingPageMessage.value = `Cargando ${routeName}...`
  next()
})

router.afterEach(() => {
  // Un pequeño retraso para evitar parpadeos en cargas muy rápidas
  setTimeout(() => {
    isLoadingPage.value = false
  }, 200) // Ajusta este valor según sea necesario
})
</script>

<style lang="scss" scoped>
.layout {
  display: flex;
  flex-direction: column;
  min-height: 100vh;

  > * {
    width: 100%;
  }

  > router-view {
    flex: 1; /* Hace que el contenido principal ocupe el espacio disponible */
  }
}

/* Asegura que el footer esté siempre abajo */
.footer-container {
  margin-top: auto;
}

/* Estilos para el spinner de carga de página, adaptados de LogIn.vue */
.loading-overlay-app {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000; /* Asegura que esté por encima de otros elementos como la NavBar */
}

.loading-content-app {
  background-color: white;
  border-radius: 12px;
  padding: 2rem;
  text-align: center;
  max-width: 300px;
  width: 90%;
  box-shadow: 0 5px 15px rgba(0,0,0,0.2);
  p {
    margin-top: 1rem;
    color: #1A4456; /* Color consistente con otros spinners */
    font-weight: 500;
    font-size: 1rem;
  }
}

.spinner-app {
  width: 50px;
  height: 50px;
  border: 5px solid #f3f3f3; /* Gris claro */
  border-top: 5px solid #1A4456; /* Color primario oscuro */
  border-radius: 50%;
  animation: spin-app 1s linear infinite;
  margin: 0 auto;
}

@keyframes spin-app {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
