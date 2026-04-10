using System.ComponentModel.DataAnnotations;

namespace EventosBackend.Models.DTOs.Requests
{
    public class CreatePaymentIntentRequest
    {
        [Required(ErrorMessage = "El ID de la reserva es obligatorio para iniciar un pago.")]
        public int IdReserva { get; set; }
    }
}