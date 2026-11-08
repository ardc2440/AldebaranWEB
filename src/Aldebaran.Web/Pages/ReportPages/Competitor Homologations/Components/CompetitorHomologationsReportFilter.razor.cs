using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Competitors;
using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Web.Pages.ReportPages.Competitor_Homologations.ViewModel;
using Aldebaran.Web.Pages.ReportPages.Item_References.Components;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Competitor_Homologations.Components
{
    /// <summary>
    /// Diálogo de filtros del reporte de Homologaciones con la competencia: jerarquía Línea -> Artículo -> Referencia
    /// acotada a lo homologado, compañías de la competencia, referencia y producto del competidor (contiene).
    /// Sin filtros devuelve todas las homologaciones activas. Devuelve un <see cref="CompetitorHomologationsFilter"/> o null si se cancela.
    /// </summary>
    public partial class CompetitorHomologationsReportFilter
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected ICompetitorCompanyService CompetitorCompanyService { get; set; }

        [Inject]
        protected ICompetitorHomologationReportService CompetitorHomologationReportService { get; set; }
        #endregion

        #region Parameters
        /// <summary>Filtro vigente del reporte, para restaurar la selección al reabrir el diálogo.</summary>
        [Parameter]
        public CompetitorHomologationsFilter Filter { get; set; }
        #endregion

        #region Variables
        protected HierarchicalReferencePicker referencePicker;
        protected IReadOnlyList<CompetitorCompany> Companies = Array.Empty<CompetitorCompany>();
        protected IEnumerable<int> CompanyIds = Enumerable.Empty<int>();
        protected bool IsLoadingReferences = true;
        protected bool IsSubmitInProgress;

        /// <summary>Copia de trabajo; el filtro vigente no se altera si se cancela.</summary>
        protected CompetitorHomologationReportFilter Criteria = new();

        private List<ItemReference> _catalog = new();
        #endregion

        #region Overrides
        protected override void OnParametersSet()
        {
            var current = Filter?.Criteria ?? new CompetitorHomologationReportFilter();
            Criteria = new CompetitorHomologationReportFilter
            {
                CompetitorReference = current.CompetitorReference,
                CompetitorProductName = current.CompetitorProductName
            };
            CompanyIds = current.CompetitorCompanyIds.ToList();
        }

        protected override async Task OnInitializedAsync()
        {
            Companies = await CompetitorCompanyService.GetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;
            await LoadPickerAsync();
        }
        #endregion

        #region Picker
        /// <summary>Carga el catálogo de referencias acotado a lo homologado y lo entrega al selector con la selección vigente.</summary>
        private async Task LoadPickerAsync()
        {
            var catalog = await ItemReferenceService.GetReportsReferencesAsync();
            _catalog = (await CompetitorHomologationReportService.FilterHomologatedAsync(catalog)).ToList();
            var current = Filter?.Criteria ?? new CompetitorHomologationReportFilter();
            referencePicker.SetAvailableItemReferencesForSelection(_catalog);
            referencePicker.RestoreSelection(current.LineIds, current.ItemIds, current.ReferenceIds);
            IsLoadingReferences = false;
            StateHasChanged();
        }
        #endregion

        #region Events
        protected void FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                var criteria = BuildCriteria();
                DialogService.Close(new CompetitorHomologationsFilter(criteria, BuildSummary(criteria)));
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected void CancelButtonClick() => DialogService.Close(null);

        private CompetitorHomologationReportFilter BuildCriteria() => new()
        {
            LineIds = referencePicker.GetSelectedLineIds(),
            ItemIds = referencePicker.GetSelectedItemIds(),
            ReferenceIds = referencePicker.GetSelectedReferenceIds(),
            CompetitorCompanyIds = (CompanyIds ?? Enumerable.Empty<int>()).ToList(),
            CompetitorReference = string.IsNullOrWhiteSpace(Criteria.CompetitorReference) ? null : Criteria.CompetitorReference.Trim(),
            CompetitorProductName = string.IsNullOrWhiteSpace(Criteria.CompetitorProductName) ? null : Criteria.CompetitorProductName.Trim()
        };

        /// <summary>Resumen legible: nivel más específico de la jerarquía y los filtros de la competencia.</summary>
        private ItemReferencesReportFilterSummary BuildSummary(CompetitorHomologationReportFilter criteria)
        {
            var hierarchy = new ItemReferencesReportFilter { LineIds = criteria.LineIds, ItemIds = criteria.ItemIds, ReferenceIds = criteria.ReferenceIds };
            var conditions = new List<string>();
            var companies = Companies.Where(c => criteria.CompetitorCompanyIds.Contains(c.CompetitorCompanyId)).Select(c => c.CompanyName).ToList();
            if (companies.Any())
                conditions.Add($"Compañías: {string.Join(", ", companies)}");
            if (criteria.CompetitorReference != null)
                conditions.Add($"Referencia del competidor contiene: {criteria.CompetitorReference}");
            if (criteria.CompetitorProductName != null)
                conditions.Add($"Producto del competidor contiene: {criteria.CompetitorProductName}");
            return ItemReferencesReportFilterSummaryBuilder.Build(hierarchy, _catalog, conditions);
        }
        #endregion
    }
}
