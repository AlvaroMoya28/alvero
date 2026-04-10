<template>
  <div class="social-media">
    <!-- Se agregan los íconos de redes sociales -->
    <a href="#" class="fa fa-facebook"></a> <!-- Facebook -->
    <a href="#" class="fa fa-instagram"></a> <!-- Instagram -->
    <a href="#" class="fa fa-twitter"></a> <!-- Twitter -->
  </div>
</template>

<script>
export default {
  name: 'SocialMedia'
}
</script>

<style lang="scss" scoped>
/* Estilo para el contenedor para las redes sociales (Facebook, Instagram y Twitter) */
.social-media {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  padding: 0.5rem 1rem;
  border-radius: 30px;
  max-width: 600px;
  margin: 0%;
}

/* Estilo para las fonts de los íconos */
.fa {
  padding: 20px;
  font-size: 30px;
  width: 60px;
  text-align: center;
  text-decoration: none;
  position: relative;
}

/* Hover effect */
.fa:hover {
  opacity: 0.7;
}

/* Hover circle background */
.fa:hover::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 50px;
  height: 50px;
  background-color: white;
  border-radius: 50%;
  opacity: 0.6;
}

/* Icon colors */
.fa-facebook,
.fa-instagram,
.fa-twitter {
  color: white;
}
</style>
