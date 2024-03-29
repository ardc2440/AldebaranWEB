using Aldebaran.Application.Services.Reports;
using Aldebaran.Web.Pages.ReportPages.Order_Shipment.Components;
using Aldebaran.Web.Pages.ReportPages.Order_Shipment.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Order_Shipment
{
    public partial class OrderShipmentReport
    {
        #region Injections
        [Inject]
        protected ILogger<OrderShipmentReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IOrderShipmentReportService OrderShipmentReportService { get; set; }
        #endregion

        #region Variables
        protected OrderShipmentFilter Filter;
        protected OrderShipmentViewModel ViewModel;
        private bool IsBusy = false;
        private readonly bool IsLoadingData = false;
        private IEnumerable<Application.Services.Models.Reports.OrderShipmentReport> DataReport { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            DataReport = await OrderShipmentReportService.GetOrderShipmentReportDataAsync();

            ViewModel = new OrderShipmentViewModel()
            {
                Orders = await GetOrdersAsync()
            };
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<OrderShipmentReportFilter>("Filtrar reporte de ordenes en tránsito", parameters: new Dictionary<string, object> { { "Filter", (OrderShipmentFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (OrderShipmentFilter)result;
            //Todo: Aplicar filtro de refenrecias al ViewModel
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }
        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                Filter = null;
                //Todo: Remover filtro de refenrecias al ViewModel
                await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            }
        }
        async Task Download()
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "order-shipment-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Ordenes en tránsito.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion

        #region Fill Data Report

        async Task<List<OrderShipmentViewModel.Order>> GetOrdersAsync(CancellationToken ct = default)
        {
            var orders = new List<OrderShipmentViewModel.Order>();

            foreach (var order in DataReport.Select(s => new { s.OrderId, s.OrderNumber, s.CreationDate, s.RequestDate, s.ExpectedReceiptDate, s.ProviderName, s.ImportNumber, s.ShipmentMethodName, s.EmbarkationPort, s.ProformaNumber, s.ForwarderName, s.ForwarderEmail, s.ForwarderFax, s.ForwarderPhone, s.ForwarderAgentName, s.AgentPhone, s.AgentFax, s.AgentEmail })
                                    .DistinctBy(d => d.OrderId)
                                    .OrderBy(o => o.OrderNumber))
            {
                orders.Add(new OrderShipmentViewModel.Order
                {
                    OrderNumber = order.OrderNumber,
                    CreationDate = order.CreationDate,
                    RequestDate = order.RequestDate,
                    ExpectedReceiptDate = order.ExpectedReceiptDate,
                    ProviderName = order.ProviderName,
                    Forwarder = new OrderShipmentViewModel.Forwarder
                    {
                        ForwarderName = order.ForwarderName,
                        Phone = order.ForwarderPhone,
                        Fax = order.ForwarderFax,
                        Email = order.ForwarderEmail
                    },
                    ForwarderAgent = new OrderShipmentViewModel.ForwarderAgent
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
                    Warehouses = await GetOrderWarehousesAsync(order.OrderId, ct)
                });
            }

            return orders;
        }

        async Task<List<OrderShipmentViewModel.Warehouse>> GetOrderWarehousesAsync(int orderId, CancellationToken ct = default)
        {
            var warehouses = new List<OrderShipmentViewModel.Warehouse>();

            foreach (var warehouse in DataReport.Where(w => w.OrderId == orderId).Select(s => new { s.WarehouseId, s.WarehouseName })
                                        .DistinctBy(d => d.WarehouseId)
                                        .OrderBy(o => o.WarehouseName))
            {
                warehouses.Add(new OrderShipmentViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    WarehouseName = warehouse.WarehouseName,
                    Lines = await GetOrderLinesAsync(orderId, warehouse.WarehouseId, ct)
                });
            }

            return warehouses;
        }

        async Task<List<OrderShipmentViewModel.Line>> GetOrderLinesAsync(int orderId, short warehouseId, CancellationToken ct = default)
        {
            var lines = new List<OrderShipmentViewModel.Line>();

            foreach (var line in DataReport.Where(w => w.OrderId == orderId && w.WarehouseId == warehouseId).Select(s => new { s.LineId, s.LineName, s.LineCode })
                                    .DistinctBy(d => d.LineId)
                                    .OrderBy(o => o.LineName))
            {
                lines.Add(new OrderShipmentViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = await GetLineItemsAsync(orderId, line.LineId, ct)
                });
            }

            return lines;
        }

        async Task<List<OrderShipmentViewModel.Item>> GetLineItemsAsync(int orderId, int lineId, CancellationToken ct = default)
        {
            var items = new List<OrderShipmentViewModel.Item>();

            foreach (var item in DataReport.Where(w => w.OrderId == orderId && w.LineId == lineId).Select(s => new { s.ItemId, s.ItemName, s.InternalReference })
                                    .DistinctBy(d => d.ItemId)
                                    .OrderBy(o => o.ItemName))
            {
                items.Add(new OrderShipmentViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = await GetItemReferencesAsync(orderId, item.ItemId, ct)
                });
            }

            return items;
        }

        async Task<List<OrderShipmentViewModel.Reference>> GetItemReferencesAsync(int orderId, int itemId, CancellationToken ct = default)
        {
            var references = new List<OrderShipmentViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.OrderId == orderId && w.ItemId == itemId).Select(s => new { s.ReferenceCode, s.ReferenceName, s.Amount, s.Volume, s.Weight })
                                            .OrderBy(o => o.ReferenceName))
            {
                references.Add(new OrderShipmentViewModel.Reference
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
