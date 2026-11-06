namespace Aldebaran.Infraestructure.Common.Utils
{
    /// <summary>
    /// Miniaturas de imágenes para incrustar en documentos (Excel de correos) sin cargar la imagen original.
    /// </summary>
    public interface IImageThumbnailService
    {
        /// <summary>
        /// Ruta de una copia reducida (JPEG, lado mayor &lt;= <paramref name="maxSize"/> px) de la imagen.
        /// La miniatura se guarda en caché y se regenera si la imagen original cambia.
        /// Si no se puede generar, devuelve la ruta original.
        /// </summary>
        string GetThumbnailPath(string sourcePath, int maxSize);
    }
}
