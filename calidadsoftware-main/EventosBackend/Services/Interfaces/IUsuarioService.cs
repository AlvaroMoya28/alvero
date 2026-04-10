using EventosBackend.Controllers;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EventosBackend.Services.Interfaces
{
  public interface IUsuarioService
  {
    Task<IEnumerable<UsuarioResponse>> GetAllAsync();
    Task<IEnumerable<UsuarioResponse>> GetTecnicosAsync();
    Task<UsuarioResponse> GetByIdAsync(string id);
    Task<UsuarioResponse> GetAuthenticatedUserAsync(ClaimsPrincipal user);
    Task<UsuarioResponse> RegistrarUsuarioAsync(UsuarioCreateRequest registroDto);
    Task<bool> EmailExisteAsync(string email);
    Task<UsuarioResponse> ActualizarUsuarioAsync(string id, UsuarioUpdateRequest request);
    Task<UsuarioResponse> LoginAsync(LoginRequest loginRequest);
    Task<bool> EliminarUsuarioAsync(string id);
    Task<UsuarioResponse> GetByEmailAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string newPassword);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<IEnumerable<ReservaResponse>> ObtenerReservasPorUsuarioAsync(string usuarioId);
  }
}