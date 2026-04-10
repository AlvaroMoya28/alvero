using EventosBackend.Models.Entities;
using EventosBackend.Models.DTOs.Responses;
using EventosBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using EventosBackend.Models.Context;
using Oracle.ManagedDataAccess.Client;

namespace EventosBackend.Repositories
{
    public class CitaRepository : ICitaRepository
    {
        private readonly OracleDbContext _context;

        public CitaRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> ObtenerTodosTecnicos()
        {
            return await _context.Usuarios
                .Where(u => u.TipoUsuario == "TECNICO" && u.Estado == "ACTIVO")
                .ToListAsync();
        }

        public async Task<List<HorarioDisponibleResponse>> ObtenerHorariosDisponiblesPorTecnico(string idTecnico, DateTime fechaDesde, DateTime fechaHasta)
        {
            var query = @"
                SELECT 
                    TH.ID_HORARIO as IdHorario,
                    TH.ID_USUARIO as IdUsuario,
                    U.NOMBRE || ' ' || U.APELLIDO1 as NombreTecnico,
                    U.EMAIL as Email,
                    TH.FECHA as Fecha,
                    TH.HORA_INICIO as HoraInicio,
                    TH.HORA_FIN as HoraFin,
                    TH.MOTIVO_BLOQUEO as MotivoBloqueo,
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 FROM CITA C 
                            WHERE C.ID_USUARIO_TECNICO = TH.ID_USUARIO 
                              AND C.FECHA_CITA = TH.FECHA 
                              AND C.HORA_INICIO = TH.HORA_INICIO
                              AND C.ESTADO IN ('PENDIENTE','CONFIRMADA')
                        ) THEN 0
                        WHEN TH.DISPONIBLE = '1' THEN 1
                        ELSE 0
                    END as DisponibleReal
                FROM TECNICO_HORARIO TH
                INNER JOIN USUARIO U ON TH.ID_USUARIO = U.ID_UNICO
                WHERE TH.ID_USUARIO = :idTecnico
                  AND TH.FECHA >= :fechaDesde
                  AND TH.FECHA <= :fechaHasta
                ORDER BY TH.FECHA, TH.HORA_INICIO";

            var result = new List<HorarioDisponibleResponse>();
            var connection = _context.Database.GetDbConnection();
            
            var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;
            
