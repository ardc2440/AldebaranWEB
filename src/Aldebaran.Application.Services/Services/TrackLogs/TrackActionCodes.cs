using Aldebaran.Application.Services.Models.TrackLogs;

namespace Aldebaran.Application.Services.TrackLogs
{
    /// <summary>Conversión entre la acción del caso de uso y el código guardado en log.track.</summary>
    internal static class TrackActionCodes
    {
        public static string? ToCode(TrackAction? action) => action switch
        {
            TrackAction.Created => "Added",
            TrackAction.Modified => "Modified",
            TrackAction.Deleted => "Deleted",
            _ => null
        };

        public static TrackAction FromCode(string code) => code switch
        {
            "Added" => TrackAction.Created,
            "Deleted" => TrackAction.Deleted,
            _ => TrackAction.Modified
        };
    }
}
