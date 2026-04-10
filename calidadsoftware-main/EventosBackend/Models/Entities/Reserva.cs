using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventosBackend.Models.Entities
{
    [Table("RESERVA")]
    public class Reserva
    {
        [Key]
        [Column("ID_RESERVA")]
        public int IdReserva { get; set; }

        [Required]
        [Column("ID_USUARIO")]
        public string IdUsuario { get; set; } = string.Empty;

        [Required]
        [Column("ID_SALA")]
        public int IdSala { get; set; }

        [Column("FECHA_INICIO")]
        public DateTime FechaInicio { get; set; }

        [Column("FECHA_FIN")]
        public DateTime FechaFin { get; set; }

        [Column("ASISTENTES_ESPERADOS")]
        public int? AsistentesEsperados { get; set; }

        [Column("ESTADO")]
        [StringLength(20)]
        public string Estado { get; set; } = string.Empty;

        [Column("OBSERVACIONES")]
        [StringLength(500)]
        public string? Observaciones { get; set; }

        [Column("FECHA_CREACION")]
        public DateTime FechaCreacion { get; set; }

        [Column("ID_CATERING")]
        public int? IdCatering { get; set; }

        [Column("PRECIO_TOTAL")]
        public decimal? PrecioTotal { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }

        // [ForeignKey("IdSala")]
        // public virtual SALA? SALA { get; set; } // Deshabilitado: Sala eliminada

        // Nueva propiedad de navegación: Reseñas asociadas a la reserva
        public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    }
}