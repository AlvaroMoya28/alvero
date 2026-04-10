using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventosBackend.Models.Entities
{
  [Table("USUARIO")]
  public class Usuario
  {
    [Key]
    [Column("ID_UNICO")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string IdUnico { get; set; }

    [Required]
    [Column("ID_USUARIO")]
    [StringLength(50)]
    public string IdUsuario { get; set; }

    [Required]
    [Column("NOMBRE")]
    [StringLength(100)]
    public string Nombre { get; set; }

    [Required]
    [Column("APELLIDO1")]
    [StringLength(150)]
    public string Apellido1 { get; set; }

    [Column("APELLIDO2")]
    [StringLength(150)]
    public string? Apellido2 { get; set; }

    [Required]
    [EmailAddress]
    [Column("EMAIL")]
    [StringLength(100)]
    public string Email { get; set; }

    [Column("TELEFONO")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [Required]
    [Column("TIPO_USUARIO")]
    [StringLength(20)]
    public string TipoUsuario { get; set; }

    [Required]
    [Column("CONTRASENA_HASH")]
    [StringLength(255)] // Eliminar
    public string ContrasenaHash { get; set; }

    [Required]
    [Column("SALT")]
    [StringLength(100)] // Eliminar
    public string Salt { get; set; }

    [Column("FECHA_REGISTRO")]
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("FECHA_NACIMIENTO")]
    public DateTime FechaNacimiento { get; set; }

    [Column("ULTIMO_LOGIN")]
    public DateTime? UltimoLogin { get; set; }

    [Required]
    [Column("ESTADO")]
    [StringLength(10)]
    public string Estado { get; set; } = "ACTIVO";
        
    [Column("STRIPE_CUSTOMER_ID")]
    [StringLength(255)]
    public string? StripeCustomerId { get; set; }
    }
}