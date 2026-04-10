using Vero.Backend.Models;

namespace Vero.Backend.Repositories;

public static class EventRepository
{
    private static readonly List<Event> Events = new()
    {
        new Event { Id = 1, Title = "Revisión de sistema", Description = "Reunión técnica para revisar el sistema", Date = DateTime.Today.AddDays(3), Location = "Sala 1" },
        new Event { Id = 2, Title = "Capacitación de operadores", Description = "Sesión de capacitación para el equipo de soporte", Date = DateTime.Today.AddDays(7), Location = "Auditorio" }
    };

    private static readonly List<Appointment> Appointments = new();

    public static IEnumerable<Event> GetEvents() => Events;
    public static Event? GetEvent(int id) => Events.FirstOrDefault(e => e.Id == id);

    public static Event AddEvent(Event evt)
    {
        evt.Id = Events.Any() ? Events.Max(e => e.Id) + 1 : 1;
        Events.Add(evt);
        return evt;
    }

    public static IEnumerable<Appointment> GetAppointments() => Appointments;

    public static Appointment AddAppointment(Appointment appointment)
    {
        appointment.Id = Appointments.Any() ? Appointments.Max(a => a.Id) + 1 : 1;
        Appointments.Add(appointment);
        return appointment;
    }
}
