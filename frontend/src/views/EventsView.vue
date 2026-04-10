<template>
  <div>
    <h2>Eventos</h2>
    <div v-if="loading" class="alert alert-info">Cargando eventos...</div>
    <div v-else>
      <div v-if="events.length === 0" class="alert alert-warning">No hay eventos registrados.</div>
      <div class="list-group">
        <router-link v-for="event in events" :key="event.id" :to="`/events/${event.id}`" class="list-group-item list-group-item-action">
          <h5 class="mb-1">{{ event.title }}</h5>
          <p class="mb-1">{{ event.description }}</p>
          <small>{{ formatDate(event.date) }} - {{ event.location }}</small>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script>
import api from '../services/api';

export default {
  name: 'EventsView',
  data() {
    return {
      loading: true,
      events: []
    };
  },
  async created() {
    try {
      const response = await api.get('/events');
      this.events = response.data;
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
