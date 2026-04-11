<template>
  <main class="page-shell" id="home">
    <section class="hero-section" id="home">
      <div class="hero-copy animate">
        <p class="eyebrow">Feliz primer año, mi amor 💙</p>
        <h1>Verónica, este es nuestro primer año convertido en un recorrido hermoso.</h1>
        <p class="hero-description">
          Esto no es solo una página. Es un pedacito de todo lo que vivimos, todo lo que siento por ti
          y todo lo que quiero seguir construyendo juntos.
        </p>

        <div class="hero-actions">
          <a class="primary-button" href="#story">Entrar a nuestra historia</a>
          <button class="secondary-button" @click="toggleNightMode">
            {{ nightMode ? 'Activar modo día' : 'Activar modo noche' }}
          </button>
        </div>
      </div>

      <div class="hero-visual animate">
        <div class="floating-hearts">
          <span class="heart" style="--i:1"></span>
          <span class="heart" style="--i:2"></span>
          <span class="heart" style="--i:3"></span>
          <span class="heart" style="--i:4"></span>
        </div>
        <div class="hero-image-wrap">
          <img src="../images/WhatsApp Image 2026-04-09 at 22.39.15 (5).jpeg" alt="Jardín romántico" />
        </div>
      </div>
    </section>

    <section class="section-block story-section" id="story">
      <div class="section-intro animate">
        <p class="eyebrow">Nuestra historia</p>
        <h2>Mini capítulos que cuentan cómo llegamos hasta acá.</h2>
        <p>
          Todo empezó en un momento que parecía normal… pero terminó siendo el inicio de lo mejor que me ha pasado.
          Cada capítulo tiene un olor a primavera y una sonrisa guardada para ti.
        </p>
      </div>

      <div class="story-grid">
        <article v-for="section in storySections" :key="section.title" class="story-card animate">
          <p class="story-label">{{ section.label }}</p>
          <h3>{{ section.title }}</h3>
          <p>{{ section.text }}</p>
        </article>
      </div>
    </section>

    <section class="gallery-section" id="gallery">
      <div class="section-intro">
        <p class="eyebrow">Galería</p>
        <h2>Recuerdos en imágenes y palabras.</h2>
        <p>Fotos que muestran cuándo empecé a enamorarme de vos en cada gesto.</p>
      </div>

      <div class="gallery-actions">
        <button class="secondary-button" @click="randomizeGallery">Ver recuerdos aleatorios</button>
        <button
          v-if="hasMoreGallery"
          class="secondary-button"
          @click="loadAllGallery"
          style="margin-left: 12px;">
          Mostrar toda la galería
        </button>
      </div>

      <div class="gallery-grid">
        <article v-for="item in visibleGallery" :key="item.src" class="gallery-item">
          <img :src="item.src" :alt="item.alt" loading="lazy" />
        </article>
      </div>
    </section>

    <section class="section-block section-soft garden-section animate" id="garden">
      <div class="section-intro">
        <p class="eyebrow">Jardín</p>
        <h2>Nuestro amor es un jardín que cuidamos juntos.</h2>
        <p>
          Cada planta simboliza un valor que alimentamos con cariño. Tocá una flor para leer un recuerdo
          o una promesa que florece con vos.
        </p>
      </div>

      <div class="garden-plot">
        <button
          v-for="plant in plantList"
          :key="plant.id"
          class="plant-card animate"
          :class="{ active: activePlant === plant.id }"
          @click="selectPlant(plant.id)">
          <span class="plant-emoji">{{ plant.icon }}</span>
          <div>
            <h3>{{ plant.label }}</h3>
            <p>{{ plant.short }}</p>
          </div>
        </button>
      </div>

      <div class="plant-detail animate">
        <h3>{{ activePlantData.label }}</h3>
        <p>{{ activePlantData.note }}</p>
      </div>
    </section>

    <section class="section-block quiz-section animate" id="quiz">
      <div class="section-intro">
        <p class="eyebrow">Juego</p>
        <h2>¿Qué tanto recordás de nosotros? 😏</h2>
        <p>Probá tus recuerdos con este quiz divertido y amoroso.</p>
      </div>

      <div class="quiz-card animate">
        <form @submit.prevent="submitQuiz">
          <div v-for="question in quiz.questions" :key="question.id" class="quiz-question">
            <p class="question-title">{{ question.question }}</p>
            <div class="question-options">
              <label v-for="option in question.options" :key="option" class="quiz-option">
                <input type="radio" :name="question.id" :value="option" v-model="quiz.answers[question.id]" />
                <span>{{ option }}</span>
              </label>
            </div>
          </div>

          <div class="quiz-actions">
            <button type="submit" class="primary-button">Ver resultado</button>
            <button type="button" class="secondary-button" @click="resetQuiz">Reiniciar</button>
          </div>
        </form>

        <div v-if="showQuizResult" class="quiz-result">
          <p class="score">Acertaste {{ quizScore }} de {{ quiz.questions.length }}</p>
          <p class="result-text">{{ quizMessage }}</p>
        </div>
      </div>
    </section>

    <section class="section-block stats-section animate" id="stats">
      <div class="section-intro">
        <p class="eyebrow">Datos curiosos</p>
        <h2>Pequeñas verdades sobre nosotros.</h2>
      </div>

      <div class="stats-grid">
        <article v-for="stat in facts" :key="stat.label" class="stat-card animate">
          <span class="stat-emoji">{{ stat.icon }}</span>
          <h3>{{ stat.label }}</h3>
          <p>{{ stat.value }}</p>
        </article>
      </div>
    </section>

    <section class="letter-section animate" id="letter">
      <div class="letter-copy animate">
        <p class="eyebrow">Carta</p>
        <h2>Mi Amor... gracias por ser tú.</h2>
        <p>
          Este primer año contigo ha sido el más bonito de mi vida. Gracias por cada momento, cada risa,
          cada abrazo y por tu forma de cuidar lo que nos rodea.
        </p>
        <p>
          No sé cómo explicar todo lo que siento por ti, pero sí sé algo: quiero seguir escribiendo esta historia
          contigo.
        </p>
        <p>Feliz primer año, mi amor 💙</p>
        <p class="letter-signature">— Álvaro</p>
      </div>

      <div class="unlock-section animate">
        <p class="unlock-intro">Mensajes desbloqueables</p>
        <div class="unlock-grid">
          <button
            v-for="message in unlockMessages"
            :key="message.id"
            class="unlock-button"
            @click="openMessage(message.id)">
            {{ message.label }}
          </button>
        </div>

        <div class="unlocked-messages">
          <article v-for="message in openedMessagesList" :key="message.id" class="message-card animate">
            <h3>{{ message.label }}</h3>
            <p>{{ message.text }}</p>
          </article>
        </div>
      </div>
    </section>
  </main>
