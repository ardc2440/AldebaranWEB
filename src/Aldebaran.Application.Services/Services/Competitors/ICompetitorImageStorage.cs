namespace Aldebaran.Application.Services.Competitors
{
    /// <summary>
    /// Almacenamiento de las imágenes de productos de la competencia (implementado en la capa web sobre la carpeta configurada).
    /// Solo maneja archivos; el nombre guardado lo registra el servicio de homologaciones.
    /// </summary>
    public interface ICompetitorImageStorage
    {
        /// <summary>Guarda la imagen y devuelve el nombre de archivo asignado.</summary>
        Task<string> SaveAsync(int competitorHomologationId, Stream content, string extension, CancellationToken ct = default);

        /// <summary>Contenido de la imagen o null si no existe (para incrustarla en el PDF del reporte).</summary>
        Task<byte[]?> ReadAsync(string? fileName, CancellationToken ct = default);

        /// <summary>Elimina el archivo si existe (sin error si no existe).</summary>
        void Delete(string? fileName);
    }
}
