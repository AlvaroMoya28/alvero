<template>
  <div class="event-type-selector">
    <h4 class="subtitle"><label for="eventos">Tipos de Evento Compatibles <span class="required">*</span></label></h4>
    <div class="tag-container">
      <span
        v-for="tipo in tiposEventoDisponibles"
        :key="tipo.idTipoEvento"
        class="tag"
        :class="{ 'tag-selected': isSelected(tipo.idTipoEvento) }"
        @click="toggleSelection(tipo.idTipoEvento)"
      >
        <i v-if="tipo.icono" :class="tipo.icono"></i>
        {{ tipo.nombre }}
      </span>
    </div>
    <div v-if="error" class="error-message">{{ error }}</div>
    <input
      type="hidden"
      :name="name"
      :value="selectedIds.join(',')"
    >
  </div>
</template>

<script>
import { ref, watch, onMounted } from 'vue'
import { getTiposEvento } from '@/services/EventTypeService'

export default {
  name: 'EventTypeSelector',
  props: {
    modelValue: {
      type: Array,
      default: () => []
    },
    name: {
      type: String,
      default: 'tiposEvento'
    },
    required: {
      type: Boolean,
      default: false
    }
  },
  emits: ['update:modelValue', 'validation'],
  setup (props, { emit }) {
    const tiposEventoDisponibles = ref([])
    const selectedIds = ref([...props.modelValue])
    const error = ref('')

    const validate = () => {
      if (props.required && selectedIds.value.length === 0) {
        error.value = 'Debe seleccionar al menos un tipo de evento'
        emit('validation', false)
        return false
      }
      error.value = ''
      emit('validation', true)
      return true
    }

    const isSelected = (id) => selectedIds.value.includes(id)

    const toggleSelection = (id) => {
      if (isSelected(id)) {
        selectedIds.value = selectedIds.value.filter(item => item !== id)
      } else {
        selectedIds.value = [...selectedIds.value, id]
      }
      emit('update:modelValue', selectedIds.value)
      validate()
    }

    onMounted(async () => {
      try {
        tiposEventoDisponibles.value = await getTiposEvento()
        validate()
      } catch (error) {
        console.error('Error al cargar tipos de evento:', error)
      }
    })

    watch(() => props.modelValue, (newVal) => {
      selectedIds.value = [...newVal]
      validate()
    })

    return {
      tiposEventoDisponibles,
      selectedIds,
      isSelected,
      toggleSelection,
      error
    }
  }
}
</script>

<style scoped>
.event-type-selector {
  margin-bottom: 1.5rem;
}

.subtitle {
  color: #1A4456;
  font-size: 22px;
  margin-bottom: 20px;
  font-weight: 600;
  padding-bottom: 10px;
  border-bottom: 1px solid #eee;
  display: flex;
}

.tag-container {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.tag {
  display: inline-flex;
  align-items: center;
  gap: 0.3rem;
  padding: 0.5rem 1rem;
  background-color: #f0f0f0;
  border-radius: 20px;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 0.9rem;
}

.tag:hover {
  background-color: #e0e0e0;
}

.tag-selected {
  background-color: #b88b4a;
  color: white;
}

.tag i {
  font-size: 1rem;
}

.error-message {
  color: #dc3545;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.required {
  color: #dc3545;
}
</style>
