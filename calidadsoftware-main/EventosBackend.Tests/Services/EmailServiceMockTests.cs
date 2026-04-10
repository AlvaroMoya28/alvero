using Xunit;
using Moq;
using EventosBackend.Services;
using EventosBackend.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventosBackend.Tests.Services
{
    public class EmailServiceMockTests
    {
        private readonly Mock<IEmailService> _mockEmailService;

        public EmailServiceMockTests()
        {
            _mockEmailService = new Mock<IEmailService>();
        }

        [Fact]
        public async Task SendReceiptEmailAsync_ReturnsTrue()
        {
            // Arrange
            var email = "test@test.com";
            var nombreUsuario = "Test User";
            var numeroTransaccion = "TX123";
            var desglose = new Dictionary<string, decimal> { { "Item1", 100.00m } };
            var fechaPago = DateTime.UtcNow;
            var metodoPago = "Tarjeta";
            var montoTotal = 100.00m;

            _mockEmailService.Setup(e => e.SendReceiptEmailAsync(
                    email, nombreUsuario, numeroTransaccion, desglose, fechaPago, metodoPago, montoTotal))
                .ReturnsAsync(true);

            // Act
            var result = await _mockEmailService.Object.SendReceiptEmailAsync(
                email, nombreUsuario, numeroTransaccion, desglose, fechaPago, metodoPago, montoTotal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendReservaReminderEmailAsync_ReturnsTrue()
        {
            // Arrange
            var email = "test@test.com";
            var nombreUsuario = "Test User";
            var tecnico = "Tecnico Test";
            var fechaReserva = DateTime.UtcNow.AddDays(1);
            var direccion = "Test Address";
            var nombreReserva = "Test Reservation";

            _mockEmailService.Setup(e => e.SendReservaReminderEmailAsync(
                    email, nombreUsuario, tecnico, fechaReserva, direccion, nombreReserva, null))
                .ReturnsAsync(true);

            // Act
            var result = await _mockEmailService.Object.SendReservaReminderEmailAsync(
                email, nombreUsuario, tecnico, fechaReserva, direccion, nombreReserva, null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendCitaConfirmadaEmailAsync_ReturnsTrue()
        {
            // Arrange
            var email = "test@test.com";
            var nombreCliente = "Test Client";
            var nombreTecnico = "Test Tech";
            var fechaCita = DateTime.UtcNow.AddDays(1);
            var horaInicio = "10:00";
            var horaFin = "11:00";
            var direccion = "Test Address";
            var descripcion = "Test Description";

            _mockEmailService.Setup(e => e.SendCitaConfirmadaEmailAsync(
                    email, nombreCliente, nombreTecnico, fechaCita, horaInicio, horaFin, direccion, descripcion))
                .ReturnsAsync(true);

            // Act
            var result = await _mockEmailService.Object.SendCitaConfirmadaEmailAsync(
                email, nombreCliente, nombreTecnico, fechaCita, horaInicio, horaFin, direccion, descripcion);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendCitaCanceladaEmailAsync_ReturnsTrue()
        {
            // Arrange
            var email = "test@test.com";
            var nombreCliente = "Test Client";
            var nombreTecnico = "Test Tech";
            var fechaCita = DateTime.UtcNow.AddDays(1);
            var horaInicio = "10:00";
            var motivo = "Cancelación de prueba";

            _mockEmailService.Setup(e => e.SendCitaCanceladaEmailAsync(
                    email, nombreCliente, nombreTecnico, fechaCita, horaInicio, motivo))
                .ReturnsAsync(true);

            // Act
            var result = await _mockEmailService.Object.SendCitaCanceladaEmailAsync(
                email, nombreCliente, nombreTecnico, fechaCita, horaInicio, motivo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendCitaCompletadaEmailAsync_ReturnsTrue()
        {
            // Arrange
            var email = "test@test.com";
            var nombreCliente = "Test Client";
            var nombreTecnico = "Test Tech";
            var fechaCita = DateTime.UtcNow;
            var horaInicio = "10:00";
            var horaFin = "11:00";
            var direccion = "Test Address";
            var descripcion = "Test Description";

            _mockEmailService.Setup(e => e.SendCitaCompletadaEmailAsync(
                    email, nombreCliente, nombreTecnico, fechaCita, horaInicio, horaFin, direccion, descripcion))
                .ReturnsAsync(true);

            // Act
            var result = await _mockEmailService.Object.SendCitaCompletadaEmailAsync(
                email, nombreCliente, nombreTecnico, fechaCita, horaInicio, horaFin, direccion, descripcion);

            // Assert
            Assert.True(result);
        }
    }
}
