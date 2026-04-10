using Xunit;
using EventosBackend.Models.Entities;
using System;

namespace EventosBackend.Tests.Models
{
    public class UsuarioEntityTests
    {
        [Fact]
        public void Usuario_DefaultConstructor_SetsDefaults()
        {
            // Act
            var usuario = new Usuario();

            // Assert
            Assert.NotEqual(DateTime.MinValue, usuario.FechaRegistro);
            Assert.Equal("ACTIVO", usuario.Estado);
        }

        [Fact]
        public void Usuario_AllProperties_CanBeSet()
        {
            // Arrange
            var usuario = new Usuario
            {
                IdUnico = "USER001",
                IdUsuario = "test user",
                Nombre = "Juan",
                Apellido1 = "Pérez",
                Apellido2 = "García",
                Email = "juan@test.com",
                Telefono = "12345678",
                TipoUsuario = "CLIENTE",
                ContrasenaHash = "hashedpass",
                Salt = "saltyvalue",
                FechaNacimiento = new DateTime(1990, 1, 1),
                UltimoLogin = DateTime.UtcNow,
                Estado = "ACTIVO",
                StripeCustomerId = "stripe_123"
            };

            // Assert
            Assert.Equal("USER001", usuario.IdUnico);
            Assert.Equal("test user", usuario.IdUsuario);
            Assert.Equal("Juan", usuario.Nombre);
            Assert.Equal("Pérez", usuario.Apellido1);
            Assert.Equal("García", usuario.Apellido2);
            Assert.Equal("juan@test.com", usuario.Email);
            Assert.Equal("12345678", usuario.Telefono);
            Assert.Equal("CLIENTE", usuario.TipoUsuario);
            Assert.Equal("hashedpass", usuario.ContrasenaHash);
            Assert.Equal("saltyvalue", usuario.Salt);
            Assert.NotNull(usuario.UltimoLogin);
            Assert.Equal("stripe_123", usuario.StripeCustomerId);
        }

        [Fact]
        public void Usuario_NullableProperties_CanBeNull()
        {
            // Arrange & Act
            var usuario = new Usuario
            {
                IdUsuario = "test",
                Nombre = "Test",
                Apellido1 = "User",
                Email = "test@test.com",
                TipoUsuario = "CLIENTE",
                ContrasenaHash = "hash",
                Salt = "salt",
                FechaNacimiento = DateTime.Today,
                Apellido2 = null,
                Telefono = null,
                UltimoLogin = null,
                StripeCustomerId = null
            };

            // Assert
            Assert.Null(usuario.Apellido2);
            Assert.Null(usuario.Telefono);
            Assert.Null(usuario.UltimoLogin);
            Assert.Null(usuario.StripeCustomerId);
        }
    }
}
