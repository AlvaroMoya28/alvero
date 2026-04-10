// EventosBackend/Services/EmailService.cs
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EventosBackend.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EventosBackend.Services
{
  public class EmailService : IEmailService
  {
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    

        public async Task<bool> SendReceiptEmailAsync(string email, string nombreUsuario, string numeroTransaccion, System.Collections.Generic.Dictionary<string, decimal> desglose, DateTime fechaPago, string metodoPago, decimal montoTotal)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            // Construir tabla de desglose
            var rows = "";
            foreach (var item in desglose)
            {
                rows += $"<tr><td style='padding:8px 12px;border-bottom:1px solid #eee'>{WebUtility.HtmlEncode(item.Key)}</td><td style='padding:8px 12px;border-bottom:1px solid #eee;text-align:right'>${item.Value:N2}</td></tr>";
            }

            string messageBody = $@"
            <!DOCTYPE html>
            <html lang=""es""> 
            <head><meta charset=""utf-8""/></head>
            <body style=""font-family: Segoe UI, sans-serif; background:#f4f4f4; margin:0; padding:20px;"">
                <table align=""center"" width=""100%"" style=""max-width:600px;background:#fff;border-radius:12px;overflow:hidden;"">
                    <tr style=""background:#1A4456;color:#fff""><td style=""padding:20px;text-align:center""><h1 style=""margin:0"">Recibo digital</h1></td></tr>
                    <tr><td style=""padding:20px"">
                        <p>Hola <strong>{WebUtility.HtmlEncode(nombreUsuario)}</strong>,</p>
                        <p>Gracias por tu pago. A continuación encontrarás el recibo de la transacción <strong>{WebUtility.HtmlEncode(numeroTransaccion)}</strong>.</p>
                        <table width=""100%"" style=""border-collapse:collapse;margin-top:12px"">
                            <thead>
                                <tr><th style=""text-align:left;padding:8px 12px;background:#f7f7f7"">Concepto</th><th style=""text-align:right;padding:8px 12px;background:#f7f7f7"">Importe</th></tr>
                            </thead>
                            <tbody>
                                {rows}
                                <tr><td style=""padding:8px 12px;font-weight:bold"">Total</td><td style=""padding:8px 12px;text-align:right;font-weight:bold"">${montoTotal:N2}</td></tr>
                            </tbody>
                        </table>
                        <p style=""margin-top:16px""><strong>Fecha pago:</strong> {fechaPago:dd/MM/yyyy HH:mm}<br/><strong>Método:</strong> {WebUtility.HtmlEncode(metodoPago)}</p>
                        <p style=""margin-top:8px;color:#666;font-size:13px"">Si tienes dudas sobre este recibo, por favor responde a este correo.</p>
                    </td></tr>
                    <tr><td style=""background:#f0f3f5;padding:12px;text-align:center;color:#888;font-size:12px"">&copy; {DateTime.Now.Year} DoctorPC</td></tr>
                </table>
            </body></html>";

            try
            {
                using var smtpClient = new SmtpClient(emailSettings["SmtpServer"]) { Port = int.Parse(emailSettings["Port"]), Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]), EnableSsl = true };
                using var mailMessage = new MailMessage { From = new MailAddress(emailSettings["FromAddress"], emailSettings["FromName"]), Subject = $"Recibo digital - {numeroTransaccion}", Body = messageBody, IsBodyHtml = true };
                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EMAIL] Recibo enviado a {email} (tx: {numeroTransaccion})");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando recibo: {ex}");
                return false;
            }
        }

        public async Task<bool> SendReservaReminderEmailAsync(string email, string nombreUsuario, string tecnico, DateTime fechaReserva, string direccion, string nombreReserva, string notas = null)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string messageBody = $@"
            <!DOCTYPE html>
            <html lang=""es""> <head><meta charset=""utf-8""/></head>
            <body style=""font-family: Segoe UI, sans-serif; background:#f4f4f4; margin:0; padding:20px;"">
                <table align=""center"" width=""100%"" style=""max-width:600px;background:#fff;border-radius:12px;overflow:hidden;"">
                    <tr style=""background:#1A4456;color:#fff""><td style=""padding:20px;text-align:center""><h1 style=""margin:0"">Recordatorio de reserva</h1></td></tr>
                    <tr><td style=""padding:20px"">
                        <p>Hola <strong>{WebUtility.HtmlEncode(nombreUsuario)}</strong>,</p>
                        <p>Este es un recordatorio de tu reserva <strong>{WebUtility.HtmlEncode(nombreReserva)}</strong>.</p>
                        <ul style=""line-height:1.6;color:#333"">
                            <li><strong>Fecha:</strong> {fechaReserva:dd/MM/yyyy HH:mm}</li>
                            <li><strong>Técnico:</strong> {WebUtility.HtmlEncode(tecnico)}</li>
                            <li><strong>Dirección:</strong> {WebUtility.HtmlEncode(direccion)}</li>
                            <li><strong>Nombre reserva:</strong> {WebUtility.HtmlEncode(nombreReserva)}</li>
                        </ul>
                        {(string.IsNullOrEmpty(notas) ? string.Empty : $"<p><strong>Notas:</strong><br/>{WebUtility.HtmlEncode(notas)}</p>")}
                        <p style=""margin-top:12px;color:#666;font-size:13px"">Por favor asegúrate de estar disponible en la fecha indicada y de proporcionar acceso al técnico si aplica.</p>
                    </td></tr>
                    <tr><td style=""background:#f0f3f5;padding:12px;text-align:center;color:#888;font-size:12px"">&copy; {DateTime.Now.Year} DoctorPC</td></tr>
                </table>
            </body></html>";

            try
            {
                using var smtpClient = new SmtpClient(emailSettings["SmtpServer"]) { Port = int.Parse(emailSettings["Port"]), Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]), EnableSsl = true };
                using var mailMessage = new MailMessage { From = new MailAddress(emailSettings["FromAddress"], emailSettings["FromName"]), Subject = "Recordatorio de reserva", Body = messageBody, IsBodyHtml = true };
                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EMAIL] Recordatorio enviado a {email} para fecha {fechaReserva}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando recordatorio: {ex}");
                return false;
            }
        }

        public async Task<bool> SendCitaConfirmadaEmailAsync(string email, string nombreCliente, string nombreTecnico, DateTime fechaCita, string horaInicio, string horaFin, string direccion, string descripcionProblema)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string messageBody = $@"
            <!DOCTYPE html>
            <html lang=""es""> 
            <head><meta charset=""utf-8""/></head>
            <body style=""font-family: Segoe UI, sans-serif; background:#f4f4f4; margin:0; padding:20px;"">
                <table align=""center"" width=""100%"" style=""max-width:600px;background:#fff;border-radius:12px;overflow:hidden;"">
                    <tr style=""background:#1A4456;color:#fff""><td style=""padding:20px;text-align:center""><h1 style=""margin:0"">✅ Cita Confirmada</h1></td></tr>
                    <tr><td style=""padding:20px"">
                        <p>Hola <strong>{WebUtility.HtmlEncode(nombreCliente)}</strong>,</p>
                        <p>Tu cita ha sido <strong>confirmada</strong> exitosamente. A continuación los detalles:</p>
                        <div style=""background:#f0f9ff;border-left:4px solid #0ea5e9;padding:16px;margin:16px 0;border-radius:4px"">
                            <ul style=""line-height:1.8;color:#333;margin:0;padding-left:20px"">
                                <li><strong>Fecha:</strong> {fechaCita:dd/MM/yyyy}</li>
                                <li><strong>Hora:</strong> {horaInicio} - {horaFin}</li>
                                <li><strong>Técnico:</strong> {WebUtility.HtmlEncode(nombreTecnico)}</li>
                                <li><strong>Dirección:</strong> {WebUtility.HtmlEncode(direccion)}</li>
                                {(!string.IsNullOrEmpty(descripcionProblema) ? $"<li><strong>Problema:</strong> {WebUtility.HtmlEncode(descripcionProblema)}</li>" : "")}
                            </ul>
                        </div>
                        <p style=""margin-top:12px;color:#666;font-size:13px"">Por favor asegúrate de estar disponible en la fecha y hora indicada. El técnico se presentará en la dirección proporcionada.</p>
                        <p style=""color:#666;font-size:13px"">Si necesitas cancelar o reprogramar, por favor contacta lo antes posible.</p>
                    </td></tr>
                    <tr><td style=""background:#f0f3f5;padding:12px;text-align:center;color:#888;font-size:12px"">&copy; {DateTime.Now.Year} DoctorPC</td></tr>
                </table>
            </body></html>";

            try
            {
                using var smtpClient = new SmtpClient(emailSettings["SmtpServer"]) { Port = int.Parse(emailSettings["Port"]), Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]), EnableSsl = true };
                using var mailMessage = new MailMessage { From = new MailAddress(emailSettings["FromAddress"], emailSettings["FromName"]), Subject = "Cita Confirmada - DoctorPC", Body = messageBody, IsBodyHtml = true };
                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EMAIL] Confirmación de cita enviada a {email} para fecha {fechaCita:dd/MM/yyyy} {horaInicio}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando confirmación de cita: {ex}");
                return false;
            }
        }

        public async Task<bool> SendCitaCanceladaEmailAsync(string email, string nombreCliente, string nombreTecnico, DateTime fechaCita, string horaInicio, string motivo = null)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string messageBody = $@"
            <!DOCTYPE html>
            <html lang=""es""> 
            <head><meta charset=""utf-8""/></head>
            <body style=""font-family: Segoe UI, sans-serif; background:#f4f4f4; margin:0; padding:20px;"">
                <table align=""center"" width=""100%"" style=""max-width:600px;background:#fff;border-radius:12px;overflow:hidden;"">
                    <tr style=""background:#dc2626;color:#fff""><td style=""padding:20px;text-align:center""><h1 style=""margin:0"">❌ Cita Cancelada</h1></td></tr>
                    <tr><td style=""padding:20px"">
                        <p>Hola <strong>{WebUtility.HtmlEncode(nombreCliente)}</strong>,</p>
                        <p>Lamentamos informarte que tu cita ha sido <strong>cancelada</strong>.</p>
                        <div style=""background:#fee;border-left:4px solid #dc2626;padding:16px;margin:16px 0;border-radius:4px"">
                            <ul style=""line-height:1.8;color:#333;margin:0;padding-left:20px"">
                                <li><strong>Fecha:</strong> {fechaCita:dd/MM/yyyy}</li>
                                <li><strong>Hora:</strong> {horaInicio}</li>
                                <li><strong>Técnico:</strong> {WebUtility.HtmlEncode(nombreTecnico)}</li>
                                {(!string.IsNullOrEmpty(motivo) ? $"<li><strong>Motivo:</strong> {WebUtility.HtmlEncode(motivo)}</li>" : "")}
                            </ul>
                        </div>
                        <p style=""margin-top:12px;color:#666;font-size:13px"">Si deseas agendar una nueva cita, puedes hacerlo en nuestro sitio web o contactarnos directamente.</p>
                        <p style=""color:#666;font-size:13px"">Disculpa las molestias ocasionadas.</p>
                    </td></tr>
                    <tr><td style=""background:#f0f3f5;padding:12px;text-align:center;color:#888;font-size:12px"">&copy; {DateTime.Now.Year} DoctorPC</td></tr>
                </table>
            </body></html>";

            try
            {
                using var smtpClient = new SmtpClient(emailSettings["SmtpServer"]) { Port = int.Parse(emailSettings["Port"]), Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]), EnableSsl = true };
                using var mailMessage = new MailMessage { From = new MailAddress(emailSettings["FromAddress"], emailSettings["FromName"]), Subject = "Cita Cancelada - DoctorPC", Body = messageBody, IsBodyHtml = true };
                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EMAIL] Cancelación de cita enviada a {email} para fecha {fechaCita:dd/MM/yyyy} {horaInicio}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando cancelación de cita: {ex}");
                return false;
            }
        }

        public async Task<bool> SendCitaCompletadaEmailAsync(string email, string nombreCliente, string nombreTecnico, DateTime fechaCita, string horaInicio, string horaFin, string direccion, string descripcionProblema)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string messageBody = @$"<!DOCTYPE html>
            <html lang=""es"">
            <head><meta charset=""utf-8""/></head>
            <body style=""font-family: Segoe UI, sans-serif; background:#f4f4f4; margin:0; padding:20px;"">
                <table align=""center"" width=""100%"" style=""max-width:600px;background:#fff;border-radius:12px;overflow:hidden;"">
                    <tr style=""background:#16a34a;color:#fff""><td style=""padding:20px;text-align:center""><h1 style=""margin:0"">✅ Cita Completada</h1></td></tr>
                    <tr><td style=""padding:20px"">
                        <p>Hola <strong>{WebUtility.HtmlEncode(nombreCliente)}</strong>,</p>
                        <p>Tu cita ha sido <strong>completada</strong> exitosamente. A continuación los detalles:</p>
                        <div style=""background:#f0fff4;border-left:4px solid #16a34a;padding:16px;margin:16px 0;border-radius:4px"">
                            <ul style=""line-height:1.8;color:#333;margin:0;padding-left:20px"">
                                <li><strong>Fecha:</strong> {fechaCita:dd/MM/yyyy}</li>
                                <li><strong>Hora:</strong> {horaInicio} - {horaFin}</li>
                                <li><strong>Técnico:</strong> {WebUtility.HtmlEncode(nombreTecnico)}</li>
                                <li><strong>Dirección:</strong> {WebUtility.HtmlEncode(direccion)}</li>
                                {(string.IsNullOrEmpty(descripcionProblema) ? string.Empty : "<li><strong>Problema atendido:</strong> " + WebUtility.HtmlEncode(descripcionProblema) + "</li>")}
                            </ul>
                        </div>
                        <p style=""margin-top:12px;color:#666;font-size:13px"">Guarda este correo como comprobante. Este registro servirá para efectos de garantía.</p>
                        <p style=""color:#666;font-size:13px"">Si necesitas agendar otra cita o soporte adicional, puedes hacerlo en nuestro sitio web.</p>
                    </td></tr>
                    <tr><td style=""background:#f0f3f5;padding:12px;text-align:center;color:#888;font-size:12px"">&copy; {DateTime.Now.Year} DoctorPC</td></tr>
                </table>
            </body></html>";

            try
            {
                using var smtpClient = new SmtpClient(emailSettings["SmtpServer"]) { Port = int.Parse(emailSettings["Port"]), Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]), EnableSsl = true };
                using var mailMessage = new MailMessage { From = new MailAddress(emailSettings["FromAddress"], emailSettings["FromName"]), Subject = "Cita Completada - DoctorPC", Body = messageBody, IsBodyHtml = true };
                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EMAIL] Cita completada enviada a {email} para {fechaCita:dd/MM/yyyy} {horaInicio}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo de cita completada: {ex}");
                return false;
            }
        }
  }
}