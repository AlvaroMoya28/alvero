using EventosBackend.Models.Entities;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using EventosBackend.Repositories.Interfaces;
using EventosBackend.Services.Interfaces;

namespace EventosBackend.Services
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IEmailService _emailService;

        public CitaService(ICitaRepository citaRepository, IEmailService emailService)
        {
            _citaRepository = citaRepository;
            _emailService = emailService;
        }

        public async Task<List<HorarioDisponibleResponse>> ObtenerHorariosDisponibles(string idTecnico, DateTime fechaDesde, DateTime fechaHasta)
        {
            return await _citaRepository.ObtenerHorariosDisponiblesPorTecnico(idTecnico, fechaDesde, fechaHasta);
        }

        public async Task<List<DisponibilidadAgregadaResponse>> ObtenerDisponibilidadAgregada(DateTime fechaDesde, DateTime fechaHasta)
        {
            // Obtener todos los técnicos del repositorio de usuarios
            var tecnicos = await _citaRepository.ObtenerTodosTecnicos();
            
            // Crear un diccionario para agrupar por fecha y hora
            var disponibilidadPorSlot = new Dictionary<string, DisponibilidadAgregadaResponse>();

            foreach (var tecnico in tecnicos)
            {
                var horariosTecnico = await _citaRepository.ObtenerHorariosDisponiblesPorTecnico(
                    tecnico.IdUnico, fechaDesde, fechaHasta);

                foreach (var horario in horariosTecnico.Where(h => h.DisponibleReal))
                {
                    var key = $"{horario.Fecha:yyyy-MM-dd}_{horario.HoraInicio}";

                    if (!disponibilidadPorSlot.ContainsKey(key))
                    {
                        disponibilidadPorSlot[key] = new DisponibilidadAgregadaResponse
                        {
                            Fecha = horario.Fecha,
                            HoraInicio = horario.HoraInicio,
                            HoraFin = horario.HoraFin,
                            TecnicosDisponibles = 0,
                            Tecnicos = new List<TecnicoDisponibleInfo>()
                        };
                    }

                    disponibilidadPorSlot[key].TecnicosDisponibles++;
                    disponibilidadPorSlot[key].Tecnicos.Add(new TecnicoDisponibleInfo
                    {
                        IdTecnico = horario.IdUsuario,
                        NombreTecnico = horario.NombreTecnico,
                        Email = horario.Email
                    });
                }
            }

            return disponibilidadPorSlot.Values
                .OrderBy(d => d.Fecha)
                .ThenBy(d => d.HoraInicio)
                .ToList();
        }

        public async Task<List<CitaResponse>> ObtenerCitasTecnico(string idTecnico, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return await _citaRepository.ObtenerCitasPorTecnico(idTecnico, fechaDesde, fechaHasta);
        }

        public async Task<List<CitaResponse>> ObtenerCitasCliente(string idCliente)
        {
            return await _citaRepository.ObtenerCitasPorCliente(idCliente);
        }

        public async Task<CitaResponse> CrearCita(string idCliente, CrearCitaRequest request)
        {
            // Log para debug
            Console.WriteLine($"[DEBUG] Creando cita para cliente: '{idCliente}'");
            Console.WriteLine($"[DEBUG] Técnico: '{request.IdUsuarioTecnico}'");
            
            // Validar que el idCliente no esté vacío
            if (string.IsNullOrWhiteSpace(idCliente))
            {
                throw new Exception("No se pudo identificar al usuario cliente. Por favor, inicie sesión nuevamente.");
            }

            // Validar disponibilidad
            var horariosDisponibles = await _citaRepository.ObtenerHorariosDisponiblesPorTecnico(
                request.IdUsuarioTecnico,
                request.FechaCita.Date,
                request.FechaCita.Date);

            var slotDisponible = horariosDisponibles.FirstOrDefault(h =>
                h.Fecha.Date == request.FechaCita.Date &&
                h.HoraInicio == request.HoraInicio &&
                h.DisponibleReal);

            if (slotDisponible == null)
            {
                throw new Exception("El horario seleccionado no está disponible");
            }

            var cita = new Cita
            {
                IdUsuarioCliente = idCliente,
                IdUsuarioTecnico = request.IdUsuarioTecnico,
                FechaCita = request.FechaCita.Date,
                HoraInicio = request.HoraInicio,
                DescripcionProblema = request.DescripcionProblema,
                Direccion = request.Direccion,
                TelefonoContacto = request.TelefonoContacto
            };

            var citaCreada = await _citaRepository.CrearCita(cita);

            return new CitaResponse
            {
                IdCita = citaCreada.IdCita,
                IdUsuarioCliente = citaCreada.IdUsuarioCliente,
                IdUsuarioTecnico = citaCreada.IdUsuarioTecnico,
                FechaCita = citaCreada.FechaCita,
                HoraInicio = citaCreada.HoraInicio,
                HoraFin = citaCreada.HoraFin,
                DescripcionProblema = citaCreada.DescripcionProblema,
                Estado = citaCreada.Estado,
                Direccion = citaCreada.Direccion,
                TelefonoContacto = citaCreada.TelefonoContacto
            };
        }

        public async Task<CitaResponse> CrearCitaPublica(CrearCitaRequest request)
        {
            // Log para debug
            Console.WriteLine($"[DEBUG] Creando cita pública");
            Console.WriteLine($"[DEBUG] Cliente: {request.NombreCliente} - {request.EmailCliente}");
            Console.WriteLine($"[DEBUG] Técnico: '{request.IdUsuarioTecnico}'");

            // Validar campos requeridos
            if (string.IsNullOrWhiteSpace(request.NombreCliente) ||
                string.IsNullOrWhiteSpace(request.EmailCliente) ||
                string.IsNullOrWhiteSpace(request.CedulaCliente))
            {
                throw new Exception("Debe proporcionar nombre, email y cédula del cliente");
            }

            // Validar disponibilidad
            var horariosDisponibles = await _citaRepository.ObtenerHorariosDisponiblesPorTecnico(
                request.IdUsuarioTecnico,
                request.FechaCita.Date,
                request.FechaCita.Date);

            var slotDisponible = horariosDisponibles.FirstOrDefault(h =>
                h.Fecha.Date == request.FechaCita.Date &&
                h.HoraInicio == request.HoraInicio &&
                h.DisponibleReal);

            if (slotDisponible == null)
            {
                throw new Exception("El horario seleccionado no está disponible");
            }

            // Crear cita con los datos del cliente directamente
            var cita = new Cita
            {
                IdUsuarioCliente = null, // No tiene cuenta de usuario
                NombreCliente = request.NombreCliente,
                EmailCliente = request.EmailCliente,
                CedulaCliente = request.CedulaCliente,
                IdUsuarioTecnico = request.IdUsuarioTecnico,
                FechaCita = request.FechaCita.Date,
                HoraInicio = request.HoraInicio,
                DescripcionProblema = request.DescripcionProblema,
                Direccion = request.Direccion,
                TelefonoContacto = request.TelefonoContacto
            };

            var citaCreada = await _citaRepository.CrearCita(cita);

            return new CitaResponse
            {
                IdCita = citaCreada.IdCita,
                IdUsuarioCliente = citaCreada.IdUsuarioCliente,
                NombreCliente = citaCreada.NombreCliente ?? request.NombreCliente,
                IdUsuarioTecnico = citaCreada.IdUsuarioTecnico,
                FechaCita = citaCreada.FechaCita,
                HoraInicio = citaCreada.HoraInicio,
                HoraFin = citaCreada.HoraFin,
                DescripcionProblema = citaCreada.DescripcionProblema,
                Estado = citaCreada.Estado,
                Direccion = citaCreada.Direccion,
                TelefonoContacto = citaCreada.TelefonoContacto
            };
        }

        public async Task<bool> CancelarCita(int idCita, string idUsuario)
        {
            Console.WriteLine($"[SERVICE] CancelarCita - IdCita: {idCita}, IdUsuario: '{idUsuario}'");
            
            var cita = await _citaRepository.ObtenerCitaPorId(idCita);
            if (cita == null)
            {
                Console.WriteLine($"[SERVICE] Cita {idCita} no encontrada");
                return false;
            }

            Console.WriteLine($"[SERVICE] Cita encontrada - Cliente: '{cita.IdUsuarioCliente}', Tecnico: '{cita.IdUsuarioTecnico}'");

            // Solo verificar permisos si idUsuario no está vacío
            // (Vacío significa que es un admin/superusuario desde el controller)
            if (!string.IsNullOrEmpty(idUsuario))
            {
                Console.WriteLine($"[SERVICE] Verificando permisos para usuario '{idUsuario}'");
                // Verificar permisos (cliente o técnico pueden cancelar)
                if (cita.IdUsuarioCliente != idUsuario && cita.IdUsuarioTecnico != idUsuario)
                {
                    Console.WriteLine($"[SERVICE] PERMISO DENEGADO - Usuario '{idUsuario}' no es cliente ni técnico");
                    throw new UnauthorizedAccessException("No tiene permiso para cancelar esta cita");
                }
                Console.WriteLine($"[SERVICE] Permisos OK");
            }
            else
            {
                Console.WriteLine($"[SERVICE] Saltando verificación de permisos (admin/superuser)");
            }

            // Enviar correo de cancelación antes de eliminar
            string emailCliente = cita.EmailCliente; // Para citas públicas
            string nombreCliente = cita.NombreCliente ?? "Cliente";
            
            if (string.IsNullOrEmpty(emailCliente) && cita.Cliente != null)
            {
                // Para clientes con cuenta
                emailCliente = cita.Cliente.Email;
                nombreCliente = $"{cita.Cliente.Nombre} {cita.Cliente.Apellido1}";
            }
            
            if (!string.IsNullOrEmpty(emailCliente))
            {
                var nombreTecnico = cita.Tecnico != null 
                    ? $"{cita.Tecnico.Nombre} {cita.Tecnico.Apellido1}"
                    : "Técnico asignado";
                
                await _emailService.SendCitaCanceladaEmailAsync(
                    emailCliente,
                    nombreCliente,
                    nombreTecnico,
                    cita.FechaCita,
                    cita.HoraInicio,
                    "Cancelada por el administrador o técnico"
                );
            }

            // Eliminar físicamente la cita de la base de datos
            var resultado = await _citaRepository.EliminarCita(idCita);
            Console.WriteLine($"[SERVICE] Resultado eliminación: {resultado}");
            return resultado;
        }

        public async Task<bool> ConfirmarCita(int idCita)
        {
            var resultado = await _citaRepository.ActualizarEstadoCita(idCita, "CONFIRMADA");
            
            if (resultado)
            {
                // Enviar correo de confirmación
                var cita = await _citaRepository.ObtenerCitaPorId(idCita);
                if (cita != null)
                {
                    // Determinar el email del cliente
                    string emailCliente = cita.EmailCliente; // Para citas públicas
                    string nombreCliente = cita.NombreCliente ?? "Cliente";
                    
                    if (string.IsNullOrEmpty(emailCliente) && cita.Cliente != null)
                    {
                        // Para clientes con cuenta
                        emailCliente = cita.Cliente.Email;
                        nombreCliente = $"{cita.Cliente.Nombre} {cita.Cliente.Apellido1}";
                    }
                    
                    if (!string.IsNullOrEmpty(emailCliente))
                    {
                        var nombreTecnico = cita.Tecnico != null 
                            ? $"{cita.Tecnico.Nombre} {cita.Tecnico.Apellido1}"
                            : "Técnico asignado";
                        
                        await _emailService.SendCitaConfirmadaEmailAsync(
                            emailCliente,
                            nombreCliente,
                            nombreTecnico,
                            cita.FechaCita,
                            cita.HoraInicio,
                            cita.HoraFin,
                            cita.Direccion ?? "Por confirmar",
                            cita.DescripcionProblema ?? "No especificado"
                        );
                    }
                }
            }
            
            return resultado;
        }

        public async Task<bool> CompletarCita(int idCita)
        {
            var actualizado = await _citaRepository.ActualizarEstadoCita(idCita, "COMPLETADA");
            if (!actualizado) return false;

            // Enviar correo de cita completada (registro para garantía)
            var cita = await _citaRepository.ObtenerCitaPorId(idCita);
            if (cita != null)
            {
                string emailCliente = cita.EmailCliente;
                string nombreCliente = cita.NombreCliente ?? "Cliente";

                if (string.IsNullOrEmpty(emailCliente) && cita.Cliente != null)
                {
                    emailCliente = cita.Cliente.Email;
                    nombreCliente = $"{cita.Cliente.Nombre} {cita.Cliente.Apellido1}";
                }

                if (!string.IsNullOrEmpty(emailCliente))
                {
                    var nombreTecnico = cita.Tecnico != null
                        ? $"{cita.Tecnico.Nombre} {cita.Tecnico.Apellido1}"
                        : "Técnico asignado";

                    await _emailService.SendCitaCompletadaEmailAsync(
                        emailCliente,
                        nombreCliente,
                        nombreTecnico,
                        cita.FechaCita,
                        cita.HoraInicio,
                        cita.HoraFin,
                        cita.Direccion ?? "",
                        cita.DescripcionProblema ?? ""
                    );
                }
            }

            return true;
        }

        public async Task<bool> BloquearHorario(string idTecnico, BloquearHorarioRequest request)
        {
            return await _citaRepository.BloquearHorario(idTecnico, request.Fecha, request.HoraInicio, request.MotivoBloqueo);
        }

        public async Task<bool> DesbloquearHorario(string idTecnico, DateTime fecha, string horaInicio)
        {
            return await _citaRepository.DesbloquearHorario(idTecnico, fecha, horaInicio);
        }

        public async Task GenerarHorariosSemana(string idTecnico, DateTime fechaInicio)
        {
            await _citaRepository.GenerarHorariosSemana(idTecnico, fechaInicio);
        }

        public async Task<List<TecnicoHorario>> ObtenerHorariosTecnico(string idTecnico, DateTime fechaDesde, DateTime fechaHasta)
        {
            return await _citaRepository.ObtenerHorariosPorTecnico(idTecnico, fechaDesde, fechaHasta);
        }
    }
}
