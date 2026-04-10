using Microsoft.EntityFrameworkCore;
using EventosBackend.Models.Entities;

namespace EventosBackend.Models.Context
{
  public class OracleDbContext : DbContext
  {
    public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options)
    {
      // Configuración adicional del constructor si es necesaria
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<PAGO> PAGOS { get; set; }
    // public DbSet<FACTURA> Facturas { get; set; }
    public DbSet<Resena> Resenas { get; set; }
    public DbSet<TIPO_EVENTO> TIPO_EVENTO { get; set; }
    public DbSet<TecnicoHorario> TecnicoHorarios { get; set; }
    public DbSet<Cita> Citas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Configuración específica para Oracle
      modelBuilder.HasDefaultSchema("EVENTOS"); // Especifica tu esquema Oracle

      // Configuración global para nombres en mayúsculas
      foreach (var entity in modelBuilder.Model.GetEntityTypes())
      {
        // Tablas
        entity.SetTableName(entity.GetTableName().ToUpper());

        // Columnas
        foreach (var property in entity.GetProperties())
        {
          property.SetColumnName(property.GetColumnName().ToUpper());
        }

        // Claves
        foreach (var key in entity.GetKeys())
        {
          key.SetName(key.GetName().ToUpper());
        }

        // Índices
        foreach (var index in entity.GetIndexes())
        {
          index.SetDatabaseName(index.GetDatabaseName().ToUpper());
        }
      }

      // Configuraciones específicas por entidad
      modelBuilder.Entity<Usuario>(entity =>
      {
        entity.ToTable("USUARIO");
        entity.HasKey(u => u.IdUsuario).HasName("PK_USUARIO");
        entity.HasIndex(u => u.IdUsuario).IsUnique(); // El "username" debe ser único

        entity.Property(u => u.IdUnico)
                  .HasColumnName("ID_UNICO")
                  .ValueGeneratedOnAdd();

        entity.Property(u => u.IdUsuario)
                  .HasColumnName("ID_USUARIO")
                  .IsRequired()
                  .HasMaxLength(50);

        entity.Property(u => u.Nombre)
                  .HasColumnName("NOMBRE")
                  .IsRequired()
                  .HasMaxLength(100);

        entity.Property(u => u.Email)
                  .HasColumnName("EMAIL")
                  .IsRequired()
                  .HasMaxLength(100);

        entity.Property(u => u.ContrasenaHash)
                  .HasColumnName("CONTRASENA_HASH")
                  .IsRequired();

        entity.Property(u => u.Salt)
                  .HasColumnName("SALT")
                  .IsRequired();

        entity.Property(u => u.TipoUsuario)
                  .HasColumnName("TIPO_USUARIO")
                  .HasDefaultValue("CLIENTE")
                  .IsRequired();

        entity.Property(u => u.Estado)
                  .HasColumnName("ESTADO")
                  .HasDefaultValue("ACTIVO")
                  .IsRequired();

        entity.HasIndex(u => u.IdUsuario)
                  .IsUnique()
                  .HasDatabaseName("IDX_USUARIO_ID_USUARIO");
      });

      modelBuilder.Entity<Reserva>(entity =>
      {
        entity.ToTable("RESERVA");
        entity.HasKey(r => r.IdReserva);

        // Relación string -> string
        entity.HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.IdUsuario) // La FK en Reserva es IdUnicoUsuario (string)
            .HasPrincipalKey(u => u.IdUsuario) // PK en Usuario: IdUsuario (string)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Reserva <-> Resena (1 a muchos)
        entity.HasMany(r => r.Resenas)
            .WithOne(res => res.Reserva)
            .HasForeignKey(res => res.IdReserva)
            .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<PAGO>(entity =>
           {
             entity.ToTable("PAGO");
             entity.HasKey(p => p.Id);

             // Indicar que la base de datos genera el valor de la PK
             entity.Property(p => p.Id)
                  .HasColumnName("ID_PAGO")
                  .ValueGeneratedOnAdd();

             entity.Property(p => p.Monto).HasColumnType("NUMBER(10,2)");

             // Relación con Usuario
             entity.HasOne<Usuario>()
                  .WithMany()
                  .HasForeignKey(p => p.IdUsuarioResponsable)
                  .HasPrincipalKey(u => u.IdUsuario) // FK(string) -> PK(string)
                  .OnDelete(DeleteBehavior.Restrict);

             // Aquí iría la relación con FACTURA cuando se tenga
             // entity.HasOne<FACTURA>().WithMany().HasForeignKey(p => p.IdFactura);
           });











      modelBuilder.Entity<Usuario>(entity =>
      {
        entity.ToTable("USUARIO");
        entity.HasKey(u => u.IdUsuario).HasName("PK_USUARIO");

        entity.Property(u => u.IdUsuario)
                  .HasColumnName("ID_USUARIO")
                  .ValueGeneratedOnAdd();
      });

      modelBuilder.Entity<Reserva>(entity =>
      {
        entity.ToTable("RESERVA");
      });





      // Apply configurations for scheduling system
      modelBuilder.ApplyConfiguration(new EventosBackend.Models.Configuration.TecnicoHorarioConfiguration());
      modelBuilder.ApplyConfiguration(new EventosBackend.Models.Configuration.CitaConfiguration());

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        // Solo para desarrollo/debug - no usar en producción
        optionsBuilder.UseOracle("TU_CADENA_CONEXION")
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
      }
    }
  }
}