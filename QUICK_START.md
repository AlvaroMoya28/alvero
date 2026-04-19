# ⚡ QUICK START - Animaciones en 3 Minutos

## 🚀 Paso 1: Instalar y Ejecutar
```bash
cd frontend
npm run serve
```

## 🌐 Paso 2: Abrir en Navegador
```
http://localhost:8080
```

## 👀 Paso 3: Ver las Animaciones
- **Carga**: Hero con fade-in escalonado
- **Scroll Down**: Aparecen story cards con efecto 3D
- **Scroll More**: Gallery items zoomean con rotación
- **Hover**: Plant emojis rotan y escalan
- **Keep Scrolling**: Stats cards bouncing, Letter section suave

---

## 🎬 Lo Que Ves

```
┌─────────────────────────────────────┐
│    🎪 HERO SECTION                  │
│  Aparece con fade-in suave          │
│  Parallax al scrollear               │
└─────────────────────────────────────┘
         ⬇️ SCROLL
┌─────────────────────────────────────┐
│    📖 STORY CARDS                   │
│  Entran una por una con 3D          │
│  Brillo animado en hover             │
└─────────────────────────────────────┘
         ⬇️ SCROLL
┌─────────────────────────────────────┐
│    🖼️ GALLERY                       │
│  Imágenes zoomean sin aburrimiento  │
│  Rotación sutil + brightness         │
└─────────────────────────────────────┘
         ⬇️ SCROLL
┌─────────────────────────────────────┐
│    🌺 PLANT CARDS                   │
│  3D rotation + emoji flip           │
│  Efecto premium en hover             │
└─────────────────────────────────────┘
         ⬇️ SCROLL
┌─────────────────────────────────────┐
│    ✨ Y MÁS...                       │
│  Quiz, Stats, Letter, Messages      │
│  Todas con sus propias anim          │
└─────────────────────────────────────┘
```

---

## 🛠️ Cambiar Velocidad (Opcional)

Abre: `frontend/src/views/HomeView.vue`

Busca: `duration: 0.8,`

Cambia:
```javascript
duration: 0.5,   // Más rápido
duration: 1.2,   // Más lento
```

---

## 💡 Agregar Más Animaciones (Avanzado)

`frontend/src/utils/advancedAnimations.js` tiene 12 ejemplos:

```javascript
import { advancedAnimations } from '@/utils/advancedAnimations';

// En onMounted:
advancedAnimations.textRevealEffect('.mi-elemento');
advancedAnimations.blurInEffect('.otro-elemento');
```

---

## 📱 Mobile Test

1. F12 (Chrome DevTools)
2. Click device icon (top-left)
3. Selecciona iPhone/Android
4. Ver animaciones optimizadas

---

## ✅ Done!

Ya está todo. Las animaciones están:
- ✅ Instaladas
- ✅ Compiladas
- ✅ Optimizadas
- ✅ Listas para ver

**Solo corre `npm run serve` y disfruta!** 🎉

---

**P.S.** Para documentación completa, ve a:
- `ANIMATIONS_GUIDE.md` - Detalles técnicos
- `SETUP_ANIMATIONS.md` - Guía completa
- `advancedAnimations.js` - Ejemplos de código
