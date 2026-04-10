using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EventosBackend.Controllers;
using EventosBackend.Services.Interfaces;
using EventosBackend.Models.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventosBackend.Tests.Controllers
{
    public class CitasControllerExtendedTests
    {
        private readonly Mock<ICitaService> _mockService;
        private readonly CitasController _controller;

        public CitasControllerExtendedTests()
        {
            _mockService = new Mock<ICitaService>();
            _controller = new CitasController(_mockService.Object);
        }

        [Fact]
        public async Task ObtenerDisponibilidadAgregada_ReturnsOk()
        {
            // Arrange
            var fechaDesde = System.DateTime.Today;
            var fechaHasta = System.DateTime.Today.AddDays(7);
            var disponibilidad = new List<DisponibilidadAgregadaResponse>
            {
                new DisponibilidadAgregadaResponse
                {
                    Fecha = System.DateTime.Today,
                    HoraInicio = "08:00",
                    TecnicosDisponibles = 2
                }
            };

            _mockService.Setup(s => s.ObtenerDisponibilidadAgregada(fechaDesde, fechaHasta))
                .ReturnsAsync(disponibilidad);

            // Act
            var result = await _controller.ObtenerDisponibilidadAgregada(fechaDesde, fechaHasta);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CompletarCita_WithValidId_ReturnsOk()
        {
            // Arrange
            var idCita = 1;
            _mockService.Setup(s => s.CompletarCita(idCita)).ReturnsAsync(true);

            // Act
            var result = await _controller.CompletarCita(idCita);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task BloquearHorario_WithValidData_ReturnsOk()
        {
            // Arrange
            var request = new EventosBackend.Models.DTOs.Requests.BloquearHorarioRequest
            {
                Fecha = System.DateTime.Today,
                HoraInicio = "10:00",
                MotivoBloqueo = "Reunión"
            };

            _mockService.Setup(s => s.BloquearHorario(It.IsAny<string>(), request)).ReturnsAsync(true);

            // Act
            var result = await _controller.BloquearHorario(request);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DesbloquearHorario_WithValidData_ReturnsOk()
        {
            // Arrange
            var request = new EventosBackend.Models.DTOs.Requests.BloquearHorarioRequest
            {
                Fecha = System.DateTime.Today,
                HoraInicio = "10:00"
            };

            _mockService.Setup(s => s.DesbloquearHorario(It.IsAny<string>(), It.IsAny<System.DateTime>(), It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.DesbloquearHorario(request);

            // Assert
            Assert.NotNull(result);
        }
    }
}
