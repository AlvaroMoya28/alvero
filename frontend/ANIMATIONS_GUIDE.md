# 🎬 Animaciones Revolucionarias - HomeView.vue

## 📋 Descripción General

Se ha reescrito completamente el sistema de animaciones de la página utilizando **GSAP (GreenSock Animation Platform)** y **ScrollTrigger**, reemplazando las animaciones básicas con efectos visuales profesionales y sofisticados.

## ✨ Animaciones Implementadas

### 1. **Hero Section** 
- **Efecto**: Entrada escalonada con opacidad y traslación
- **Duración**: 1-1.2 segundos
- **Easing**: power3.out
- **Interactividad**: Las imágenes tienen efecto parallax al scrollear
- **Efectos Hover**: Brillo aumentado (brightness: 1.05)

### 2. **Story Cards**
- **Efecto**: Reveal con rotación 3D y efecto de brillo interno (shine effect)
- **Duración**: 0.8 segundos
- **Easing**: back.out(1.7)
- **Stagger**: 0.15 segundos entre elementos
- **Trigger**: Se activan al scrollear (top 70% en viewport)
- **Hover**: Efecto de sombra moderna y shine overlay animado

### 3. **Gallery Items**
- **Efecto**: Scale y fade simultáneamente
- **Duración**: 0.8 segundos
- **Easing**: back.out(1.5) para efecto elástico
- **Stagger**: 0.1 segundos
- **Hover**: Zoom 1.1x + rotación 2deg + aumento de brillo

### 4. **Plant Cards (Jardín)**
- **Efecto**: Rotación 3D (rotationX) con translación Y
- **Duración**: 0.7 segundos
- **Easing**: back.out(1.4)
- **Stagger**: 0.1 segundos
- **Interactividad**: El emoji rota y escala (1.3x) al hover/active
- **Indicador Visual**: Gradiente radial en corners

### 5. **Quiz Section**
- **Efecto**: Deslizamiento desde la izquierda con fade
- **Duración**: 0.9 segundos
- **Easing**: power2.out
- **Trigger**: Al scrollear a la sección de quiz

### 6. **Statistics Cards**
- **Efecto**: Elastic bounce + escala
- **Duración**: 0.7 segundos
- **Easing**: elastic.out(1, 0.8)
- **Stagger**: 0.08 segundos
- **Hover**: Los emojis rotan 12deg y escalan 1.25x

### 7. **Letter Section**
- **Efecto**: Fade y deslizamiento desde la izquierda
- **Duración**: 1 segundo
- **Easing**: power2.out
- **Característica**: Títulos con gradiente lineal animado

### 8. **Unlock Section**
- **Efecto**: Bounce in con scale
- **Duración**: 0.8 segundos
- **Easing**: back.out(1.7)
- **Botones**: Efecto de shine al pasar el mouse (shimmer animation)

### 9. **Message Cards**
- **Efecto**: Staggered reveal con traslación Y
- **Duración**: 0.6 segundos
- **Easing**: back.out(1.5)
- **Stagger**: 0.1 segundos
- **Indicador**: Gradiente animado al hover

### 10. **Floating Hearts** (Original + Mejorado)
- **Efecto**: Flotación continua con variaciones de duración
- **Duración**: 5.5s - 7s según el corazón
- **Animación**: Movimiento vertical suave + opacidad

## 🎯 Características Técnicas

### Plugins GSAP Utilizados
- **ScrollTrigger**: Para detectar elementos en viewport y activar animaciones
- **Pseudo-elementos CSS**: Para efectos de brillo y gradientes

### Optimizaciones de Rendimiento
- Uso de `will-change` en CSS para mejorar rendering
- Batch operations con ScrollTrigger para eficiencia
- Transform-only animations para mejor performance
- Clip-path y opacity en lugar de height/width

### Responsive
- Las animaciones se adaptan a todos los tamaños de pantalla
- Distancias y duraciones optimizadas para mobile

## 🔧 Uso de Composables (Opcional)

Se ha creado un composable `useScrollAnimations()` que puede usarse para agregar más animaciones:

```javascript
import { useScrollAnimations } from '@/composables/useScrollAnimations';

const { animateOnScroll, parallaxEffect, scaleReveal } = useScrollAnimations();

// Usar en setupscroll
onMounted(() => {
  scaleReveal('.mi-elemento');
  parallaxEffect('.elemento-parallax', 100);
});
```

## 📊 Comparativa: Antes vs Después

| Aspecto | Antes | Después |
|---------|-------|---------|
| Sistema | Intersection Observer manual | GSAP + ScrollTrigger |
| Easing | 2-3 tipos básicos | 10+ tipos avanzados |
| Hover Effects | Solo transform | Shine, gradients, 3D |
| Performance | requestAnimationFrame | GPU-accelerated |
| Parallax | No | Sí, dinámico |
| Control | Manual | Trigger-based automático |

## 🎨 Colores y Estilos

Las animaciones respetan el tema de la página (light/dark mode) y usan:
- Sombras dinámicas: `box-shadow` con rgba variables
- Gradientes: Lineales y radiales con transparencias
- Filtros: brightness, contrast, saturación

## 🚀 Para Activar

El sistema ya está activo. Solo asegúrate de:

1. Verificar que GSAP está instalado: `npm install gsap`
2. Los imports están correctos en HomeView.vue
3. El CSS tiene los estilos de `will-change` para performance

## 📝 Notas de Implementación

- Las animaciones son "lazy": Solo se crean cuando se scrollea a la sección
- ScrollTrigger se refresca automáticamente después del montaje
- El composable maneja la limpieza (cleanup) al desmontar el componente
- Las animaciones son suaves incluso en conexiones lentas (easing cúbico)

---

**Estado**: ✅ Completo y optimizado
**Última actualización**: 2026-04-19
**Compatible con**: Vue 3.2+, GSAP 3.12+
