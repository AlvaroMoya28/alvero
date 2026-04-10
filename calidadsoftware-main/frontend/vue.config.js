const { defineConfig } = require('@vue/cli-service')
const webpack = require('webpack')

module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    proxy: {
      '/api': {
        target: 'https://localhost:5001', 
        changeOrigin: true
      }
    }
  },
  chainWebpack: config => {
    config.plugin('html').tap(args => {
      args[0].title = 'DoctorPC'
      return args
    })
    
    // Configurar feature flags de Vue 3
    config.plugin('define').tap((definitions) => {
      Object.assign(definitions[0], {
        __VUE_OPTIONS_API__: 'true',
        __VUE_PROD_DEVTOOLS__: 'false',
        __VUE_PROD_HYDRATION_MISMATCH_DETAILS__: 'false'
      })
      return definitions
    })
  }
})
