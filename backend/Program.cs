using Vero.Backend.Models;
using Vero.Backend.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("AllowLocal", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocal");

app.MapGet("/api/events", () => Results.Ok(EventRepository.GetEvents()));
app.MapGet("/api/events/{id}", (int id) => EventRepository.GetEvent(id) is Event? evt ? Results.Ok(evt) : Results.NotFound());
app.MapPost("/api/events", (Event evt) =>
{
    var created = EventRepository.AddEvent(evt);
    return Results.Created($"/api/events/{created.Id}", created);
});

app.MapGet("/api/appointments", () => Results.Ok(EventRepository.GetAppointments()));
app.MapPost("/api/appointments", (Appointment appointment) =>
{
    var created = EventRepository.AddAppointment(appointment);
    return Results.Created($"/api/appointments/{created.Id}", created);
});

app.Run();
