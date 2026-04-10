using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using EventosBackend.Models.Entities;
using EventosBackend.Repositories.Interfaces;
using EventosBackend.Services;
using EventosBackend.Services.Interfaces;
using Moq;
using Xunit;

namespace EventosBackend.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _mockRepository = new Mock<IUsuarioRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _service = new UsuarioService(_mockRepository.Object, _mockMapper.Object, _mockPasswordHasher.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsuarios()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Usuario { IdUsuario = "1", Nombre = "User 1" },
                new Usuario { IdUsuario = "2", Nombre = "User 2" }
            };
            var usuariosResponse = usuarios.Select(u => new UsuarioResponse { IdUsuario = u.IdUsuario, Nombre = u.Nombre });

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(usuarios);
            _mockMapper.Setup(m => m.Map<IEnumerable<UsuarioResponse>>(usuarios)).Returns(usuariosResponse);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTecnicosAsync_ReturnsTecnicos()
        {
            // Arrange
            var tecnicos = new List<Usuario>
            {
                new Usuario { IdUsuario = "1", Nombre = "Tech 1", TipoUsuario = "tecnico" },
                new Usuario { IdUsuario = "2", Nombre = "Tech 2", TipoUsuario = "tecnico" }
            };
            var tecnicosResponse = tecnicos.Select(t => new UsuarioResponse { IdUsuario = t.IdUsuario, Nombre = t.Nombre });

            _mockRepository.Setup(r => r.GetTecnicosAsync()).ReturnsAsync(tecnicos);
            _mockMapper.Setup(m => m.Map<IEnumerable<UsuarioResponse>>(tecnicos)).Returns(tecnicosResponse);

            // Act
            var result = await _service.GetTecnicosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetTecnicosAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsUsuario()
        {
            // Arrange
            var usuario = new Usuario { IdUsuario = "1", Nombre = "Test User", Email = "test@test.com" };
            var usuarioResponse = new UsuarioResponse { IdUsuario = "1", Nombre = "Test User", Email = "test@test.com" };

            _mockRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(usuario);
            _mockMapper.Setup(m => m.Map<UsuarioResponse>(usuario)).Returns(usuarioResponse);

            // Act
            var result = await _service.GetByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.IdUsuario);
            Assert.Equal("Test User", result.Nombre);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenUsuarioNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Usuario)null);
            _mockMapper.Setup(m => m.Map<UsuarioResponse>(null)).Returns((UsuarioResponse)null);

            // Act
            var result = await _service.GetByIdAsync("999");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task EmailExisteAsync_ReturnsTrue_WhenEmailExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExisteEmailAsync("existing@test.com")).ReturnsAsync(true);

            // Act
            var result = await _service.EmailExisteAsync("existing@test.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task EmailExisteAsync_ReturnsFalse_WhenEmailDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExisteEmailAsync("new@test.com")).ReturnsAsync(false);

            // Act
            var result = await _service.EmailExisteAsync("new@test.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAuthenticatedUserAsync_ReturnsUser_WhenClaimsPrincipalValid()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var usuario = new Usuario { IdUsuario = "1", Email = "test@test.com", Nombre = "Test User" };
            var usuarioResponse = new UsuarioResponse { IdUsuario = "1", Email = "test@test.com", Nombre = "Test User" };

            _mockRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(usuario);
            _mockMapper.Setup(m => m.Map<UsuarioResponse>(usuario)).Returns(usuarioResponse);

            // Act
            var result = await _service.GetAuthenticatedUserAsync(claimsPrincipal);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.IdUsuario);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsUsuario_WhenExists()
        {
            // Arrange
            var usuario = new Usuario { IdUsuario = "1", Email = "test@test.com", Nombre = "Test User" };
            var usuarioResponse = new UsuarioResponse { IdUsuario = "1", Email = "test@test.com", Nombre = "Test User" };

            _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(usuario);
            _mockMapper.Setup(m => m.Map<UsuarioResponse>(usuario)).Returns(usuarioResponse);

            // Act
            var result = await _service.GetByEmailAsync("test@test.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@test.com", result.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByEmailAsync("nonexistent@test.com")).ReturnsAsync((Usuario)null);
            _mockMapper.Setup(m => m.Map<UsuarioResponse>(null)).Returns((UsuarioResponse)null);

            // Act
            var result = await _service.GetByEmailAsync("nonexistent@test.com");

            // Assert
            Assert.Null(result);
        }
    }
}
