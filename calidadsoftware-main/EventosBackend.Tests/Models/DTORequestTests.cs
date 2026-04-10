using Xunit;
using EventosBackend.Models.DTOs.Requests;
using System;

namespace EventosBackend.Tests.Models
{
    public class DTORequestTests
    {
        [Fact]
        public void UsuarioCreateRequest_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var request = new UsuarioCreateRequest
            {
                IdUsuario = "USER001",
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

            // Assert
            Assert.Equal("USER001", request.IdUsuario);
            Assert.Equal("Juan", request.Nombre);
            Assert.Equal("Pérez", request.Apellido1);
            Assert.Equal("García", request.Apellido2);
            Assert.Equal("juan@test.com", request.Email);
            Assert.Equal("12345678", request.Telefono);
            Assert.Equal("Password123!", request.Contrasena);
            Assert.Equal("Password123!", request.ConfirmarContrasena);
            Assert.Equal("CLIENTE", request.Rol);
        }

        [Fact]
        public void LoginRequest_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var request = new LoginRequest
            {
                Id = "testuser",
                Password = "Password123!"
            };

            // Assert
            Assert.Equal("testuser", request.Id);
            Assert.Equal("Password123!", request.Password);
        }

        [Fact]
        public void UsuarioUpdateRequest_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var request = new UsuarioUpdateRequest
            {
                Nombre = "Juan Updated",
                Apellido1 = "Pérez",
                Apellido2 = "García",
                Email = "juan.new@test.com",
                Telefono = "87654321"
            };

            // Assert
            Assert.Equal("Juan Updated", request.Nombre);
            Assert.Equal("Pérez", request.Apellido1);
            Assert.Equal("García", request.Apellido2);
            Assert.Equal("juan.new@test.com", request.Email);
            Assert.Equal("87654321", request.Telefono);
        }

        [Fact]
        public void CrearCitaRequest_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var request = new CrearCitaRequest
            {
                NombreCliente = "Cliente Test",
                EmailCliente = "cliente@test.com",
                CedulaCliente = "123456789",
                IdUsuarioTecnico = "TEC001",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = "10:00",
                Direccion = "Test Address 123"
            };

            // Assert
            Assert.Equal("Cliente Test", request.NombreCliente);
            Assert.Equal("cliente@test.com", request.EmailCliente);
            Assert.Equal("123456789", request.CedulaCliente);
            Assert.Equal("TEC001", request.IdUsuarioTecnico);
            Assert.Equal("10:00", request.HoraInicio);
            Assert.Equal("Test Address 123", request.Direccion);
        }

        [Fact]
        public void BloquearHorarioRequest_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var request = new BloquearHorarioRequest
            {
                Fecha = DateTime.Today.AddDays(1),
                HoraInicio = "10:00",
                MotivoBloqueo = "Reunión importante"
            };

            // Assert
            Assert.Equal("10:00", request.HoraInicio);
            Assert.Equal("Reunión importante", request.MotivoBloqueo);
            Assert.True(request.Fecha > DateTime.Today);
        }

        [Fact]
        public void GenerarHorariosRequest_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var request = new GenerarHorariosRequest
            {
                FechaInicio = DateTime.Today
            };

            // Assert
            Assert.Equal(DateTime.Today, request.FechaInicio);
        }
    }
}
