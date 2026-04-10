import { createApp } from 'vue';
import App from './App.vue';
import 'bootstrap/dist/css/bootstrap.min.css';
import './assets/vero.css';
import 'bootstrap';
import router from './router';
import { createPinia } from 'pinia';

const app = createApp(App);
app.use(router);
app.use(createPinia());
app.mount('#app');
