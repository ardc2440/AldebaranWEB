namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_GENERAL_LOG_OCCURRENCES: ocurrencia de un mensaje del log general. Keyless.</summary>
    public class GeneralLogOccurrenceEntry
    {
        public long LogId { get; set; }
        public DateTime EventDate { get; set; }
        public string? LogLevel { get; set; }
        public string? Source { get; set; }
        public string? Message { get; set; }
        public bool HasException { get; set; }
        public int TotalRows { get; set; }
    }
}
