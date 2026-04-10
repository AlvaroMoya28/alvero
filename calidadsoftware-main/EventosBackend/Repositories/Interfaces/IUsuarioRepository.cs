using EventosBackend.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventosBackend.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetTecnicosAsync();
        Task<Usuario?> GetByIdAsync(string id);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<bool> ExisteEmailAsync(string email);
        Task<Usuario> UpdateAsync(Usuario usuario);
        Task DeleteAsync(Usuario usuario);
        Task<Usuario> GetByEmailAsync(string email);
        Task<bool> ExisteIdUsuarioAsync(string idUsuario);
        Task<Usuario> GetByIdUsuarioAsync(string idUsuario);
        Task<IEnumerable<Reserva>> ObtenerReservasPorUsuarioAsync(string usuarioId);
    }   
}