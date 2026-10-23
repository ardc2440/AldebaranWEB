using Aldebaran.Application.Services.Models.OrderDocumentLogs;
using Aldebaran.Application.Services.OrderDocumentLogs;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>Logs de modificación y cancelación de despachos y traslados a proceso (el tipo llega en la URL).</summary>
    public partial class OrderDocumentLog
    {
        private const int DefaultDays = 30;

        #region Injections
        [Inject] protected ILogger<OrderDocumentLog> Logger { get; set; }
        [Inject] protected DialogService DialogService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IOrderDocumentLogService OrderDocumentLogService { get; set; }
        #endregion

        #region Parameters
        [Parameter] public string LogRoute { get; set; }
        #endregion

        #region Variables
        protected OrderDocumentLogInfo info;
        protected IEnumerable<OrderDocumentLogItem> items;
        protected LocalizedDataGrid<OrderDocumentLogItem> logGrid;
        protected DateTime? dateFrom;
        protected DateTime? dateTo;
        protected string search = "";
        protected string errorMessage;
        protected bool isLoading;
        protected int count;
        private int skip;
        private int top = 20;
        #endregion

        #region Lifecycle
        protected override async Task OnParametersSetAsync()
        {
            info = OrderDocumentLogInfo.FromRoute(LogRoute);
            if (info == null)
            {
                NavigationManager.NavigateTo("unauthorized");
                return;
            }

            dateTo = DateTime.Today;
            dateFrom = DateTime.Today.AddDays(-DefaultDays);
            search = "";
            errorMessage = null;
            if (logGrid != null)
                await ApplyFiltersAsync();
        }
        #endregion

        #region Events
        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip ?? 0;
            top = args.Top ?? top;
            await GetLogAsync();
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ApplyFiltersAsync();
        }

        protected async Task ApplyFiltersAsync()
        {
            skip = 0;
            await logGrid.FirstPage(true);
        }

        protected async Task ShowCustomerOrderAsync(int customerOrderId) =>
            await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido",
                new Dictionary<string, object> { { "CustomerOrderId", customerOrderId } },
                new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
        #endregion

        #region Private
        private string ValidateFilters()
        {
            if (dateFrom == null || dateTo == null)
                return "El rango de fechas es obligatorio.";
            if (dateFrom > dateTo)
                return "La fecha inicial no puede ser mayor que la final.";
            return null;
        }

        private async Task GetLogAsync()
        {
            errorMessage = ValidateFilters();
            if (errorMessage != null)
            {
                items = Array.Empty<OrderDocumentLogItem>();
                count = 0;
                return;
            }

            try
            {
                isLoading = true;
                var page = await OrderDocumentLogService.GetLogAsync(info.LogType, dateFrom!.Value, dateTo!.Value, search, skip, top);
                items = page.Items;
                count = page.TotalCount;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetLogAsync));
                errorMessage = "No fue posible consultar el log.";
            }
            finally
            {
                isLoading = false;
            }
        }
        #endregion
    }
}
