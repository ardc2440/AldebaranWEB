using Aldebaran.Application.Services.Reports;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Text.Encodings.Web;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Orders
{
    public partial class CustomerOrderReport
    {
        #region Injections
        [Inject]
        protected ILogger<CustomerOrderReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected ICustomerOrderReportService CustomerOrderReportService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        #endregion

        #region Variables
        protected CustomerOrderFilter Filter;
        protected CustomerOrderViewModel ViewModel;
        private bool IsBusy = false;
        private readonly bool IsLoadingData = false;
        protected IEnumerable<Application.Services.Models.Reports.CustomerOrderReport> DataReport { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            DataReport = await CustomerOrderReportService.GetCustomerOrderReportDataAsync();

            ViewModel = new CustomerOrderViewModel
            {
                Customers = await GetCustomersAsync()
            };
        }
        #endregion

        #region Fill Data Report

        async Task<List<CustomerOrderViewModel.Customer>> GetCustomersAsync(CancellationToken ct = default)
        {
            var customers = new List<CustomerOrderViewModel.Customer>();

            foreach (var customer in DataReport.Select(s => new { s.CustomerId, s.CustomerName, s.CustomerNotes, s.Fax, s.Phone })
                                        .DistinctBy(d => d.CustomerId).OrderBy(o => o.CustomerName))
            {

                customers.Add(new CustomerOrderViewModel.Customer
                {
                    CustomerName = customer.CustomerName,
                    Fax = customer.Fax,
                    Phone = customer.Phone,
                    Orders = await GetCustomerOrdersAsync(customer.CustomerId, ct)
                });
            }

            return customers;
        }

        async Task<List<CustomerOrderViewModel.Order>> GetCustomerOrdersAsync(int customerId, CancellationToken ct = default)
        {
            var customerOrders = new List<CustomerOrderViewModel.Order>();

            foreach (var order in DataReport.Where(w => w.CustomerId == customerId).Select(s => new { s.OrderId, s.OrderCreationDate, s.OrderDate, s.OrderNumber, s.OrderStatus, s.InternalNotes, s.CustomerNotes, s.EstimatedDeliveryDate })
                                        .DistinctBy(d => d.OrderId).OrderBy(o => o.OrderNumber))
            {
                customerOrders.Add(new CustomerOrderViewModel.Order
                {
                    CreationDate = order.OrderCreationDate,
                    OrderDate = order.OrderDate,
                    OrderNumber = order.OrderNumber,
                    Status = order.OrderStatus,
                    InternalNotes = order.InternalNotes,
                    CustomerNotes = order.CustomerNotes,
                    EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                    References = await GetOrderReferencesAsync(order.OrderId, ct)
                });
            }

            return customerOrders;
        }

        async Task<List<CustomerOrderViewModel.Reference>> GetOrderReferencesAsync(int orderId, CancellationToken ct = default)
        {
            var orderReferences = new List<CustomerOrderViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.OrderId == orderId)
                                        .Select(s => new { s.OrderDetailId, s.DetailStatus, s.OrderDetailAmount, s.DeliveredAmount, s.InProcessAmount, s.OrderDetailItemName, s.OrderDetailItemReference, s.OrderDetailReferenceCode, s.OrderDetailReferenceName })
                                        .DistinctBy(d => d.OrderDetailId).OrderBy(o => o.OrderDetailItemName))
            {
                orderReferences.Add(new CustomerOrderViewModel.Reference
                {
                    Status = reference.DetailStatus,
                    Amount = reference.OrderDetailAmount,
                    DeliveredAmount = reference.DeliveredAmount,
                    InProcessAmount = reference.InProcessAmount,
                    ItemName = reference.OrderDetailItemName,
                    ItemReference = reference.OrderDetailItemReference,
                    ReferenceCode = reference.OrderDetailReferenceCode,
                    ReferenceName = reference.OrderDetailReferenceName,
                    Shipments = await GetReferenceShipments(reference.OrderDetailId, ct)
                });
            }

            return orderReferences;
        }

        async Task<List<CustomerOrderViewModel.Shipment>> GetReferenceShipments(int orderDetailId, CancellationToken ct = default)
        {
            var referenceShipments = new List<CustomerOrderViewModel.Shipment>();

            foreach (var shipment in DataReport.Where(w => w.OrderDetailId == orderDetailId && w.ShipmentId > 0).Select(s => new { s.ShipmentId, s.ShipmentDate, s.DeliveryNote, s.TrackingNumber, s.ShipmentMethodName, s.Notes })
                                    .DistinctBy(d => d.ShipmentId).OrderBy(o => o.ShipmentDate))
            {
                referenceShipments.Add(new CustomerOrderViewModel.Shipment
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

        async Task<List<CustomerOrderViewModel.ShipmentReference>> GetShipmentReferences(int shipmentId, int orderDetailId, CancellationToken ct = default)
        {
            var shipmentReferences = new List<CustomerOrderViewModel.ShipmentReference>();

            foreach (var shipmentReference in DataReport.Where(w => w.ShipmentId == shipmentId && w.OrderDetailId == orderDetailId).Select(s => new { s.ShipmentDetailId, s.ShipmentDetailItemReference, s.ShipmentDetailItemName, s.ShipmentDetailReferenceCode, s.ShipmentDetailReferenceName, s.ShipmentDetailAmount })
                .DistinctBy(d => d.ShipmentDetailId).OrderBy(o => o.ShipmentDetailItemName))
            {
                shipmentReferences.Add(new CustomerOrderViewModel.ShipmentReference
                {
                    ItemName = shipmentReference.ShipmentDetailItemName,
                    ItemReference = shipmentReference.ShipmentDetailItemReference,
                    ReferenceCode = shipmentReference.ShipmentDetailReferenceCode,
                    ReferenceName = shipmentReference.ShipmentDetailReferenceName,
                    Amount = shipmentReference.ShipmentDetailAmount
                });
            }

            return shipmentReferences;
        }

        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<CustomerOrderReportFilter>("Filtrar reporte de ordenes por cliente", parameters: new Dictionary<string, object> { { "Filter", (CustomerOrderFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (CustomerOrderFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-order-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Ordenes por cliente.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                NavigationManager.NavigateTo($"export/aldebarandb/customer-order/csv(fileName='{UrlEncoder.Default.Encode("Ordenes por cliente")}')", true);
            }

            if (args == null || args.Value == "xlsx")
            {
                NavigationManager.NavigateTo($"export/AldebaranDb/customer-order/excel(fileName='{UrlEncoder.Default.Encode("Ordenes por cliente")}')", true);
            }
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
