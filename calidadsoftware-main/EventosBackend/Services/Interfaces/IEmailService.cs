using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace EventosBackend.Services.Interfaces
{
  public interface IEmailService
  {

    Task<bool> SendReceiptEmailAsync(string email, string nombreUsuario, string numeroTransaccion, Dictionary<string, decimal> desglose, DateTime fechaPago, string metodoPago, decimal montoTotal);
    Task<bool> SendReservaReminderEmailAsync(string email, string nombreUsuario, string tecnico, DateTime fechaReserva, string direccion, string nombreReserva, string notas = null);
    Task<bool> SendCitaConfirmadaEmailAsync(string email, string nombreCliente, string nombreTecnico, DateTime fechaCita, string horaInicio, string horaFin, string direccion, string descripcionProblema);
    Task<bool> SendCitaCanceladaEmailAsync(string email, string nombreCliente, string nombreTecnico, DateTime fechaCita, string horaInicio, string motivo = null);
    Task<bool> SendCitaCompletadaEmailAsync(string email, string nombreCliente, string nombreTecnico, DateTime fechaCita, string horaInicio, string horaFin, string direccion, string descripcionProblema);
    // Task<bool> SendWelcomeEmailAsync(string email, string userName);
    // Task<bool> SendNotificationEmailAsync(string email, string message);
  }
}