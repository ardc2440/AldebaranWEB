using Aldebaran.Application.Services.Models.TrackLogs;

namespace Aldebaran.Application.Services.TrackLogs
{
    /// <summary>
    /// Arma la lista campo / anterior / nuevo:
    /// - Creación: todos los campos como nuevos.
    /// - Eliminación: los valores que tenía el registro, como anteriores.
    /// - Modificación: compara contra el movimiento anterior del mismo registro; si no hay anterior,
    ///   muestra los valores nuevos sin poder marcar qué cambió.
    /// </summary>
    internal static class TrackDiff
    {
        public static IReadOnlyList<TrackChange> Build(TrackAction action, IReadOnlyDictionary<string, string?> current, IReadOnlyDictionary<string, string?>? previous) => action switch
        {
            TrackAction.Created => current.Select(c => new TrackChange(c.Key, null, c.Value, true)).ToList(),
            TrackAction.Deleted => current.Select(c => new TrackChange(c.Key, c.Value, null, true)).ToList(),
            _ => previous == null ? WithoutPrevious(current) : Compare(current, previous)
        };

        private static IReadOnlyList<TrackChange> WithoutPrevious(IReadOnlyDictionary<string, string?> current) =>
            current.Select(c => new TrackChange(c.Key, null, c.Value, false)).ToList();

        private static IReadOnlyList<TrackChange> Compare(IReadOnlyDictionary<string, string?> current, IReadOnlyDictionary<string, string?> previous) =>
            current.Keys.Union(previous.Keys)
                .Select(field =>
                {
                    previous.TryGetValue(field, out var before);
                    current.TryGetValue(field, out var after);
                    return new TrackChange(field, before, after, !string.Equals(before, after, StringComparison.Ordinal));
                })
                .OrderByDescending(c => c.Changed)
                .ToList();
    }
}
