using Aldebaran.Application.Services.Reports;
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

        [Inject]
        protected IMinimumWarehouseStockReportService MinimumWarehouseStockReportService { get; set; }
        #endregion

        #region Variables
        protected MinimumWarehouseStockFilter Filter;
        protected MinimumWarehouseStockViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        private IEnumerable<Application.Services.Models.Reports.MinimumWarehouseStockReport> DataReport { get; set; }
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

                // Obtener datos del procedimiento almacenado
                DataReport = await MinimumWarehouseStockReportService.GetMinimumWarehouseStockReportDataAsync(filter, ct);

                // Convertir datos planos en estructura agrupada
                ViewModel = GroupDataByItem(DataReport);
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Convierte los datos planos del SP en una estructura agrupada por artículo
        /// </summary>
        private MinimumWarehouseStockViewModel GroupDataByItem(IEnumerable<Application.Services.Models.Reports.MinimumWarehouseStockReport> flatData)
        {
            var viewModel = new MinimumWarehouseStockViewModel();

            if (flatData?.Any() != true)
                return viewModel;

            // Agrupar por artículo
            var itemGroups = flatData.GroupBy(x => new { x.ItemId, x.ItemName, x.InternalReference, x.LineName });

            foreach (var itemGroup in itemGroups)
            {
                var itemWithReferences = new ItemWithReferences
                {
                    ItemId = itemGroup.Key.ItemId,
                    ItemName = itemGroup.Key.ItemName,
                    InternalReference = itemGroup.Key.InternalReference,
                    LineName = itemGroup.Key.LineName
                };

                // Agrupar referencias dentro del artículo
                var referenceGroups = itemGroup.GroupBy(x => new { x.ReferenceId, x.ReferenceName, x.ReferenceCode });

                foreach (var referenceGroup in referenceGroups)
                {
                    var reference = new ReferenceWithStock
                    {
                        ReferenceId = referenceGroup.Key.ReferenceId,
                        ReferenceName = referenceGroup.Key.ReferenceName,
                        ReferenceCode = referenceGroup.Key.ReferenceCode,
                        TotalPhysicalStock = referenceGroup.FirstOrDefault()?.TotalPhysicalStock ?? 0,
                        CommittedQuantity = referenceGroup.FirstOrDefault()?.CommittedQuantity ?? 0,
                        AvailableQuantity = referenceGroup.FirstOrDefault()?.AvailableQuantity ?? 0
                    };

                    // Agregar existencias por bodega (solo las que tienen datos)
                    reference.WarehouseStocks = referenceGroup
                        .Where(x => x.WarehouseId.HasValue && x.PhysicalStock.HasValue)
                        .Select(x => new WarehouseStock
                        {
                            WarehouseId = x.WarehouseId.Value,
                            WarehouseName = x.WarehouseName ?? string.Empty,
                            PhysicalStock = x.PhysicalStock.Value
                        })
                        .ToList();

                    itemWithReferences.References.Add(reference);
                }

                viewModel.Items.Add(itemWithReferences);
            }

            return viewModel;
        }

        async Task<string> SetReportFilter(MinimumWarehouseStockFilter filter, CancellationToken ct = default)
        {
            var filterResult = string.Empty;

            // @MaximumQuantityTreshold INT - Campo obligatorio
            if (filter.UmbralMaximoExistencias.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@MaximumQuantityTreshold = {filter.UmbralMaximoExistencias.Value}";

            // @WharehouseId INT = NULL
            if (filter.Warehouse != null && filter.Warehouse.WarehouseId > 0)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@WharehouseId = {filter.Warehouse.WarehouseId}";

            // @ItemList VARCHAR(MAX) = NULL
            if (!string.IsNullOrEmpty(filter.ItemIds))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ItemList = '{filter.ItemIds}'";

            // @ReferenceList VARCHAR(MAX) = NULL
            if (!string.IsNullOrEmpty(filter.ReferenceIds))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceList = '{filter.ReferenceIds}'";

            // Filtros de texto libre para campos descriptivos del proveedor
            if (!string.IsNullOrEmpty(filter.ProviderItemName))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderArticleName = '{filter.ProviderItemName}'";

            if (!string.IsNullOrEmpty(filter.ProviderReference))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderInternalReference = '{filter.ProviderReference}'";

            if (!string.IsNullOrEmpty(filter.ProviderReferenceName))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderReferenceName = '{filter.ProviderReferenceName}'";

            if (!string.IsNullOrEmpty(filter.ProviderReferenceCode))
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderReferenceCode = '{filter.ProviderReferenceCode}'";

            // Filtros de checkbox de 3 estados para Artículos (solo se envían si no son null)
            if (filter.ItemIsActive.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ArticleActive = {(filter.ItemIsActive.Value ? 1 : 0)}";

            if (filter.ItemIsDomesticProduct.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@DomesticProduct = {(filter.ItemIsDomesticProduct.Value ? 1 : 0)}";

            if (filter.ItemIsSpecialImport.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@SpetialImport = {(filter.ItemIsSpecialImport.Value ? 1 : 0)}";

            if (filter.ItemIsSaleOff.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@Offert = {(filter.ItemIsSaleOff.Value ? 1 : 0)}";

            if (filter.ItemIsCatalogVisible.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@CatalogVisible = {(filter.ItemIsCatalogVisible.Value ? 1 : 0)}";

            // Filtros de checkbox de 3 estados para Referencias (solo se envían si no son null)
            if (filter.ReferenceIsActive.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceActive = {(filter.ReferenceIsActive.Value ? 1 : 0)}";

            if (filter.ReferenceHasAlarmMinimumQuantity.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@MinimumQuantityAlarm = {(filter.ReferenceHasAlarmMinimumQuantity.Value ? 1 : 0)}";

            if (filter.ReferenceIsSoldOut.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@OutOfStockBIT = {(filter.ReferenceIsSoldOut.Value ? 1 : 0)}";

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
        /// <summary>
        /// Obtiene las líneas de artículos agrupadas para mostrar en el reporte
        /// </summary>
        /// <param name="ct">Token de cancelación</param>
        /// <returns>Lista de líneas con sus artículos y referencias</returns>
        protected async Task<List<ItemWithReferences>> GetReportItemsAsync(CancellationToken ct = default)
        {
            var items = new List<ItemWithReferences>();

            if (DataReport?.Any() != true)
                return items;

            // Agrupar por artículo
            var itemGroups = DataReport.GroupBy(x => new { x.ItemId, x.ItemName, x.InternalReference, x.LineName })
                                     .OrderBy(g => g.Key.LineName)
                                     .ThenBy(g => g.Key.ItemName);

            foreach (var itemGroup in itemGroups)
            {
                var itemWithReferences = new ItemWithReferences
                {
                    ItemId = itemGroup.Key.ItemId,
                    ItemName = itemGroup.Key.ItemName,
                    InternalReference = itemGroup.Key.InternalReference,
                    LineName = itemGroup.Key.LineName,
                    References = await GetItemReferencesAsync(itemGroup, ct)
                };

                items.Add(itemWithReferences);
            }

            return items;
        }

        /// <summary>
        /// Obtiene las referencias de un artículo específico con su información de stock
        /// </summary>
        /// <param name="itemGroup">Grupo de datos del artículo</param>
        /// <param name="ct">Token de cancelación</param>
        /// <returns>Lista de referencias con información de stock</returns>
        private async Task<List<ReferenceWithStock>> GetItemReferencesAsync(
            IGrouping<object, Application.Services.Models.Reports.MinimumWarehouseStockReport> itemGroup, 
            CancellationToken ct = default)
        {
            var references = new List<ReferenceWithStock>();

            // Agrupar por referencia dentro del artículo
            var referenceGroups = itemGroup.GroupBy(x => new { x.ReferenceId, x.ReferenceName, x.ReferenceCode })
                                          .OrderBy(g => g.Key.ReferenceName);

            foreach (var referenceGroup in referenceGroups)
            {
                var firstReference = referenceGroup.FirstOrDefault();
                if (firstReference == null) continue;

                var reference = new ReferenceWithStock
                {
                    ReferenceId = referenceGroup.Key.ReferenceId,
                    ReferenceName = referenceGroup.Key.ReferenceName,
                    ReferenceCode = referenceGroup.Key.ReferenceCode,
                    TotalPhysicalStock = firstReference.TotalPhysicalStock,
                    CommittedQuantity = firstReference.CommittedQuantity,
                    AvailableQuantity = firstReference.AvailableQuantity,
                    WarehouseStocks = GetWarehouseStockList(referenceGroup)
                };

                references.Add(reference);
            }

            return references;
        }

        /// <summary>
        /// Obtiene la lista de existencias por bodega para una referencia específica
        /// </summary>
        /// <param name="referenceGroup">Grupo de datos de la referencia</param>
        /// <returns>Lista de existencias por bodega</returns>
        private List<WarehouseStock> GetWarehouseStockList(
            IGrouping<object, Application.Services.Models.Reports.MinimumWarehouseStockReport> referenceGroup)
        {
            return referenceGroup
                .Where(x => x.WarehouseId.HasValue && x.PhysicalStock.HasValue)
                .Select(x => new WarehouseStock
                {
                    WarehouseId = x.WarehouseId.Value,
                    WarehouseName = x.WarehouseName ?? string.Empty,
                    PhysicalStock = x.PhysicalStock.Value
                })
                .OrderBy(w => w.WarehouseName)
                .ToList();
        }

        /// <summary>
        /// Obtiene las bodegas únicas presentes en el reporte
        /// </summary>
        /// <returns>Lista de bodegas únicas</returns>
        protected List<WarehouseStock> GetUniqueWarehouses()
        {
            if (ViewModel?.Items?.Any() != true)
                return new List<WarehouseStock>();

            return ViewModel.Items
                .SelectMany(i => i.References)
                .SelectMany(r => r.WarehouseStocks)
                .GroupBy(w => w.WarehouseId)
                .Select(g => g.First())
                .OrderBy(w => w.WarehouseName)
                .ToList();
        }

        /// <summary>
        /// Calcula el total de existencias físicas para todas las referencias
        /// </summary>
        /// <returns>Total de existencias físicas</returns>
        protected int GetTotalPhysicalStock()
        {
            if (ViewModel?.Items?.Any() != true)
                return 0;

            return ViewModel.Items
                .SelectMany(i => i.References)
                .Sum(r => r.TotalPhysicalStock);
        }

        /// <summary>
        /// Calcula el total de cantidad comprometida para todas las referencias
        /// </summary>
        /// <returns>Total de cantidad comprometida</returns>
        protected int GetTotalCommittedQuantity()
        {
            if (ViewModel?.Items?.Any() != true)
                return 0;

            return ViewModel.Items
                .SelectMany(i => i.References)
                .Sum(r => r.CommittedQuantity);
        }

        /// <summary>
        /// Calcula el total de cantidad disponible para todas las referencias
        /// </summary>
        /// <returns>Total de cantidad disponible</returns>
        protected int GetTotalAvailableQuantity()
        {
            if (ViewModel?.Items?.Any() != true)
                return 0;

            return ViewModel.Items
                .SelectMany(i => i.References)
                .Sum(r => r.AvailableQuantity);
        }

        /// <summary>
        /// Obtiene el número total de artículos en el reporte
        /// </summary>
        /// <returns>Número total de artículos</returns>
        protected int GetTotalItemsCount()
        {
            return ViewModel?.Items?.Count ?? 0;
        }

        /// <summary>
        /// Obtiene el número total de referencias en el reporte
        /// </summary>
        /// <returns>Número total de referencias</returns>
        protected int GetTotalReferencesCount()
        {
            if (ViewModel?.Items?.Any() != true)
                return 0;

            return ViewModel.Items.Sum(i => i.References.Count);
        }

        /// <summary>
        /// Verifica si hay datos para mostrar en el reporte
        /// </summary>
        /// <returns>True si hay datos, false en caso contrario</returns>
        protected bool HasReportData()
        {
            return ViewModel?.Items?.Any() == true && 
                   ViewModel.Items.Any(i => i.References?.Any() == true);
        }
        #endregion

        /// <summary>
        /// Obtiene el stock de una bodega específica para una referencia
        /// </summary>
        /// <param name="reference">La referencia</param>
        /// <param name="warehouseId">ID de la bodega</param>
        /// <returns>El stock o "-" si no hay stock</returns>
        protected string GetWarehouseStockForReference(ReferenceWithStock reference, short warehouseId)
        {
            var warehouseStock = reference.WarehouseStocks.FirstOrDefault(w => w.WarehouseId == warehouseId);
            return warehouseStock?.PhysicalStock.ToString("N0") ?? "-";
        }

        /// <summary>
        /// Obtiene los artículos agrupados por línea para mostrar en el reporte
        /// </summary>
        /// <returns>Los artículos agrupados por línea</returns>
        protected IEnumerable<IGrouping<string, ItemWithReferences>> GetItemsByLine()
        {
            if (ViewModel?.Items?.Any() != true)
                return Enumerable.Empty<IGrouping<string, ItemWithReferences>>();

            return ViewModel.Items.GroupBy(i => i.LineName).OrderBy(g => g.Key);
        }
    }
}