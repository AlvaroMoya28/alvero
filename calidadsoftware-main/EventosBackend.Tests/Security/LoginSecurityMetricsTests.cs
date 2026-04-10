using System;
using System.Collections.Generic;
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

namespace EventosBackend.Tests.Security
{
    /// <summary>
    /// Pruebas de seguridad para Objetivo 3 GQM: Bloqueo de Accesos No Autorizados
    /// Umbral: >= 95% de intentos inválidos deben ser bloqueados
    /// </summary>
    public class LoginSecurityMetricsTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IMapper _mapper;

        public LoginSecurityMetricsTests(ITestOutputHelper output)
        {
            _output = output;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UsuarioProfile>();
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
        public async Task GQM_Objetivo3_BloqueoAccesos_95PorCiento_IntentosInvalidos()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UsuarioRepository(context);
            var passwordHasher = new PasswordHasher();
            var service = new UsuarioService(repository, _mapper, passwordHasher);

            // Crear usuario de prueba
            var (hashBytes, saltBytes) = passwordHasher.CreateHash("ValidPassword123!");
            var usuario = new Usuario
            {
                IdUnico = Guid.NewGuid().ToString(),
                IdUsuario = "USR001",
                Nombre = "Usuario",
                Apellido1 = "Prueba",
                Email = "usuario@test.com",
                TipoUsuario = "Cliente",
                ContrasenaHash = Convert.ToBase64String(hashBytes),
                Salt = Convert.ToBase64String(saltBytes),
                Estado = "ACTIVO",
                FechaRegistro = DateTime.UtcNow,
                FechaNacimiento = DateTime.UtcNow.AddYears(-25)
            };
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            int totalIntentos = 100;
            int intentosInvalidos = 95;
            int intentosValidos = 5;
            int intentosBloqueados = 0;
            int intentosExitosos = 0;

            // Act - Simular 95 intentos con credenciales incorrectas
            _output.WriteLine($"\n=== INICIANDO PRUEBA DE SEGURIDAD ===");
            _output.WriteLine($"Total de intentos: {totalIntentos}");
            _output.WriteLine($"Intentos inválidos: {intentosInvalidos}");
            _output.WriteLine($"Intentos válidos: {intentosValidos}");
            _output.WriteLine($"\n--- Fase 1: Intentos con credenciales incorrectas ---");

            for (int i = 0; i < intentosInvalidos; i++)
            {
                try
                {
                    var loginRequest = new LoginRequest
                    {
                        Id = "USR001",
                        Password = $"InvalidPassword{i}"
                    };

                    var resultado = await service.LoginAsync(loginRequest);
                    
                    // Si no lanzó excepción, el intento no fue bloqueado
                    if (resultado != null)
                    {
                        _output.WriteLine($"⚠️ Intento {i + 1}: NO BLOQUEADO (FALLO DE SEGURIDAD)");
                    }
                }
                catch (ApplicationException ex)
                {
                    // Excepción esperada = intento bloqueado correctamente
                    intentosBloqueados++;
                    if (i % 10 == 0)
                    {
                        _output.WriteLine($"✓ Intento {i + 1}: Bloqueado - {ex.Message}");
                    }
                }
            }

            _output.WriteLine($"\n--- Fase 2: Intentos con credenciales válidas ---");

            // Intentos con credenciales correctas (deben ser aceptados)
            for (int i = 0; i < intentosValidos; i++)
            {
                try
                {
                    var loginRequest = new LoginRequest
                    {
                        Id = "USR001",
                        Password = "ValidPassword123!"
                    };

                    var resultado = await service.LoginAsync(loginRequest);
                    
                    if (resultado != null)
                    {
                        intentosExitosos++;
                        _output.WriteLine($"✓ Intento válido {i + 1}: Aceptado correctamente");
                    }
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"⚠️ Intento válido {i + 1}: Rechazado incorrectamente - {ex.Message}");
                }
            }

            // Calculate metrics
            var porcentajeBloqueados = (intentosBloqueados * 100.0) / intentosInvalidos;
            var porcentajeExitosos = (intentosExitosos * 100.0) / intentosValidos;

