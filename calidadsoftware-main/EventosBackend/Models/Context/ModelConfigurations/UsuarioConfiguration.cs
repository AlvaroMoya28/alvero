using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventosBackend.Models.Entities;

namespace EventosBackend.Models.Context.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("USUARIO");
            builder.HasKey(u => u.IdUsuario);
            
            builder.Property(u => u.IdUsuario)
                .HasColumnName("ID_USUARIO")
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Nombre)
                .HasColumnName("NOMBRE")
                .IsRequired()
                .HasMaxLength(100);

            // ... otras configuraciones de propiedades
        }
    }
}