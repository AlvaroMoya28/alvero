import { onMounted, onUnmounted } from 'vue';
import gsap from 'gsap';
import ScrollTrigger from 'gsap/ScrollTrigger';

gsap.registerPlugin(ScrollTrigger);

export function useScrollAnimations() {
  let scrollTriggerInstances = [];

  const animateOnScroll = (selector, config = {}) => {
    const {
      duration = 0.8,
      ease = 'back.out(1.7)',
      stagger = 0.15,
      startTrigger = 'top 70%',
      fromState = { opacity: 0, y: 40 },
      toState = { opacity: 1, y: 0 },
    } = config;

    gsap.set(selector, fromState);

    const trigger = ScrollTrigger.batch(selector, {
      onEnter: (batch) =>
        gsap.to(batch, {
          duration,
          ease,
          stagger,
          ...toState,
        }),
      start: startTrigger,
    });

    scrollTriggerInstances.push(trigger);
  };

  const parallaxEffect = (selector, distance = 50) => {
    const trigger = gsap.to(selector, {
      scrollTrigger: {
        trigger: selector,
        start: 'top center',
        end: 'bottom center',
        scrub: 1,
      },
      y: distance,
      ease: 'none',
    });

    scrollTriggerInstances.push(trigger.scrollTrigger);
  };

  const counterEffect = (selector) => {
    gsap.to(selector, {
      scrollTrigger: {
        trigger: selector,
        start: 'top 75%',
      },
      textContent: 0,
      duration: 2,
      snap: { textContent: 1 },
      ease: 'power2.out',
    });
  };

  const revealEffect = (selector, direction = 'up') => {
    const fromState = {
      up: { y: 60, opacity: 0 },
      left: { x: -60, opacity: 0 },
      right: { x: 60, opacity: 0 },
      down: { y: -60, opacity: 0 },
    }[direction];

    gsap.set(selector, fromState);

    const trigger = ScrollTrigger.batch(selector, {
      onEnter: (batch) =>
        gsap.to(batch, {
          duration: 0.8,
          opacity: 1,
          x: 0,
          y: 0,
          stagger: 0.1,
          ease: 'power3.out',
        }),
      start: 'top 75%',
    });

    scrollTriggerInstances.push(trigger);
  };

  const scaleReveal = (selector) => {
    gsap.set(selector, { opacity: 0, scale: 0.8 });

    const trigger = ScrollTrigger.batch(selector, {
      onEnter: (batch) =>
        gsap.to(batch, {
          duration: 0.8,
          opacity: 1,
          scale: 1,
          stagger: 0.1,
          ease: 'back.out(1.5)',
        }),
      start: 'top 75%',
    });

    scrollTriggerInstances.push(trigger);
  };

  const rotationReveal = (selector) => {
    gsap.set(selector, { opacity: 0, rotationX: -25, y: 50 });

    const trigger = ScrollTrigger.batch(selector, {
      onEnter: (batch) =>
        gsap.to(batch, {
          duration: 0.8,
          opacity: 1,
          rotationX: 0,
          y: 0,
          stagger: 0.1,
          ease: 'back.out(1.4)',
        }),
      start: 'top 70%',
    });

    scrollTriggerInstances.push(trigger);
  };

  const cleanup = () => {
    scrollTriggerInstances.forEach((trigger) => {
      if (trigger && typeof trigger.kill === 'function') {
        trigger.kill();
      }
    });
    scrollTriggerInstances = [];
  };

  onUnmounted(() => {
    cleanup();
  });

  return {
    animateOnScroll,
    parallaxEffect,
    counterEffect,
    revealEffect,
    scaleReveal,
    rotationReveal,
    cleanup,
  };
}

export function useScrollTriggerRefresh() {
  const refresh = () => {
    ScrollTrigger.refresh();
  };

  return { refresh };
}
