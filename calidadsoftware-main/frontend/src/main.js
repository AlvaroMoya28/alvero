import { createApp } from 'vue'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap'
import './assets/doctorpc.css'
import router from './router'
import { createPinia } from 'pinia'

const app = createApp(App)
app.use(router)
app.use(createPinia())
app.mount('#app')
