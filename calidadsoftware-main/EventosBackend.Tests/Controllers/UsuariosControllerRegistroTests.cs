using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EventosBackend.Controllers;
using EventosBackend.Services.Interfaces;
using EventosBackend.Repositories.Interfaces;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using EventosBackend.Models.Entities;
using System;
using System.Threading.Tasks;

namespace EventosBackend.Tests.Controllers
{
    public class UsuariosControllerRegistroTests
    {
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IUsuarioRepository> _mockRepository;
        private readonly UsuariosController _controller;

        public UsuariosControllerRegistroTests()
        {
            _mockUsuarioService = new Mock<IUsuarioService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockJwtService = new Mock<IJwtService>();
            _mockRepository = new Mock<IUsuarioRepository>();
            _controller = new UsuariosController(
                _mockUsuarioService.Object,
                _mockEmailService.Object,
                _mockJwtService.Object,
                _mockRepository.Object
            );
        }

        [Fact]
        public async Task RegistrarUsuario_ConClienteYDatosValidos_ReturnsCreated()
        {
            // Arrange
            var request = new UsuarioCreateRequest
            {
                IdUsuario = "CLIENT001",
                Nombre = "Juan",
                Apellido1 = "Pérez",
                Apellido2 = "García",
                Email = "juan@test.com",
                Telefono = "12345678",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Contrasena = "Password123!",
                ConfirmarContrasena = "Password123!",
                Rol = "CLIENTE"
            };

            var usuarioResponse = new UsuarioResponse
            {
                IdUnico = "USER1",
                IdUsuario = "CLIENT001",
                Nombre = "Juan",
                Email = "juan@test.com"
            };

            _mockUsuarioService.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<UsuarioCreateRequest>()))
                .ReturnsAsync(usuarioResponse);

            // Act
            var result = await _controller.RegistrarUsuario(request);

            // Assert
            var createdResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task RegistrarUsuario_ConTecnicoYPasswordPatron_ReturnsCreated()
        {
            // Arrange
            var request = new UsuarioCreateRequest
            {
                IdUsuario = "TEC001",
                Nombre = "María",
                Apellido1 = "López",
                Apellido2 = "Sánchez",
                Email = "maria@test.com",
                Telefono = "87654321",
                FechaNacimiento = new DateTime(1985, 5, 15),
                Contrasena = "sanchez1985",
                ConfirmarContrasena = "sanchez1985",
                Rol = "TECNICO"
            };

            var usuarioResponse = new UsuarioResponse
            {
                IdUnico = "USER2",
                IdUsuario = "TEC001",
                Nombre = "María",
                Email = "maria@test.com"
            };

            _mockUsuarioService.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<UsuarioCreateRequest>()))
                .ReturnsAsync(usuarioResponse);

            // Act
            var result = await _controller.RegistrarUsuario(request);

            // Assert
            var createdResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task RegistrarUsuario_EmailDuplicado_ReturnsBadRequest()
        {
            // Arrange
            var request = new UsuarioCreateRequest
            {
                IdUsuario = "CLIENT002",
                Nombre = "Pedro",
                Apellido1 = "Martínez",
                Email = "pedro@test.com",
                Contrasena = "Password123!",
                ConfirmarContrasena = "Password123!",
                Rol = "CLIENTE"
            };

            _mockUsuarioService.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<UsuarioCreateRequest>()))
                .ThrowsAsync(new InvalidOperationException("El email ya está registrado"));

            // Act
            var result = await _controller.RegistrarUsuario(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task RegistrarUsuario_ConAdministrador_ReturnsCreated()
        {
            // Arrange
            var request = new UsuarioCreateRequest
            {
                IdUsuario = "ADMIN001",
                Nombre = "Admin",
                Apellido1 = "System",
                Email = "admin@test.com",
                Contrasena = "AdminPass123!",
                ConfirmarContrasena = "AdminPass123!",
                Rol = "ADMINISTRADOR"
            };

            var usuarioResponse = new UsuarioResponse
            {
                IdUnico = "USER3",
                IdUsuario = "ADMIN001",
                Nombre = "Admin",
                Email = "admin@test.com"
            };

            _mockUsuarioService.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<UsuarioCreateRequest>()))
                .ReturnsAsync(usuarioResponse);

            // Act
            var result = await _controller.RegistrarUsuario(request);

            // Assert
            var createdResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task RegistrarUsuario_ConSuperusuario_ReturnsCreated()
        {
            // Arrange
            var request = new UsuarioCreateRequest
            {
                IdUsuario = "SUPER001",
                Nombre = "Super",
                Apellido1 = "User",
                Email = "super@test.com",
                Contrasena = "SuperPass123!",
                ConfirmarContrasena = "SuperPass123!",
                Rol = "SUPERUSUARIO"
            };

            var usuarioResponse = new UsuarioResponse
            {
                IdUnico = "USER4",
                IdUsuario = "SUPER001",
                Nombre = "Super",
                Email = "super@test.com"
            };

            _mockUsuarioService.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<UsuarioCreateRequest>()))
                .ReturnsAsync(usuarioResponse);

            // Act
            var result = await _controller.RegistrarUsuario(request);

            // Assert
            var createdResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task RegistrarUsuario_ExceptionGeneral_ReturnsInternalServerError()
        {
            // Arrange
            var request = new UsuarioCreateRequest
            {
                IdUsuario = "CLIENT003",
                Nombre = "Error",
                Apellido1 = "Test",
                Email = "error@test.com",
                Contrasena = "Password123!",
                ConfirmarContrasena = "Password123!",
                Rol = "CLIENTE"
            };

            _mockUsuarioService.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<UsuarioCreateRequest>()))
                .ThrowsAsync(new Exception("Error de base de datos"));

            // Act
            var result = await _controller.RegistrarUsuario(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
