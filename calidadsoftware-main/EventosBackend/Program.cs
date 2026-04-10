using AutoMapper;
using EventosBackend.Models.Context;
using EventosBackend.Repositories;
using EventosBackend.Repositories.Interfaces;
using EventosBackend.Services;
using EventosBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using EventosBackend.Models.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var emailConfig = builder.Configuration.GetSection("EmailSettings");
builder.Services.Configure<EmailSettings>(emailConfig);
var jwtConfig = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtConfig);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// AutoMapper Configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Database Configuration
builder.Services.AddDbContext<OracleDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"),
    oracleOptions => oracleOptions.UseOracleSQLCompatibility("11")));

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DoctorPC API",
        Version = "v1",
        Description = "API for doctorPC appointment management",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your@email.com"
        }
    });

    // Add JWT Authentication to Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, Array.Empty<string>()}
    });
});

// Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:ValidIssuer"],
        ValidAudience = builder.Configuration["JwtSettings:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        // Mapear el rol al claim personalizado "tipoUsuario" para compatibilidad con tokens antiguos
        RoleClaimType = "tipoUsuario"
    };
});

// Dependency Injection
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<ICitaService, CitaService>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:8080",
                "https://localhost:8080",
                "http://yourproductiondomain.com",
                "https://yourproductiondomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventosBackend v1");
        c.RoutePrefix = "api-docs";
    });
}

// Exception Handling
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        await context.Response.WriteAsJsonAsync(new
        {
            StatusCode = context.Response.StatusCode,
            Message = "An error occurred while processing your request",
            Detail = exception?.Message,
            StackTrace = app.Environment.IsDevelopment() ? exception?.StackTrace : null
        });
    });
});

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

// Database connection test
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OracleDbContext>();
    if (db.Database.CanConnect())
    {
        Console.WriteLine("✅ ORACLE CONNECTION SUCCESSFUL");
    }
    else
    {
        Console.WriteLine("❌ ORACLE CONNECTION FAILED");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ DATABASE CONNECTION ERROR: {ex.Message}");
}

app.Run();