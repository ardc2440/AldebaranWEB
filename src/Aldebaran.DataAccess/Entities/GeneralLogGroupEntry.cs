namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_GENERAL_LOG_GROUPS: mensaje agrupado por plantilla, nivel y origen. Keyless.</summary>
    public class GeneralLogGroupEntry
    {
        public string? GroupKey { get; set; }
        public string? LogLevel { get; set; }
        public string? Source { get; set; }
        public string? MessageTemplate { get; set; }
        public int Occurrences { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }
        public long LastId { get; set; }
        public int TotalRows { get; set; }
    }
}
