using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel;
using Microsoft.AspNetCore.Components;
using ReportFilter = Aldebaran.Application.Services.Models.Reports.AutomaticPendingCustomerOrderInProcessReportFilter;
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

        async Task RedrawReportAsync(ReportFilter filter, CancellationToken ct = default)
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

            await RedrawReportAsync(ToReportFilter(Filter));
        }

        async Task RemoveFilters()
        {
            await Reset();
        }

        /// <summary>Convierte el filtro de la pantalla en el filtro del caso de uso (sin armar SQL).</summary>
        private static ReportFilter ToReportFilter(PendingAutomaticCustomerOrderInProcessFilter filter) => new()
        {
            OrderNumber = string.IsNullOrWhiteSpace(filter.OrderNumber) ? null : filter.OrderNumber.Trim(),
            OrderDateFrom = filter.OrderDate?.StartDate,
            OrderDateTo = filter.OrderDate?.EndDate,
            ProcessDateFrom = filter.ProcessDate?.StartDate,
            ProcessDateTo = filter.ProcessDate?.EndDate,
            CustomerId = filter.CustomerId,
            StatusDocumentTypeId = filter.StatusDocumentTypeId,
            ReferenceIds = filter.ItemReferences.Count > 0 ? string.Join(",", filter.ItemReferences.Select(s => s.ReferenceId)) : null,
            IsSpecialOrder = filter.IsSpecialOrder
        };
        #endregion

        #region Fill Data Report - Mock Implementation
        async Task<List<PendingAutomaticCustomerOrderInProcessViewModel.CustomerOrder>> GetCustomerOrdersAsync(CancellationToken ct = default)
        {

            var customers = DataReport
                .Select(s => new { s.CustomerOrderId, s.OrderNumber, s.IsSpecialOrder, s.OrderDate, s.EstimatedDeliveryDate, s.StatusOrderName, s.CustomerName, s.CustomerIdentity, s.CustomerOrderEmployeeName })
                .DistinctBy(d => d.CustomerOrderId)
                .OrderBy(o => o.OrderDate)
                .Select(async customer => new PendingAutomaticCustomerOrderInProcessViewModel.CustomerOrder
                {
                    CustomerOrderId = customer.CustomerOrderId,
                    OrderNumber = customer.OrderNumber,
                    IsSpecialOrder = customer.IsSpecialOrder,
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
                                                                  .OrderBy(o => o.InternalReference).ThenBy(o => o.ReferenceName)
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
