using Xunit;
using EventosBackend.Models.DTOs.Responses;
using System;

namespace EventosBackend.Tests.Models
{
    public class DTOResponseTests
    {
        [Fact]
        public void UsuarioResponse_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var response = new UsuarioResponse
            {
                IdUnico = "USER001",
                IdUsuario = "testuser",
                Nombre = "Juan",
                Apellido1 = "Pérez",
                Apellido2 = "García",
                Email = "juan@test.com",
                Telefono = "12345678",
                TipoUsuario = "CLIENTE",
                Estado = "ACTIVO"
            };

            // Assert
            Assert.Equal("USER001", response.IdUnico);
            Assert.Equal("testuser", response.IdUsuario);
            Assert.Equal("Juan", response.Nombre);
            Assert.Equal("Pérez", response.Apellido1);
            Assert.Equal("García", response.Apellido2);
            Assert.Equal("juan@test.com", response.Email);
            Assert.Equal("12345678", response.Telefono);
            Assert.Equal("CLIENTE", response.TipoUsuario);
            Assert.Equal("ACTIVO", response.Estado);
        }

        [Fact]
        public void CitaResponse_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var response = new CitaResponse
            {
                IdCita = 1,
                NombreCliente = "Cliente Test",
                IdUsuarioTecnico = "TEC001",
                NombreTecnico = "Técnico Test",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = "10:00",
                HoraFin = "11:00",
                Estado = "CONFIRMADA",
                Direccion = "Test Address"
            };

            // Assert
            Assert.Equal(1, response.IdCita);
            Assert.Equal("Cliente Test", response.NombreCliente);
            Assert.Equal("TEC001", response.IdUsuarioTecnico);
            Assert.Equal("Técnico Test", response.NombreTecnico);
            Assert.Equal("10:00", response.HoraInicio);
            Assert.Equal("11:00", response.HoraFin);
            Assert.Equal("CONFIRMADA", response.Estado);
            Assert.Equal("Test Address", response.Direccion);
        }

        [Fact]
        public void HorarioDisponibleResponse_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var response = new HorarioDisponibleResponse
            {
                IdHorario = 1,
                Fecha = DateTime.Today.AddDays(1),
                HoraInicio = "08:00",
                HoraFin = "09:00"
            };

            // Assert
            Assert.Equal(1, response.IdHorario);
            Assert.Equal("08:00", response.HoraInicio);
            Assert.Equal("09:00", response.HoraFin);
            Assert.True(response.Fecha > DateTime.Today);
        }

        [Fact]
        public void DisponibilidadAgregadaResponse_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var response = new DisponibilidadAgregadaResponse
            {
                Fecha = DateTime.Today.AddDays(1),
                HoraInicio = "08:00",
                TecnicosDisponibles = 5
            };

            // Assert
            Assert.Equal("08:00", response.HoraInicio);
            Assert.Equal(5, response.TecnicosDisponibles);
            Assert.True(response.Fecha > DateTime.Today);
        }

        [Fact]
        public void ReservaResponse_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var response = new ReservaResponse
            {
                IdReserva = 1,
                IdUsuario = "USER001",
                Estado = "CONFIRMADA",
                FechaCreacion = DateTime.UtcNow
            };

            // Assert
            Assert.Equal(1, response.IdReserva);
            Assert.Equal("USER001", response.IdUsuario);
            Assert.Equal("CONFIRMADA", response.Estado);
        }
    }
}
