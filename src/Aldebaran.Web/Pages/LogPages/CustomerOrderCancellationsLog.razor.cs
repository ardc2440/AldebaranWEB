using Aldebaran.Application.Services;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;


namespace Aldebaran.Web.Pages.LogPages
{
    public partial class CustomerOrderCancellationsLog
    {
        #region Injections
        [Inject]
        protected ILogger<CustomerOrderCancellationsLog> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }
        #endregion

        #region Variables
        protected IEnumerable<ServiceModel.CanceledCustomerOrder> CanceledCustomerOrders;
        protected LocalizedDataGrid<ServiceModel.CanceledCustomerOrder> CanceledCustomerOrdersDataGrid;
        protected string search = "";
        protected bool isLoadingInProgress;

        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;
        #endregion

        #region Events

        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip.Value;
            top = args.Top.Value;
            await GetDataLogAsync(search);
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task GetDataLogAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            (CanceledCustomerOrders, count) = await CustomerOrderService.GetCustomerOrderCancellationsLogAsync(skip, top, searchKey, ct);
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await CanceledCustomerOrdersDataGrid.GoToPage(0);
            await GetDataLogAsync(search);
        }

        public async Task CustomerOrderDetailInfo(int customerOrderId)
        {
            var reasonResult = await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido", new Dictionary<string, object> { { "CustomerOrderId", customerOrderId } }, options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
            if (reasonResult == null)
                return;
        }
        #endregion

    }
}
