// Silenciar logs y warnings de consola solo en este archivo
let errorSpy, warnSpy
beforeAll(() => {
  errorSpy = jest.spyOn(console, 'error').mockImplementation((...args) => {
    if (
      typeof args[0] === 'string' &&
      (
        args[0].includes('[Vue warn]') ||
        args[0].includes('Error al') ||
        args[0].includes('Error en') ||
        args[0].includes('Network Error')
      )
    ) {
      // Oculta logs de error irrelevantes
    }
  })
  warnSpy = jest.spyOn(console, 'warn').mockImplementation((...args) => {
    if (
      typeof args[0] === 'string' &&
      (
        args[0].includes('[Vue warn]') ||
        args[0].includes('No match found for location')
      )
    ) {
      // Oculta logs de warning irrelevantes
    }
  })
})
afterAll(() => {
  errorSpy.mockRestore()
  warnSpy.mockRestore()
})
/* global jest, describe, it, expect, beforeEach, afterEach */

import axios from 'axios'
import * as UserService from '@/services/UserService.js'
import { jwtDecode as realJwtDecode } from 'jwt-decode'

// Mock jwt-decode para poder controlar el resultado en tests
jest.mock('jwt-decode', () => ({
  jwtDecode: jest.fn()
}))

jest.mock('axios')

// Helper para simular localStorage (mock explícito para toHaveBeenCalled)


