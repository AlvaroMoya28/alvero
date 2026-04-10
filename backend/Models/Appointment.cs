namespace Vero.Backend.Models;

public class Appointment
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string AttendeeName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
}
