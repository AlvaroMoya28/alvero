using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventosBackend.Models.Entities
{
    [Table("RESENA", Schema = "EVENTOS")]
    public class Resena
    {
        [Key]
        [Column("ID_RESENA")]
        public int Id { get; set; }

        [Column("ID_RESERVA")]
        public int IdReserva { get; set; }

        // Propiedad de navegación a Reserva
        [ForeignKey("IdReserva")]
        public virtual Reserva? Reserva { get; set; }

        [Column("ID_USUARIO")]
        [MaxLength(50)]
        public string IdUsuario { get; set; } = string.Empty;

        [Column("CALIFICACION")]
        [Range(1, 5)]
        public int Calificacion { get; set; }

        [Column("COMENTARIO")]
        [MaxLength(1000)]
        public string Comentario { get; set; } = string.Empty;

        [Column("FECHA_CREACION")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
