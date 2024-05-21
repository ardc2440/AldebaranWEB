namespace Aldebaran.DataAccess.Core
{
    /// <summary>
    /// Tabla en donde se guardaran las modificaciones de las tablas marcadas para este fin
    /// </summary>
    public class Track
    {
        public long Id { get; set; }
        /// <summary>
        /// Tabla que ha sufrido la modificacion
        /// </summary>
        public string EntityName { get; set; } = null!;
        /// <summary>
        /// Llave primaria de la tabla que fue modificada
        /// </summary>
        public string? EntityKey { get; set; }
        /// <summary>
        /// Accion de modificacion
        /// </summary>
        public string Action { get; set; } = null!;
        /// <summary>
        /// Representa el registro completo de la tabla que fue modificada
        /// </summary>
        public string DataLog { get; set; } = null!;
        /// <summary>
        /// Fecha de la modificacion
        /// </summary>
        public DateTime ModifiedDate { get; set; }
        /// <summary>
        /// Usuario con el cual se registrara la modificacion de los registros
        /// </summary>
        public string ModifierName { get; set; } = null!;
    }
}
