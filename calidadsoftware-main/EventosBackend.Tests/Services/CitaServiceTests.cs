using Xunit;
using Moq;
using EventosBackend.Services;
using EventosBackend.Services.Interfaces;
using EventosBackend.Repositories.Interfaces;
using EventosBackend.Models.Entities;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using EventosBackend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventosBackend.Tests.Services
{
    public class CitaServiceTests
    {
        private readonly Mock<ICitaRepository> _mockRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly CitaService _service;
        private readonly Mock<IEmailService> _mockEmailService;

        public CitaServiceTests()
        {
            _mockRepository = new Mock<ICitaRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _service = new CitaService(_mockRepository.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task ObtenerHorariosDisponibles_ReturnsHorarios()
        {
            // Arrange
            var idTecnico = "TEC001";
            var fechaDesde = DateTime.Today;
            var fechaHasta = DateTime.Today.AddDays(7);
            var horariosEsperados = new List<HorarioDisponibleResponse>
            {
                new HorarioDisponibleResponse
                {
                    IdHorario = 1,
                    IdUsuario = idTecnico,
                    Fecha = DateTime.Today,
                    HoraInicio = "08:00",
                    HoraFin = "09:00",
                    DisponibleReal = true
                }
            };

            _mockRepository.Setup(r => r.ObtenerHorariosDisponiblesPorTecnico(idTecnico, fechaDesde, fechaHasta))
                .ReturnsAsync(horariosEsperados);

            // Act
            var resultado = await _service.ObtenerHorariosDisponibles(idTecnico, fechaDesde, fechaHasta);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("08:00", resultado[0].HoraInicio);
        }

        [Fact]
        public async Task CrearCitaPublica_ConDatosValidos_CreaCitaExitosamente()
        {
            // Arrange
            var request = new CrearCitaRequest
            {
                NombreCliente = "Juan Pérez",
                EmailCliente = "juan@email.com",
                CedulaCliente = "123456789",
                IdUsuarioTecnico = "TEC001",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = "10:00",
                DescripcionProblema = "Reparación de laptop",
                Direccion = "San José, Costa Rica",
                TelefonoContacto = "8888-9999"
            };

            var horariosDisponibles = new List<HorarioDisponibleResponse>
            {
                new HorarioDisponibleResponse
                {
                    IdHorario = 1,
                    Fecha = request.FechaCita,
                    HoraInicio = "10:00",
                    DisponibleReal = true
                }
            };

            var citaCreada = new Cita
            {
                IdCita = 1,
                NombreCliente = request.NombreCliente,
                EmailCliente = request.EmailCliente,
                CedulaCliente = request.CedulaCliente,
                IdUsuarioTecnico = request.IdUsuarioTecnico,
                FechaCita = request.FechaCita,
                HoraInicio = request.HoraInicio,
                HoraFin = "11:00",
                Estado = "PENDIENTE"
            };

            _mockRepository.Setup(r => r.ObtenerHorariosDisponiblesPorTecnico(
                    It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(horariosDisponibles);

            _mockRepository.Setup(r => r.CrearCita(It.IsAny<Cita>()))
                .ReturnsAsync(citaCreada);

            // Act
            var resultado = await _service.CrearCitaPublica(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.IdCita);
            Assert.Equal("Juan Pérez", resultado.NombreCliente);
            Assert.Equal("PENDIENTE", resultado.Estado);
        }

        [Fact]
        public async Task CrearCitaPublica_SinNombreCliente_LanzaExcepcion()
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

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CrearCitaPublica(request));
        }

        [Fact]
        public async Task CrearCitaPublica_HorarioNoDisponible_LanzaExcepcion()
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

            var horariosDisponibles = new List<HorarioDisponibleResponse>
            {
                new HorarioDisponibleResponse
                {
                    IdHorario = 1,
                    Fecha = request.FechaCita,
                    HoraInicio = "10:00",
                    DisponibleReal = false // No disponible
                }
            };

            _mockRepository.Setup(r => r.ObtenerHorariosDisponiblesPorTecnico(
                    It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(horariosDisponibles);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CrearCitaPublica(request));
            Assert.Contains("no está disponible", exception.Message);
        }

        [Fact]
        public async Task ObtenerCitasCliente_ReturnsListaDeCitas()
        {
            // Arrange
            var idCliente = "CLI001";
            var citasEsperadas = new List<CitaResponse>
            {
                new CitaResponse
                {
                    IdCita = 1,
                    IdUsuarioCliente = idCliente,
                    FechaCita = DateTime.Today,
                    Estado = "PENDIENTE"
                }
            };

            _mockRepository.Setup(r => r.ObtenerCitasPorCliente(idCliente))
                .ReturnsAsync(citasEsperadas);

            // Act
            var resultado = await _service.ObtenerCitasCliente(idCliente);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(idCliente, resultado[0].IdUsuarioCliente);
        }

        [Fact]
        public async Task CancelarCita_ConUsuarioAutorizado_CancelaCita()
        {
            // Arrange
            var idCita = 1;
            var idUsuario = "CLI001";
            var cita = new Cita
            {
                IdCita = idCita,
                IdUsuarioCliente = idUsuario,
                Estado = "PENDIENTE",
                EmailCliente = "test@test.com",
                NombreCliente = "Test"
            };

            _mockRepository.Setup(r => r.ObtenerCitaPorId(idCita))
                .ReturnsAsync(cita);

            _mockRepository.Setup(r => r.EliminarCita(idCita))
                .ReturnsAsync(true);
            
            _mockEmailService.Setup(e => e.SendCitaCanceladaEmailAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _service.CancelarCita(idCita, idUsuario);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task CancelarCita_ConUsuarioNoAutorizado_LanzaExcepcion()
        {
            // Arrange
            var idCita = 1;
            var idUsuario = "CLI002";
            var cita = new Cita
            {
                IdCita = idCita,
                IdUsuarioCliente = "CLI001", // Diferente usuario
                IdUsuarioTecnico = "TEC001",
                Estado = "PENDIENTE"
            };

            _mockRepository.Setup(r => r.ObtenerCitaPorId(idCita))
                .ReturnsAsync(cita);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.CancelarCita(idCita, idUsuario));
        }

        [Fact]
        public async Task ConfirmarCita_ActualizaEstadoCorrectamente()
        {
            // Arrange
            var idCita = 1;
            _mockRepository.Setup(r => r.ActualizarEstadoCita(idCita, "CONFIRMADA"))
                .ReturnsAsync(true);

            // Act
            var resultado = await _service.ConfirmarCita(idCita);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task GenerarHorariosSemana_LlamaRepositorioCorrectamente()
        {
            // Arrange
            var idTecnico = "TEC001";
            var fechaInicio = DateTime.Today;

            _mockRepository.Setup(r => r.GenerarHorariosSemana(idTecnico, fechaInicio))
                .Returns(Task.CompletedTask);

            // Act
            await _service.GenerarHorariosSemana(idTecnico, fechaInicio);

            // Assert
            _mockRepository.Verify(r => r.GenerarHorariosSemana(idTecnico, fechaInicio), Times.Once);
        }

        [Fact]
        public async Task BloquearHorario_ConDatosValidos_BloqueaExitosamente()
        {
            // Arrange
            var idTecnico = "TEC001";
            var request = new BloquearHorarioRequest
            {
                Fecha = DateTime.Today,
                HoraInicio = "10:00",
                MotivoBloqueo = "Reunión interna"
            };

            _mockRepository.Setup(r => r.BloquearHorario(
                    idTecnico, request.Fecha, request.HoraInicio, request.MotivoBloqueo))
                .ReturnsAsync(true);

            // Act
            var resultado = await _service.BloquearHorario(idTecnico, request);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task CompletarCita_ActualizaEstadoCorrectamente()
        {
            // Arrange
            var idCita = 1;
            var cita = new Cita
            {
                IdCita = idCita,
                EmailCliente = "test@test.com",
                NombreCliente = "Test Cliente",
                FechaCita = DateTime.Today,
                HoraInicio = "10:00",
                HoraFin = "11:00"
            };

            _mockRepository.Setup(r => r.ActualizarEstadoCita(idCita, "COMPLETADA"))
                .ReturnsAsync(true);
            _mockRepository.Setup(r => r.ObtenerCitaPorId(idCita))
                .ReturnsAsync(cita);
            _mockEmailService.Setup(e => e.SendCitaCompletadaEmailAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _service.CompletarCita(idCita);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task DesbloquearHorario_ConDatosValidos_DesbloquéaExitosamente()
        {
            // Arrange
            var idTecnico = "TEC001";
            var fecha = DateTime.Today;
            var horaInicio = "10:00";

            _mockRepository.Setup(r => r.DesbloquearHorario(idTecnico, fecha, horaInicio))
                .ReturnsAsync(true);

            // Act
            var resultado = await _service.DesbloquearHorario(idTecnico, fecha, horaInicio);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task ObtenerDisponibilidadAgregada_ReturnsDisponibilidad()
        {
            // Arrange
            var fechaDesde = DateTime.Today;
            var fechaHasta = DateTime.Today.AddDays(7);
            var tecnicos = new List<Usuario>
            {
                new Usuario { IdUnico = "TEC001", Nombre = "Tecnico1", TipoUsuario = "TECNICO" }
            };
            var horarios = new List<HorarioDisponibleResponse>
            {
                new HorarioDisponibleResponse
                {
                    IdHorario = 1,
                    IdUsuario = "TEC001",
                    Fecha = DateTime.Today,
                    HoraInicio = "08:00",
                    HoraFin = "09:00",
                    DisponibleReal = true,
                    NombreTecnico = "Tecnico1",
                    Email = "tec1@test.com"
                }
            };

            _mockRepository.Setup(r => r.ObtenerTodosTecnicos()).ReturnsAsync(tecnicos);
            _mockRepository.Setup(r => r.ObtenerHorariosDisponiblesPorTecnico(
                    It.IsAny<string>(), fechaDesde, fechaHasta))
                .ReturnsAsync(horarios);

            // Act
            var resultado = await _service.ObtenerDisponibilidadAgregada(fechaDesde, fechaHasta);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task ObtenerCitasTecnico_ReturnsListaDeCitas()
        {
            // Arrange
            var idTecnico = "TEC001";
            var fechaDesde = DateTime.Today;
            var fechaHasta = DateTime.Today.AddDays(7);
            var citas = new List<CitaResponse>
            {
                new CitaResponse { IdCita = 1, IdUsuarioTecnico = idTecnico }
            };

            _mockRepository.Setup(r => r.ObtenerCitasPorTecnico(idTecnico, fechaDesde, fechaHasta))
                .ReturnsAsync(citas);

            // Act
            var resultado = await _service.ObtenerCitasTecnico(idTecnico, fechaDesde, fechaHasta);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
        }

        [Fact]
        public async Task CrearCita_ConDatosValidos_CreaCita()
        {
            // Arrange
            var idCliente = "CLI001";
            var request = new CrearCitaRequest
            {
                IdUsuarioTecnico = "TEC001",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = "10:00",
                DescripcionProblema = "Problema test"
            };

            var horariosDisponibles = new List<HorarioDisponibleResponse>
            {
                new HorarioDisponibleResponse
                {
                    Fecha = request.FechaCita,
                    HoraInicio = "10:00",
                    DisponibleReal = true
                }
            };

            var citaCreada = new Cita
            {
                IdCita = 1,
                IdUsuarioCliente = idCliente,
                IdUsuarioTecnico = request.IdUsuarioTecnico,
                Estado = "PENDIENTE"
            };

            _mockRepository.Setup(r => r.ObtenerHorariosDisponiblesPorTecnico(
                    It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(horariosDisponibles);
            _mockRepository.Setup(r => r.CrearCita(It.IsAny<Cita>()))
                .ReturnsAsync(citaCreada);

            // Act
            var resultado = await _service.CrearCita(idCliente, request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.IdCita);
        }

        [Fact]
        public async Task ObtenerHorariosTecnico_ReturnsHorarios()
        {
            // Arrange
            var idTecnico = "TEC001";
            var fechaDesde = DateTime.Today;
            var fechaHasta = DateTime.Today.AddDays(7);
            var horarios = new List<TecnicoHorario>
            {
                new TecnicoHorario { IdHorario = 1, IdUsuario = idTecnico }
            };

            _mockRepository.Setup(r => r.ObtenerHorariosPorTecnico(idTecnico, fechaDesde, fechaHasta))
                .ReturnsAsync(horarios);

            // Act
            var resultado = await _service.ObtenerHorariosTecnico(idTecnico, fechaDesde, fechaHasta);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
        }
    }
}
