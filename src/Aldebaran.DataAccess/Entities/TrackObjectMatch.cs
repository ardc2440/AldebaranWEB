namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_SEARCH_TRACK_OBJECTS: objeto que coincide con el texto buscado. Keyless.</summary>
    public class TrackObjectMatch
    {
        public int ObjectId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
