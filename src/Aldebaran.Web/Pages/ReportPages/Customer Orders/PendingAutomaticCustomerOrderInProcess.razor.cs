using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Orders
{
    public partial class PendingAutomaticCustomerOrderInProcess
    {
        #region Injections
        [Inject]
        protected ILogger<PendingAutomaticCustomerOrderInProcess> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IAutomaticPurchaseOrderAssigmentReportService PendingCustomerOrderInProcessReportService { get; set; }
        #endregion

        #region Variables
        protected PendingAutomaticCustomerOrderInProcessFilter Filter;
        protected PendingAutomaticCustomerOrderInProcessViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;

        protected IEnumerable<Application.Services.Models.Reports.AutomaticPendingCustomerOrderInProcessReport> DataReport { get; set; }
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
            await OpenFilters();
        }

        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;

                // Implement when service is available
                DataReport = await PendingCustomerOrderInProcessReportService.GetAutomaticPendingCustomerOrderInProcessReportDataAsync(filter, ct);

                // Mock data for now - remove this when implementing real service
                ViewModel = new PendingAutomaticCustomerOrderInProcessViewModel
                {
                    CustomerOrders = await GetCustomerOrdersAsync(ct)
                };

                Logger.LogInformation("Data report loaded successfully.");
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<PendingAutomaticCustomerOrderInProcessReportFilter>("Filtrar reporte de pedidos bloqueados por traslado automático a proceso",
                parameters: new Dictionary<string, object> { { "Filter", (PendingAutomaticCustomerOrderInProcessFilter)Filter?.Clone() } },
                options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (PendingAutomaticCustomerOrderInProcessFilter)result;

            await RedrawReportAsync(await SetReportFilterAsync(Filter));
        }

        async Task RemoveFilters()
        {
            await Reset();
        }

        async Task<string> SetReportFilterAsync(PendingAutomaticCustomerOrderInProcessFilter filter, CancellationToken ct = default)
        {
            var filterResult = string.Empty;

            if (!string.IsNullOrEmpty(filter.OrderNumber))
                filterResult += $"@OrderNumber = '{filter.OrderNumber}', ";

            if (filter.OrderDate?.StartDate.HasValue == true)
                filterResult += $"@OrderDateFrom = '{filter.OrderDate.StartDate:yyyyMMdd}', ";

            if (filter.OrderDate?.EndDate.HasValue == true)
                filterResult += $"@OrderDateTo = '{filter.OrderDate.EndDate:yyyyMMdd}', ";

            if (filter.ProcessDate?.StartDate.HasValue == true)
                filterResult += $"@ProcessDateFrom = '{filter.ProcessDate.StartDate:yyyyMMdd}', ";

            if (filter.ProcessDate?.EndDate.HasValue == true)
                filterResult += $"@ProcessDateTo = '{filter.ProcessDate.EndDate:yyyyMMdd}', ";

            if (filter.CustomerId.HasValue)
                filterResult += $"@CustomerId = {filter.CustomerId}, ";

            if (filter.StatusDocumentTypeId.HasValue)
                filterResult += $"@StatusDocumentTypeId = {filter.StatusDocumentTypeId}, ";

            if (filter.ItemReferences.Count > 0)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIds = '{String.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId))}'";


            return filterResult.TrimEnd(',', ' ');
        }
        #endregion

        #region Fill Data Report - Mock Implementation
        async Task<List<PendingAutomaticCustomerOrderInProcessViewModel.CustomerOrder>> GetCustomerOrdersAsync(CancellationToken ct = default)
        {

            var customers = DataReport
                .Select(s => new { s.CustomerOrderId, s.OrderNumber, s.OrderDate, s.EstimatedDeliveryDate, s.StatusOrderName, s.CustomerName, s.CustomerIdentity, s.CustomerOrderEmployeeName })
                .DistinctBy(d => d.CustomerOrderId)
                .OrderBy(o => o.OrderDate)
                .Select(async customer => new PendingAutomaticCustomerOrderInProcessViewModel.CustomerOrder
                {
                    CustomerOrderId = customer.CustomerOrderId,
                    OrderNumber = customer.OrderNumber,
                    OrderDate = customer.OrderDate,
                    EstimatedDeliveryDate = customer.EstimatedDeliveryDate,
                    StatusOrderName = customer.StatusOrderName,
                    CustomerName = customer.CustomerName,
                    CustomerIdentity = customer.CustomerIdentity,
                    EmployeeName = customer.CustomerOrderEmployeeName,
                    CustomerOrdersInProcess = await GetCustomerOrdersInProcessAsync(customer.CustomerOrderId, ct)
                });

            return (await Task.WhenAll(customers)).ToList();
        }

        async Task<List<PendingAutomaticCustomerOrderInProcessViewModel.CustomerOrderInProcess>> GetCustomerOrdersInProcessAsync(int customerOrderId, CancellationToken ct = default)
        {
            var inProcessOrders = DataReport.Where(w => w.CustomerOrderId == customerOrderId)
                                                              .Select(s => new { s.CustomerOrderInProcessId, s.ProcessDate, s.TransferDatetime, s.Notes })
                                                              .DistinctBy(d => d.CustomerOrderInProcessId)
                                                              .OrderBy(o => o.ProcessDate)
                                                              .Select(async inProcessOrder => new PendingAutomaticCustomerOrderInProcessViewModel.CustomerOrderInProcess
                                                              {
                                                                  CustomerOrderInProcessId = inProcessOrder.CustomerOrderInProcessId,
                                                                  ProcessDate = inProcessOrder.ProcessDate,
                                                                  TransferDatetime = inProcessOrder.TransferDatetime,
                                                                  Notes = inProcessOrder.Notes,
                                                                  CustomerOrderInProcessDetails = await GetCustomerOrderInProcessDetailsAsync(inProcessOrder.CustomerOrderInProcessId, ct)
                                                              });
            return (await Task.WhenAll(inProcessOrders)).ToList();
        }

        async Task<List<PendingAutomaticCustomerOrderInProcessViewModel.CustomerOrderInProcessDetail>> GetCustomerOrderInProcessDetailsAsync(int customerOrderInProcessId, CancellationToken ct = default)
        {
            var inProcessOrderDetails = DataReport.Where(w => w.CustomerOrderInProcessId == customerOrderInProcessId)
                                                                  .Select(s => new { s.ItemName, s.InternalReference, s.ReferenceName, s.Brand, s.WarehouseName, s.Quantity })
                                                                  .OrderBy(o => new { o.InternalReference, o.ReferenceName })
                                                                  .Select(async inProcessOrder => new PendingAutomaticCustomerOrderInProcessViewModel.CustomerOrderInProcessDetail
                                                                  {
                                                                      ItemName = inProcessOrder.ItemName,
                                                                      InternalReference = inProcessOrder.InternalReference,
                                                                      ReferenceName = inProcessOrder.ReferenceName,
                                                                      Brand = inProcessOrder.Brand,
                                                                      WarehouseName = inProcessOrder.WarehouseName,
                                                                      Quantity = inProcessOrder.Quantity,
                                                                  });
            return (await Task.WhenAll(inProcessOrderDetails)).ToList();
        }

        #endregion
    }
}
