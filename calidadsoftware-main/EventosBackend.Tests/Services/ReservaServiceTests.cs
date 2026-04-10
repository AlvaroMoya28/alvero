using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventosBackend.Models.Context;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EventosBackend.Tests.Services
{
    public class ReservaServiceTests : IDisposable
    {
        private readonly OracleDbContext _context;
        private readonly ReservaService _service;

        public ReservaServiceTests()
        {
            var options = new DbContextOptionsBuilder<OracleDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new OracleDbContext(options);
            _service = new ReservaService(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsReserva_WhenExists()
        {
            // Arrange
            var reserva = new Reserva
            {
                IdReserva = 1,
                IdUsuario = "user1",
                IdSala = 1,
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddHours(2),
                Estado = "CONFIRMADA"
            };
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.IdReserva);
            Assert.Equal("CONFIRMADA", result.Estado);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_CreatesReserva_WhenNoConflict()
        {
            // Arrange
            var request = new ReservaRequest
            {
                IdUsuario = "user1",
                IdSala = 1,
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow.AddDays(1).AddHours(2),
                AsistentesEsperados = 10,
                Observaciones = "Test reservation",
                PrecioTotal = 100.00m
            };

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user1", result.IdUsuario);
            Assert.Equal(1, result.IdSala);
            Assert.Equal("PENDIENTE", result.Estado);
            Assert.Equal(100.00m, result.PrecioTotal);
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenReservationOverlaps()
        {
            // Arrange
            var existingReserva = new Reserva
            {
                IdReserva = 1,
                IdUsuario = "user1",
                IdSala = 1,
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow.AddDays(1).AddHours(2),
                Estado = "CONFIRMADA"
            };
            _context.Reservas.Add(existingReserva);
            await _context.SaveChangesAsync();

            var overlappingRequest = new ReservaRequest
            {
                IdUsuario = "user2",
                IdSala = 1,
                FechaInicio = DateTime.UtcNow.AddDays(1).AddHours(1), // Overlaps
                FechaFin = DateTime.UtcNow.AddDays(1).AddHours(3),
                AsistentesEsperados = 5,
                PrecioTotal = 50.00m
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(overlappingRequest));
        }

        [Fact]
        public async Task CreateAsync_AllowsReservation_WhenExistingIsCancelled()
        {
            // Arrange
            var cancelledReserva = new Reserva
            {
                IdReserva = 1,
                IdUsuario = "user1",
                IdSala = 1,
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow.AddDays(1).AddHours(2),
                Estado = "CANCELADA"
            };
            _context.Reservas.Add(cancelledReserva);
            await _context.SaveChangesAsync();

            var newRequest = new ReservaRequest
            {
                IdUsuario = "user2",
                IdSala = 1,
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow.AddDays(1).AddHours(2),
                AsistentesEsperados = 5,
                PrecioTotal = 50.00m
            };

            // Act
            var result = await _service.CreateAsync(newRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user2", result.IdUsuario);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllReservas()
        {
            // Arrange
            var reserva1 = new Reserva
            {
                IdReserva = 1,
                IdUsuario = "user1",
                IdSala = 1,
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddHours(2),
                Estado = "CONFIRMADA"
            };
            var reserva2 = new Reserva
            {
                IdReserva = 2,
                IdUsuario = "user2",
                IdSala = 2,
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow.AddDays(1).AddHours(2),
                Estado = "PENDIENTE"
            };
            _context.Reservas.Add(reserva1);
            _context.Reservas.Add(reserva2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreateAsync_WithCatering_SavesCateringId()
        {
            // Arrange
            var request = new ReservaRequest
            {
                IdUsuario = "user1",
                IdSala = 1,
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow.AddDays(1).AddHours(2),
                AsistentesEsperados = 10,
                IdCatering = 5,
                PrecioTotal = 200.00m
            };

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.IdCatering);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
