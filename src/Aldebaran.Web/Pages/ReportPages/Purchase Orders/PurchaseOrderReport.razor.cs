using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Purchase_Orders.Components;
using Aldebaran.Web.Pages.ReportPages.Purchase_Orders.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Purchase_Orders
{
    public partial class PurchaseOrderReport
    {
        #region Injections
        [Inject]
        protected ILogger<PurchaseOrderReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IPurchaseOrderReportService PurchaseOrderReportService { get; set; }
        #endregion

        #region Variables
        protected PurchaseOrderFilter Filter;
        protected PurchaseOrderViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        private IEnumerable<Application.Services.Models.Reports.PurchaseOrderReport> DataReport { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await RedrawReportAsync();
        }
        #endregion

        #region Events
        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {

            try
            {
                IsLoadingData = true;

                DataReport = await PurchaseOrderReportService.GetPurchaseOrderReportDataAsync(filter, ct);

                ViewModel = new PurchaseOrderViewModel()
                {
                    Orders = await GetOrdersAsync(ct)
                };
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        async Task<string> SetReportFilterAsync(PurchaseOrderFilter filter, CancellationToken ct = default)
        {
            var filterResult = string.Empty;

            if (filter.CreationDate.StartDate.HasValue)
                filterResult += $"@CreationDateFrom = '{(DateTime)filter.CreationDate.StartDate:yyyyMMdd}', @CreationDateTo = '{(DateTime)filter.CreationDate.EndDate:yyyyMMdd}'";

            if (filter.RequestDate.StartDate.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@RequestDateFrom = '{(DateTime)filter.RequestDate.StartDate:yyyyMMdd}', @RequestDateTo = '{(DateTime)filter.RequestDate.EndDate:yyyyMMdd}'";

            if (filter.ExpectedReceiptDate.StartDate.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ExpectedReceiptDateFrom = '{(DateTime)filter.ExpectedReceiptDate.StartDate:yyyyMMdd}', @ExpectedReceiptDateTo = '{(DateTime)filter.ExpectedReceiptDate.EndDate:yyyyMMdd}'";

            if (!filter.OrderNumber.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@OrderNumber = '{filter.OrderNumber}'";

            if (!filter.ImportNumber.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ImportNumber = '{filter.ImportNumber}'";

            if (filter.ItemReferences.Count > 0)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIds = '{String.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId))}'";

            if (!filter.EmbarkationPort.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@EmbarkationPort = '{filter.EmbarkationPort}'";

            if (!filter.ProformaNumber.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProformaNumber = '{filter.ProformaNumber}'";

            if (filter.ProviderId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderId = {filter.ProviderId}";

            if (filter.ForwarderId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ForwarderId = {filter.ForwarderId}";

            if (filter.ForwarderAgentId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ForwarderAgentId = {filter.ForwarderAgentId}";

            if (filter.ShipmentMethodId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ShipmentMethodId = {filter.ShipmentMethodId}";

            if (filter.WarehouseId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@WarehouseId = {filter.WarehouseId}";

            if (filter.StatusDocumentId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@StatusDocumentTypeId = {filter.StatusDocumentId}";

            return filterResult;
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<PurchaseOrderReportFilter>("Filtrar reporte de ordenes de compra", parameters: new Dictionary<string, object> { { "Filter", (PurchaseOrderFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (PurchaseOrderFilter)result;

            await RedrawReportAsync(await SetReportFilterAsync(Filter));

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }

        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                Filter = null;

                await RedrawReportAsync();

                await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            }
        }
        async Task Save(RadzenSplitButtonItem args)
        {
            if (args?.Value == null)
                return;
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "purchase-order-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Ordenes de compra.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "purchase-order-report-container");
            }
            IsBusy = false;
        }

        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion

        #region Fill Data Report

        async Task<List<PurchaseOrderViewModel.Order>> GetOrdersAsync(CancellationToken ct = default)
        {
            var orders = new List<PurchaseOrderViewModel.Order>();

            foreach (var order in DataReport.Select(s => new { s.OrderId, s.StatusDocumentTypeName, s.OrderNumber, s.CreationDate, s.RequestDate, s.ExpectedReceiptDate, s.ProviderName, s.ImportNumber, s.ShipmentMethodName, s.EmbarkationPort, s.ProformaNumber, s.ForwarderName, s.ForwarderEmail, s.ForwarderFax, s.ForwarderPhone, s.ForwarderAgentName, s.AgentPhone, s.AgentFax, s.AgentEmail })
                                    .DistinctBy(d => d.OrderId)
                                    .OrderBy(o => o.OrderNumber))
            {
                orders.Add(new PurchaseOrderViewModel.Order
                {
                    OrderNumber = order.OrderNumber,
                    CreationDate = order.CreationDate,
                    RequestDate = order.RequestDate,
                    ExpectedReceiptDate = order.ExpectedReceiptDate,
                    ProviderName = order.ProviderName,
                    Forwarder = new PurchaseOrderViewModel.Forwarder
                    {
                        ForwarderName = order.ForwarderName,
                        Phone = order.ForwarderPhone,
                        Fax = order.ForwarderFax,
                        Email = order.ForwarderEmail
                    },
                    ForwarderAgent = new PurchaseOrderViewModel.ForwarderAgent
                    {
                        ForwarderAgentName = order.ForwarderAgentName,
                        Phone = order.AgentPhone,
                        Fax = order.AgentFax,
                        Email = order.AgentEmail
                    },
                    ImportNumber = order.ImportNumber,
                    ShipmentMethodName = order.ShipmentMethodName,
                    EmbarkationPort = order.EmbarkationPort,
                    ProformaNumber = order.ProformaNumber,
                    Warehouses = await GetOrderWarehousesAsync(order.OrderId, ct),
                    StatusDocumentName = order.StatusDocumentTypeName
                });
            }

            return orders;
        }

        async Task<List<PurchaseOrderViewModel.Warehouse>> GetOrderWarehousesAsync(int orderId, CancellationToken ct = default)
        {
            var warehouses = new List<PurchaseOrderViewModel.Warehouse>();

            foreach (var warehouse in DataReport.Where(w => w.OrderId == orderId).Select(s => new { s.WarehouseId, s.WarehouseName })
                                        .DistinctBy(d => d.WarehouseId)
                                        .OrderBy(o => o.WarehouseName))
            {
                warehouses.Add(new PurchaseOrderViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    WarehouseName = warehouse.WarehouseName,
                    Lines = await GetOrderLinesAsync(orderId, warehouse.WarehouseId, ct)
                });
            }

            return warehouses;
        }

        async Task<List<PurchaseOrderViewModel.Line>> GetOrderLinesAsync(int orderId, short warehouseId, CancellationToken ct = default)
        {
            var lines = new List<PurchaseOrderViewModel.Line>();

            foreach (var line in DataReport.Where(w => w.OrderId == orderId && w.WarehouseId == warehouseId).Select(s => new { s.LineId, s.LineName, s.LineCode })
                                    .DistinctBy(d => d.LineId)
                                    .OrderBy(o => o.LineName))
            {
                lines.Add(new PurchaseOrderViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = await GetLineItemsAsync(orderId, warehouseId, line.LineId, ct)
                });
            }

            return lines;
        }

        async Task<List<PurchaseOrderViewModel.Item>> GetLineItemsAsync(int orderId, short warehouseId, int lineId, CancellationToken ct = default)
        {
            var items = new List<PurchaseOrderViewModel.Item>();

            foreach (var item in DataReport.Where(w => w.OrderId == orderId && w.WarehouseId == warehouseId && w.LineId == lineId).Select(s => new { s.ItemId, s.ItemName, s.InternalReference })
                                    .DistinctBy(d => d.ItemId)
                                    .OrderBy(o => o.ItemName))
            {
                items.Add(new PurchaseOrderViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = await GetItemReferencesAsync(orderId, warehouseId, item.ItemId, ct)
                });
            }

            return items;
        }

        async Task<List<PurchaseOrderViewModel.Reference>> GetItemReferencesAsync(int orderId, short warehouseId, int itemId, CancellationToken ct = default)
        {
            var references = new List<PurchaseOrderViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.OrderId == orderId && w.WarehouseId == warehouseId && w.ItemId == itemId).Select(s => new { s.ReferenceCode, s.ReferenceName, s.Amount, s.Volume, s.Weight })
                                            .OrderBy(o => o.ReferenceName))
            {
                references.Add(new PurchaseOrderViewModel.Reference
                {
                    ReferenceCode = reference.ReferenceCode,
                    ReferenceName = reference.ReferenceName,
                    Amount = reference.Amount,
                    Weight = reference.Weight,
                    Volume = reference.Volume
                });
            }

            return references;
        }

        #endregion
    }
}
