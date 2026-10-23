namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_TRACK_OBJECT_TYPES: tipo de objeto de negocio por el que se puede buscar en el Track. Keyless.</summary>
    public class TrackObjectType
    {
        public byte ObjectType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Hint { get; set; } = string.Empty;
    }
}
