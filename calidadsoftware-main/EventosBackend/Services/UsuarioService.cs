using AutoMapper;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using EventosBackend.Models.Entities;
using EventosBackend.Repositories.Interfaces;
using EventosBackend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EventosBackend.Services
{
  public class UsuarioService : IUsuarioService
  {
    private readonly IUsuarioRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioService(
        IUsuarioRepository repository,
        IMapper mapper,
        IPasswordHasher passwordHasher)
    {
      _repository = repository;
      _mapper = mapper;
      _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<UsuarioResponse>> GetAllAsync()
    {
      var usuarios = await _repository.GetAllAsync();
      return _mapper.Map<IEnumerable<UsuarioResponse>>(usuarios);
    }

    public async Task<IEnumerable<UsuarioResponse>> GetTecnicosAsync()
    {
      var tecnicos = await _repository.GetTecnicosAsync();
      return _mapper.Map<IEnumerable<UsuarioResponse>>(tecnicos);
    }

    public async Task<UsuarioResponse> GetByIdAsync(string id)
    {
      var usuario = await _repository.GetByIdAsync(id);
      return _mapper.Map<UsuarioResponse>(usuario);
    }

  public async Task<UsuarioResponse> GetAuthenticatedUserAsync(ClaimsPrincipal user)
    {
      if (user == null)
        throw new ArgumentNullException(nameof(user));

      var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (string.IsNullOrEmpty(userIdClaim))
      throw new ApplicationException("No se pudo extraer el ID del token.");

      var usuario = await _repository.GetByIdAsync(userIdClaim);

      if (usuario == null)
        throw new ApplicationException("Usuario no encontrado.");

      return _mapper.Map<UsuarioResponse>(usuario);
    }

    public async Task<UsuarioResponse> RegistrarUsuarioAsync(UsuarioCreateRequest registroDto)
    {
      // Validar si el email ya existe
      if (await EmailExisteAsync(registroDto.Email))
        throw new ApplicationException("El email ya está registrado");

      // Validar si el ID de usuario ya existe
      if (await _repository.ExisteIdUsuarioAsync(registroDto.IdUsuario))
        throw new ApplicationException("El ID de usuario ya está en uso");

      // Crear hash de la contraseña
      var (hashBytes, saltBytes) = _passwordHasher.CreateHash(registroDto.Contrasena);

      // Convertir a Base64
      string hashBase64 = Convert.ToBase64String(hashBytes);
      string saltBase64 = Convert.ToBase64String(saltBytes);

      // Mapear DTO a entidad
      var usuario = _mapper.Map<Usuario>(registroDto);
      usuario.ContrasenaHash = hashBase64;
      usuario.Salt = saltBase64;
      usuario.TipoUsuario = registroDto.Rol;
      usuario.Estado = "ACTIVO"; // Valor por defecto
      usuario.FechaRegistro = DateTime.UtcNow;

      // Guardar en base de datos
      var usuarioCreado = await _repository.CreateAsync(usuario);

      // Mapear a DTO de respuesta
      return _mapper.Map<UsuarioResponse>(usuarioCreado);
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
      return await _repository.ExisteEmailAsync(email);
    }
    public async Task<UsuarioResponse> ActualizarUsuarioAsync(string id, UsuarioUpdateRequest request)
    {
      // Buscar el usuario en la base de datos
      var usuario = await _repository.GetByIdAsync(id);
      if (usuario == null)
        return null; // Retorna null si no existe el usuario

      // Verifica si el correo ha cambiado y si ya existe otro usuario con ese correo
      if (request.Email != usuario.Email && await EmailExisteAsync(request.Email))
        throw new ApplicationException("El email ya está registrado");

      // Actualizar los campos del usuario
      usuario.Nombre = request.Nombre ?? usuario.Nombre;
      usuario.Email = request.Email ?? usuario.Email;
      usuario.Telefono = request.Telefono ?? usuario.Telefono;
      usuario.Apellido1 = request.Apellido1 ?? usuario.Apellido1;
      usuario.Apellido2 = request.Apellido2 ?? usuario.Apellido2;
      usuario.TipoUsuario = request.TipoUsuario ?? usuario.TipoUsuario;
      usuario.FechaNacimiento = request.FechaNacimiento ?? usuario.FechaNacimiento;
      usuario.FechaRegistro = request.FechaRegistro ?? usuario.FechaRegistro;
      usuario.UltimoLogin = request.UltimoLogin ?? usuario.UltimoLogin;
      usuario.Estado = request.Estado ?? usuario.Estado;
      // Agrega más campos si es necesario

      // Si la contraseña fue proporcionada, actualiza también
      if (!string.IsNullOrEmpty(request.Contrasena))
      {
        var (hashBytes, saltBytes) = _passwordHasher.CreateHash(request.Contrasena);
        usuario.ContrasenaHash = Convert.ToBase64String(hashBytes);
        usuario.Salt = Convert.ToBase64String(saltBytes);
      }

      // Guardar los cambios en la base de datos
      var usuarioActualizado = await _repository.UpdateAsync(usuario);

      // Mapear a DTO de respuesta
      return _mapper.Map<UsuarioResponse>(usuarioActualizado);
    }
    public async Task<UsuarioResponse> GetByEmailAsync(string email)
    {
      // Obtener el usuario del repositorio
      var usuario = await _repository.GetByEmailAsync(email);

      // Si no se encuentra el usuario, retornar null
      if (usuario == null)
        return null;

      // Mapear a DTO de respuesta
      return _mapper.Map<UsuarioResponse>(usuario);
    }

    public async Task<bool> ResetPasswordAsync(string email, string newPassword)
    {
      // 1. Obtener el usuario por email
      var usuario = await _repository.GetByEmailAsync(email);
      if (usuario == null)
        return false; // No se encontró el usuario

      // 2. Generar nuevo hash para la contraseña
      var (hashBytes, saltBytes) = _passwordHasher.CreateHash(newPassword);
      usuario.ContrasenaHash = Convert.ToBase64String(hashBytes);
      usuario.Salt = Convert.ToBase64String(saltBytes);

      // 3. Actualizar el usuario en la base de datos
      var usuarioActualizado = await _repository.UpdateAsync(usuario);

      // 4. Retornar true si se actualizó correctamente
      return usuarioActualizado != null;
    }

    public async Task<UsuarioResponse> LoginAsync(LoginRequest loginRequest)
    {
      // Buscar usuario por ID
      var usuario = await _repository.GetByIdUsuarioAsync(loginRequest.Id);
      if (usuario == null)
        throw new ApplicationException("Usuario no encontrado");

      // Verificar si el usuario está activo
      if (usuario.Estado != "ACTIVO")
        throw new ApplicationException("Usuario inactivo o bloqueado");

      // Verificar contraseña
      var hashBytes = Convert.FromBase64String(usuario.ContrasenaHash);
      var saltBytes = Convert.FromBase64String(usuario.Salt);

      if (!_passwordHasher.VerifyPassword(loginRequest.Password, hashBytes, saltBytes))
        throw new ApplicationException("Contraseña incorrecta");

      // Actualizar último login
      usuario.UltimoLogin = DateTime.UtcNow;
      await _repository.UpdateAsync(usuario);

      return _mapper.Map<UsuarioResponse>(usuario);
    }
    public async Task<bool> EliminarUsuarioAsync(string id)
    {
      var usuario = await _repository.GetByIdAsync(id);
      if (usuario == null)
        return false;

      await _repository.DeleteAsync(usuario);
      return true;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
      var usuario = await _repository.GetByIdAsync(userId);
      if (usuario == null)
      {
        // Esto no debería suceder si el userId proviene de un token válido.
        throw new ApplicationException("Usuario no encontrado.");
      }

      // Verificar la contraseña actual
      var currentPasswordHashBytes = Convert.FromBase64String(usuario.ContrasenaHash);
      var currentPasswordSaltBytes = Convert.FromBase64String(usuario.Salt);

      if (!_passwordHasher.VerifyPassword(currentPassword, currentPasswordHashBytes, currentPasswordSaltBytes))
      {
        // Contraseña actual incorrecta. Devolver false para un manejo específico en el controlador.
        return false;
      }

      // Si la contraseña actual es correcta, hashear y guardar la nueva.
      var (newHashBytes, newSaltBytes) = _passwordHasher.CreateHash(newPassword);
      usuario.ContrasenaHash = Convert.ToBase64String(newHashBytes);
      usuario.Salt = Convert.ToBase64String(newSaltBytes);

      var updatedUsuario = await _repository.UpdateAsync(usuario);
      return updatedUsuario != null;
    }

        public async Task<IEnumerable<ReservaResponse>> ObtenerReservasPorUsuarioAsync(string usuarioId)
        {
            var reservas = await _repository.ObtenerReservasPorUsuarioAsync(usuarioId);
            var responses = new List<ReservaResponse>();
            foreach (var reserva in reservas)
            {
                // Verificar si el usuario ya dejó reseña para esta reserva
                bool tieneResena = reserva.Resenas != null && reserva.Resenas.Any(r => r.IdUsuario == usuarioId);
                responses.Add(new ReservaResponse
                {
                    IdReserva = reserva.IdReserva,
                    IdUsuario = reserva.IdUsuario,
                    FechaInicio = reserva.FechaInicio,
                    FechaFin = reserva.FechaFin,
                    Estado = reserva.Estado,
                    FechaCreacion = reserva.FechaCreacion,
                    IdSala = reserva.IdSala,
                    SalaNombre = "",
                    SalaDescripcionCorta = "",
                    SalaPrecioBase = 0,
                    TieneResena = tieneResena,
                    PrecioTotal = reserva.PrecioTotal
                });
            }
            return responses;
        }
  }
}
