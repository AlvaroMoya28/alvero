// EventosBackend/Services/FileService.cs
using EventosBackend.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventosBackend.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, string subfolder)
        {
            // ... (tu método SaveImageAsync sin cambios) ...
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("No se proporcionó un archivo de imagen.");
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Formato de archivo no permitido. Solo se aceptan imágenes (.jpg, .png, .gif).");
            }
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(uploadsFolderPath);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            var relativePath = Path.Combine("uploads", subfolder, uniqueFileName).Replace(Path.DirectorySeparatorChar, '/');
            return $"/{relativePath}";
        }

        public void DeleteFile(string relativePath)
        {
            Console.WriteLine("--- Iniciando borrado de archivo ---");
            if (string.IsNullOrEmpty(relativePath))
            {
                Console.WriteLine("La ruta relativa está vacía o es nula. No se puede borrar.");
                return;
            }

            // 1. Imprimir la información que recibimos
            Console.WriteLine($"Ruta relativa recibida: {relativePath}");
            Console.WriteLine($"WebRootPath del servidor: {_hostingEnvironment.WebRootPath}");

            // 2. Construir la ruta física completa
            // Quitamos la '/' inicial para que Path.Combine funcione correctamente
            var pathWithoutLeadingSlash = relativePath.TrimStart('/');
            var physicalPath = Path.Combine(_hostingEnvironment.WebRootPath, pathWithoutLeadingSlash);
            Console.WriteLine($"Ruta física construida: {physicalPath}");

            // 3. Verificar si el archivo existe
            if (File.Exists(physicalPath))
            {
                Console.WriteLine("¡Éxito! El archivo SÍ existe en la ruta física.");
                try
                {
                    Console.WriteLine("Intentando eliminar el archivo...");
                    File.Delete(physicalPath);
                    Console.WriteLine("Archivo eliminado exitosamente del disco.");
                }
                catch (IOException ex)
                {
                    // Capturar y mostrar cualquier error durante el borrado
                    Console.WriteLine($"!!! ERROR DE IO al eliminar el archivo: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"!!! ERROR DE PERMISOS al eliminar el archivo: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("!!! FALLO: El archivo NO existe en la ruta física especificada.");
            }
            Console.WriteLine("--- Finalizado borrado de archivo ---");
        }
    }
}