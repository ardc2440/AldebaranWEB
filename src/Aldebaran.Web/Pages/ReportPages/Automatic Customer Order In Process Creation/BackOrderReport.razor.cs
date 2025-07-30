using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Automatic_Customer_Order_In_Process_Creation.Components;
using Aldebaran.Web.Pages.ReportPages.Automatic_Customer_Order_In_Process_Creation.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Text.Encodings.Web;

namespace Aldebaran.Web.Pages.ReportPages.Automatic_Customer_Order_In_Process_Creation
{
    public partial class BackOrderReport
    {
        #region Injections
        [Inject]
        protected ILogger<BackOrderReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected ICustomerOrderReportService CustomerOrderReportService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        #endregion

        #region Variables
        protected BackOrderFilter Filter;
        protected BackOrderViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        protected IEnumerable<Application.Services.Models.Reports.BackOrderReport> DataReport { get; set; }
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

        #region Fill Data Report

        async Task<List<BackOrderViewModel.Customer>> GetCustomersAsync(CancellationToken ct = default)
        {
            var customers = new List<BackOrderViewModel.Customer>();

            // Para un backorder, necesitamos ordenar por fecha de creación de cada orden individual
            // CADA ORDEN SERÁ UNA ENTRADA SEPARADA, NO AGRUPADA POR CLIENTE
            
            var allRecords = DataReport.ToList();
            
            // Obtener todas las órdenes ordenadas por fecha de creación (más antigua primero)
            var ordersOrderedByDate = allRecords
                .Select(x => new { x.OrderId, x.OrderCreationDate, x.CustomerId, x.CustomerName })
                .DistinctBy(x => x.OrderId)
                .OrderBy(x => x.OrderCreationDate) // Más antigua primero para backorder
                .ToList();

            // CREAR UNA ENTRADA SEPARADA PARA CADA ORDEN (NO AGRUPAR POR CLIENTE)
            foreach (var orderInfo in ordersOrderedByDate)
            {
                // Obtener información del cliente para esta orden
                var customerData = allRecords.First(x => x.CustomerId == orderInfo.CustomerId);
                
                // SIEMPRE crear un nuevo cliente para cada orden (no agrupar)
                var customerForThisOrder = new BackOrderViewModel.Customer
                {
                    CustomerName = customerData.CustomerName,
                    Fax = customerData.Fax,
                    Phone = customerData.Phone,
                    Orders = new List<BackOrderViewModel.Order>()
                };

                // Agregar solo esta orden al cliente
                var orderDetails = await GetSingleOrderAsync(orderInfo.OrderId, ct);
                if (orderDetails != null)
                {
                    customerForThisOrder.Orders.Add(orderDetails);
                }

                // Agregar el cliente (con una sola orden) a la lista
                customers.Add(customerForThisOrder);
            }

            return customers;
        }

        // Nuevo método para obtener una orden específica
        async Task<BackOrderViewModel.Order> GetSingleOrderAsync(int orderId, CancellationToken ct = default)
        {
            var orderData = DataReport.Where(w => w.OrderId == orderId)
                .Select(s => new { s.OrderId, s.OrderCreationDate, s.OrderDate, s.OrderNumber, s.OrderStatus, s.InternalNotes, s.CustomerNotes, s.EstimatedDeliveryDate })
                .FirstOrDefault();

            if (orderData == null)
                return null;

            return new BackOrderViewModel.Order
            {
                CreationDate = orderData.OrderCreationDate,
                OrderDate = orderData.OrderDate,
                OrderNumber = orderData.OrderNumber,
                Status = orderData.OrderStatus,
                InternalNotes = orderData.InternalNotes,
                CustomerNotes = orderData.CustomerNotes,
                EstimatedDeliveryDate = orderData.EstimatedDeliveryDate,
                References = await GetOrderReferencesAsync(orderData.OrderId, ct)
            };
        }

        async Task<List<BackOrderViewModel.Reference>> GetOrderReferencesAsync(int orderId, CancellationToken ct = default)
        {
            var orderReferences = new List<BackOrderViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.OrderId == orderId)
                                        .Select(s => new { s.OrderDetailId, s.DetailStatus, s.OrderDetailPendingAmount, s.OrderDetailItemName, s.OrderDetailItemReference, s.OrderDetailReferenceCode, s.OrderDetailReferenceName })
                                        .DistinctBy(d => d.OrderDetailId).OrderBy(o => o.OrderDetailItemName).ThenBy(o => o.OrderDetailReferenceName))
            {
                orderReferences.Add(new BackOrderViewModel.Reference
                {
                    Status = reference.DetailStatus,
                    PendingAmount = reference.OrderDetailPendingAmount,
                    ItemName = reference.OrderDetailItemName,
                    ItemReference = reference.OrderDetailItemReference,
                    ReferenceCode = reference.OrderDetailReferenceCode,
                    ReferenceName = reference.OrderDetailReferenceName
                });
            }

            return orderReferences;
        }

        // Método para obtener todas las órdenes ordenadas cronológicamente para mostrar en el reporte
        protected List<(BackOrderViewModel.Customer Customer, BackOrderViewModel.Order Order)> GetOrdersInChronologicalOrder()
        {
            var ordersWithCustomers = new List<(BackOrderViewModel.Customer Customer, BackOrderViewModel.Order Order)>();

            if (ViewModel?.Customers?.Any() == true)
            {
                // Ahora cada cliente tiene solo una orden, así que es más simple
                foreach (var customer in ViewModel.Customers)
                {
                    if (customer.Orders?.Any() == true)
                    {
                        // Como cada cliente tiene solo una orden, tomamos la primera (y única)
                        var order = customer.Orders.First();
                        ordersWithCustomers.Add((customer, order));
                    }
                }
            }

            // Ya vienen ordenadas cronológicamente por el método GetCustomersAsync
            return ordersWithCustomers;
        }

        // Propiedad para simplificar el acceso desde Razor
        protected List<(BackOrderViewModel.Customer Customer, BackOrderViewModel.Order Order)> ChronologicalOrders => GetOrdersInChronologicalOrder();

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
        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;
                DataReport = await CustomerOrderReportService.GetBackOrderReportDataAsync(filter, ct);

                ViewModel = new BackOrderViewModel
                {
                    Customers = await GetCustomersAsync(ct)
                };
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }

        }

        async Task<string> SetReportFilterAsync(BackOrderFilter filter, CancellationToken ct = default)
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

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<BackOrderReportFilter>("Filtrar reporte de ordenes por cliente", parameters: new Dictionary<string, object> { { "Filter", (BackOrderFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (BackOrderFilter)result;

            await RedrawReportAsync(await SetReportFilterAsync(Filter));

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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "back-order-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Backorder por cliente.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "back-order-report-container");
            }
            IsBusy = false;
        }
        async Task ExportClick(RadzenSplitButtonItem args)
        {
            var filter = Filter == null ? "NoFilter" : await SetReportFilterAsync(Filter);

            if (args?.Value == "csv")
            {
                NavigationManager.NavigateTo($"export/aldebarandb/back-order/csv(fileName='{UrlEncoder.Default.Encode("Backorder por cliente")}',filter='{UrlEncoder.Default.Encode(filter)}')", true);
            }

            if (args == null || args.Value == "xlsx")
            {
                NavigationManager.NavigateTo($"export/AldebaranDb/back-order/excel(fileName='{UrlEncoder.Default.Encode("Backorder por cliente")}',filter='{UrlEncoder.Default.Encode(filter)}')", true);
            }
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
    }
}
