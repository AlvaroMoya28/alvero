using EventosBackend.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventosBackend.Repositories.Interfaces
{
    public interface ITipoEventoRepository
    {
        Task<IEnumerable<TIPO_EVENTO>> ObtenerTodosAsync();
        Task<TIPO_EVENTO?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(TIPO_EVENTO tipoEvento);
        Task ActualizarAsync(TIPO_EVENTO tipoEvento);
        Task EliminarAsync(TIPO_EVENTO tipoEvento);
    }
}