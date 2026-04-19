/**
 * GSAP Advanced Examples para HomeView
 * Ejemplos de animaciones complejas que pueden agregarse
 */

/**
 * 1. TIMELINE SECUENCIAL
 * Animar múltiples elementos en secuencia
 */
export const sequentialAnimation = () => {
  const tl = gsap.timeline();
  
  tl.to('.hero-copy', { duration: 0.8, opacity: 1, y: 0 }, 0)
    .to('.hero-visual', { duration: 0.8, opacity: 1, x: 0 }, 0.2)
    .to('.floating-hearts', { duration: 1, y: -20, stagger: 0.1 }, 0.4);
    
  return tl;
};

/**
 * 2. MOUSE TRACKING EFFECT
 * Seguimiento del mouse en elementos específicos
 */
export const mouseTrackingEffect = () => {
  let proxy = { skew: 0, skewMax: 7, force: 100 },
    clamp = gsap.utils.clamp(-proxy.skewMax, proxy.skewMax),
    parseFloat = gsap.utils.unitize(parseFloat),
    txt = document.querySelector('.hero-copy'),
    cursor = gsap.set({}, { x: 0, y: 0 }),
    getSkew = () => (proxy.skew = clamp(gsap.getProperty(txt, 'x') - cursor.getProperty("x")) / proxy.force),
    skewSetter = gsap.quickSetter(txt, "skewY", "deg"),
    clampedSkew = () => skewSetter(getSkew());

  gsap.ticker.add(clampedSkew);

  document.addEventListener("mousemove", (e) => {
    cursor.to({ x: e.clientX, y: e.clientY }, { duration: 0.8, overwrite: "auto" });
  });
};

/**
 * 3. TEXT REVEAL - Animación de letras character by character
 */
export const textRevealEffect = (selector, duration = 0.05) => {
  const elements = document.querySelectorAll(selector);
  
  elements.forEach((el) => {
    const text = el.innerText;
    const chars = text.split("").map((char) => `<span>${char}</span>`).join("");
    el.innerHTML = chars;

    gsap.from(el.querySelectorAll("span"), {
      scrollTrigger: {
        trigger: el,
        start: "top 75%",
      },
      opacity: 0,
      y: 20,
      duration: duration,
      stagger: 0.05,
      ease: "back.out",
    });
  });
};

/**
 * 4. MORPHING SHAPES
 * Cambio de forma con SVG morph
 */
export const morphingShapes = () => {
  const shapes = document.querySelectorAll(".story-card");
  
  shapes.forEach((shape) => {
    gsap.to(shape, {
      scrollTrigger: {
        trigger: shape,
        start: "top center",
        end: "bottom center",
        scrub: 1,
      },
      borderRadius: "40%",
      duration: 1,
    });
  });
};

/**
 * 5. COUNTER ANIMATION - Números que se incrementan
 */
export const counterAnimation = (selector, startValue = 0, endValue = 100) => {
  const elements = document.querySelectorAll(selector);
  
  elements.forEach((el) => {
    const numericValue = { value: startValue };
    
    gsap.to(numericValue, {
      value: endValue,
      duration: 2,
      scrollTrigger: {
        trigger: el,
        start: "top 75%",
      },
      onUpdate: () => {
        el.innerText = ~~numericValue.value;
      },
      ease: "power2.out",
    });
  });
};

/**
 * 6. HORIZONTAL SCROLL EFFECT
 * Desplazamiento horizontal al hacer scroll vertical
 */
export const horizontalScroll = () => {
  const sections = gsap.utils.toArray(".story-card");
  
  gsap.to(sections, {
    xPercent: -100 * (sections.length - 1),
    ease: "none",
    scrollTrigger: {
      trigger: ".story-section",
      pin: true,
      scrub: 1,
      end: `+=${sections.length * 500}`,
      markers: false,
    },
  });
};

/**
 * 7. BLUR-IN EFFECT
 * Desenfoque progresivo al aparecer
 */
