<template>
  <footer class="footer-container">
    <div class="footer-content">
      <div class="footer-section">
        <h3>Políticas</h3>
        <nav>
          <ul>
            <li><router-link to="/terminos">Términos y condiciones</router-link></li>
            <li><router-link to="/reembolsos">Reembolsos</router-link></li>
            <li><router-link to="/privacidad">Privacidad</router-link></li>
            <li><router-link to="/cookies">Cookies</router-link></li>
          </ul>
        </nav>
      </div>

      <div class="footer-section">
        <h3>Ayuda</h3>
        <nav>
          <ul>
            <li><router-link to="/garantia">Solicitar garantía</router-link></li>
            <li><router-link to="/contacto">Contáctanos</router-link></li>
            <li><router-link to="/reservar">Reservar cita</router-link></li>
          </ul>
        </nav>
      </div>

      <div class="footer-section">
        <h3>Servicios</h3>
        <nav>
          <ul>
            <li><router-link to="/diagnostico">Diagnóstico</router-link></li>
            <li><router-link to="/reparacion-hardware">Reparación hardware</router-link></li>
            <li><router-link to="/recuperacion-datos">Recuperación de datos</router-link></li>
          </ul>
        </nav>
      </div>
    </div>

    <div class="footer-bottom">
      <div class="footer-logo-copyright">
        <img src="../assets/logo2.png" alt="DoctorPC Logo" class="footer-logo">
        <div class="footer-meta">
          <p>&copy; 2025 DoctorPC. Todos los derechos reservados.</p>
          <p class="contact-small">Tel: +56 9 1234 5678 · email: contacto@doctorpc.example</p>
        </div>
      </div>
    </div>
  </footer>
</template>

<style lang="scss" scoped>
.footer-container {
  background-color: var(--dp-darkest);
  color: var(--dp-text);
  padding: 2rem 0 0 0;
  width: 100%;
}

.footer-content {
  display: flex;
  justify-content: space-around;
  flex-wrap: wrap;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 2rem;
}

.footer-section {
  width: 30%;
  min-width: 200px;
  margin-bottom: 1.5rem;

  h3 {
    color: var(--dp-text);
    font-size: 1.1rem;
    margin-bottom: 0.75rem;
    border-bottom: 1px solid rgba(255,255,255,0.06);
    padding-bottom: 0.4rem;
  }
}

nav ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

nav a {
  text-decoration: none;
  color: rgba(255, 255, 255, 0.75);
  display: block;
  padding: 0.3rem 0;
  transition: color 0.2s ease;

  &:hover {
    color: var(--dp-accent);
  }
}

.footer-bottom {
  background-color: var(--dp-dark);
  padding: 1rem;
  text-align: center;
  margin-top: 1rem;

  .footer-logo-copyright {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 1rem;
    flex-wrap: wrap;

    .footer-meta { text-align: left; }

    p {
      margin: 0;
      color: rgba(255, 255, 255, 0.85);
      font-size: 0.9rem;
    }
    .contact-small { font-size: 0.8rem; opacity: 0.9; }
  }

  .footer-logo {
    height: 36px;
    width: auto;
    opacity: 1;
    transition: opacity 0.3s ease;

    &:hover {
      opacity: 0.9;
    }
  }
}

@media (max-width: 768px) {
  .footer-section {
    width: 45%;
  }
}

@media (max-width: 480px) {
  .footer-section {
    width: 100%;
    text-align: center;
  }
}
</style>
