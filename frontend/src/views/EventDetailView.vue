<template>
  <div>
    <button class="btn btn-link mb-3" @click="$router.push('/events')">← Volver</button>
    <div v-if="loading" class="alert alert-info">Cargando evento...</div>
    <div v-else-if="event">
      <h2>{{ event.title }}</h2>
      <p>{{ event.description }}</p>
      <p><strong>Fecha:</strong> {{ formatDate(event.date) }}</p>
      <p><strong>Ubicación:</strong> {{ event.location }}</p>
    </div>
    <div v-else class="alert alert-warning">Evento no encontrado.</div>
  </div>
</template>

<script>
import api from '../services/api';

export default {
  name: 'EventDetailView',
  data() {
    return {
      loading: true,
      event: null
    };
  },
  async created() {
    const id = this.$route.params.id;
    try {
      const response = await api.get(`/events/${id}`);
      this.event = response.data;
    } catch {
      this.event = null;
    } finally {
      this.loading = false;
    }
  },
  methods: {
    formatDate(value) {
      return new Date(value).toLocaleString();
    }
  }
};
</script>
