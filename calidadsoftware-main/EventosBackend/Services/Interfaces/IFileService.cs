// EventosBackend/Services/Interfaces/IFileService.cs
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EventosBackend.Services.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Guarda un archivo de imagen en el servidor dentro de una subcarpeta específica y devuelve la ruta relativa.
        /// </summary>
        /// <param name="imageFile">El archivo IFormFile recibido del request.</param>
        /// <param name="subfolder">La subcarpeta dentro de 'wwwroot/uploads' donde se guardará la imagen (ej. "salas").</param>
        /// <returns>La ruta relativa de la imagen guardada (ej. "/uploads/salas/nombre-unico.jpg").</returns>
        Task<string> SaveImageAsync(IFormFile imageFile, string subfolder);

        /// <summary>
        /// Elimina un archivo del sistema de archivos del servidor.
        /// </summary>
        /// <param name="relativePath">La ruta relativa del archivo a eliminar (ej. "/uploads/salas/imagen.jpg").</param>
        void DeleteFile(string relativePath);
    }
}