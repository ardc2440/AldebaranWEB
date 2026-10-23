namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_TRACK_OBJECT_DESCRIPTIONS: descripción de un Id que aparece en el detalle de un movimiento. Keyless.</summary>
    public class TrackObjectDescription
    {
        public string FieldName { get; set; } = string.Empty;
        public int ObjectId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
