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
    public class UsuariosControllerExtendedTests
    {
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IUsuarioRepository> _mockRepository;
        private readonly UsuariosController _controller;

        public UsuariosControllerExtendedTests()
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
        public async Task GetUsuarioPorId_WithValidId_ReturnsUsuario()
        {
            // Arrange
            var id = "USER1";
            var usuarioResponse = new UsuarioResponse
            {
                IdUnico = id,
                Nombre = "Test",
                Email = "test@test.com"
            };

            _mockUsuarioService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(usuarioResponse);

            // Act
            var result = await _controller.GetUsuarioPorId(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetUsuarioPorId_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = "INVALID";
            _mockUsuarioService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((UsuarioResponse)null);

            // Act
            var result = await _controller.GetUsuarioPorId(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ActualizarUsuario_WithValidData_ReturnsOk()
        {
            // Arrange
            var id = "USER1";
            var request = new UsuarioUpdateRequest
            {
                Nombre = "UpdatedName",
                Email = "updated@test.com"
            };
            var usuarioResponse = new UsuarioResponse { IdUnico = id };

            _mockUsuarioService.Setup(s => s.ActualizarUsuarioAsync(id, request))
                .ReturnsAsync(usuarioResponse);

            // Act
            var result = await _controller.ActualizarUsuario(id, request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.NotNull(objectResult.Value);
        }

        [Fact]
        public async Task ActualizarUsuario_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = "INVALID";
            var request = new UsuarioUpdateRequest { Nombre = "Test" };

            _mockUsuarioService.Setup(s => s.ActualizarUsuarioAsync(id, request))
                .ReturnsAsync((UsuarioResponse)null);

            // Act
            var result = await _controller.ActualizarUsuario(id, request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task EliminarUsuario_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var id = "USER1";
            _mockUsuarioService.Setup(s => s.EliminarUsuarioAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.EliminarUsuario(id);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task EliminarUsuario_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = "INVALID";
            _mockUsuarioService.Setup(s => s.EliminarUsuarioAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.EliminarUsuario(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
