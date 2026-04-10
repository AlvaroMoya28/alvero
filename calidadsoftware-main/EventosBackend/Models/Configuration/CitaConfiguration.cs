using EventosBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventosBackend.Models.Configuration
{
    public class CitaConfiguration : IEntityTypeConfiguration<Cita>
    {
        public void Configure(EntityTypeBuilder<Cita> builder)
        {
            builder.ToTable("CITA");

            builder.HasKey(c => c.IdCita);

            builder.Property(c => c.IdCita)
                .HasColumnName("ID_CITA")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.IdUsuarioCliente)
                .HasColumnName("ID_USUARIO_CLIENTE")
                .IsRequired(false);

            builder.Property(c => c.NombreCliente)
                .HasColumnName("NOMBRE_CLIENTE")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(c => c.EmailCliente)
                .HasColumnName("EMAIL_CLIENTE")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(c => c.CedulaCliente)
                .HasColumnName("CEDULA_CLIENTE")
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(c => c.IdUsuarioTecnico)
                .HasColumnName("ID_USUARIO_TECNICO")
                .IsRequired();

            builder.Property(c => c.FechaCita)
                .HasColumnName("FECHA_CITA")
                .IsRequired();

            builder.Property(c => c.HoraInicio)
                .HasColumnName("HORA_INICIO")
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(c => c.HoraFin)
                .HasColumnName("HORA_FIN")
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(c => c.DescripcionProblema)
                .HasColumnName("DESCRIPCION_PROBLEMA")
                .HasMaxLength(1000);

            builder.Property(c => c.Estado)
                .HasColumnName("ESTADO")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.Direccion)
                .HasColumnName("DIRECCION")
                .HasMaxLength(300);

            builder.Property(c => c.TelefonoContacto)
                .HasColumnName("TELEFONO_CONTACTO")
                .HasMaxLength(20);

            builder.Property(c => c.FechaCreacion)
                .HasColumnName("FECHA_CREACION")
                .IsRequired();

            builder.Property(c => c.FechaActualizacion)
                .HasColumnName("FECHA_ACTUALIZACION");

            // Relationships
            builder.HasOne(c => c.Cliente)
                .WithMany()
                .HasForeignKey(c => c.IdUsuarioCliente)
                .HasPrincipalKey(u => u.IdUnico)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(c => c.Tecnico)
                .WithMany()
                .HasForeignKey(c => c.IdUsuarioTecnico)
                .HasPrincipalKey(u => u.IdUnico)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
