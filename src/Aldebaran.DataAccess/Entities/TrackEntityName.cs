namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_TRACK_ENTITIES: entidad trazada con su nombre para mostrar. Entidad keyless (solo lectura).</summary>
    public class TrackEntityName
    {
        public string EntityName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
