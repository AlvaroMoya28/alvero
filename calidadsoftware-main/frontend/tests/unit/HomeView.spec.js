import { mount } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import CarouselBrand from '@/components/CarruselMarca.vue'
import Testimonials from '@/components/TestimonialsComp.vue'
import LogosMarquee from '@/components/LogosMarquee.vue'

describe('HomeView.vue', () => {
  let router

  beforeEach(() => {
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/', component: { template: '<div>Home</div>' } },
        { path: '/reservar', component: { template: '<div>Reservar</div>' } },
        { path: '/servicios', component: { template: '<div>Servicios</div>' } }
      ]
    })
  })

  it('renders home view', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        components: {
          CarouselBrand,
          Testimonials,
          LogosMarquee
        },
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.find('.home-view').exists()).toBe(true)
  })

  it('displays hero section', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.find('.landing-hero').exists()).toBe(true)
  })

  it('displays main hero title', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('Pon tu PC en manos de expertos')
  })

  it('displays hero subtitle', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('Diagnóstico sin costo')
  })

  it('has CTA buttons in hero', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const ctaButtons = wrapper.findAll('.cta-row a')
    expect(ctaButtons.length).toBe(2)
  })

  it('has link to reservar page', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const reservarLink = wrapper.find('a[href="/reservar"]')
    expect(reservarLink.exists()).toBe(true)
  })

  it('has link to servicios page', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const serviciosLink = wrapper.find('a[href="/servicios"]')
    expect(serviciosLink.exists()).toBe(true)
  })

  it('displays trust indicators', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('4.9/5 satisfacción')
    expect(wrapper.text()).toContain('24h promedio')
    expect(wrapper.text()).toContain('90 días de garantía')
  })

  it('displays device card visual', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.find('.device-card').exists()).toBe(true)
  })

  it('displays features section', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.find('.features').exists()).toBe(true)
  })

  it('displays 4 feature cards', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const featureCards = wrapper.findAll('.feature-card')
    expect(featureCards.length).toBe(4)
  })

  it('displays repair feature', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('Reparación experta')
  })

  it('displays home pickup feature', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('Retiro a domicilio')
  })

  it('displays free diagnosis feature', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('Diagnóstico gratis')
  })

  it('displays warranty feature', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('Garantía real')
  })

  it('displays steps section', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.find('.steps').exists()).toBe(true)
    expect(wrapper.text()).toContain('¿Cómo funciona?')
  })

  it('displays 3 steps', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const steps = wrapper.findAll('.step')
    expect(steps.length).toBe(3)
  })

  it('displays step 1 - agenda', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('Agenda en línea')
  })

  it('displays step 2 - diagnostico', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('Diagnóstico')
  })

  it('displays step 3 - reparacion', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.text()).toContain('Reparación y entrega')
  })

  it('displays CTA strip section', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.find('.cta-strip').exists()).toBe(true)
  })

  it('has agenda ahora button in CTA strip', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const ctaButton = wrapper.find('.cta-strip a')
    expect(ctaButton.text()).toContain('Agendar ahora')
  })

  it('renders LogosMarquee component', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.findComponent({ name: 'LogosMarquee' }).exists()).toBe(true)
  })

  it('renders Testimonials component', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.findComponent({ name: 'Testimonials' }).exists()).toBe(true)
  })

  it('has background blobs for decoration', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const blobs = wrapper.findAll('.bg-blob')
    expect(blobs.length).toBe(2)
  })

  it('has proper hero grid structure', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    expect(wrapper.find('.hero-grid').exists()).toBe(true)
    expect(wrapper.find('.hero-copy').exists()).toBe(true)
    expect(wrapper.find('.hero-visual').exists()).toBe(true)
  })

  it('displays badge in hero', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const badge = wrapper.find('.badge-soft')
    expect(badge.exists()).toBe(true)
    expect(badge.text()).toContain('Soporte integral')
  })

  it('has feature card icons', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const icons = wrapper.findAll('.feature-card .icon')
    expect(icons.length).toBe(4)
  })

  it('has step numbers displayed', () => {
    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          CarouselBrand: true,
          Testimonials: true,
          LogosMarquee: true
        }
      }
    })

    const stepNums = wrapper.findAll('.step-num')
    expect(stepNums.length).toBe(3)
    expect(stepNums[0].text()).toBe('1')
    expect(stepNums[1].text()).toBe('2')
    expect(stepNums[2].text()).toBe('3')
  })
})
