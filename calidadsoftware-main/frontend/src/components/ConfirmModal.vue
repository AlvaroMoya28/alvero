<template>
  <teleport to="body">
    <transition name="modal-fade">
      <div v-if="modelValue" class="modal-overlay" @click.self="handleCancel">
        <div class="modal-container" :class="modalClass">
          <div class="modal-header" v-if="title">
            <h3>{{ title }}</h3>
            <button class="modal-close" @click="handleCancel" v-if="showClose">
              <i class="bi bi-x-lg"></i>
            </button>
          </div>

          <div class="modal-body">
            <div v-if="icon" class="modal-icon" :class="`icon-${type}`">
              <i :class="icon"></i>
            </div>
            <p class="modal-message">{{ message }}</p>
          </div>

          <div class="modal-footer">
            <button
              v-if="showCancel"
              class="btn btn-secondary"
              @click="handleCancel"
            >
              {{ cancelText }}
            </button>
            <button
              class="btn"
              :class="confirmClass"
              @click="handleConfirm"
            >
              {{ confirmText }}
            </button>
          </div>
        </div>
      </div>
    </transition>
  </teleport>
</template>

<script setup>
import { computed, defineProps, defineEmits } from 'vue'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: ''
  },
  message: {
    type: String,
    required: true
  },
  type: {
    type: String,
    default: 'info', // info, success, warning, danger, question
    validator: (value) => ['info', 'success', 'warning', 'danger', 'question'].includes(value)
  },
  confirmText: {
    type: String,
    default: 'Aceptar'
  },
  cancelText: {
    type: String,
    default: 'Cancelar'
  },
  showCancel: {
    type: Boolean,
    default: false
  },
  showClose: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['update:modelValue', 'confirm', 'cancel'])

const modalClass = computed(() => `modal-${props.type}`)

const confirmClass = computed(() => {
  const classMap = {
    info: 'btn-primary',
    success: 'btn-success',
    warning: 'btn-warning',
    danger: 'btn-danger',
    question: 'btn-primary'
  }
  return classMap[props.type] || 'btn-primary'
})

const icon = computed(() => {
  const iconMap = {
    info: 'bi bi-info-circle-fill',
    success: 'bi bi-check-circle-fill',
    warning: 'bi bi-exclamation-triangle-fill',
    danger: 'bi bi-x-circle-fill',
    question: 'bi bi-question-circle-fill'
  }
  return iconMap[props.type]
})

const handleConfirm = () => {
  emit('confirm')
  emit('update:modelValue', false)
}

const handleCancel = () => {
  emit('cancel')
  emit('update:modelValue', false)
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  backdrop-filter: blur(2px);
}

.modal-container {
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow: hidden;
  animation: slideDown 0.3s ease-out;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.modal-header {
  padding: 20px 24px;
  border-bottom: 1px solid #e9ecef;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #2c3e50;
}

.modal-close {
  background: none;
  border: none;
  font-size: 1.25rem;
  color: #6c757d;
  cursor: pointer;
  padding: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: all 0.2s;
}

.modal-close:hover {
  background: #f8f9fa;
  color: #495057;
}

.modal-body {
  padding: 24px;
  text-align: center;
}

.modal-icon {
  font-size: 3.5rem;
  margin-bottom: 16px;
}

.icon-info {
  color: #0d6efd;
}

.icon-success {
  color: #198754;
}

.icon-warning {
  color: #ffc107;
}

.icon-danger {
  color: #dc3545;
}

.icon-question {
  color: #0dcaf0;
}

.modal-message {
  font-size: 1rem;
  color: #495057;
  margin: 0;
  line-height: 1.6;
}

.modal-footer {
  padding: 16px 24px;
  background: #f8f9fa;
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  border-top: 1px solid #e9ecef;
}

.btn {
  padding: 10px 24px;
  border: none;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 0.95rem;
}

.btn-primary {
  background: #0d6efd;
  color: white;
}

.btn-primary:hover {
  background: #0b5ed7;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(13, 110, 253, 0.3);
}

.btn-success {
  background: #198754;
  color: white;
}

.btn-success:hover {
  background: #157347;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(25, 135, 84, 0.3);
}

.btn-warning {
  background: #ffc107;
  color: #000;
}

.btn-warning:hover {
  background: #ffca2c;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(255, 193, 7, 0.3);
}

.btn-danger {
  background: #dc3545;
  color: white;
}

.btn-danger:hover {
  background: #bb2d3b;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(220, 53, 69, 0.3);
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background: #5c636a;
  transform: translateY(-1px);
}

.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

@media (max-width: 576px) {
  .modal-container {
    width: 95%;
  }

  .modal-footer {
    flex-direction: column-reverse;
  }

  .btn {
    width: 100%;
  }
}
</style>