</template>

<script setup>
import { computed, nextTick, onMounted, reactive, ref } from 'vue';

const nightMode = ref(false);

const storySections = [
  {
    label: 'El carnaval de Palmares',
    title: 'Un viernes que ya no era igual',
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (5).jpeg'),
    text: 'El 24 de enero del 2025 llegué a Palmares con planes distintos, pero el universo tenía otros para mí: conocerte fue lo mejor que me ha pasado. Fue una tarde inesperada y hermosa, de esas que te cambian el corazón.',
  },
  {
    label: 'Ese instante al verte',
    title: 'La sorpresa de un rostro familiar',
    text: 'Conocía a Valeria, pero no sabía que tenía hermana. Verte fue como encontrar un rostro conocido que ya no reconocía: habías cambiado mucho desde el colegio y me dejaste pensando, hasta te dije esa noche que parecías un año mayor.',
  },
  {
    label: 'El match que soñaba',
    title: 'Las preguntas de tu prima y mi hermana',
    text: 'Era lo que yo quería desde que te vi. Cuando tu prima y mi hermana me preguntaron si había match entre nosotras, supe que estaba a punto de comenzar algo enorme.',
  },
  {
    label: 'Las mariposas en la fila',
    title: 'La charla de la pizza',
    text: 'Quizás la fila para la pizza no fue la conversación más profunda, pero para mí fue clave: ahí empecé a conocerte, a entender tus metas, tu inteligencia, tu competitividad y tu corazón. Fue el primer momento en que sentí mariposas en el estómago.',
  },
  {
    label: 'La salida espontánea',
    title: 'El segundo día que no resistimos',
    text: 'No pudimos esperar hasta el domingo y decidimos vernos. Fue un día solo de vos y de mí, como si el mundo dejara de existir. Fue hermoso y lo recuerdo con tanto cariño.',
  },
  {
    label: 'Conocer a tu familia',
    title: 'El domingo que lo cambió todo',
    text: 'Ese día en que conocí a tu familia fue un detonante. Ver el amor que te tienen me hizo entender lo increíble que eres y definió muchas cosas en mi visión sobre vos.',
  },
  {
    label: 'El primer mes juntos',
    title: 'Cuando te pedí que fueras mi novia',
    text: 'Después de un mes y poco de estar juntos, llegó el día que estaba más nervioso que nunca. Todo salió tal como lo planeé, con la sorpresa en mi cuarto y un fin de semana increíble contigo. Imposible olvidar ese 10 de abril.',
  },
  {
    label: 'Nuestro primer viaje',
    title: 'Herradura y la primera vez como pareja oficial',
    text: 'Nuestro primer viaje oficial fue a Herradura, la playita no era perfecta, pero fue hermoso porque fuimos los dos como pareja oficial. Ese viaje quedó guardado con todo el cariño.',
  },
  {
    label: 'Tu primera vez en mi apartamento',
    title: 'Una cita en mi espacio',
    text: 'A principios de mayo te llevé por primera vez a mi apartamento. Lo preparé como si fuera nuestra primera cita y me encantó sentirme con vos en mi espacio, ya parte de mí.',
  },
  {
    label: 'Aprender a vernos con distancia',
    title: 'San Carlos y San Pedro',
    text: 'Empezamos a organizarnos entre semana: uno iba a San Carlos, el otro a San Pedro. La distancia dejó de pesar, porque cada viernes era un motivo para ir a estar contigo.',
  },
  {
    label: 'La Esperanza por primera vez',
    title: 'El primer viaje nervioso',
    text: 'Principios de junio nos llevó a La Esperanza. Estaba nervioso por si te iba a gustar, pero el viaje fue hermoso. Desde entonces fue el primero de muchos viajes increíbles juntos.',
  },
  {
    label: 'Nuestro segundo viaje',
    title: 'Curú, caminatas y la camaroneada',
    text: 'En ese viaje conociste Curú conmigo y caminamos solitos. La camaroneada fue inolvidable: te vi emocionada, feliz y apuntada, y fue mi primera vez camaroneando, un pequeño secreto. Fue hermoso vivirlo contigo.',
  },
  {
    label: 'Tu primer pez',
    title: 'Aventuras nuevas contigo',
    text: 'En ese mismo viaje fuimos a pescar y agarraste tu primer pez. No sabes cuánto significa para mí que vivas nuevas experiencias conmigo y me acompañes en cada aventura.',
  },
  {
    label: 'Vacaciones con tu tía',
    title: 'Un julio lleno de playas',
    text: 'A finales de julio volvimos a La Esperanza con tu tía. Paseamos por todas las playas y fue gracias a vos que dejé la pereza de viajar: nunca había paseado tanto en mi vida.',
  },
  {
    label: 'Caminar hasta Cartago',
    title: 'Un 2 de agosto difícil pero bonito',
    text: 'Acompañarte caminando hasta Cartago fue duro, pero la alegría de hacerlo contigo lo hizo valioso. Aunque no sé si lo repetiría, fue un recuerdo que me marcó.',
  },
  {
    label: 'Madres y familia',
    title: 'El Día de las Madres con mi mamá',
    text: 'Ese día te vi compartir con mi mamá y mis tías, tomando vino y jugando naipe. Fue muy especial, porque supe que eso era lo que quería y me encantó verte tan tú con ellas.',
  },
  {
    label: 'Tu cumpleaños',
    title: '28 de septiembre inolvidable',
    text: 'Hicimos tu cumpleaños en mi casa, con tu familia, fotos y risas. Fue uno de los mejores días de mi vida: me encantó celebrarlo, preparar el queque y ver que te gustara todo.',
  },
  {
    label: 'Graduación juntos',
    title: 'Fotos y baile en noviembre',
    text: 'La cena de graduación de nuestras hermanas fue una suerte hermosa que coincidiera. Vivirla juntos, tomarnos fotos y bailar fue una experiencia muy linda a tu lado.',
  },
  {
    label: 'Las pulgas por primera vez',
    title: 'El 14 de diciembre que me encantó',
    text: 'Fui a las pulgas contigo por primera vez y aunque no sabía si me iba a gustar, resultó ser hermoso porque lo viví contigo. Ahora esas fechas me dan nostalgia y alegría.',
  },
  {
    label: 'Nuestra primera Navidad',
    title: '25 de diciembre en moto y regalos',
    text: 'Nuestro primer viaje en moto para Navidad fue la mejor navidad de mi vida. Amo los regalos que me diste y todo lo que significaron para mí.',
  },
  {
    label: 'Otra playa con tu tía',
    title: 'Casa rodante y caídas',
    text: 'En el viaje del 27 de diciembre fuimos a la playa con tu tía y la casa rodante. Fue hermoso aunque casi me mato cuando me caí, y eso lo hace aún más memorable.',
  },
  {
    label: 'Año nuevo juntos',
    title: '31 de diciembre viajando en moto',
    text: 'Nuestro primer fin de año juntos fue a La Esperanza en moto. Fue hermoso vivir algo distinto y sentir que ya somos una pareja que disfruta sus experiencias sin depender de nada.',
  },
  {
    label: 'La Cali con cariño',
    title: '30 de enero diferente y especial',
    text: 'Nuestra salida a La Cali fue una experiencia distinta que no vivía contigo desde hace un año. Fue hermoso y lo recuerdo con mucho cariño, porque ya estábamos consolidados como pareja.',
  },
  {
    label: 'San Valentín otra vez',
    title: 'La Esperanza con tus tías',
    text: 'El viaje de San Valentín a La Esperanza con tus tías fue cansado pero hermoso. Me encantó que ya me incluyeras en tus cosas y que pudiéramos vivir tantas experiencias juntos.',
  },
  {
    label: 'La fiesta de tu abuelo',
    title: '15 de marzo especial',
    text: 'Fue un honor compartir la fiesta de tu abuelo contigo. Sentirme incluido y parte de tu familia fue muy significativo y me hizo quererte aún más.',
  },
  {
    label: 'Semana Santa',
    title: 'Vivir lo cotidiano de otra manera',
    text: 'La Semana Santa fue especial porque viví algo tan común contigo y se sintió distinto. Salir en las procesiones y abrirme contigo fue un gran paso que me llenó de ternura.',
  },
  {
    label: 'La Esperanza otra vez',
    title: '1 de abril del 2026, viaje inesperado',
    text: 'El último viaje a La Esperanza fue inesperado, pero lo vivimos de la mejor manera. Entendí que no importa el lugar si estamos juntos: contigo siempre es hermoso.',
  },
];

