using Aldebaran.Application.Services.Models.TrackLogs;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.TrackLogs
{
    /// <summary>Agrega la descripción a los Id de objetos conocidos en el detalle: "4160" -> "4160 · [TE-122] Soporte ...".</summary>
    internal sealed class TrackObjectLabels
    {
        private readonly IReadOnlyDictionary<(string Field, int Id), string> _labels;

        private TrackObjectLabels(IReadOnlyDictionary<(string Field, int Id), string> labels) => _labels = labels;

        public static TrackObjectLabels From(IEnumerable<Entities.TrackObjectDescription> rows) =>
            new(rows.GroupBy(r => (r.FieldName, r.ObjectId)).ToDictionary(g => g.Key, g => g.First().Description));

        public TrackChange Apply(TrackChange change) =>
            change with { PreviousValue = Label(change.Field, change.PreviousValue), NewValue = Label(change.Field, change.NewValue) };

        private string? Label(string field, string? value) =>
            value != null && int.TryParse(value, out var id) && _labels.TryGetValue((field, id), out var description)
                ? $"{value} · {description}"
                : value;
    }
}