            try
            {
                if (shouldCloseConnection)
                {
                    await connection.OpenAsync();
                }
                
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.Add(new OracleParameter("idTecnico", idTecnico));
                    command.Parameters.Add(new OracleParameter("fechaDesde", fechaDesde.Date));
                    command.Parameters.Add(new OracleParameter("fechaHasta", fechaHasta.Date));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new HorarioDisponibleResponse
                            {
                                IdHorario = reader.GetInt32(reader.GetOrdinal("IdHorario")),
                                IdUsuario = reader.GetString(reader.GetOrdinal("IdUsuario")),
                                NombreTecnico = reader.GetString(reader.GetOrdinal("NombreTecnico")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                                HoraInicio = reader.GetString(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetString(reader.GetOrdinal("HoraFin")),
                                MotivoBloqueo = reader.IsDBNull(reader.GetOrdinal("MotivoBloqueo")) 
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("MotivoBloqueo")),
                                DisponibleReal = reader.GetInt32(reader.GetOrdinal("DisponibleReal")) == 1
                            });
                        }
                    }
                }
            }
            finally
            {
                if (shouldCloseConnection && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            
            return result;
        }

        public async Task<List<CitaResponse>> ObtenerCitasPorTecnico(string idTecnico, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            var query = _context.Set<Cita>()
                .Where(c => c.IdUsuarioTecnico == idTecnico);

            if (fechaDesde.HasValue)
                query = query.Where(c => c.FechaCita >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(c => c.FechaCita <= fechaHasta.Value.Date);

            var citas = await query
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();

            return citas.Select(c => new CitaResponse
            {
                IdCita = c.IdCita,
                IdUsuarioCliente = c.IdUsuarioCliente,
                NombreCliente = $"{c.Cliente?.Nombre} {c.Cliente?.Apellido1}",
                IdUsuarioTecnico = c.IdUsuarioTecnico,
                NombreTecnico = $"{c.Tecnico?.Nombre} {c.Tecnico?.Apellido1}",
                FechaCita = c.FechaCita,
                HoraInicio = c.HoraInicio,
                HoraFin = c.HoraFin,
                DescripcionProblema = c.DescripcionProblema,
                Estado = c.Estado,
                Direccion = c.Direccion,
                TelefonoContacto = c.TelefonoContacto
            }).ToList();
        }

        public async Task<List<CitaResponse>> ObtenerCitasPorCliente(string idCliente)
        {
            var citas = await _context.Set<Cita>()
                .Where(c => c.IdUsuarioCliente == idCliente)
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .OrderByDescending(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();

            return citas.Select(c => new CitaResponse
            {
                IdCita = c.IdCita,
                IdUsuarioCliente = c.IdUsuarioCliente,
                NombreCliente = $"{c.Cliente?.Nombre} {c.Cliente?.Apellido1}",
                IdUsuarioTecnico = c.IdUsuarioTecnico,
                NombreTecnico = $"{c.Tecnico?.Nombre} {c.Tecnico?.Apellido1}",
                FechaCita = c.FechaCita,
                HoraInicio = c.HoraInicio,
                HoraFin = c.HoraFin,
                DescripcionProblema = c.DescripcionProblema,
                Estado = c.Estado,
                Direccion = c.Direccion,
                TelefonoContacto = c.TelefonoContacto
            }).ToList();
        }

        public async Task<Cita?> ObtenerCitaPorId(int idCita)
        {
            return await _context.Set<Cita>()
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .FirstOrDefaultAsync(c => c.IdCita == idCita);
        }

        public async Task<Cita> CrearCita(Cita cita)
        {
            cita.FechaCreacion = DateTime.Now;
            cita.Estado = "PENDIENTE";
            
            // Calcular hora fin (1 hora después)
            var horaInicioInt = int.Parse(cita.HoraInicio.Split(':')[0]);
            cita.HoraFin = $"{horaInicioInt + 1:D2}:00";

            _context.Set<Cita>().Add(cita);
            await _context.SaveChangesAsync();
            return cita;
        }

        public async Task<bool> ActualizarEstadoCita(int idCita, string nuevoEstado)
        {
            var cita = await _context.Set<Cita>().FindAsync(idCita);
            if (cita == null) return false;

            cita.Estado = nuevoEstado;
            cita.FechaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelarCita(int idCita)
        {
            Console.WriteLine($"[REPOSITORY] CancelarCita - IdCita: {idCita}");
            try
            {
                var resultado = await ActualizarEstadoCita(idCita, "CANCELADA");
                Console.WriteLine($"[REPOSITORY] Resultado ActualizarEstadoCita: {resultado}");
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPOSITORY ERROR] CancelarCita: {ex.Message}");
                Console.WriteLine($"[REPOSITORY ERROR] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> EliminarCita(int idCita)
        {
            Console.WriteLine($"[REPOSITORY] EliminarCita - IdCita: {idCita}");
            try
            {
                var cita = await _context.Set<Cita>().FindAsync(idCita);
                if (cita == null)
                {
                    Console.WriteLine($"[REPOSITORY] Cita {idCita} no encontrada para eliminar");
                    return false;
                }

                _context.Set<Cita>().Remove(cita);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[REPOSITORY] Cita {idCita} eliminada exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPOSITORY ERROR] EliminarCita: {ex.Message}");
                Console.WriteLine($"[REPOSITORY ERROR] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> BloquearHorario(string idTecnico, DateTime fecha, string horaInicio, string motivoBloqueo)
        {
            var horario = await _context.Set<TecnicoHorario>()
                .FirstOrDefaultAsync(h => h.IdUsuario == idTecnico && h.Fecha.Date == fecha.Date && h.HoraInicio == horaInicio);

            if (horario == null) return false;

            horario.Disponible = false;
            horario.MotivoBloqueo = motivoBloqueo;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DesbloquearHorario(string idTecnico, DateTime fecha, string horaInicio)
        {
            var horario = await _context.Set<TecnicoHorario>()
                .FirstOrDefaultAsync(h => h.IdUsuario == idTecnico && h.Fecha.Date == fecha.Date && h.HoraInicio == horaInicio);

            if (horario == null) return false;

            horario.Disponible = true;
            horario.MotivoBloqueo = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task GenerarHorariosSemana(string idTecnico, DateTime fechaInicio)
        {
            var horariosExistentes = await _context.Set<TecnicoHorario>()
                .Where(h => h.IdUsuario == idTecnico && h.Fecha >= fechaInicio.Date && h.Fecha < fechaInicio.Date.AddDays(7))
                .ToListAsync();

            for (int dia = 0; dia < 7; dia++)
            {
                var fecha = fechaInicio.Date.AddDays(dia);
                var diaSemana = (int)fecha.DayOfWeek; // 0=Dom, 1=Lun, ..., 5=Vie

                // Solo L-V (1-5)
                if (diaSemana >= 1 && diaSemana <= 5)
                {
                    // 8-12 y 13-17 (excluye 12-13 almuerzo)
                    for (int hora = 8; hora <= 16; hora++)
                    {
                        if (hora == 12) continue; // Saltar almuerzo

                        var horaInicio = $"{hora:D2}:00";
                        var horaFin = $"{hora + 1:D2}:00";

                        // Verificar si ya existe
                        var existe = horariosExistentes.Any(h =>
                            h.Fecha.Date == fecha && h.HoraInicio == horaInicio);

                        if (!existe)
                        {
                            _context.Set<TecnicoHorario>().Add(new TecnicoHorario
                            {
                                IdUsuario = idTecnico,
                                Fecha = fecha,
                                HoraInicio = horaInicio,
                                HoraFin = horaFin,
                                Disponible = true,
                                FechaCreacion = DateTime.Now
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<TecnicoHorario>> ObtenerHorariosPorTecnico(string idTecnico, DateTime fechaDesde, DateTime fechaHasta)
        {
            return await _context.Set<TecnicoHorario>()
                .Where(h => h.IdUsuario == idTecnico && h.Fecha >= fechaDesde.Date && h.Fecha <= fechaHasta.Date)
                .OrderBy(h => h.Fecha)
                .ThenBy(h => h.HoraInicio)
                .ToListAsync();
        }
    }
}