const galleryItems = [
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15.jpeg'),
    alt: 'Foto 1',
    caption: 'Nuestro primer viaje en moto.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (1).jpeg'),
    alt: 'Foto 2',
    caption: 'Una mirada que me hizo entender que esto era real.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (2).jpeg'),
    alt: 'Foto 3',
    caption: 'Sonrisas compartidas en un día que nunca olvidaré.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (3).jpeg'),
    alt: 'Foto 4',
    caption: 'Una tarde que quedó grabada en mi memoria.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (4).jpeg'),
    alt: 'Foto 5',
    caption: 'Cada detalle de ese momento me hace sonreír.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (5).jpeg'),
    alt: 'Foto 6',
    caption: 'Nuestro primer año en imágenes, lleno de ternura.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (6).jpeg'),
    alt: 'Foto 7',
    caption: 'Me gustó verte reír así, sin preocupaciones.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (7).jpeg'),
    alt: 'Foto 8',
    caption: 'Un instante tranquilo que guardo con cariño.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (8).jpeg'),
    alt: 'Foto 9',
    caption: 'Ese día nos acercó un poco más.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (9).jpeg'),
    alt: 'Foto 10',
    caption: 'La complicidad de dos que ya se entienden.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (10).jpeg'),
    alt: 'Foto 11',
    caption: 'Una imagen de esas que me hacen querer más momentos juntos.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.15 (11).jpeg'),
    alt: 'Foto 12',
    caption: 'Tu sonrisa lo dice todo, siempre hermosa.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14.jpeg'),
    alt: 'Foto 13',
    caption: 'Un segundo de calma que ya es un recuerdo eterno.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (1).jpeg'),
    alt: 'Foto 14',
    caption: 'Cada imagen de vos me hace valorar este año.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (2).jpeg'),
    alt: 'Foto 15',
    caption: 'Esas charlas largas que nunca quiero que terminen.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (3).jpeg'),
    alt: 'Foto 16',
    caption: 'Una tarde linda, una foto que refleja paz.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (4).jpeg'),
    alt: 'Foto 17',
    caption: 'Contigo todo se siente más bonito.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (5).jpeg'),
    alt: 'Foto 18',
    caption: 'Nuestros recuerdos están llenos de luz y cariño.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (6).jpeg'),
    alt: 'Foto 19',
    caption: 'Este momento fue especial por lo natural que fue.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (7).jpeg'),
    alt: 'Foto 20',
    caption: 'Mi lugar favorito es cualquier lugar contigo.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (8).jpeg'),
    alt: 'Foto 21',
    caption: 'Esa complicidad silenciosa que nos une.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (9).jpeg'),
    alt: 'Foto 22',
    caption: 'Cada día contigo suma más ganas de seguir.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (10).jpeg'),
    alt: 'Foto 23',
    caption: 'Tu mirada me hizo sentir siempre seguro.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (11).jpeg'),
    alt: 'Foto 24',
    caption: 'Un recuerdo que no quiero borrar.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (12).jpeg'),
    alt: 'Foto 25',
    caption: 'Una foto llena de amor y ternura.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (13).jpeg'),
    alt: 'Foto 26',
    caption: 'Ese momento me hizo apreciar cada detalle.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (14).jpeg'),
    alt: 'Foto 27',
    caption: 'Tu risa es la melodía que más amo.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (15).jpeg'),
    alt: 'Foto 28',
    caption: 'Gracias por cada momento que compartimos.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.14 (16).jpeg'),
    alt: 'Foto 29',
    caption: 'Un instante simple, pero lleno de nosotros.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.13.jpeg'),
    alt: 'Foto 30',
    caption: 'La vida junto a vos se siente como un regalo.',
  },
  {
    src: require('../images/WhatsApp Image 2026-04-09 at 22.39.13 (1).jpeg'),
    alt: 'Foto 31',
    caption: 'Esta foto cierra el año con una sonrisa compartida.',
  },
];

