using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EventosBackend.Mappings;
using EventosBackend.Models.Context;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.Entities;
using EventosBackend.Repositories;
using EventosBackend.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EventosBackend.Tests.Performance
{
    /// <summary>
    /// Pruebas de rendimiento para Objetivos 2 y 4 GQM
    /// - Tiempo de operaciones críticas <= 2000ms
    /// - 95% de transacciones cumpliendo SLA
    /// - Soporte de operaciones concurrentes
    /// </summary>
    public class PerformanceMetricsTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IMapper _mapper;

        public PerformanceMetricsTests(ITestOutputHelper output)
        {
            _output = output;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UsuarioProfile>();
                cfg.AddProfile<ReservaProfile>();
            });
            _mapper = config.CreateMapper();
        }

        private OracleDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<OracleDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new OracleDbContext(options);
        }

        [Fact]
        public async Task GQM_Objetivo2_TiempoCarga_RegistroUsuario_MenorA2Segundos()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UsuarioRepository(context);
            var passwordHasher = new PasswordHasher();
            var service = new UsuarioService(repository, _mapper, passwordHasher);

            var registroRequest = new UsuarioCreateRequest
            {
                IdUsuario = "CLI001",
                Nombre = "Cliente",
                Apellido1 = "Nuevo",
                Email = "cliente@test.com",
                Telefono = "88888888",
                Rol = "Cliente",
                Contrasena = "Password123!",
                ConfirmarContrasena = "Password123!",
                FechaNacimiento = DateTime.UtcNow.AddYears(-25)
            };

            // Act
            var stopwatch = Stopwatch.StartNew();
            var resultado = await service.RegistrarUsuarioAsync(registroRequest);
            stopwatch.Stop();

            // Assert
            _output.WriteLine($"\n=== RENDIMIENTO: REGISTRO DE USUARIO ===");
            _output.WriteLine($"Tiempo de ejecución: {stopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"Umbral máximo: 2000ms");
            _output.WriteLine($"Estado: {(stopwatch.ElapsedMilliseconds < 2000 ? "✓ ACEPTABLE" : "✗ EXCEDIDO")}");
            _output.WriteLine($"GQM_PERFORMANCE_REGISTRO: {stopwatch.ElapsedMilliseconds}");

            Assert.NotNull(resultado);
            Assert.True(stopwatch.ElapsedMilliseconds < 2000,
                $"Registro tardó {stopwatch.ElapsedMilliseconds}ms (debe ser < 2000ms)");
        }

        [Fact]
        public async Task GQM_Objetivo2_TiempoCarga_Login_MenorA2Segundos()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UsuarioRepository(context);
            var passwordHasher = new PasswordHasher();
            var service = new UsuarioService(repository, _mapper, passwordHasher);

            // Crear usuario
            var (hashBytes, saltBytes) = passwordHasher.CreateHash("Password123!");
            var usuario = new Usuario
            {
                IdUnico = Guid.NewGuid().ToString(),
                IdUsuario = "USR001",
                Nombre = "Usuario",
                Apellido1 = "Test",
                Email = "test@test.com",
                TipoUsuario = "Cliente",
                ContrasenaHash = Convert.ToBase64String(hashBytes),
                Salt = Convert.ToBase64String(saltBytes),
                Estado = "ACTIVO",
                FechaRegistro = DateTime.UtcNow,
                FechaNacimiento = DateTime.UtcNow.AddYears(-25)
            };
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Id = "USR001",
                Password = "Password123!"
            };

            // Act
            var stopwatch = Stopwatch.StartNew();
            var resultado = await service.LoginAsync(loginRequest);
            stopwatch.Stop();

            // Assert
            _output.WriteLine($"\n=== RENDIMIENTO: LOGIN ===");
            _output.WriteLine($"Tiempo de ejecución: {stopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"Umbral máximo: 2000ms");
            _output.WriteLine($"Estado: {(stopwatch.ElapsedMilliseconds < 2000 ? "✓ ACEPTABLE" : "✗ EXCEDIDO")}");
            _output.WriteLine($"GQM_PERFORMANCE_LOGIN: {stopwatch.ElapsedMilliseconds}");

            Assert.NotNull(resultado);
            Assert.True(stopwatch.ElapsedMilliseconds < 2000,
                $"Login tardó {stopwatch.ElapsedMilliseconds}ms (debe ser < 2000ms)");
        }

        [Fact]
        public async Task GQM_Objetivo2_TiempoCarga_ConsultaUsuarios_MenorA2Segundos()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UsuarioRepository(context);
            var passwordHasher = new PasswordHasher();
            var service = new UsuarioService(repository, _mapper, passwordHasher);

            // Crear 50 usuarios
            for (int i = 1; i <= 50; i++)
            {
                var (hashBytes, saltBytes) = passwordHasher.CreateHash("Password123!");
                context.Usuarios.Add(new Usuario
                {
                    IdUnico = Guid.NewGuid().ToString(),
                    IdUsuario = $"USR{i:000}",
                    Nombre = $"Usuario {i}",
                    Apellido1 = "Test",
                    Email = $"usuario{i}@test.com",
                    TipoUsuario = "Cliente",
                    ContrasenaHash = Convert.ToBase64String(hashBytes),
                    Salt = Convert.ToBase64String(saltBytes),
                    Estado = "ACTIVO",
                    FechaRegistro = DateTime.UtcNow,
                    FechaNacimiento = DateTime.UtcNow.AddYears(-25)
                });
            }
            await context.SaveChangesAsync();

            // Act
            var stopwatch = Stopwatch.StartNew();
            var resultado = await service.GetAllAsync();
            stopwatch.Stop();

            var listaUsuarios = resultado.ToList();

            // Assert
            _output.WriteLine($"\n=== RENDIMIENTO: CONSULTA DE USUARIOS ===");
            _output.WriteLine($"Usuarios consultados: {listaUsuarios.Count}");
            _output.WriteLine($"Tiempo de ejecución: {stopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"Umbral máximo: 2000ms");
            _output.WriteLine($"Estado: {(stopwatch.ElapsedMilliseconds < 2000 ? "✓ ACEPTABLE" : "✗ EXCEDIDO")}");
            _output.WriteLine($"GQM_PERFORMANCE_CONSULTA: {stopwatch.ElapsedMilliseconds}");

            Assert.True(listaUsuarios.Count > 0);
            Assert.True(stopwatch.ElapsedMilliseconds < 2000,
                $"Consulta tardó {stopwatch.ElapsedMilliseconds}ms (debe ser < 2000ms)");
        }

        [Fact]
        public async Task GQM_Objetivo4_PorcentajeTransacciones_95Porciento_Bajo2Segundos()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UsuarioRepository(context);
            var passwordHasher = new PasswordHasher();
            var service = new UsuarioService(repository, _mapper, passwordHasher);

            int totalTransacciones = 100;
            int transaccionesBajo2s = 0;
            var tiempos = new List<long>();

            _output.WriteLine($"\n=== PRUEBA DE TRANSACCIONES (Objetivo 4) ===");
            _output.WriteLine($"Total de transacciones: {totalTransacciones}");
            _output.WriteLine($"Umbral: 95% deben completarse en < 2000ms");

            // Act - Ejecutar 100 transacciones (operaciones de registro)
            for (int i = 0; i < totalTransacciones; i++)
            {
                var registroRequest = new UsuarioCreateRequest
                {
                    IdUsuario = $"USER{i:000}",
                    Nombre = $"Usuario {i}",
                    Apellido1 = "Test",
                    Email = $"usuario{i}@test.com",
                    Telefono = "88888888",
                    Rol = "Cliente",
                    Contrasena = "Password123!",
                    ConfirmarContrasena = "Password123!",
                    FechaNacimiento = DateTime.UtcNow.AddYears(-25)
                };

                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await service.RegistrarUsuarioAsync(registroRequest);
                    stopwatch.Stop();
                    
                    tiempos.Add(stopwatch.ElapsedMilliseconds);
                    
                    if (stopwatch.ElapsedMilliseconds < 2000)
                    {
                        transaccionesBajo2s++;
                    }

                    if (i % 10 == 0)
                    {
                        _output.WriteLine($"Transacción {i + 1}: {stopwatch.ElapsedMilliseconds}ms");
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _output.WriteLine($"⚠️ Transacción {i + 1} falló: {ex.Message}");
                }
            }

            // Calculate metrics
            var porcentajeExito = (transaccionesBajo2s * 100.0) / totalTransacciones;
            var tiempoPromedio = tiempos.Average();
            var tiempoMinimo = tiempos.Min();
            var tiempoMaximo = tiempos.Max();
            var p95 = tiempos.OrderBy(t => t).ElementAt((int)(tiempos.Count * 0.95));

            // Assert
            _output.WriteLine($"\n=== RESULTADOS DE RENDIMIENTO ===");
            _output.WriteLine($"Transacciones completadas: {totalTransacciones}");
            _output.WriteLine($"Transacciones < 2000ms: {transaccionesBajo2s}");
            _output.WriteLine($"Porcentaje de éxito: {porcentajeExito:F2}%");
            _output.WriteLine($"Tiempo promedio: {tiempoPromedio:F2}ms");
            _output.WriteLine($"Tiempo mínimo: {tiempoMinimo}ms");
            _output.WriteLine($"Tiempo máximo: {tiempoMaximo}ms");
            _output.WriteLine($"Percentil 95 (P95): {p95}ms");
            _output.WriteLine($"Estado: {(porcentajeExito >= 95 ? "✓ ACEPTABLE" : "✗ REQUIERE MEJORA")}");
            _output.WriteLine($"GQM_PERFORMANCE_SLA: {porcentajeExito:F2}");
            _output.WriteLine($"GQM_PERFORMANCE_P95: {p95}");

            Assert.True(porcentajeExito >= 95,
                $"Solo {porcentajeExito:F2}% de transacciones cumplieron el SLA (esperado >= 95%)");
        }

        [Fact]
        public async Task GQM_Objetivo4_OperacionesConcurrentes_25Usuarios()
        {
            // Arrange
            var databaseName = $"TestDb_Conc25_{Guid.NewGuid()}";
            int usuariosConcurrentes = 25;
            var tasks = new List<Task<(bool exito, long tiempo)>>();

            _output.WriteLine($"\n=== PRUEBA DE CONCURRENCIA ===");
            _output.WriteLine($"Usuarios concurrentes: {usuariosConcurrentes}");

            var stopwatchTotal = Stopwatch.StartNew();

            // Act - Simular operaciones concurrentes con contextos independientes
            var timestamp = Guid.NewGuid().ToString("N").Substring(0, 8);
            for (int i = 0; i < usuariosConcurrentes; i++)
            {
                var index = i;
                var task = Task.Run(async () =>
                {
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        // Crear contexto independiente para cada operación
                        var options = new DbContextOptionsBuilder<OracleDbContext>()
                            .UseInMemoryDatabase(databaseName: databaseName)
                            .Options;
                        var context = new OracleDbContext(options);
                        var repository = new UsuarioRepository(context);
                        var passwordHasher = new PasswordHasher();
                        var service = new UsuarioService(repository, _mapper, passwordHasher);

                        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
                        var registroRequest = new UsuarioCreateRequest
                        {
                            IdUsuario = $"CONC{timestamp}{index:00}{uniqueId.Substring(0,4)}",
                            Nombre = $"Usuario Concurrente {index}",
                            Apellido1 = "Test",
                            Email = $"concurrent{timestamp}{index}{uniqueId}@test.com",
                            Telefono = "88888888",
                            Rol = "Cliente",
                            Contrasena = "Password123!",
                            ConfirmarContrasena = "Password123!",
                            FechaNacimiento = DateTime.UtcNow.AddYears(-25)
                        };

                        await service.RegistrarUsuarioAsync(registroRequest);
                        sw.Stop();
                        return (true, sw.ElapsedMilliseconds);
                    }
                    catch
                    {
                        sw.Stop();
                        return (false, sw.ElapsedMilliseconds);
                    }
                });
                tasks.Add(task);
            }

            var resultados = await Task.WhenAll(tasks);
            stopwatchTotal.Stop();

            // Analyze results
            var exitosos = resultados.Count(r => r.exito);
            var tiempos = resultados.Select(r => r.tiempo).ToList();
            var tiempoPromedio = tiempos.Average();
            var tiempoMaximo = tiempos.Max();
            var tiempoMinimo = tiempos.Min();

            // Assert
            _output.WriteLine($"\n=== RESULTADOS DE CONCURRENCIA ===");
            _output.WriteLine($"Operaciones exitosas: {exitosos}/{usuariosConcurrentes}");
            _output.WriteLine($"Tiempo total: {stopwatchTotal.ElapsedMilliseconds}ms");
            _output.WriteLine($"Tiempo promedio por operación: {tiempoPromedio:F2}ms");
            _output.WriteLine($"Tiempo mínimo: {tiempoMinimo}ms");
            _output.WriteLine($"Tiempo máximo: {tiempoMaximo}ms");
            _output.WriteLine($"Throughput: {(usuariosConcurrentes * 1000.0 / stopwatchTotal.ElapsedMilliseconds):F2} ops/segundo");
            _output.WriteLine($"GQM_CONCURRENT_USERS: {usuariosConcurrentes}");
            _output.WriteLine($"GQM_CONCURRENT_SUCCESS: {exitosos}");

            Assert.True(exitosos >= usuariosConcurrentes * 0.9,
                $"Solo {exitosos} de {usuariosConcurrentes} operaciones fueron exitosas (mínimo 90%: {usuariosConcurrentes * 0.9})");
            Assert.True(tiempoPromedio < 2000,
                $"Tiempo promedio {tiempoPromedio:F2}ms excede los 2000ms");
        }

        [Fact]
        public async Task GQM_Objetivo4_OperacionesConcurrentes_50Usuarios()
        {
            // Arrange
            int usuariosConcurrentes = 50;
            var tasks = new List<Task<bool>>();
            var timestamp = Guid.NewGuid().ToString("N").Substring(0, 8);

            _output.WriteLine($"\n=== PRUEBA DE CONCURRENCIA (50 usuarios) ===");

            var stopwatch = Stopwatch.StartNew();

            // Act - Crear contextos independientes
            for (int i = 0; i < usuariosConcurrentes; i++)
            {
                int index = i;
                var task = Task.Run(async () =>
                {
                    try
                    {
                        // Contexto independiente por operación
                        var options = new DbContextOptionsBuilder<OracleDbContext>()
                            .UseInMemoryDatabase(databaseName: $"TestDb_Conc50_{timestamp}_{index}")
                            .Options;
                        var context = new OracleDbContext(options);
                        var repository = new UsuarioRepository(context);
                        var passwordHasher = new PasswordHasher();
                        var service = new UsuarioService(repository, _mapper, passwordHasher);

                        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 6);
                        var registroRequest = new UsuarioCreateRequest
                        {
                            IdUsuario = $"C50{timestamp}{index:00}{uniqueId}",
                            Nombre = $"Usuario {index}",
                            Apellido1 = "Test",
                            Email = $"conc50{timestamp}{index}{uniqueId}@test.com",
                            Telefono = "88888888",
                            Rol = "Cliente",
                            Contrasena = "Password123!",
                            ConfirmarContrasena = "Password123!",
                            FechaNacimiento = DateTime.UtcNow.AddYears(-25)
                        };

                        await service.RegistrarUsuarioAsync(registroRequest);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });
                tasks.Add(task);
            }

            var resultados = await Task.WhenAll(tasks);
            stopwatch.Stop();

            var exitosos = resultados.Count(r => r);

            // Assert
            _output.WriteLine($"Operaciones exitosas: {exitosos}/{usuariosConcurrentes}");
            _output.WriteLine($"Tiempo total: {stopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"GQM_CONCURRENT_50: {exitosos}");

            Assert.True(exitosos >= usuariosConcurrentes * 0.85,
                $"Muy pocas operaciones exitosas: {exitosos}/{usuariosConcurrentes} (mínimo 85%: {usuariosConcurrentes * 0.85})");
        }

        [Fact]
        public async Task GQM_Objetivo4_OperacionesConcurrentes_100Usuarios()
        {
            // Arrange
            int usuariosConcurrentes = 100;
            var tasks = new List<Task<bool>>();
            var timestamp = Guid.NewGuid().ToString("N").Substring(0, 8);

            _output.WriteLine($"\n=== PRUEBA DE CONCURRENCIA (100 usuarios - Umbral GQM) ===");

            var stopwatch = Stopwatch.StartNew();

            // Act - Crear contextos independientes
            for (int i = 0; i < usuariosConcurrentes; i++)
            {
                int index = i;
                var task = Task.Run(async () =>
                {
                    try
                    {
                        // Contexto independiente por operación
                        var options = new DbContextOptionsBuilder<OracleDbContext>()
                            .UseInMemoryDatabase(databaseName: $"TestDb_Conc100_{timestamp}_{index}")
                            .Options;
                        var context = new OracleDbContext(options);
                        var repository = new UsuarioRepository(context);
                        var passwordHasher = new PasswordHasher();
                        var service = new UsuarioService(repository, _mapper, passwordHasher);

                        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 6);
                        var registroRequest = new UsuarioCreateRequest
                        {
                            IdUsuario = $"C100{timestamp}{index:000}{uniqueId.Substring(0,2)}",
                            Nombre = $"Usuario {index}",
                            Apellido1 = "Test",
                            Email = $"conc100{timestamp}{index}{uniqueId}@test.com",
                            Telefono = "88888888",
                            Rol = "Cliente",
                            Contrasena = "Password123!",
                            ConfirmarContrasena = "Password123!",
                            FechaNacimiento = DateTime.UtcNow.AddYears(-25)
                        };

                        await service.RegistrarUsuarioAsync(registroRequest);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });
                tasks.Add(task);
            }

            var resultados = await Task.WhenAll(tasks);
            stopwatch.Stop();

            var exitosos = resultados.Count(r => r);
            var throughput = (usuariosConcurrentes * 1000.0) / stopwatch.ElapsedMilliseconds;

            // Assert
            _output.WriteLine($"\n=== RESULTADOS (Umbral: 100 usuarios concurrentes) ===");
            _output.WriteLine($"Operaciones exitosas: {exitosos}/{usuariosConcurrentes}");
            _output.WriteLine($"Tiempo total: {stopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"Throughput: {throughput:F2} ops/segundo");
            _output.WriteLine($"Estado: {(exitosos >= usuariosConcurrentes * 0.90 ? "✓ ACEPTABLE" : "✗ REQUIERE MEJORA")}");
            _output.WriteLine($"GQM_CONCURRENT_100: {exitosos}");
            _output.WriteLine($"GQM_THROUGHPUT: {throughput:F2}");

            Assert.True(exitosos >= usuariosConcurrentes * 0.80,
                $"El sistema debe soportar al menos 80% de éxito con 100 usuarios: {exitosos}/{usuariosConcurrentes}");
        }
    }
}
