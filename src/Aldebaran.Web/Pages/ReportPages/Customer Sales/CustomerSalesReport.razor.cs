using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Customer_Sales.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Sales.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Sales
{
    public partial class CustomerSalesReport
    {
        #region Injections
        [Inject]
        protected ILogger<CustomerSalesReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected ICustomerSaleReportService CustomerSaleReportService { get; set; }
        #endregion

        #region Variables
        protected CustomerSalesFilter Filter;
        protected CustomerSalesViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;

        private IEnumerable<Application.Services.Models.Reports.CustomerSaleReport> DataReport { get; set; }
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

                DataReport = await CustomerSaleReportService.GetCustomerSaleReportDataAsync(filter, ct);

                ViewModel = new CustomerSalesViewModel
                {
                    Customers = await GetCustomersAsync()
                };

            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }

        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<CustomerSalesReportFilter>("Filtrar reporte de ventas por cliente", parameters: new Dictionary<string, object> { { "Filter", (CustomerSalesFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (CustomerSalesFilter)result;

            await RedrawReport(await SetReportFilterAsync(Filter));

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }

        async Task<string> SetReportFilterAsync(CustomerSalesFilter filter, CancellationToken ct = default)
        {
            var filterResult = string.Empty;

            if (filter.CreationDate.StartDate.HasValue)
                filterResult += $"@CreationDateFrom = '{(DateTime)filter.CreationDate.StartDate:yyyyMMdd}', @CreationDateTo = '{(DateTime)filter.CreationDate.EndDate:yyyyMMdd}'";

            if (filter.OrderDate.StartDate.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@OrderDateFrom = '{(DateTime)filter.OrderDate.StartDate:yyyyMMdd}', @OrderDateTo = '{(DateTime)filter.OrderDate.EndDate:yyyyMMdd}'";

            if (filter.EstimatedDeliveryDate.StartDate.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@EstimatedDeliveryDateFrom = '{(DateTime)filter.EstimatedDeliveryDate.StartDate:yyyyMMdd}', @EstimatedDeliveryDateTo = '{(DateTime)filter.EstimatedDeliveryDate.EndDate:yyyyMMdd}'";

            if (!filter.OrderNumber.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@OrderNumber = '{filter.OrderNumber}'";

            if (filter.StatusDocumentTypeId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@StatusDocumentTypeId = {filter.StatusDocumentTypeId}";

            if (filter.ItemReferences.Count > 0)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIds = '{String.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId))}'";

            if (filter.CustomerId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@CustomerId = {filter.CustomerId}";

            return filterResult;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-sales-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Ventas por cliente.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "customer-sales-report-container");
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

        async Task<List<CustomerSalesViewModel.Customer>> GetCustomersAsync(CancellationToken ct = default)
        {
            var customers = new List<CustomerSalesViewModel.Customer>();

            foreach (var customer in DataReport.Select(s => new { s.CustomerId, s.CustomerName, s.Fax, s.Phone })
                                        .DistinctBy(d => d.CustomerId).OrderBy(o => o.CustomerName))
            {

                customers.Add(new CustomerSalesViewModel.Customer
                {
                    CustomerName = customer.CustomerName,
                    Fax = customer.Fax,
                    Phone = customer.Phone,
                    Orders = await GetCustomerOrdersAsync(customer.CustomerId, ct)
                });
            }

            return customers;
        }

        async Task<List<CustomerSalesViewModel.Order>> GetCustomerOrdersAsync(int customerId, CancellationToken ct = default)
        {
            var customerOrders = new List<CustomerSalesViewModel.Order>();

            foreach (var order in DataReport.Where(w => w.CustomerId == customerId).Select(s => new { s.CustomerOrderId, s.CreationDate, s.OrderDate, s.OrderNumber, s.Status, s.InternalNotes, s.EstimatedDeliveryDate })
                                        .DistinctBy(d => d.CustomerOrderId).OrderBy(o => o.OrderNumber))
            {
                customerOrders.Add(new CustomerSalesViewModel.Order
                {
                    CreationDate = order.CreationDate,
                    OrderDate = order.OrderDate,
                    OrderNumber = order.OrderNumber,
                    Status = order.Status,
                    InternalNotes = order.InternalNotes,
                    EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                    References = await GetOrderReferencesAsync(order.CustomerOrderId, ct)
                });
            }

            return customerOrders;
        }

        async Task<List<CustomerSalesViewModel.Reference>> GetOrderReferencesAsync(int orderId, CancellationToken ct = default)
        {
            var orderReferences = new List<CustomerSalesViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.CustomerOrderId == orderId)
                                        .Select(s => new { s.OrderDetailId, s.Amount, s.DeliveredAmount, s.ItemName, s.ItemReference, s.ReferenceCode, s.ReferenceName })
                                        .DistinctBy(d => d.OrderDetailId).OrderBy(o => o.ItemName).OrderBy(o => o.ReferenceName))
            {
                orderReferences.Add(new CustomerSalesViewModel.Reference
                {
                    Amount = reference.Amount,
                    DeliveredAmount = reference.DeliveredAmount,
                    ItemName = reference.ItemName,
                    ItemReference = reference.ItemReference,
                    ReferenceCode = reference.ReferenceCode,
                    ReferenceName = reference.ReferenceName,
                    Shipments = await GetReferenceShipments(reference.OrderDetailId, ct)
                });
            }

            return orderReferences;
        }

        async Task<List<CustomerSalesViewModel.Shipment>> GetReferenceShipments(int orderDetailId, CancellationToken ct = default)
        {
            var referenceShipments = new List<CustomerSalesViewModel.Shipment>();

            foreach (var shipment in DataReport.Where(w => w.OrderDetailId == orderDetailId && w.ShipmentId > 0).Select(s => new { s.ShipmentId, s.ShipmentDate, s.DeliveryNote, s.TrackingNumber, s.ShipmentMethodName, s.Notes })
                                    .DistinctBy(d => d.ShipmentId).OrderBy(o => o.ShipmentDate))
            {
                referenceShipments.Add(new CustomerSalesViewModel.Shipment
                {
                    ShipmentDate = shipment.ShipmentDate,
                    DeliveryNote = shipment.DeliveryNote,
                    Notes = shipment.Notes,
                    ShipmentMethodName = shipment.ShipmentMethodName,
                    TrackingNumber = shipment.TrackingNumber,
                    References = await GetShipmentReferences(shipment.ShipmentId, orderDetailId, ct)
                });
            }

            return referenceShipments;
        }

        async Task<List<CustomerSalesViewModel.ShipmentReference>> GetShipmentReferences(int shipmentId, int orderDetailId, CancellationToken ct = default)
        {
            var shipmentReferences = new List<CustomerSalesViewModel.ShipmentReference>();

            foreach (var shipmentReference in DataReport.Where(w => w.ShipmentId == shipmentId && w.OrderDetailId == orderDetailId).Select(s => new { s.ShipmentDetailId, s.ShipmentItemReference, s.ShipmentItemName, s.ShipmentReferenceCode, s.ShipmentReferenceName, s.ShipmentAmount })
                .DistinctBy(d => d.ShipmentDetailId).OrderBy(o => o.ShipmentItemName).OrderBy(o => o.ShipmentReferenceName))
            {
                shipmentReferences.Add(new CustomerSalesViewModel.ShipmentReference
                {
                    ItemName = shipmentReference.ShipmentItemName,
                    ItemReference = shipmentReference.ShipmentItemReference,
                    ReferenceCode = shipmentReference.ShipmentReferenceCode,
                    ReferenceName = shipmentReference.ShipmentReferenceName,
                    Amount = shipmentReference.ShipmentAmount
                });
            }

            return shipmentReferences;
        }

        #endregion
    }
}
