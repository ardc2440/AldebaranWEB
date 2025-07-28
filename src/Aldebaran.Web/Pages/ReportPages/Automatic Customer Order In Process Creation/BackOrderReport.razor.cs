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

            foreach (var customer in DataReport.Select(s => new { s.CustomerId, s.CustomerName, s.Fax, s.Phone })
                                        .DistinctBy(d => d.CustomerId).OrderBy(o => o.CustomerName))
            {

                customers.Add(new BackOrderViewModel.Customer
                {
                    CustomerName = customer.CustomerName,
                    Fax = customer.Fax,
                    Phone = customer.Phone,
                    Orders = await GetCustomerOrdersAsync(customer.CustomerId, ct)
                });
            }

            return customers;
        }

        async Task<List<BackOrderViewModel.Order>> GetCustomerOrdersAsync(int customerId, CancellationToken ct = default)
        {
            var customerOrders = new List<BackOrderViewModel.Order>();

            foreach (var order in DataReport.Where(w => w.CustomerId == customerId).Select(s => new { s.OrderId, s.OrderCreationDate, s.OrderDate, s.OrderNumber, s.OrderStatus, s.InternalNotes, s.CustomerNotes, s.EstimatedDeliveryDate })
                                        .DistinctBy(d => d.OrderId).OrderBy(o => o.OrderNumber))
            {
                customerOrders.Add(new BackOrderViewModel.Order
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
