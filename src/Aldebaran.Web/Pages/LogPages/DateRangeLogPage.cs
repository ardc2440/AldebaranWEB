using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>Base de las pantallas de log con rango de fechas obligatorio, búsqueda y paginación en servidor.</summary>
    public abstract class DateRangeLogPage<TItem> : ComponentBase
    {
        private const int DefaultDays = 30;

        [Inject] protected ILogger<DateRangeLogPage<TItem>> Logger { get; set; }
        [Inject] protected DialogService DialogService { get; set; }

        protected IEnumerable<TItem> items;
        protected LocalizedDataGrid<TItem> logGrid;
        protected DateTime? dateFrom = DateTime.Today.AddDays(-DefaultDays);
        protected DateTime? dateTo = DateTime.Today;
        protected string search = "";
        protected string errorMessage;
        protected bool isLoading;
        protected int count;
        private int skip;
        private int top = 20;

        /// <summary>Consulta una página del log.</summary>
        protected abstract Task<PagedResult<TItem>> FetchAsync(DateTime from, DateTime to, string searchKey, int skip, int take);

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

        protected async Task ShowPurchaseOrderAsync(int purchaseOrderId) =>
            await DialogService.OpenAsync<PurchaseOrderPages.PurchaseOrderDetails>("Detalles de la orden de compra",
                new Dictionary<string, object> { { "PurchaseOrderId", purchaseOrderId } },
                new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });

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
                items = Array.Empty<TItem>();
                count = 0;
                return;
            }

            try
            {
                isLoading = true;
                var page = await FetchAsync(dateFrom!.Value, dateTo!.Value, search, skip, top);
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
    }
}
