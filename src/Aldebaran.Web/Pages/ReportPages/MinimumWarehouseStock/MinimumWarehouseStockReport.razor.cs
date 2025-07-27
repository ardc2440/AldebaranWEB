using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.MinimumWarehouseStock.Components;
using Aldebaran.Web.Pages.ReportPages.MinimumWarehouseStock.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.MinimumWarehouseStock
{
    public partial class MinimumWarehouseStockReport
    {
        #region Injections
        [Inject]
        protected ILogger<MinimumWarehouseStockReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        // TODO: Crear el servicio en el futuro
        // [Inject]
        // protected IMinimumWarehouseStockReportService MinimumWarehouseStockReportService { get; set; }
        #endregion

        #region Variables
        protected MinimumWarehouseStockFilter Filter;
        protected MinimumWarehouseStockViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        // TODO: Definir el modelo de datos del reporte
        // private IEnumerable<Application.Services.Models.Reports.MinimumWarehouseStockReport> DataReport { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Reset();
            }
        }
        #endregion

        #region Events
        async Task Reset()
        {
            Filter = null;
            ViewModel = null;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            await OpenFilters();
        }

        async Task RedrawReport(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;

                // TODO: Implementar cuando tengamos el servicio
                // DataReport = await MinimumWarehouseStockReportService.GetMinimumWarehouseStockReportDataAsync(filter, ct);

                // Por ahora, solo creamos un ViewModel vacío para mostrar que los filtros funcionan
                ViewModel = new MinimumWarehouseStockViewModel();
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }

        async Task<string> SetReportFilter(MinimumWarehouseStockFilter filter, CancellationToken ct = default)
        {
            var filterResult = string.Empty;

            if (filter.UmbralMaximoExistencias.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@UmbralMaximoExistencias = {filter.UmbralMaximoExistencias.Value}";

            if (filter.Warehouse != null)
                if (filter.Warehouse.WarehouseId > 0)
                    filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@WarehouseId = {filter.Warehouse.WarehouseId}";

            if (!string.IsNullOrEmpty(filter.ProviderIds))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderIds = '{filter.ProviderIds}'";

            if (!string.IsNullOrEmpty(filter.ItemIds))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ItemIds = '{filter.ItemIds}'";

            if (!string.IsNullOrEmpty(filter.ReferenceIds))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIds = '{filter.ReferenceIds}'";

            // Filtros de texto libre para campos del proveedor
            if (!string.IsNullOrEmpty(filter.ProviderItemName))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderItemName = '{filter.ProviderItemName}'";

            if (!string.IsNullOrEmpty(filter.ProviderReference))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderReference = '{filter.ProviderReference}'";

            if (!string.IsNullOrEmpty(filter.ProviderReferenceName))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderReferenceName = '{filter.ProviderReferenceName}'";

            if (!string.IsNullOrEmpty(filter.ProviderReferenceCode))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderReferenceCode = '{filter.ProviderReferenceCode}'";

            // Filtros de checkbox de 3 estados para Artículos (solo se envían si no son null)
            if (filter.ItemIsActive.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ItemIsActive = {(filter.ItemIsActive.Value ? 1 : 0)}";

            if (filter.ItemIsDomesticProduct.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ItemIsDomesticProduct = {(filter.ItemIsDomesticProduct.Value ? 1 : 0)}";

            if (filter.ItemIsSpecialImport.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ItemIsSpecialImport = {(filter.ItemIsSpecialImport.Value ? 1 : 0)}";

            if (filter.ItemIsSaleOff.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ItemIsSaleOff = {(filter.ItemIsSaleOff.Value ? 1 : 0)}";

            if (filter.ItemIsCatalogVisible.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ItemIsCatalogVisible = {(filter.ItemIsCatalogVisible.Value ? 1 : 0)}";

            // Filtros de checkbox de 3 estados para Referencias (solo se envían si no son null)
            if (filter.ReferenceIsActive.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIsActive = {(filter.ReferenceIsActive.Value ? 1 : 0)}";

            if (filter.ReferenceHasAlarmMinimumQuantity.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceHasAlarmMinimumQuantity = {(filter.ReferenceHasAlarmMinimumQuantity.Value ? 1 : 0)}";

            if (filter.ReferenceIsSoldOut.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIsSoldOut = {(filter.ReferenceIsSoldOut.Value ? 1 : 0)}";

            if (filter.OnlyBelowMinimum)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@OnlyBelowMinimum = 1";

            return filterResult;
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<MinimumWarehouseStockReportFilter>("Filtrar reporte de existencias mínimas en bodega", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (MinimumWarehouseStockFilter)result;

            await RedrawReport(await SetReportFilter(Filter));

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }

        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                await Reset();
            }
        }

        async Task Save(RadzenSplitButtonItem args)
        {
            if (args?.Value == null)
                return;
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "minimum-warehouse-stock-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Referencias con existencia mínima en bodega.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "minimum-warehouse-stock-report-container");
            }
            IsBusy = false;
        }

        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }

        async Task ToggleReadMorePage()
        {
            await JSRuntime.InvokeVoidAsync("readMoreTogglePage", "toggleLinkPage");
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion

        #region Fill Data Report
        // TODO: Implementar métodos para llenar los datos del reporte cuando tengamos el servicio
        #endregion
    }
}