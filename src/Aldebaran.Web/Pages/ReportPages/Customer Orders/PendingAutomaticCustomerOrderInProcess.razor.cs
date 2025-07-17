using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

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

        // TODO: Inject the appropriate service when created
        // [Inject]
        // protected IPendingCustomerOrderInProcessReportService PendingCustomerOrderInProcessReportService { get; set; }
        #endregion

        #region Variables
        protected PendingCustomerOrderInProcessFilter Filter;
        protected PendingCustomerOrderInProcessViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        
        // TODO: Replace with actual data report model when service is created
        // protected IEnumerable<Application.Services.Models.Reports.PendingCustomerOrderInProcessReport> DataReport { get; set; }
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

                // TODO: Implement when service is available
                // DataReport = await PendingCustomerOrderInProcessReportService.GetPendingCustomerOrderInProcessReportDataAsync(filter, ct);

                // Mock data for now - remove this when implementing real service
                ViewModel = new PendingCustomerOrderInProcessViewModel
                {
                    CustomerOrders = await GetCustomerOrdersAsync(ct)
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
            var result = await DialogService.OpenAsync<PendingCustomerOrderInProcessReportFilter>("Filtrar reporte de pedidos bloqueados por traslado automático a proceso", 
                parameters: new Dictionary<string, object> { { "Filter", (PendingCustomerOrderInProcessFilter)Filter?.Clone() } }, 
                options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (PendingCustomerOrderInProcessFilter)result;

            await RedrawReportAsync(await SetReportFilterAsync(Filter));
        }

        async Task RemoveFilters()
        {
            await Reset();
        }

        async Task Save(RadzenSplitButtonItem args)
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "pending-customer-order-inprocess-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Pedidos en Proceso Pendientes.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "pending-customer-order-inprocess-container");
            }
            IsBusy = false;
        }

        async Task<string> SetReportFilterAsync(PendingCustomerOrderInProcessFilter filter, CancellationToken ct = default)
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

            return filterResult.TrimEnd(',', ' ');
        }
        #endregion

        #region Fill Data Report - Mock Implementation
        
        // TODO: Replace with real data implementation when service is available
        async Task<List<PendingCustomerOrderInProcessViewModel.CustomerOrder>> GetCustomerOrdersAsync(CancellationToken ct = default)
        {
            // Mock data for demonstration - replace with real data service call
            return new List<PendingCustomerOrderInProcessViewModel.CustomerOrder>
            {
                new PendingCustomerOrderInProcessViewModel.CustomerOrder
                {
                    CustomerOrderId = 1,
                    OrderNumber = "PED-2024-001",
                    OrderDate = DateTime.Now.AddDays(-15),
                    EstimatedDeliveryDate = DateTime.Now.AddDays(5),
                    StatusOrderName = "En Proceso",
                    CustomerName = "Cliente Ejemplo S.A.",
                    CustomerIdentity = "900123456-7",
                    EmployeeName = "Juan Pérez",
                    CustomerOrdersInProcess = await GetCustomerOrdersInProcessAsync(1, ct)
                }
            };
        }

        async Task<List<PendingCustomerOrderInProcessViewModel.CustomerOrderInProcess>> GetCustomerOrdersInProcessAsync(int customerOrderId, CancellationToken ct = default)
        {
            // Mock data for demonstration - replace with real data service call
            return new List<PendingCustomerOrderInProcessViewModel.CustomerOrderInProcess>
            {
                new PendingCustomerOrderInProcessViewModel.CustomerOrderInProcess
                {
                    CustomerOrderInProcessId = 1,
                    ProcessNumber = "PROC-2024-001",
                    ProcessDate = DateTime.Now.AddDays(-5),
                    TransferDatetime = DateTime.Now.AddDays(-5),
                    StatusName = "Pendiente",
                    ProcessSatelliteName = "Satélite Principal",
                    EmployeeName = "Juan Pérez",
                    EmployeeRecipientName = "María García",
                    Notes = "Proceso pendiente de completar",
                    CustomerOrderInProcessDetails = await GetCustomerOrderInProcessDetailsAsync(1, ct)
                }
            };
        }

        async Task<List<PendingCustomerOrderInProcessViewModel.CustomerOrderInProcessDetail>> GetCustomerOrderInProcessDetailsAsync(int customerOrderInProcessId, CancellationToken ct = default)
        {
            // Mock data for demonstration - replace with real data service call
            return new List<PendingCustomerOrderInProcessViewModel.CustomerOrderInProcessDetail>
            {
                new PendingCustomerOrderInProcessViewModel.CustomerOrderInProcessDetail
                {
                    CustomerOrderInProcessDetailId = 1,
                    ReferenceName = "REF-001 - Producto Ejemplo",
                    ItemName = "Producto Ejemplo",
                    Brand = "Marca A",
                    WarehouseName = "Bodega Principal",
                    ProcessedQuantity = 50,
                    PendingQuantity = 25,
                    Status = "Pendiente"
                },
                new PendingCustomerOrderInProcessViewModel.CustomerOrderInProcessDetail
                {
                    CustomerOrderInProcessDetailId = 2,
                    ReferenceName = "REF-002 - Producto Ejemplo 2",
                    ItemName = "Producto Ejemplo 2",
                    Brand = "Marca B",
                    WarehouseName = "Bodega Secundaria",
                    ProcessedQuantity = 30,
                    PendingQuantity = 15,
                    Status = "En Proceso"
                }
            };
        }

        #endregion
    }
}
