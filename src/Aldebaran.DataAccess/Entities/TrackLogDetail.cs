namespace Aldebaran.DataAccess.Entities
{
    /// <summary>
    /// Fila de SP_GET_TRACK_DETAIL: datos del movimiento y del movimiento anterior de la misma entidad y llave.
    /// Entidad keyless (solo lectura).
    /// </summary>
    public class TrackLogDetail
    {
        public long Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string DataLog { get; set; } = string.Empty;
        public long? PreviousId { get; set; }
        public string? PreviousDataLog { get; set; }
    }
}
