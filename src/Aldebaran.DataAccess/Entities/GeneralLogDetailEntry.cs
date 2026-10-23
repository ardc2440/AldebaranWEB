namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_GENERAL_LOG_DETAIL: detalle técnico de una ocurrencia del log general. Keyless.</summary>
    public class GeneralLogDetailEntry
    {
        public long LogId { get; set; }
        public DateTime EventDate { get; set; }
        public string? LogLevel { get; set; }
        public string? Source { get; set; }
        public string? Message { get; set; }
        public string? MessageTemplate { get; set; }
        public string? Exception { get; set; }
        public string? Properties { get; set; }
    }
}
