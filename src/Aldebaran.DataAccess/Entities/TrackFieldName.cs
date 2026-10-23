namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_TRACK_FIELD_NAMES: nombre para el usuario de un campo del Track en una entidad. Keyless.</summary>
    public class TrackFieldName
    {
        public string FieldName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsTechnical { get; set; }
    }
}
