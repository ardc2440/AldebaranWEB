using Microsoft.AspNetCore.Components;

namespace Aldebaran.Web.Pages.ReportPages.Item_References.Components
{
    /// <summary>
    /// Muestra el resumen de filtros aplicados agrupado (Líneas, Artículos, Referencias, Condiciones).
    /// Cada grupo muestra los primeros valores y permite ver todos con "Ver más".
    /// </summary>
    public partial class ItemReferencesReportFilterSummaryView
    {
        private const int DefaultVisiblePerGroup = 2;

        [Parameter]
        public ItemReferencesReportFilterSummary Summary { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string CssClass { get; set; }

        /// <summary>true = muestra todos los valores sin "Ver más" (ej. encabezado para imprimir).</summary>
        [Parameter]
        public bool ExpandAll { get; set; }

        protected bool ShowAll;

        protected IReadOnlyList<FilterGroup> Groups => BuildGroups();

        protected bool IsTruncated => !ExpandAll && Groups.Any(g => g.Values.Count > DefaultVisiblePerGroup);

        protected IEnumerable<string> Visible(IReadOnlyList<string> values) =>
            ExpandAll || ShowAll ? values : values.Take(DefaultVisiblePerGroup);

        protected int HiddenCount(IReadOnlyList<string> values) =>
            ExpandAll || ShowAll ? 0 : Math.Max(0, values.Count - DefaultVisiblePerGroup);

        protected void ToggleShowAll() => ShowAll = !ShowAll;

        private IReadOnlyList<FilterGroup> BuildGroups() =>
            Summary?.Groups().Select(g => new FilterGroup(g.Caption, g.Values)).ToList()
            ?? (IReadOnlyList<FilterGroup>)Array.Empty<FilterGroup>();

        protected sealed record FilterGroup(string Caption, IReadOnlyList<string> Values);
    }
}
