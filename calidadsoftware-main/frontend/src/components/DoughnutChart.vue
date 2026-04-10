<template>
  <div class="chart-container">
    <canvas ref="chartCanvas"></canvas>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, defineProps } from 'vue'
import Chart from 'chart.js/auto'

const props = defineProps({
  chartData: {
    type: Object,
    required: true
  }
})

const chartCanvas = ref(null)
let chartInstance = null

onMounted(() => {
  renderChart()
})

watch(() => props.chartData, () => {
  if (chartInstance) {
    chartInstance.destroy()
  }
  renderChart()
}, { deep: true })

function renderChart () {
  if (chartCanvas.value) {
    chartInstance = new Chart(chartCanvas.value, {
      type: 'doughnut',
      data: props.chartData,
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'right'
          }
        }
      }
    })
  }
}
</script>

<style scoped>
.chart-container {
  position: relative;
  height: 300px;
  width: 100%;
}
</style>
