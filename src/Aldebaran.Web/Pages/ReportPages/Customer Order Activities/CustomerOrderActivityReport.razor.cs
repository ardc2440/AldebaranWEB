using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities
{
    public partial class CustomerOrderActivityReport
    {
        #region Injections
        [Inject]
        protected ILogger<CustomerOrderActivityReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected ICustomerOrderActivityReportService CustomerOrderActivityReportService { get; set; }
        #endregion

        #region Variables
        protected CustomerOrderActivityFilter Filter;
        protected CustomerOrderActivityViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;

        private IEnumerable<Application.Services.Models.Reports.CustomerOrderActivityReport> DataReport { get; set; }
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

                DataReport = await CustomerOrderActivityReportService.GetCustomerOrderActivityReportDataAsync(filter, ct);

                ViewModel = new CustomerOrderActivityViewModel
                {
                    Customers = await GetCustomersAsync(ct)
                };
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        async Task<string> SetReportFilterAsync(CustomerOrderActivityFilter filter, CancellationToken ct = default)
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
            var result = await DialogService.OpenAsync<CustomerOrderActivityReportFilter>("Filtrar reporte de actividades de pedidos", parameters: new Dictionary<string, object> { { "Filter", (CustomerOrderActivityFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (CustomerOrderActivityFilter)result;

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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-order-activity-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Actividades de pedidos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "customer-order-activity-report-container");
            }
            IsBusy = false;
        }

        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion

        #region Fill Data Report
        async Task<List<CustomerOrderActivityViewModel.Customer>> GetCustomersAsync(CancellationToken ct = default)
        {
            var customers = new List<CustomerOrderActivityViewModel.Customer>();

            foreach (var customer in DataReport.Select(s => new { s.CustomerId, s.CustomerName, s.Phone, s.Fax })
                                        .DistinctBy(d => d.CustomerId)
                                        .OrderBy(o => o.CustomerName))
            {
                customers.Add(new CustomerOrderActivityViewModel.Customer
                {
                    CustomerName = customer.CustomerName,
                    Fax = customer.Fax,
                    Phone = customer.Phone,
                    Orders = await GetCustomerOrdersAsync(customer.CustomerId, ct)
                });
            }

            return customers;
        }

        async Task<List<CustomerOrderActivityViewModel.Order>> GetCustomerOrdersAsync(int customerId, CancellationToken ct = default)
        {
            var orders = new List<CustomerOrderActivityViewModel.Order>();

            foreach (var order in DataReport.Where(w => w.CustomerId == customerId).Select(s => new { s.OrderId, s.OrderNumber, s.CreationDate, s.OrderDate, s.EstimatedDeliveryDate, s.StatusOrder, s.InternalNotes, s.CustomerNotes })
                                    .DistinctBy(d => d.OrderId)
                                    .OrderBy(o => o.OrderNumber))

            {
                orders.Add(new CustomerOrderActivityViewModel.Order
                {
                    CreationDate = order.CreationDate,
                    Status = order.StatusOrder,
                    OrderNumber = order.OrderNumber,
                    CustomerNotes = order.CustomerNotes,
                    EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                    InternalNotes = order.InternalNotes,
                    OrderDate = order.OrderDate,
                    Activities = await GetOrderActivitiesAsync(order.OrderId, ct),
                    References = await GetOrderReferencesAsync(order.OrderId, ct)
                });
            }

            return orders;

        }

        async Task<List<CustomerOrderActivityViewModel.Activity>> GetOrderActivitiesAsync(int orderId, CancellationToken ct = default)
        {
            var activities = new List<CustomerOrderActivityViewModel.Activity>();

            foreach (var activity in DataReport.Where(w => w.OrderId == orderId && !(w.ActivityId == 0)).Select(s => new { s.ActivityId, s.CreationDateActivity, s.AreaName, s.EmployeeName, s.Notes })
                                        .DistinctBy(d => d.ActivityId)
                                        .OrderBy(o => o.CreationDateActivity))
            {
                activities.Add(new CustomerOrderActivityViewModel.Activity
                {
                    AreaName = activity.AreaName,
                    CreationDate = activity.CreationDateActivity,
                    EmployeeName = activity.EmployeeName,
                    Notes = activity.Notes,
                    Details = await GetActivityDetailsAsync(activity.ActivityId, ct)
                });
            }

            return activities;
        }

        async Task<List<CustomerOrderActivityViewModel.Reference>> GetOrderReferencesAsync(int orderId, CancellationToken ct = default)
        {
            var references = new List<CustomerOrderActivityViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.OrderId == orderId).Select(s => new { s.ReferenceId, s.ItemReference, s.ItemName, s.ReferenceCode, s.ReferenceName, s.Amount, s.DeliveredAmount, s.InProcessAmount, s.StatusDetail })
                                        .DistinctBy(d => d.ReferenceId)
                                        .OrderBy(o => o.ItemName)
                                        .OrderBy(o => o.ReferenceName))
            {
                references.Add(new CustomerOrderActivityViewModel.Reference
                {
                    Amount = reference.Amount,
                    DeliveredAmount = reference.DeliveredAmount,
                    InProcessAmount = reference.InProcessAmount,
                    ItemName = reference.ItemName,
                    ItemReference = reference.ItemReference,
                    ReferenceCode = reference.ReferenceCode,
                    ReferenceName = reference.ReferenceName,
                    Status = reference.StatusDetail
                });
            }

            return references;
        }

        async Task<List<CustomerOrderActivityViewModel.ActivityDetail>> GetActivityDetailsAsync(int activityId, CancellationToken ct = default)
        {
            var activityDetails = new List<CustomerOrderActivityViewModel.ActivityDetail>();

            foreach (var activityDetail in DataReport.Where(w => w.ActivityId == activityId && !w.ActivityType.IsNullOrEmpty()).Select(s => new { s.ActivityType, s.EmployeNameDetail })
                                        .DistinctBy(d => d.ActivityType)
                                        .OrderBy(o => o.ActivityType))
            {
                activityDetails.Add(new CustomerOrderActivityViewModel.ActivityDetail
                {
                    ActivityTypeName = activityDetail.ActivityType,
                    EmployeeName = activityDetail.EmployeNameDetail
                });
            }

            return activityDetails;
        }

        #endregion
    }
}
