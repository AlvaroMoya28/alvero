module.exports = {
  moduleFileExtensions: ['js', 'json', 'vue', 'mjs', 'cjs'],
  transform: {
    '^.+\\.vue$': 'vue-jest',
    '^.+\\.js$': 'babel-jest',
    '^.+\\.mjs$': 'babel-jest',
    '^.+\\.cjs$': 'babel-jest'
  },
  testMatch: ['**/tests/**/*.spec.[jt]s?(x)'],
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1',
    // Mock para archivos estáticos (imágenes, etc)
    '\\.(jpg|jpeg|png|gif|webp|svg)$': '<rootDir>/tests/__mocks__/fileMock.js'
  },
  transformIgnorePatterns: [
    '/node_modules/(?!(axios)/)'
  ],
  collectCoverage: false,
  collectCoverageFrom: [
    "src/components/**/*.vue", // incluye todos los componentes Vue
    "src/**/*.js",             // incluye todos los archivos JS en src
    "!src/main.js",            // ejemplo para excluir un archivo que no quieres contar
    "!src/router/**"           // ejemplo para excluir carpetas específicas (opcional)
  ],
  coverageReporters: ['text', 'lcov'], // tipos de reporte, puedes ajustar
}
