using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using EventosBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using EventosBackend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EventosBackend.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UsuariosController : ControllerBase
  {
    private readonly IUsuarioService _usuarioService;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;
    private readonly IUsuarioRepository _repository;

    public UsuariosController(
      IUsuarioService usuarioService,
      IEmailService emailService,
      IJwtService jwtService,
      IUsuarioRepository repository)
    {
      _usuarioService = usuarioService;
      _emailService = emailService;
      _jwtService = jwtService;
      _repository = repository;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> RegistrarUsuario([FromBody] UsuarioCreateRequest request)
    {
      try
      {
        // Si el rol es TECNICO y la contraseña enviada sigue el patrón esperado
        // (segundo apellido en minúsculas + año de nacimiento), eliminamos
        // cualquier error de ModelState relacionado con la contraseña para permitir la creación.
        var rolTemp = request.Rol?.ToUpper() ?? string.Empty;
        if (rolTemp == "TECNICO")
        {
          if (!string.IsNullOrEmpty(request.Apellido2) && !string.IsNullOrEmpty(request.Contrasena))
          {
            try
            {
              var expected = request.Apellido2.ToLower() + request.FechaNacimiento.Year.ToString();
              if (request.Contrasena == expected && request.ConfirmarContrasena == request.Contrasena)
              {
                // Eliminar cualquier clave de ModelState que contenga 'contras' o 'confirm' (insensible a mayúsculas)
                var keys = ModelState.Keys.ToList();
                foreach (var key in keys)
                {
                  var lower = key.ToLowerInvariant();
                  if (lower.Contains("contras") || lower.Contains("confirm"))
                  {
                    ModelState.Remove(key);
                  }
                }
              }
            }
            catch
            {
              // Si hay algún problema con FechaNacimiento (formato), no hacemos nada aquí;
              // se seguirá aplicando la validación normal y se devolverán errores.
            }
          }
        }

        if (!ModelState.IsValid)
        {
          var rol = request.Rol?.ToUpper() ?? string.Empty;
          bool allowTecnicoPassword = false;

          if (rol == "TECNICO")
          {
            if (!string.IsNullOrEmpty(request.Apellido2) && !string.IsNullOrEmpty(request.Contrasena))
            {
              var expected = request.Apellido2.ToLower() + request.FechaNacimiento.Year.ToString();
              if (request.Contrasena == expected && request.ConfirmarContrasena == request.Contrasena)
              {
                allowTecnicoPassword = true;
              }
            }
          }

          if (!allowTecnicoPassword)
          {
            var errors = ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value.Errors.Select(error => error.ErrorMessage).ToArray()
                );

            return BadRequest(new
            {
              message = "Error de validación",
              errors = errors
            });
          }

        }

        request.Apellido2 ??= null;
        request.Telefono ??= null;

        var usuarioRegistrado = await _usuarioService.RegistrarUsuarioAsync(request);

        return CreatedAtAction(
            nameof(GetUsuarioPorId),
            new { id = usuarioRegistrado.IdUnico },
            usuarioRegistrado);
      }
      catch (ApplicationException ex)
      {
        return BadRequest(new
        {
          message = ex.Message,
          errors = new Dictionary<string, string[]>() {
                { "Contrasena", new[] { ex.Message } }
            }
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new
        {
          message = "Error interno del servidor",
          error = ex.Message
        });
      }
    }

    [HttpPost("registro-tecnico")]
    public async Task<IActionResult> RegistrarTecnico([FromBody] Models.DTOs.Requests.UsuarioCreateRequestNoValidation request)
    {
      try
      {
        // Forzamos el rol a TECNICO independientemente de lo que envíe el cliente
        request.Rol = "TECNICO";

        // Validaciones básicas
        if (!ModelState.IsValid)
        {
          var errors = ModelState
              .Where(e => e.Value.Errors.Count > 0)
              .ToDictionary(
                  e => e.Key,
                  e => e.Value.Errors.Select(error => error.ErrorMessage).ToArray()
              );

          return BadRequest(new { message = "Error de validación", errors = errors });
        }

        // Confirmar que contraseñas coinciden
        if (request.Contrasena != request.ConfirmarContrasena)
        {
          return BadRequest(new { message = "Las contraseñas no coinciden" });
        }

        // Verificar patrón esperado (segundo apellido + año)
        if (string.IsNullOrEmpty(request.Apellido2))
        {
          return BadRequest(new { message = "El segundo apellido es obligatorio para crear técnicos" });
        }

        var expected = request.Apellido2.ToLower() + request.FechaNacimiento.Year.ToString();
        if (request.Contrasena != expected)
        {
          return BadRequest(new { message = "La contraseña temporal no coincide con el patrón esperado (apellido2+year)" });
        }

        // Mapear al DTO original que usa el servicio (sin revalidar)
        var dto = new UsuarioCreateRequest
        {
          IdUsuario = request.IdUsuario,
          Nombre = request.Nombre,
          Apellido1 = request.Apellido1,
          Apellido2 = request.Apellido2,
          Email = request.Email,
          Telefono = request.Telefono,
          FechaNacimiento = request.FechaNacimiento,
          Contrasena = request.Contrasena,
          ConfirmarContrasena = request.ConfirmarContrasena,
          Rol = "TECNICO"
        };

        var usuarioRegistrado = await _usuarioService.RegistrarUsuarioAsync(dto);

        return CreatedAtAction(
            nameof(GetUsuarioPorId),
            new { id = usuarioRegistrado.IdUnico },
            usuarioRegistrado);
      }
      catch (ApplicationException ex)
      {
        return BadRequest(new
        {
          message = ex.Message,
          errors = new Dictionary<string, string[]>() {
                { "Contrasena", new[] { ex.Message } }
            }
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new
        {
          message = "Error interno del servidor",
          error = ex.Message
        });
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsuarioPorId(string id)
    {
      var usuario = await _usuarioService.GetByIdAsync(id);
      if (usuario == null)
        return NotFound();

      
      var response = new UsuarioResponse
      {
        IdUnico = usuario.IdUnico,
        IdUsuario = usuario.IdUsuario,
        Nombre = usuario.Nombre,
        Apellido1 = usuario.Apellido1,
        Apellido2 = usuario.Apellido2,
        Email = usuario.Email,
        Telefono = usuario.Telefono,
        TipoUsuario = usuario.TipoUsuario,
        Estado = usuario.Estado
      };

      return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUsuario()
    {
      var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userIdString))
      {
        return Unauthorized(new { message = "Token inválido o sin ID de usuario." });
      }

      var usuario = await _usuarioService.GetByIdAsync(userIdString);
      if (usuario == null)
      {
        return NotFound(new { message = "Usuario no encontrado." });
      }

      var response = new UsuarioResponse
      {
        IdUsuario = usuario.IdUsuario,
        Nombre = usuario.Nombre,
        Apellido1 = usuario.Apellido1,
        Apellido2 = usuario.Apellido2,
        Email = usuario.Email,
        Telefono = usuario.Telefono,
        TipoUsuario = usuario.TipoUsuario,
        Estado = usuario.Estado
      };

      return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarUsuario(string id, [FromBody] UsuarioUpdateRequest request)
    {
      try
      {
        if (!ModelState.IsValid)
          return BadRequest(ModelState);

        // Verificar que el usuario que hace la petición tiene permisos
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUser = await _usuarioService.GetByIdAsync(currentUserId);

        // Un usuario no puede modificarse a sí mismo (para evitar escalamiento de privilegios)
        if (id == currentUserId)
        {
          return BadRequest(new { message = "No puedes modificar tu propio usuario" });
        }

        var usuarioActualizado = await _usuarioService.ActualizarUsuarioAsync(id, request);

        if (usuarioActualizado == null)
          return NotFound(new { message = "Usuario no encontrado" });

        return Ok(usuarioActualizado);
      }
      catch (ApplicationException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error interno del servidor" + ex.Message });
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarUsuario(string id)
    {
      try
      {
        var eliminado = await _usuarioService.EliminarUsuarioAsync(id);

        if (!eliminado)
          return NotFound(new { message = "Usuario no encontrado" });

        return Ok(new { message = "Usuario eliminado correctamente" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error interno del servidor: " + ex.Message });
      }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
      // Validación del modelo antes de procesar
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var usuarioResponse = await _usuarioService.LoginAsync(request);
        var usuario = await _repository.GetByIdUsuarioAsync(usuarioResponse.IdUsuario);

        // Generar token JWT
        var token = _jwtService.GenerateToken(usuario);

        // Crea respuesta segura sin información sensible
        var response = new
        {
          Token = token,
          UserInfo = new
          {
            IdUnico = usuario.IdUnico,
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            TipoUsuario = usuario.TipoUsuario
          }
        };

        return Ok(response);
      }
      catch (ApplicationException ex)
      {
        return Unauthorized(new { message = ex.Message });
      }
      catch (FormatException)
      {
        return BadRequest(new { message = "El ID debe ser un número válido" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error interno del servidor" });
      }
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
      try
      {
        var usuarios = await _usuarioService.GetAllAsync();
        return Ok(usuarios);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error al obtener usuarios: " + ex.Message });
      }
    }

    // Endpoint público para listar técnicos (datos básicos) sin requerir autenticación
    [HttpGet("tecnicos-publicos")]
    public async Task<IActionResult> GetTecnicosPublicos()
    {
      try
      {
        var tecnicos = await _usuarioService.GetTecnicosAsync();
        return Ok(tecnicos);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error al obtener técnicos: " + ex.Message });
      }
    }

    [HttpGet("{usuarioId}/reservas")]
    public async Task<IActionResult> ObtenerReservasPorUsuario(string usuarioId)
    {
      try
      {
        var reservas = await _usuarioService.ObtenerReservasPorUsuarioAsync(usuarioId);
        return Ok(reservas);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }
  }

}