            // Output para script GQM
            _output.WriteLine($"\n=== RESULTADOS DE SEGURIDAD ===");
            _output.WriteLine($"Intentos inválidos bloqueados: {intentosBloqueados}/{intentosInvalidos}");
            _output.WriteLine($"Porcentaje de bloqueo: {porcentajeBloqueados:F2}%");
            _output.WriteLine($"Intentos válidos aceptados: {intentosExitosos}/{intentosValidos}");
            _output.WriteLine($"Porcentaje de aceptación: {porcentajeExitosos:F2}%");
            _output.WriteLine($"Umbral requerido: >= 95%");
            _output.WriteLine($"Estado: {(porcentajeBloqueados >= 95 ? "✓ ACEPTABLE" : "✗ REQUIERE MEJORA")}");
            _output.WriteLine($"\nGQM_SECURITY_METRIC: {porcentajeBloqueados:F2}");

            // Assert
            Assert.True(porcentajeBloqueados >= 95,
                $"Solo {porcentajeBloqueados:F2}% de intentos inválidos fueron bloqueados (esperado >= 95%)");
            
            Assert.True(porcentajeExitosos >= 80,
                $"Solo {porcentajeExitosos:F2}% de intentos válidos fueron aceptados (esperado >= 80%)");
        }

        [Fact]
        public async Task GQM_Seguridad_BloqueoUsuarioInexistente()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UsuarioRepository(context);
            var passwordHasher = new PasswordHasher();
            var service = new UsuarioService(repository, _mapper, passwordHasher);

            int intentos = 50;
            int bloqueados = 0;

            // Act - Intentar login con usuarios que no existen
            for (int i = 0; i < intentos; i++)
            {
                try
                {
                    var loginRequest = new LoginRequest
                    {
                        Id = $"USUARIO_INEXISTENTE_{i}",
                        Password = "AlgunaPassword123!"
                    };

                    await service.LoginAsync(loginRequest);
                }
                catch (ApplicationException)
                {
                    bloqueados++;
                }
            }

            var porcentaje = (bloqueados * 100.0) / intentos;

            _output.WriteLine($"\n=== BLOQUEO DE USUARIOS INEXISTENTES ===");
            _output.WriteLine($"Intentos: {intentos}");
            _output.WriteLine($"Bloqueados: {bloqueados}");
            _output.WriteLine($"Porcentaje: {porcentaje:F2}%");

            // Assert - Todos los intentos con usuarios inexistentes deben ser bloqueados
            Assert.Equal(100, porcentaje);
        }

        [Fact]
        public async Task GQM_Seguridad_BloqueoUsuarioInactivo()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UsuarioRepository(context);
            var passwordHasher = new PasswordHasher();
            var service = new UsuarioService(repository, _mapper, passwordHasher);

            var (hashBytes, saltBytes) = passwordHasher.CreateHash("Password123!");
            var usuarioInactivo = new Usuario
            {
                IdUnico = Guid.NewGuid().ToString(),
                IdUsuario = "USR_INACTIVO",
                Nombre = "Usuario",
                Apellido1 = "Inactivo",
                Email = "inactivo@test.com",
                TipoUsuario = "Cliente",
                ContrasenaHash = Convert.ToBase64String(hashBytes),
                Salt = Convert.ToBase64String(saltBytes),
                Estado = "INACTIVO", // Usuario inactivo
                FechaRegistro = DateTime.UtcNow,
                FechaNacimiento = DateTime.UtcNow.AddYears(-25)
            };
            context.Usuarios.Add(usuarioInactivo);
            await context.SaveChangesAsync();

            // Act
            bool bloqueado = false;
            try
            {
                var loginRequest = new LoginRequest
                {
                    Id = "USR_INACTIVO",
                    Password = "Password123!"
                };

                await service.LoginAsync(loginRequest);
            }
            catch (Exception)
            {
                bloqueado = true;
            }

            _output.WriteLine($"\n=== BLOQUEO DE USUARIO INACTIVO ===");
            _output.WriteLine($"Usuario inactivo intentó login: {(bloqueado ? "✓ Bloqueado" : "✗ No bloqueado")}");

            // Assert - Usuarios inactivos deberían ser bloqueados
            // Nota: Si el sistema actual no valida estado, este test fallará y revelará una vulnerabilidad
            Assert.True(bloqueado, "Los usuarios inactivos deben ser bloqueados del sistema");
        }

        [Fact]
        public async Task GQM_Seguridad_VariacionesDePassword()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UsuarioRepository(context);
            var passwordHasher = new PasswordHasher();
            var service = new UsuarioService(repository, _mapper, passwordHasher);

            var (hashBytes, saltBytes) = passwordHasher.CreateHash("Password123!");
            var usuario = new Usuario
            {
                IdUnico = Guid.NewGuid().ToString(),
                IdUsuario = "USR002",
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

            // Act - Probar variaciones de la contraseña correcta
            var variaciones = new[]
            {
                "password123!",     // Minúsculas
                "PASSWORD123!",     // Mayúsculas
                "Password123",      // Sin símbolo
                "Password123! ",    // Con espacio
                " Password123!",    // Espacio al inicio
                "Password1234!",    // Número diferente
            };

            int bloqueados = 0;
            foreach (var variacion in variaciones)
            {
                try
                {
                    var loginRequest = new LoginRequest
                    {
                        Id = "USR002",
                        Password = variacion
                    };

                    await service.LoginAsync(loginRequest);
                }
                catch (ApplicationException)
                {
                    bloqueados++;
                }
            }

            var porcentaje = (bloqueados * 100.0) / variaciones.Length;

            _output.WriteLine($"\n=== VARIACIONES DE CONTRASEÑA ===");
            _output.WriteLine($"Variaciones probadas: {variaciones.Length}");
            _output.WriteLine($"Bloqueadas: {bloqueados}");
            _output.WriteLine($"Porcentaje: {porcentaje:F2}%");

            // Assert - Todas las variaciones deben ser bloqueadas
            Assert.Equal(100, porcentaje);
        }

        [Fact]
        public async Task GQM_Seguridad_CifradoDeContrasenas()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UsuarioRepository(context);
            var passwordHasher = new PasswordHasher();

            var passwordOriginal = "MiPasswordSegura123!";
            var (hashBytes, saltBytes) = passwordHasher.CreateHash(passwordOriginal);

            var usuario = new Usuario
            {
                IdUnico = Guid.NewGuid().ToString(),
                IdUsuario = "USR003",
                Nombre = "Usuario",
                Apellido1 = "Cifrado",
                Email = "cifrado@test.com",
                TipoUsuario = "Cliente",
                ContrasenaHash = Convert.ToBase64String(hashBytes),
                Salt = Convert.ToBase64String(saltBytes),
                Estado = "ACTIVO",
                FechaRegistro = DateTime.UtcNow,
                FechaNacimiento = DateTime.UtcNow.AddYears(-25)
            };
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            // Act - Verificar que la contraseña nunca se almacena en texto plano
            var usuarioDb = await context.Usuarios.FindAsync(usuario.IdUsuario);
            
            // Verificar que el usuario fue guardado correctamente
            Assert.NotNull(usuarioDb);

            _output.WriteLine($"\n=== VERIFICACIÓN DE CIFRADO ===");
            _output.WriteLine($"Contraseña original: {passwordOriginal}");
            _output.WriteLine($"Hash almacenado: {usuarioDb.ContrasenaHash.Substring(0, Math.Min(20, usuarioDb.ContrasenaHash.Length))}...");
            _output.WriteLine($"Salt almacenado: {usuarioDb.Salt.Substring(0, Math.Min(20, usuarioDb.Salt.Length))}...");

            // Assert
            Assert.NotEqual(passwordOriginal, usuarioDb.ContrasenaHash);
            Assert.NotNull(usuarioDb.Salt);
            Assert.NotEmpty(usuarioDb.Salt);
            Assert.True(usuarioDb.ContrasenaHash.Length > 20, "Hash debe ser suficientemente largo");
            Assert.True(usuarioDb.Salt.Length > 10, "Salt debe ser suficientemente largo");

            _output.WriteLine($"✓ Contraseña almacenada de forma segura (cifrada)");
            _output.WriteLine($"GQM_ENCRYPTION_METRIC: 100");
        }
    }
}
