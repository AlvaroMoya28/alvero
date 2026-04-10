using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventosBackend.Models.Entities
{
    [Table("TIPO_EVENTO")]
    public class TIPO_EVENTO
    {
        [Key]
        [Column("ID_TIPO_EVENTO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTipoEvento { get; set; }

        [Required]
        [Column("NOMBRE")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Column("DESCRIPCION")]
        [StringLength(200)]
        public string? Descripcion { get; set; }

        [Column("ICONO")]
        [StringLength(50)]
        public string? Icono { get; set; }

        // public virtual ICollection<SALA> Salas { get; set; } = new List<SALA>(); // Deshabilitado: Sala eliminada
        // public virtual ICollection<SALA_TIPO_EVENTO> SalaTipoEventos { get; set; } = new List<SALA_TIPO_EVENTO>(); // Deshabilitado: Sala eliminada
    }
}