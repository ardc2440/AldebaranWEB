using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Web.Pages.ReportPages.Item_References.Components
{
    /// <summary>Filtro aplicado en el reporte junto con su descripción legible para el usuario.</summary>
    public sealed record ItemReferencesReportFilterResult(ItemReferencesReportFilter Filter, ItemReferencesReportFilterSummary Summary);

    /// <summary>Descripción legible de los filtros aplicados (pantalla y encabezado del reporte).</summary>
    public sealed record ItemReferencesReportFilterSummary(
        IReadOnlyList<string> Lines,
        IReadOnlyList<string> Items,
        IReadOnlyList<string> References,
        IReadOnlyList<string> Conditions)
    {
        public static ItemReferencesReportFilterSummary Empty { get; } =
            new(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());

        public bool HasAny => Lines.Count + Items.Count + References.Count + Conditions.Count > 0;

        /// <summary>Grupos con valores, en el orden en que se presentan.</summary>
        public IReadOnlyList<(string Caption, IReadOnlyList<string> Values)> Groups() =>
            new (string Caption, IReadOnlyList<string> Values)[]
            {
                ("Líneas", Lines),
                ("Artículos", Items),
                ("Referencias", References),
                ("Condiciones", Conditions)
            }.Where(g => g.Values.Count > 0).ToList();
    }

    /// <summary>Construye la descripción de los filtros a partir de los Ids seleccionados y el catálogo.</summary>
    public static class ItemReferencesReportFilterSummaryBuilder
    {
        public static ItemReferencesReportFilterSummary Build(ItemReferencesReportFilter filter, IEnumerable<ItemReference> catalog, IEnumerable<string> activeConditions)
        {
            if (filter == null)
                return ItemReferencesReportFilterSummary.Empty;

            var references = catalog?.ToList() ?? new List<ItemReference>();
            var none = Array.Empty<string>();
            var conditions = activeConditions?.ToList() ?? new List<string>();

            // Solo se describe el nivel más específico seleccionado (ya incluye a sus padres),
            // igual que la regla del SP: Referencias > Artículos > Líneas.
            if (filter.ReferenceIds?.Any() == true)
                return new ItemReferencesReportFilterSummary(none, none, DescribeReferences(filter.ReferenceIds, references), conditions);
            if (filter.ItemIds?.Any() == true)
                return new ItemReferencesReportFilterSummary(none, DescribeItems(filter.ItemIds, references), none, conditions);
            return new ItemReferencesReportFilterSummary(DescribeLines(filter.LineIds, references), none, none, conditions);
        }

        private static IReadOnlyList<string> DescribeLines(IEnumerable<short> lineIds, IEnumerable<ItemReference> catalog)
        {
            var ids = lineIds?.ToHashSet() ?? new HashSet<short>();
            return catalog.Select(r => r.Item.Line).Where(l => l != null && ids.Contains(l.LineId))
                          .DistinctBy(l => l.LineId).Select(l => l.LineName).OrderBy(n => n).ToList();
        }

        private static IReadOnlyList<string> DescribeItems(IEnumerable<int> itemIds, IEnumerable<ItemReference> catalog)
        {
            var ids = itemIds?.ToHashSet() ?? new HashSet<int>();
            return catalog.Select(r => r.Item).Where(i => ids.Contains(i.ItemId))
                          .DistinctBy(i => i.ItemId).OrderBy(i => i.ItemName)
                          .Select(i => $"[{i.InternalReference}] {i.ItemName}").ToList();
        }

        private static IReadOnlyList<string> DescribeReferences(IEnumerable<int> referenceIds, IEnumerable<ItemReference> catalog)
        {
            var ids = referenceIds?.ToHashSet() ?? new HashSet<int>();
            return catalog.Where(r => ids.Contains(r.ReferenceId))
                          .OrderBy(r => r.Item.ItemName).ThenBy(r => r.ReferenceName)
                          .Select(r => $"[{r.Item.InternalReference}] {r.Item.ItemName} - {r.ReferenceName}").ToList();
        }
    }
}