const initialGalleryCount = galleryItems.length;
const visibleGallery = ref(galleryItems.slice(0, initialGalleryCount));

const hasMoreGallery = computed(() => visibleGallery.value.length < galleryItems.length);

const setupRevealAnimations = () => {
  nextTick(() => {
    const elements = Array.from(document.querySelectorAll('.animate'));
    if (!elements.length) return;

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add('is-visible');
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.18 }
    );

    elements.forEach((el) => observer.observe(el));
  });
};

onMounted(() => {
  window.history.replaceState(null, '', '#home');
  window.scrollTo(0, 0);
  setupRevealAnimations();
});

const plantList = [
  {
    id: 'confianza',
    icon: '🌱',
    label: 'Confianza',
    short: 'la base de cada paso',
    note: 'Recuerdo cómo confiaste en mí desde el primer día y eso hizo que todo creciera con fuerza.',
  },
  {
    id: 'amor',
    icon: '🌸',
    label: 'Amor',
    short: 'lo que florece cada mañana',
    note: 'Nuestro amor es la flor que decidió quedarse y nos regala colores en cada estación.',
  },
  {
    id: 'paciencia',
    icon: '🌿',
    label: 'Paciencia',
    short: 'lo que cuida el jardín',
    note: 'Cada paso lento y cada gesto de espera son semillas que hacen crecer todo más bonito.',
  },
  {
    id: 'felicidad',
    icon: '🌻',
    label: 'Felicidad',
    short: 'la luz que nos acompaña',
    note: 'Cuando estamos juntos, todo tiene un brillo especial y el día se siente más cálido.',
  },
];

