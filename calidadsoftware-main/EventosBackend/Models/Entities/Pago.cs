using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventosBackend.Models.Entities
{
    [Table("PAGO")]
    public class PAGO
    {
        [Key]
        [Column("ID_PAGO")]
        public int Id { get; set; }

        [Required]
        [Column("ID_FACTURA")]
        public int? IdFactura { get; set; }


        [Required]
        [Column("MONTO")]
        public decimal Monto { get; set; }

        // Relación con RESERVA
        [Required]
        [Column("ID_RESERVA")]
        public int IdReserva { get; set; }

        [ForeignKey("IdReserva")]
        public virtual Reserva Reserva { get; set; }

        [Required]
        [Column("FECHA_PAGO")]
        public DateTime FechaPago { get; set; }

        [Required]
        [Column("METODO_PAGO")]
        [MaxLength(50)]
        public string MetodoPago { get; set; }

        [Column("REFERENCIA_PAGO")]
        [MaxLength(100)]
        public string? ReferenciaPago { get; set; }

        [Required]
        [Column("ESTADO")]
        [MaxLength(20)]
        public string Estado { get; set; }

        [Required]
        [Column("ID_USUARIO_RESPONSABLE")]
        public string IdUsuarioResponsable { get; set; }
    }
}