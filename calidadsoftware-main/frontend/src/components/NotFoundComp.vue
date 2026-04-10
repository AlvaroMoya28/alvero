<template>
    <div class="container text-center mt-5">
        <div class="d-flex flex-column align-items-center">
            <i class="pi pi-exclamation-triangle display-1 mb-4 nf-icon"></i>
            <h2 class="mb-3">404 Not Found</h2>
            <p class="mb-4">
                Lo sentimos, la página que buscas no existe o fue movida.
            </p>
            <RouterLink to="/"><button class="btn mb-5">Regresar</button></RouterLink>
        </div>
    </div>
</template>

<script setup>
import { RouterLink } from 'vue-router'
</script>

<style lang="scss" scoped>
.container {
    margin: auto;
}

h2.mb-3 { color: var(--dp-primary); font-weight: 700; }
p.mb-4 { color: rgba(0,0,0,0.65); }
.nf-icon { color: var(--dp-accent); }

.btn {
    color: #ffffff;
    background-color: var(--dp-primary);
}

.btn-check:checked+.btn, .btn.active, .btn.show, .btn:first-child:active, :not(.btn-check)+.btn:active {
    color: #ffffff;
    background-color: var(--dp-dark);
    border-color:  var(--dp-dark);
}

.btn-check:focus+.btn-success, .btn-success:focus {
    color: #fff;
    background-color: var(--dp-primary);
    border-color: var(--dp-primary);
    box-shadow: 0 0 0 .25rem rgba(11, 91, 215, 0.35);
}
</style>
