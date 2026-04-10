using EventosBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventosBackend.Models.Configuration
{
    public class TecnicoHorarioConfiguration : IEntityTypeConfiguration<TecnicoHorario>
    {
        public void Configure(EntityTypeBuilder<TecnicoHorario> builder)
        {
            builder.ToTable("TECNICO_HORARIO");

            builder.HasKey(t => t.IdHorario);

            builder.Property(t => t.IdHorario)
                .HasColumnName("ID_HORARIO")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.IdUsuario)
                .HasColumnName("ID_USUARIO")
                .IsRequired();

            builder.Property(t => t.Fecha)
                .HasColumnName("FECHA")
                .IsRequired();

            builder.Property(t => t.HoraInicio)
                .HasColumnName("HORA_INICIO")
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(t => t.HoraFin)
                .HasColumnName("HORA_FIN")
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(t => t.Disponible)
                .HasColumnName("DISPONIBLE")
                .HasConversion(
                    v => v ? "1" : "0",
                    v => v == "1")
                .HasMaxLength(1)
                .IsRequired();

            builder.Property(t => t.MotivoBloqueo)
                .HasColumnName("MOTIVO_BLOQUEO")
                .HasMaxLength(200);

            builder.Property(t => t.FechaCreacion)
                .HasColumnName("FECHA_CREACION")
                .IsRequired();

            // Relationship
            builder.HasOne(t => t.Usuario)
                .WithMany()
                .HasForeignKey(t => t.IdUsuario)
                .HasPrincipalKey(u => u.IdUnico)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