describe('UserService', () => {
  beforeEach(() => {
    jest.clearAllMocks()
    // Mock robusto de localStorage para todos los tests
    Object.defineProperty(global, 'localStorage', {
      value: {
        getItem: jest.fn(),
        setItem: jest.fn(),
        removeItem: jest.fn()
      },
      configurable: true
    })
  })

  it('solicitarReseteoPassword llama al endpoint correcto', async () => {
    axios.post.mockResolvedValueOnce({ data: { ok: true } })
    const result = await UserService.solicitarReseteoPassword({ email: 'test@mail.com' })
    expect(axios.post).toHaveBeenCalledWith(expect.stringContaining('/forgot-password'), { email: 'test@mail.com' })
    expect(result).toEqual({ ok: true })
  })

  it('restablecerNuevaPassword llama al endpoint correcto', async () => {
    axios.post.mockResolvedValueOnce({ data: { ok: true } })
    const result = await UserService.restablecerNuevaPassword({ email: 'a', code: 'b', newPassword: 'c' })
    expect(axios.post).toHaveBeenCalledWith(expect.stringContaining('/reset-password'), { email: 'a', code: 'b', newPassword: 'c' })
    expect(result).toEqual({ ok: true })
  })

  it('getUsuarioPorId llama al endpoint correcto', async () => {
    axios.get.mockResolvedValueOnce({ data: { nombre: 'Test' } })
    const result = await UserService.getUsuarioPorId('123')
    expect(axios.get).toHaveBeenCalledWith(expect.stringContaining('/123'), expect.any(Object))
    expect(result).toEqual({ nombre: 'Test' })
  })

  it('usuarioService.getAllUsers retorna usuarios', async () => {
    axios.get.mockResolvedValueOnce({ data: [{ id: 1 }] })
    const result = await UserService.usuarioService.getAllUsers()
    expect(axios.get).toHaveBeenCalled()
    expect(result).toEqual([{ id: 1 }])
  })

  it('usuarioService.updateUser actualiza usuario', async () => {
    axios.put.mockResolvedValueOnce({ data: { ok: true } })
    const result = await UserService.usuarioService.updateUser('1', { nombre: 'Nuevo' })
    expect(axios.put).toHaveBeenCalledWith(expect.stringContaining('/1'), { nombre: 'Nuevo' })
    expect(result).toEqual({ ok: true })
  })

  it('usuarioService.deleteUser elimina usuario', async () => {
    axios.delete.mockResolvedValueOnce({ data: { ok: true } })
    const result = await UserService.usuarioService.deleteUser('1')
    expect(axios.delete).toHaveBeenCalledWith(expect.stringContaining('/1'))
    expect(result).toEqual({ ok: true })
  })

  it('usuarioService.registrarUsuario llama al endpoint y retorna datos', async () => {
    axios.post.mockResolvedValueOnce({ data: { idUsuario: 'nuevo' } })
    const usuario = { idUsuario: 'nuevo', contrasena: 'Password1!', confirmarContrasena: 'Password1!' }
    const result = await UserService.usuarioService.registrarUsuario(usuario)
    expect(axios.post).toHaveBeenCalledWith(expect.stringContaining('/registro'), expect.objectContaining({ idUsuario: 'nuevo' }), expect.any(Object))
    expect(result).toEqual({ idUsuario: 'nuevo' })
  })

  it('usuarioService.registrarUsuario lanza error si la contraseña es corta', async () => {
    await expect(UserService.usuarioService.registrarUsuario({ contrasena: '123', confirmarContrasena: '123' })).rejects.toThrow('La contraseña debe tener al menos 8 caracteres')
  })

  it('usuarioService.registrarUsuario maneja error 400 de validación', async () => {
    axios.post.mockRejectedValueOnce({ response: { status: 400, data: { errors: { Contrasena: ['Debe tener mayúsculas'] }, message: 'Error' } } })
    await expect(UserService.usuarioService.registrarUsuario({ contrasena: 'Password1!', confirmarContrasena: 'Password1!' })).rejects.toThrow(/mayúscula/)
  })

  it('usuarioService.registrarUsuario maneja error 409 de conflicto', async () => {
    axios.post.mockRejectedValueOnce({ response: { status: 409, data: { message: 'Ya existe' } } })
    await expect(UserService.usuarioService.registrarUsuario({ contrasena: 'Password1!', confirmarContrasena: 'Password1!' })).rejects.toThrow('Ya existe')
  })

  it('usuarioService.registrarUsuario maneja error de red', async () => {
    axios.post.mockRejectedValueOnce(new Error('Network Error'))
    await expect(UserService.usuarioService.registrarUsuario({ contrasena: 'Password1!', confirmarContrasena: 'Password1!' })).rejects.toThrow('Network Error')
  })

  it('updateUsuario actualiza usuario', async () => {
    axios.put.mockResolvedValueOnce({ data: { ok: true } })
    const result = await UserService.updateUsuario('1', { nombre: 'Nuevo' })
    expect(axios.put).toHaveBeenCalledWith(expect.stringContaining('/1'), { nombre: 'Nuevo' })
    expect(result).toEqual({ ok: true })
  })

  it('usuarioLogIn.login guarda token y retorna usuario', async () => {
    axios.post.mockResolvedValueOnce({ data: { token: 'fake.jwt.token' } })
    const mockUser = { idUsuario: '1', nombre: 'Test', apellido1: 'A', apellido2: 'B', email: 'a', telefono: 'b', tipoUsuario: 'ADMIN', estado: 'ACTIVO' }
    const { jwtDecode } = require('jwt-decode')
    jwtDecode.mockReturnValueOnce(mockUser)
    const result = await UserService.usuarioLogIn.login({ idUsuario: '1', contrasena: 'Password1!' })
    expect(global.localStorage.setItem).toHaveBeenCalledWith('token', 'fake.jwt.token')
    expect(result.usuario).toEqual(expect.objectContaining({ idUsuario: '1', nombre: 'Test' }))
  })

  it('usuarioLogIn.login lanza error si no hay token', async () => {
    axios.post.mockResolvedValueOnce({ data: {} })
    // El mock de jwtDecode no debe ser llamado, pero si lo es, que lance error
    const { jwtDecode } = require('jwt-decode')
    jwtDecode.mockImplementation(() => { throw new Error('Invalid token specified: invalid json for part #2') })
    await expect(UserService.usuarioLogIn.login({})).rejects.toThrow('La respuesta del servidor no tiene el formato esperado')
  })

  it('usuarioLogIn.login maneja error de backend', async () => {
    axios.post.mockRejectedValueOnce({ response: { data: { message: 'Credenciales inválidas' }, status: 401 } })
    // El mock de jwtDecode no debe ser llamado, pero si lo es, que lance error
    const { jwtDecode } = require('jwt-decode')
    jwtDecode.mockImplementation(() => { throw new Error('Invalid token specified: invalid json for part #2') })
    await expect(UserService.usuarioLogIn.login({})).rejects.toThrow('Credenciales inválidas')
  })

  it('deleteUsuario elimina usuario', async () => {
    axios.delete.mockResolvedValueOnce({ data: { ok: true } })
    const result = await UserService.deleteUsuario('1')
    expect(axios.delete).toHaveBeenCalledWith(expect.stringContaining('/1'))
    expect(result).toEqual({ ok: true })
  })

  it('changePasswordAuthenticated llama al endpoint', async () => {
    // Caso éxito
    axios.post.mockResolvedValueOnce({ data: { ok: true } })
    const result = await UserService.changePasswordAuthenticated({ currentPassword: 'a', newPassword: 'b' })
    expect(axios.post).toHaveBeenCalledWith(expect.stringContaining('/change-password'), { currentPassword: 'a', newPassword: 'b' })
    expect(result).toEqual({ ok: true })
    // Caso error backend
    axios.post.mockRejectedValueOnce({ response: { data: { message: 'Credenciales inválidas' } } })
    await expect(UserService.changePasswordAuthenticated({ currentPassword: 'a', newPassword: 'b' })).rejects.toThrow('Credenciales inválidas')
    // Caso error genérico: el servicio retorna mensaje genérico
    axios.post.mockRejectedValueOnce(new Error('Network Error'))
    await expect(UserService.changePasswordAuthenticated({ currentPassword: 'a', newPassword: 'b' })).rejects.toThrow('Error al intentar cambiar la contraseña.')
  })

  it('getUserReservas retorna reservas', async () => {
    axios.get.mockResolvedValueOnce({ data: [{ id: 1 }] })
    const result = await UserService.getUserReservas('1')
    expect(axios.get).toHaveBeenCalledWith(expect.stringContaining('/1/reservas'))
    expect(result).toEqual([{ id: 1 }])
  })

  it('payReserva llama al endpoint de pago', async () => {
    axios.post.mockResolvedValueOnce({ data: { ok: true } })
    const result = await UserService.payReserva('123')
    expect(axios.post).toHaveBeenCalledWith(expect.stringContaining('/reservas/123/pagar'))
    expect(result).toEqual({ ok: true })
  })
})
