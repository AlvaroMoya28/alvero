<template>
  <div v-if="show" class="modal-backdrop">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header d-flex flex-column align-items-start" style="gap: 0.5rem;">
          <h5 class="modal-title mb-2">Agregar Reseña</h5>
          <div class="w-100 mb-2">
            <label class="form-label mb-1" style="font-weight: 600;">Calificación</label>
            <div class="star-rating mb-1">
              <span v-for="n in 5" :key="n" class="star"
                    :class="{ filled: n <= calificacion }"
                    @click="calificacion = n"
                    @mouseover="hovered = n"
                    @mouseleave="hovered = 0"
                    style="cursor:pointer; font-size:2rem; color:#b88b4a;">
                <i :class="['bi', (n <= (hovered || calificacion)) ? 'bi-star-fill' : 'bi-star']"></i>
              </span>
            </div>
          </div>

        </div>
        <form @submit.prevent="submitReview">
          <div class="modal-body">
            <div class="mb-3">
              <label for="comentario" class="form-label">Comentario</label>
              <textarea v-model="comentario" id="comentario" class="form-control" rows="4" maxlength="1000" placeholder="Escribe tu reseña..." required></textarea>
            </div>
          </div>
          <div class="modal-footer d-flex justify-content-between align-items-center">
            <button type="button" class="btn btn-secondary btn-modal" @click="$emit('close')">Cancelar</button>
            <button type="submit" class="btn btn-enviar btn-modal">Enviar Reseña</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, defineProps, defineEmits } from 'vue'

const props = defineProps({
  show: { type: Boolean, required: true },
  reservaId: { type: [Number, String], required: true },
  usuarioId: { type: [Number, String], required: true }
})
const emit = defineEmits(['close', 'submit'])

const calificacion = ref(0)
const hovered = ref(0)
const comentario = ref('')

watch(props, () => {
  calificacion.value = ''
  comentario.value = ''
})

function submitReview () {
  emit('submit', {
    reservaId: props.reservaId,
    usuarioId: props.usuarioId,
    calificacion: Number(calificacion.value),
    comentario: comentario.value.trim()
  })
}
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.4);
  z-index: 1050;
  display: flex;
  align-items: center;
  justify-content: center;
}
.modal-dialog {
  max-width: 400px;
  width: 100%;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.18);
  overflow: hidden;
}
.modal-content {
  background: #fff;
  border-radius: 8px;
  box-shadow: none;
  padding: 24px 28px 20px 28px;
}
</style>

<style scoped>
.btn-modal {
  min-width: 120px;
  font-weight: 500;
  border-radius: 6px;
  padding: 0.5rem 1.2rem;
  font-size: 1rem;
  margin-top: 0.2rem;
  margin-bottom: 0.2rem;
  box-shadow: none;
  transition: background 0.2s, border 0.2s;
}
.btn-modal:focus {
  outline: none;
  box-shadow: none;
}

.btn-enviar {
  background: #1A4456;
  border-color: #1A4456;
  color: #fff;
}
.btn-enviar:hover {
  background: #122e3a;
  border-color: #122e3a;
  color: #fff;
}
</style>
