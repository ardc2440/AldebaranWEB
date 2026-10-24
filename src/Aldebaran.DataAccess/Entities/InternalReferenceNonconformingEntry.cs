namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Referencia de artículo que empieza por el código pero no cumple el formato de consecutivo. Keyless.</summary>
    public class InternalReferenceNonconformingEntry
    {
        public string InternalReference { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
