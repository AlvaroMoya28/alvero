# 🎬 Guía de Ejecución - Animaciones Revolucionarias

## 🚀 Inicio Rápido

### 1. Instalar Dependencias
```bash
cd frontend
npm install
```

### 2. Ejecutar en Desarrollo
```bash
npm run serve
```
Luego abre: **http://localhost:8080**

### 3. Ver las Animaciones
- Abre la página en tu navegador
- **Recarga la página** para ver las animaciones del hero
- **Desplázate hacia abajo** (scroll) para ver todas las animaciones activarse
- **Pasa el mouse** sobre elementos para ver los efectos hover

---

## 📱 Testing en Diferentes Dispositivos

### Desktop
- ✅ Full animations
- ✅ Parallax effect
- ✅ Hover effects
- ✅ 3D transforms

### Tablet
- ✅ Touch-optimized
- ✅ Reduced parallax
- ✅ Staggered reveals

### Mobile
- ✅ Smooth animations
- ✅ Optimized for performance
- ✅ Touch interactions

---

## 🎯 Animaciones por Sección (Al hacer Scroll)

| Sección | Animación | Trigger | Duración |
|---------|-----------|---------|----------|
| **Hero** | Fade + Slide | Inmediata | 1-1.2s |
| **Story** | Reveal 3D + Shine | Al scrollear | 0.8s |
| **Gallery** | Scale + Zoom | Al scrollear | 0.8s |
| **Garden** | RotationX + Emoji | Al scrollear | 0.7s |
| **Quiz** | Slide Left | Al scrollear | 0.9s |
| **Stats** | Elastic Bounce | Al scrollear | 0.7s |
| **Letter** | Fade + Slide | Al scrollear | 1.0s |
| **Messages** | Bounce In | Al scrollear | 0.6s |

---

## 🔧 Estructura de Archivos

```
frontend/
├── src/
│   ├── views/
│   │   └── HomeView.vue ✨ (Animaciones principales)
│   ├── composables/
│   │   └── useScrollAnimations.js (Funciones reutilizables)
│   ├── utils/
│   │   └── advancedAnimations.js (Ejemplos avanzados)
│   └── assets/
│       └── vero.css (Estilos de animación)
├── ANIMATIONS_GUIDE.md (Documentación)
└── package.json (Incluye gsap@latest)
```

---

## 🎨 Personalización

### Cambiar Duración de Animaciones

En `HomeView.vue`, modifica `setupRevealAnimations()`:

```javascript
// Para más rápido
gsap.to('.story-card', {
  duration: 0.5,  // Cambiar de 0.8
  // ...
});

// Para más lento
gsap.to('.story-card', {
  duration: 1.2,  // Cambiar de 0.8
  // ...
});
```

### Cambiar Tipo de Easing

```javascript
// Disponible en GSAP
ease: 'back.out(1.7)'     // Rebote (actual)
ease: 'elastic.out(1,0.8)' // Elástico
ease: 'power3.out'        // Suave
ease: 'bounce.out'        // Impacto
ease: 'sine.inOut'        // Sinusoidal
```

### Cambiar Stagger (Retraso entre elementos)

```javascript
// Rápido
stagger: 0.05,

// Lento
stagger: 0.3,

// Con patrón
stagger: {
  each: 0.1,
  from: "center",  // "start", "center", "end"
}
```

---

## 🔍 Debugging

### Ver ScrollTrigger Markers
```javascript
// En setupRevealAnimations, agrega:
ScrollTrigger: {
  trigger: '.story-card',
  markers: true, // ← Esto muestra los puntos de activación
  start: 'top 70%',
}
```

### Monitorear Performance
Usa Chrome DevTools:
1. F12 → Performance
2. Click en Record
3. Haz scroll en la página
4. Click Stop
5. Analiza los frames (objetivo: 60fps)

---

## ⚡ Optimizaciones Aplicadas

✅ GPU acceleration (transform + opacity only)
✅ Batch operations (múltiples elementos a la vez)
✅ Auto cleanup (no memory leaks)
✅ Lazy initialization (solo cuando se necesita)
✅ Will-change hints (optimización CSS)
✅ RequestAnimationFrame (60fps)

---

## 🎬 Agregar Nuevas Animaciones

### Opción 1: Usar el Composable
```javascript
import { useScrollAnimations } from '@/composables/useScrollAnimations';

const { scaleReveal } = useScrollAnimations();

onMounted(() => {
  scaleReveal('.mi-elemento');
});
```

### Opción 2: Usar Ejemplos Avanzados
```javascript
import { advancedAnimations } from '@/utils/advancedAnimations';

onMounted(() => {
  advancedAnimations.blurInEffect('.mi-elemento');
  advancedAnimations.textRevealEffect('.titulo');
});
```

### Opción 3: GSAP Directo
```javascript
import gsap from 'gsap';

onMounted(() => {
  gsap.to('.mi-elemento', {
    duration: 0.8,
    opacity: 1,
    y: 0,
    ease: 'back.out(1.7)',
  });
});
```

---

## 📊 Checklist de Verificación

- [ ] npm install completado
- [ ] npm run build sin errores
- [ ] GSAP se muestra en DevTools (Applications → Packages)
- [ ] Hero anima al cargar
- [ ] Story cards animan al scrollear
- [ ] Gallery items zoomean en hover
- [ ] Plant emojis rotan en hover
- [ ] Stats cards bouncing al scrollear
- [ ] Messages de unlock aparecen suave
- [ ] 60fps en DevTools Performance

---

## 🐛 Troubleshooting

### Las animaciones no funcionan
```bash
# Reinstalar GSAP
npm uninstall gsap
npm install gsap
```

### Animaciones muy lentas
- Revisar Performance en DevTools
- Reducir duración en código
- Desactivar Markers en ScrollTrigger

### Memory leak
- Verificar que onUnmounted limpia triggers
- Revisar console por warnings

### Blur o pixelación
- Aumentar `will-change: transform, opacity`
- Usar `backface-visibility: hidden`

---

## 📻 Build para Producción

```bash
# Generar build optimizado
npm run build

# Deploy a Vercel/Netlify
npm install -g vercel
vercel               # Se preguntará por configuración
```

El build incluirá:
- ✅ Code splitting automático
- ✅ Minificación GSAP
- ✅ CSS optimization
- ✅ Image optimization

---

## 📞 Soporte

Si hay problemas:
1. Revisar la consola (F12 → Console)
2. Checar ANIMATIONS_GUIDE.md
3. Ver ejemplos en advancedAnimations.js
4. Documentación GSAP: https://gsap.com

---

## 🎉 ¡Listo!

Ya está todo configurado. Ahora solo:

```bash
npm run serve
# Abre http://localhost:8080
# ¡Disfruta las animaciones! 🚀
```

---

**Última actualización**: 2026-04-19
**Versión**: 2.0 - Revolucionaria
**Estado**: Ready for Production ✅
