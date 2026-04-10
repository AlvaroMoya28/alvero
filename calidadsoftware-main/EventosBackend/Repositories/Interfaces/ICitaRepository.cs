using EventosBackend.Models.Entities;
using EventosBackend.Models.DTOs.Responses;

namespace EventosBackend.Repositories.Interfaces
{
    public interface ICitaRepository
    {
        Task<List<Usuario>> ObtenerTodosTecnicos();
        Task<List<HorarioDisponibleResponse>> ObtenerHorariosDisponiblesPorTecnico(string idTecnico, DateTime fechaDesde, DateTime fechaHasta);
        Task<List<CitaResponse>> ObtenerCitasPorTecnico(string idTecnico, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<List<CitaResponse>> ObtenerCitasPorCliente(string idCliente);
        Task<Cita?> ObtenerCitaPorId(int idCita);
        Task<Cita> CrearCita(Cita cita);
        Task<bool> ActualizarEstadoCita(int idCita, string nuevoEstado);
        Task<bool> CancelarCita(int idCita);
        Task<bool> EliminarCita(int idCita);
        Task<bool> BloquearHorario(string idTecnico, DateTime fecha, string horaInicio, string motivoBloqueo);
        Task<bool> DesbloquearHorario(string idTecnico, DateTime fecha, string horaInicio);
        Task GenerarHorariosSemana(string idTecnico, DateTime fechaInicio);
        Task<List<TecnicoHorario>> ObtenerHorariosPorTecnico(string idTecnico, DateTime fechaDesde, DateTime fechaHasta);
    }
}
