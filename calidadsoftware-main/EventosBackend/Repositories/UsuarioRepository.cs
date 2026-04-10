using EventosBackend.Models.Context;
using EventosBackend.Models.Entities;
using EventosBackend.Repositories;
using EventosBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventosBackend.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly OracleDbContext _context;

        public UsuarioRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetTecnicosAsync()
        {
            return await _context.Usuarios
                .Where(u => u.TipoUsuario == "TECNICO")
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(string id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<Usuario> UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task DeleteAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExisteIdUsuarioAsync(string idUsuario)
        {
            return await _context.Usuarios.AnyAsync(u => u.IdUsuario == idUsuario);
        }

        public async Task<Usuario> GetByIdUsuarioAsync(string idUsuario)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
        }

        public async Task<IEnumerable<Reserva>> ObtenerReservasPorUsuarioAsync(string usuarioId)
        {
            return await _context.Reservas
                .Where(r => r.IdUsuario == usuarioId)
                .Include(r => r.Resenas)
                .ToListAsync();
        }

        public OracleDbContext Context => _context;
    }
}