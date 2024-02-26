namespace Aldebaran.Application.Services.Notificator.Models
{
    /// <summary>
    /// Reprensenta los archivos adjuntos que puede contener un correo electronico
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// Nombre del archivl
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Tipo de archivo
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Hash del archivo
        /// </summary>
        public string Hash { get; set; }
    }
}
