using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventosBackend.Services.Interfaces;
using EventosBackend.Models.DTOs.Requests;
using System.Security.Claims;

namespace EventosBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly ICitaService _citaService;

        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        // GET: api/citas/tecnicos
        [HttpGet("tecnicos")]
        [Authorize(Roles = "ADMINISTRADOR,SUPERUSUARIO")]
        public async Task<IActionResult> ObtenerTodosTecnicos()
        {
            // Este endpoint podría obtener lista de técnicos disponibles
            // Por ahora retornamos OK, implementar según necesidad
            return Ok(new { message = "Endpoint para listar técnicos disponibles" });
        }

        // GET: api/citas/disponibilidad-general
        [HttpGet("disponibilidad-general")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerDisponibilidadAgregada(
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta)
        {
            try
            {
                var desde = fechaDesde ?? DateTime.Today;
                var hasta = fechaHasta ?? DateTime.Today.AddDays(7);

                var disponibilidad = await _citaService.ObtenerDisponibilidadAgregada(desde, hasta);
                return Ok(disponibilidad);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/citas/horarios-disponibles/{idTecnico}
        [HttpGet("horarios-disponibles/{idTecnico}")]
        public async Task<IActionResult> ObtenerHorariosDisponibles(
            string idTecnico,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta)
        {
            try
            {
                var desde = fechaDesde ?? DateTime.Today;
                var hasta = fechaHasta ?? DateTime.Today.AddDays(7);

                var horarios = await _citaService.ObtenerHorariosDisponibles(idTecnico, desde, hasta);
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/citas/tecnico/mis-citas
        [HttpGet("tecnico/mis-citas")]
        [Authorize(Roles = "TECNICO")]
        public async Task<IActionResult> ObtenerMisCitasTecnico([FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
        {
            try
            {
                var idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                var citas = await _citaService.ObtenerCitasTecnico(idUsuario, fechaDesde, fechaHasta);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/citas/tecnico/{idTecnico}
        [HttpGet("tecnico/{idTecnico}")]
        public async Task<IActionResult> ObtenerCitasTecnico(
            string idTecnico,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta)
        {
            try
            {
                var citas = await _citaService.ObtenerCitasTecnico(idTecnico, fechaDesde, fechaHasta);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/citas/mis-citas
        [HttpGet("mis-citas")]
        public async Task<IActionResult> ObtenerMisCitas()
        {
            try
            {
                var idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                var citas = await _citaService.ObtenerCitasCliente(idUsuario);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/citas
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CrearCita([FromBody] CrearCitaRequest request)
        {
            try
            {
                // Permitir citas sin autenticación - usar datos del request
                var cita = await _citaService.CrearCitaPublica(request);
                return Ok(cita);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error en CrearCita: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/citas/{idCita}/confirmar
        [HttpPut("{idCita}/confirmar")]
        [Authorize(Roles = "TECNICO,ADMINISTRADOR,SUPERUSUARIO")]
        public async Task<IActionResult> ConfirmarCita(int idCita)
        {
            try
            {
                // Logging para debug
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                
                Console.WriteLine($"=== CONFIRMAR CITA BACKEND ===");
                Console.WriteLine($"ID Cita: {idCita}");
                Console.WriteLine($"Usuario ID: {userId}");
                Console.WriteLine($"Usuario Rol: {userRole}");
                Console.WriteLine($"Usuario Nombre: {userName}");
                Console.WriteLine($"Usuario autenticado: {User.Identity?.IsAuthenticated}");
                
                var resultado = await _citaService.ConfirmarCita(idCita);
                if (!resultado) return NotFound(new { message = "Cita no encontrada" });
                return Ok(new { message = "Cita confirmada" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ConfirmarCita: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/citas/{idCita}/completar
        [HttpPut("{idCita}/completar")]
        [Authorize(Roles = "TECNICO,ADMINISTRADOR,SUPERUSUARIO")]
        public async Task<IActionResult> CompletarCita(int idCita)
        {
            try
            {
                var resultado = await _citaService.CompletarCita(idCita);
                if (!resultado) return NotFound(new { message = "Cita no encontrada" });
                return Ok(new { message = "Cita completada" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/citas/{idCita}
        [HttpDelete("{idCita}")]
        [Authorize(Roles = "TECNICO,ADMINISTRADOR,SUPERUSUARIO")]
        public async Task<IActionResult> CancelarCita(int idCita)
        {
            try
            {
                var idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value
                              ?? User.FindFirst("tipoUsuario")?.Value;

                Console.WriteLine("=== CANCELAR CITA BACKEND ===");
                Console.WriteLine($"ID Cita: {idCita}");
                Console.WriteLine($"Usuario ID: {idUsuario}");
                Console.WriteLine($"Usuario Rol: {userRole}");
                Console.WriteLine($"Usuario autenticado: {User.Identity?.IsAuthenticated}");
                
                // TECNICO, ADMINISTRADOR y SUPERUSUARIO pueden cancelar cualquier cita
                // No validar permisos específicos para estos roles
                var resultado = await _citaService.CancelarCita(idCita, string.Empty);
                
                if (!resultado) return NotFound(new { message = "Cita no encontrada" });
                return Ok(new { message = "Cita cancelada exitosamente" });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"[FORBID] CancelarCita: {ex.Message}");
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] CancelarCita: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/citas/horarios/bloquear
        [HttpPost("horarios/bloquear")]
        [Authorize(Roles = "TECNICO,ADMINISTRADOR,SUPERUSUARIO")]
        public async Task<IActionResult> BloquearHorario([FromBody] BloquearHorarioRequest request)
        {
            try
            {
                var idTecnico = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                var resultado = await _citaService.BloquearHorario(idTecnico, request);
                if (!resultado) return NotFound(new { message = "Horario no encontrado" });
                return Ok(new { message = "Horario bloqueado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/citas/horarios/desbloquear
        [HttpPost("horarios/desbloquear")]
        [Authorize(Roles = "TECNICO,ADMINISTRADOR,SUPERUSUARIO")]
        public async Task<IActionResult> DesbloquearHorario([FromBody] BloquearHorarioRequest request)
        {
            try
            {
                var idTecnico = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                var resultado = await _citaService.DesbloquearHorario(idTecnico, request.Fecha, request.HoraInicio);
                if (!resultado) return NotFound(new { message = "Horario no encontrado" });
                return Ok(new { message = "Horario desbloqueado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/citas/horarios/generar
        [HttpPost("horarios/generar")]
        public async Task<IActionResult> GenerarHorariosSemana([FromBody] GenerarHorariosRequest request)
        {
            try
            {
                await _citaService.GenerarHorariosSemana(request.IdTecnico, request.FechaInicio);
                return Ok(new { message = "Horarios generados correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/citas/tecnico/{idTecnico}/horarios
        [HttpGet("tecnico/{idTecnico}/horarios")]
        [Authorize(Roles = "TECNICO,ADMINISTRADOR,SUPERUSUARIO")]
        public async Task<IActionResult> ObtenerHorariosTecnico(
            string idTecnico,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta)
        {
            try
            {
                var desde = fechaDesde ?? DateTime.Today;
                var hasta = fechaHasta ?? DateTime.Today.AddDays(7);

                var horarios = await _citaService.ObtenerHorariosTecnico(idTecnico, desde, hasta);
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
