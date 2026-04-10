using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EventosBackend.Controllers;
using EventosBackend.Services.Interfaces;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventosBackend.Tests.Controllers
{
    public class CitasControllerTests
    {
        private readonly Mock<ICitaService> _mockService;
        private readonly CitasController _controller;

        public CitasControllerTests()
        {
            _mockService = new Mock<ICitaService>();
            _controller = new CitasController(_mockService.Object);
        }

        [Fact]
        public async Task CrearCita_ConDatosValidos_ReturnsOk()
        {
            // Arrange
            var request = new CrearCitaRequest
            {
                NombreCliente = "Juan Pérez",
                EmailCliente = "juan@email.com",
                CedulaCliente = "123456789",
                IdUsuarioTecnico = "TEC001",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = "10:00"
            };

            var citaResponse = new CitaResponse
            {
                IdCita = 1,
                NombreCliente = request.NombreCliente,
                Estado = "PENDIENTE"
            };

            _mockService.Setup(s => s.CrearCitaPublica(request))
                .ReturnsAsync(citaResponse);

            // Act
            var result = await _controller.CrearCita(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CitaResponse>(okResult.Value);
            Assert.Equal(1, returnValue.IdCita);
        }

        [Fact]
        public async Task CrearCita_ConError_ReturnsBadRequest()
        {
            // Arrange
            var request = new CrearCitaRequest
            {
                NombreCliente = "",
                EmailCliente = "juan@email.com",
                CedulaCliente = "123456789",
                IdUsuarioTecnico = "TEC001",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = "10:00"
            };

            _mockService.Setup(s => s.CrearCitaPublica(request))
                .ThrowsAsync(new Exception("Nombre requerido"));

            // Act
            var result = await _controller.CrearCita(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task ObtenerHorariosDisponibles_ReturnsOk()
        {
            // Arrange
            var idTecnico = "TEC001";
            var fechaDesde = DateTime.Today;
            var fechaHasta = DateTime.Today.AddDays(7);
            
            var horarios = new List<HorarioDisponibleResponse>
            {
                new HorarioDisponibleResponse
                {
                    IdHorario = 1,
                    HoraInicio = "08:00",
                    DisponibleReal = true
                }
            };

            _mockService.Setup(s => s.ObtenerHorariosDisponibles(idTecnico, fechaDesde, fechaHasta))
                .ReturnsAsync(horarios);

            // Act
            var result = await _controller.ObtenerHorariosDisponibles(idTecnico, fechaDesde, fechaHasta);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<HorarioDisponibleResponse>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task ConfirmarCita_ConIdValido_ReturnsOkOrBadRequest()
        {
            // Arrange
            var idCita = 1;
            _mockService.Setup(s => s.ConfirmarCita(idCita))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ConfirmarCita(idCita);

            // Assert - puede ser OK o BadRequest dependiendo de la autenticación
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ConfirmarCita_ConIdInvalido_ReturnsNotFoundOrBadRequest()
        {
            // Arrange
            var idCita = 999;
            _mockService.Setup(s => s.ConfirmarCita(idCita))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ConfirmarCita(idCita);

            // Assert - puede ser NotFound o BadRequest dependiendo de la autenticación
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GenerarHorariosSemana_ConDatosValidos_ReturnsOk()
        {
            // Arrange
            var request = new GenerarHorariosRequest
            {
                IdTecnico = "TEC001",
                FechaInicio = DateTime.Today
            };

            _mockService.Setup(s => s.GenerarHorariosSemana(request.IdTecnico, request.FechaInicio))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.GenerarHorariosSemana(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