const activePlant = ref('confianza');

const activePlantData = computed(() => plantList.find((plant) => plant.id === activePlant.value) || plantList[0]);

const quiz = reactive({
  answers: {
    q1: '',
    q2: '',
    q3: '',
  },
  questions: [
    {
      id: 'q1',
      question: '¿Dónde fue nuestra primera vez?',
      options: ['En mi casa', 'En mi apartamento', 'En tu casa'],
      correct: 'En mi casa',
    },
    {
      id: 'q2',
      question: '¿Quién dijo "te amo" primero?',
      options: ['Álvaro', 'Verónica', 'Los dos al mismo tiempo'],
      correct: 'Álvaro',
    },
    {
      id: 'q3',
      question: '¿Qué comida nos gusta más juntos?',
      options: ['Pizza', 'Papas con lo que sea', 'Helado'],
      correct: 'Papas con lo que sea',
    },
  ],
});

const showQuizResult = ref(false);

const quizScore = computed(() =>
  quiz.questions.reduce((score, question) => {
    return score + (quiz.answers[question.id] === question.correct ? 1 : 0);
  }, 0)
);

const quizMessage = computed(() => {
  if (!showQuizResult.value) return '';
  if (quizScore.value === quiz.questions.length) return 'Perfecto, sos el amor de mi vida 💙';
  if (quizScore.value >= 2) return 'Casi perfecto, te amo igual 😘';
  return '¡Te falta, pero igual te amo 😘';
});