export const blurInEffect = (selector) => {
  gsap.set(selector, { filter: "blur(10px)" });
  
  ScrollTrigger.batch(selector, {
    onEnter: (batch) =>
      gsap.to(batch, {
        filter: "blur(0px)",
        opacity: 1,
        duration: 0.8,
        stagger: 0.15,
        ease: "power2.out",
      }),
    start: "top 80%",
  });

  gsap.set(selector, { opacity: 0 });
};

/**
 * 8. FLIP CARD ANIMATION
 * Efecto de volteo de tarjeta 3D
 */
export const flipCardAnimation = (selector) => {
  gsap.set(selector, { rotationY: -180, opacity: 0 });
  
  ScrollTrigger.batch(selector, {
    onEnter: (batch) =>
      gsap.to(batch, {
        rotationY: 0,
        opacity: 1,
        duration: 0.8,
        stagger: 0.1,
        ease: "back.out(1.7)",
      }),
    start: "top 75%",
  });
};

/**
 * 9. PROGRESSIVE COLOR FILL
 * Cambio de color progresivo
 */
export const colorFillAnimation = (selector) => {
  ScrollTrigger.batch(selector, {
    onEnter: (batch) =>
      gsap.to(batch, {
        background: "linear-gradient(135deg, #a8cdf8, #fee0ea)",
        duration: 0.8,
        stagger: 0.1,
        ease: "power2.out",
      }),
    start: "top 70%",
  });
};

/**
 * 10. ADVANCED PARALLAX WITH DEPTH
 * Parallax con múltiples capas
 */
export const advancedParallax = (layerSelector, depth = 3) => {
  const layers = document.querySelectorAll(layerSelector);
  
  layers.forEach((layer, index) => {
    const layerDepth = (index + 1) * depth;
    gsap.to(layer, {
      scrollTrigger: {
        trigger: layer.parentElement,
        start: "top center",
        end: "bottom center",
        scrub: 1,
      },
      y: layerDepth * 10,
      ease: "none",
    });
  });
};

/**
 * 11. STAGGER FROM GRID
 * Stagger automático desde puntos de grilla
 */
export const staggerFromGrid = (containerSelector, itemSelector, columns = 3) => {
  const container = document.querySelector(containerSelector);
  const items = container.querySelectorAll(itemSelector);
  
  gsap.set(items, { opacity: 0, scale: 0.5 });
  
  gsap.to(items, {
    scrollTrigger: {
      trigger: container,
      start: "top 70%",
    },
    opacity: 1,
    scale: 1,
    duration: 0.6,
    stagger: {
      grid: [Math.ceil(items.length / columns), columns],
      from: "center",
      amount: 0.8,
    },
    ease: "back.out(1.7)",
  });
};

/**
 * 12. SPLIT TEXT ANIMATION
 * Animar líneas de texto por separado
 */
export const splitTextAnimation = (selector) => {
  const elements = document.querySelectorAll(selector);
  
  elements.forEach((el) => {
    const text = el.innerText;
    const lines = text.split("\n").map((line) => `<div class="line">${line}</div>`).join("");
    el.innerHTML = lines;

    gsap.from(el.querySelectorAll(".line"), {
      scrollTrigger: {
        trigger: el,
        start: "top 80%",
      },
      opacity: 0,
      y: 30,
      duration: 0.5,
      stagger: 0.1,
      ease: "power2.out",
    });
  });
};

// ============ EXPORTAR TODAS LAS FUNCIONES ============

export const advancedAnimations = {
  sequentialAnimation,
  mouseTrackingEffect,
  textRevealEffect,
  morphingShapes,
  counterAnimation,
  horizontalScroll,
  blurInEffect,
  flipCardAnimation,
  colorFillAnimation,
  advancedParallax,
  staggerFromGrid,
  splitTextAnimation,
};

// ============ EJEMPLO DE USO ============

/**
 * En HomeView.vue:
 * 
 * import { advancedAnimations } from '@/utils/advancedAnimations.js';
 * 
 * onMounted(() => {
 *   // Usar cualquiera de las animaciones
 *   advancedAnimations.textRevealEffect('.letter-copy p');
 *   advancedAnimations.blurInEffect('.stat-card');
 * });
 */
