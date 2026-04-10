using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventosBackend.Models.Entities
{
    [Table("FAVORITOS")]
    public class FAVORITO
    {
        [Column("USUARIO_ID", Order = 0)]
        public string UsuarioId { get; set; }

        [Key, Column("SALA_ID", Order = 1)]
        public int SalaId { get; set; }

        [Column("FECHA_AGREGADO")]
        public DateTime FechaAgregado { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;

        // [ForeignKey("SalaId")]
        // public SALA SALA { get; set; } = null!; // Deshabilitado: Sala eliminada
    }
}
