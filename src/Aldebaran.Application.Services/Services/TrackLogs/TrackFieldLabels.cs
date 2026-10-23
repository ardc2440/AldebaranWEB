using Aldebaran.Application.Services.Models.TrackLogs;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.TrackLogs
{
    /// <summary>Pone a cada campo su nombre para el usuario y marca los campos técnicos.</summary>
    internal sealed class TrackFieldLabels
    {
        private readonly IReadOnlyDictionary<string, Entities.TrackFieldName> _names;

        private TrackFieldLabels(IReadOnlyDictionary<string, Entities.TrackFieldName> names) => _names = names;

        public static TrackFieldLabels From(IEnumerable<Entities.TrackFieldName> rows) =>
            new(rows.GroupBy(r => r.FieldName, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase));

        public TrackChange Apply(TrackChange change) =>
            _names.TryGetValue(change.Field, out var name)
                ? change with { DisplayName = name.DisplayName, IsTechnical = name.IsTechnical }
                : change;
    }
}