const facts = [
  { icon: '📅', label: 'Días juntos', value: '365 días juntos 💙' },
  { icon: '😂', label: 'Risas compartidas', value: 'Incontables' },
  { icon: '✨', label: 'Momentos favoritos', value: 'Todos contigo' },
  { icon: '😅', label: 'Discusiones ganadas', value: 'Spoiler: tú' },
];

// ========== MENSAJES DESBLOQUEABLES ACTUALIZADOS (COSAS BONITAS) ==========
const unlockMessages = [
  {
    id: 'feliz',
    label: 'Abrir cuando estés feliz',
    text: 'Tu felicidad es mi canción favorita. Que nunca deje de sonar, porque verte así me llena el alma de luz y me recuerda por qué cada día te elijo. 💙',
  },
  {
    id: 'extraña',
    label: 'Abrir cuando me extrañes',
    text: 'Cuando me extrañas, yo también te extraño el doble. Pero sabé que cada kilómetro o cada hora sin verte solo sirven para que mi corazón se prepare para abrazarte más fuerte cuando vuelvas a estar cerca. Te llevo conmigo a donde voy. 🌟',
  },
  {
    id: 'sonreír',
    label: 'Abrir cuando quieras sonreír',
    text: 'Sonreí, amor, porque tu sonrisa es el milagro más bonito que me regaló la vida. Si hoy no encontraste una razón, recordá que existo yo, y que solo pensar en vos ya me saca una sonrisa enorme. 😊✨',
  },
];
// =========================================================================

const openedMessages = ref([]);

const openedMessagesList = computed(() =>
  unlockMessages.filter((message) => openedMessages.value.includes(message.id))
);

const randomizeGallery = () => {
  const count = visibleGallery.value.length || initialGalleryCount;
  visibleGallery.value = [...galleryItems].sort(() => Math.random() - 0.5).slice(0, count);
};

const loadAllGallery = () => {
  visibleGallery.value = [...galleryItems];
};

const selectPlant = (id) => {
  activePlant.value = id;
};

const submitQuiz = () => {
  showQuizResult.value = true;
};

const resetQuiz = () => {
  quiz.answers.q1 = '';
  quiz.answers.q2 = '';
  quiz.answers.q3 = '';
  showQuizResult.value = false;
};

const openMessage = (id) => {
  if (!openedMessages.value.includes(id)) {
    openedMessages.value.push(id);
  }
};

const toggleNightMode = () => {
  nightMode.value = !nightMode.value;
  document.body.classList.toggle('night-mode', nightMode.value);
};
</script>