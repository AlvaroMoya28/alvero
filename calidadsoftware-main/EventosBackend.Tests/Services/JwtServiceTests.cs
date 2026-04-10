using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using EventosBackend.Services;
using EventosBackend.Models.Configuration;
using EventosBackend.Models.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace EventosBackend.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly JwtService _service;
        private readonly JwtSettings _jwtSettings;

        public JwtServiceTests()
        {
            _jwtSettings = new JwtSettings
            {
                SecretKey = "SuperSecretKeyForTestingPurposes123456789",
                ValidIssuer = "TestIssuer",
                ValidAudience = "TestAudience",
                ExpiryInMinutes = 60
            };

            var mockOptions = new Mock<IOptions<JwtSettings>>();
            mockOptions.Setup(o => o.Value).Returns(_jwtSettings);

            _service = new JwtService(mockOptions.Object);
        }

        [Fact]
        public void GenerateToken_CreatesValidToken()
        {
            // Arrange
            var usuario = new Usuario
            {
                IdUnico = "USER123",
                IdUsuario = "testuser",
                Email = "test@example.com",
                Nombre = "Test",
                Apellido1 = "User",
                Apellido2 = "Apellido",
                Telefono = "1234567890",
                TipoUsuario = "CLIENTE",
                Estado = "ACTIVO"
            };

            // Act
            var token = _service.GenerateToken(usuario);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Validar estructura del token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal(_jwtSettings.ValidIssuer, jwtToken.Issuer);
            Assert.Equal(_jwtSettings.ValidAudience, jwtToken.Audiences.First());
        }

        [Fact]
        public void GenerateToken_ContainsCorrectClaims()
        {
            // Arrange
            var usuario = new Usuario
            {
                IdUnico = "USER123",
                IdUsuario = "testuser",
                Email = "test@example.com",
                Nombre = "Test",
                Apellido1 = "User",
                Apellido2 = "Apellido",
                Telefono = "1234567890",
                TipoUsuario = "TECNICO",
                Estado = "ACTIVO"
            };

            // Act
            var token = _service.GenerateToken(usuario);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Contains(jwtToken.Claims, c => c.Type == "idUsuario" && c.Value == usuario.IdUsuario);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Email && c.Value == usuario.Email);
            Assert.Contains(jwtToken.Claims, c => c.Type == "nombre" && c.Value == usuario.Nombre);
            Assert.Contains(jwtToken.Claims, c => c.Value == "TECNICO");
        }

        [Fact]
        public void GenerateToken_HasCorrectExpiration()
        {
            // Arrange
            var usuario = new Usuario
            {
                IdUnico = "USER123",
                IdUsuario = "testuser",
                Email = "test@example.com",
                Nombre = "Test",
                Apellido1 = "User",
                TipoUsuario = "CLIENTE",
                Estado = "ACTIVO"
            };

            // Act
            var token = _service.GenerateToken(usuario);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var expectedExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);
            var actualExpiry = jwtToken.ValidTo;

            // Verificar que la expiración está dentro de un rango de 1 minuto
            Assert.True(Math.Abs((expectedExpiry - actualExpiry).TotalMinutes) < 1);
        }

        [Fact]
        public void GenerateToken_WithNullApellido2_HandlesGracefully()
        {
            // Arrange
            var usuario = new Usuario
            {
                IdUnico = "USER123",
                IdUsuario = "testuser",
                Email = "test@example.com",
                Nombre = "Test",
                Apellido1 = "User",
                Apellido2 = null, // Nullable
                TipoUsuario = "CLIENTE",
                Estado = "ACTIVO"
            };

            // Act
            var token = _service.GenerateToken(usuario);

            // Assert
            Assert.NotNull(token);
            
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var apellido2Claim = jwtToken.Claims.FirstOrDefault(c => c.Type == "apellido2");
            
            Assert.NotNull(apellido2Claim);
            Assert.Equal(string.Empty, apellido2Claim.Value);
        }
    }
}
