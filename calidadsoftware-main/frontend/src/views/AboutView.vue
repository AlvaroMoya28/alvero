<template>
  <div class="container py-5">
    <h1 class="mb-4 text-primary-custom">Sobre Nosotros</h1>
    <div class="about-section mb-5">
      <h2 class="text-primary-custom mb-3">¿Quiénes somos?</h2>
      <p class="text-secondary-custom">
        Somos un equipo multidisciplinario con experiencia en desarrollo de software, gestión de proyectos y atención al cliente. Nuestra meta es transformar la manera en que se gestionan y disfrutan los eventos, combinando innovación, calidad y cercanía humana.
      </p>
    </div>
    <div class="about-section mb-5">
      <h2 class="text-primary-custom mb-3">Integrantes del Grupo</h2>
      <ul class="team-list">
        <li><i class="bi bi-person-circle me-2"></i> <strong>Alejandro Solórzano</strong> <span class="email">alejandro.solorzanobaudrit@ucr.ac.cr</span></li>
        <li><i class="bi bi-person-circle me-2"></i> <strong>Álvaro Moya</strong> <span class="email">alvaro.moyaarrieta@ucr.ac.cr</span></li>
        <li><i class="bi bi-person-circle me-2"></i> <strong>Antony Picado</strong> <span class="email">antony.picado@ucr.ac.cr</span></li>
        <li><i class="bi bi-person-circle me-2"></i> <strong>Kenneth Osorio</strong> <span class="email">kenneth.osorio@ucr.ac.cr</span></li>
      </ul>
    </div>
    <div class="about-section mb-5">
      <h2 class="text-primary-custom mb-3">Nuestra Historia</h2>
      <p class="text-secondary-custom">
        Este proyecto nació como un reto académico y se convirtió en una oportunidad para demostrar nuestro compromiso, creatividad y capacidad de trabajo en equipo. Cada línea de código y cada decisión de diseño reflejan nuestro esfuerzo por ofrecer una plataforma robusta, intuitiva y confiable.
      </p>
    </div>
    <div class="about-section mb-5">
      <h2 class="text-primary-custom mb-3">Nuestros Valores</h2>
      <ul class="about-values">
        <li><strong>Innovación:</strong> Buscamos siempre nuevas formas de mejorar la experiencia de nuestros usuarios.</li>
        <li><strong>Calidad:</strong> Nos esforzamos por entregar productos y servicios que superen las expectativas.</li>
        <li><strong>Trabajo en equipo:</strong> Creemos que la colaboración es la clave para lograr grandes resultados.</li>
        <li><strong>Compromiso:</strong> Nos tomamos en serio cada proyecto y cada cliente.</li>
        <li><strong>Empatía:</strong> Escuchamos y entendemos las necesidades de quienes confían en nosotros.</li>
      </ul>
    </div>
    <div class="about-section">
      <h2 class="text-primary-custom mb-3">Nuestro Compromiso</h2>
      <p class="text-secondary-custom">
        Seguiremos aprendiendo, mejorando y creciendo para que cada evento gestionado a través de nuestra plataforma sea una experiencia memorable. ¡Gracias por confiar en nosotros!
      </p>
    </div>
  </div>
</template>

<script setup>
// Página estática, no requiere lógica JS
</script>

<style scoped>
.text-primary-custom {
  color: #1A4456;
}
.text-secondary-custom {
  color: #6c757d;
}
.about-section {
  background: #f8f9fa;
  border-radius: 14px;
  box-shadow: 0 4px 16px rgba(26,68,86,0.07);
  padding: 2rem 2.5rem;
  margin-bottom: 2.5rem;
}
.about-values {
  list-style: none;
  padding-left: 0;
}
.about-values li {
  margin-bottom: 0.8rem;
  font-size: 1.1rem;
  position: relative;
  padding-left: 1.5rem;
}
.about-values li:before {
  content: '\2022';
  color: #1A4456;
  font-weight: bold;
  display: inline-block;
  width: 1em;
  margin-left: -1em;
}
.team-list {
  list-style: none;
  padding-left: 0;
  margin-bottom: 0;
}
.team-list li {
  font-size: 1.1rem;
  margin-bottom: 0.7rem;
  display: flex;
  align-items: center;
  flex-wrap: wrap;
}
.team-list i {
  color: #1A4456;
  font-size: 1.4rem;
  margin-right: 0.5rem;
}
.email {
  color: #6c757d;
  font-size: 1rem;
  margin-left: 0.7rem;
  word-break: break-all;
}
@media (max-width: 768px) {
  .about-section {
    padding: 1.2rem 1rem;
  }
}
</style>
