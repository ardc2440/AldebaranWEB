namespace Aldebaran.Application.Services.Models.GeneralLogs
{
    /// <summary>Filtros del log general.</summary>
    /// <param name="Level">Nivel (Information, Warning, Error); null = todos.</param>
    /// <param name="Source">Origen (aplicación o servicio); null = todos.</param>
    public sealed record GeneralLogFilter(DateTime DateFrom, DateTime DateTo, string? Level, string? Source, string? SearchKey);

    /// <summary>Mensaje agrupado: misma plantilla, nivel y origen.</summary>
    public sealed record GeneralLogGroup(string? GroupKey, string Level, string Source, string Message, int Occurrences, DateTime FirstDate, DateTime LastDate);

    /// <summary>Ocurrencia de un mensaje.</summary>
    public sealed record GeneralLogOccurrence(long LogId, DateTime EventDate, string Level, string Source, string Message, bool HasException);

    /// <summary>Detalle técnico de una ocurrencia.</summary>
    public sealed record GeneralLogDetail(long LogId, DateTime EventDate, string Level, string Source, string Message, string MessageTemplate, string? Exception, string? Properties);

    /// <summary>Niveles del log y su nombre para el usuario.</summary>
    public static class GeneralLogLevels
    {
        public static readonly IReadOnlyDictionary<string, string> Names = new Dictionary<string, string>
        {
            ["Information"] = "Información",
            ["Warning"] = "Advertencia",
            ["Error"] = "Error",
            ["Fatal"] = "Fatal",
            ["Debug"] = "Depuración"
        };

        public static string DisplayName(string? level) => level != null && Names.TryGetValue(level, out var name) ? name : level ?? string.Empty;
    }
}
