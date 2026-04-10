using EventosBackend.Models.Entities;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using EventosBackend.Repositories.Interfaces;

namespace EventosBackend.Services.Interfaces
{
    public interface ICitaService
    {
        Task<List<HorarioDisponibleResponse>> ObtenerHorariosDisponibles(string idTecnico, DateTime fechaDesde, DateTime fechaHasta);
        Task<List<DisponibilidadAgregadaResponse>> ObtenerDisponibilidadAgregada(DateTime fechaDesde, DateTime fechaHasta);
        Task<List<CitaResponse>> ObtenerCitasTecnico(string idTecnico, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<CitaResponse>> ObtenerCitasCliente(string idCliente);
        Task<CitaResponse> CrearCita(string idCliente, CrearCitaRequest request);
        Task<CitaResponse> CrearCitaPublica(CrearCitaRequest request);
        Task<bool> CancelarCita(int idCita, string idUsuario);
        Task<bool> ConfirmarCita(int idCita);
        Task<bool> CompletarCita(int idCita);
        Task<bool> BloquearHorario(string idTecnico, BloquearHorarioRequest request);
        Task<bool> DesbloquearHorario(string idTecnico, DateTime fecha, string horaInicio);
        Task GenerarHorariosSemana(string idTecnico, DateTime fechaInicio);
        Task<List<TecnicoHorario>> ObtenerHorariosTecnico(string idTecnico, DateTime fechaDesde, DateTime fechaHasta);
    }
}
