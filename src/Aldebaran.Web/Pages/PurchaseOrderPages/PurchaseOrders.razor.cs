using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class PurchaseOrders
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        protected IEnumerable<Models.AldebaranDb.PurchaseOrder> purchaseOrders;

        protected RadzenDataGrid<Models.AldebaranDb.PurchaseOrder> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            purchaseOrders = await AldebaranDbService.GetPurchaseOrders(new Query { Filter = $@"i => i.ORDER_NUMBER.Contains(@0) || i.ASPNETUSER_ID.Contains(@0) || i.IMPORT_NUMBER.Contains(@0) || i.EMBARKATION_PORT.Contains(@0) || i.PROFORMA_NUMBER.Contains(@0)", FilterParameters = new object[] { search }, Expand = "ForwarderAgent,Provider,ShipmentForwarderAgentMethod" });
        }
        protected override async Task OnInitializedAsync()
        {
            purchaseOrders = await AldebaranDbService.GetPurchaseOrders(new Query { Filter = $@"i => i.ORDER_NUMBER.Contains(@0) || i.ASPNETUSER_ID.Contains(@0) || i.IMPORT_NUMBER.Contains(@0) || i.EMBARKATION_PORT.Contains(@0) || i.PROFORMA_NUMBER.Contains(@0)", FilterParameters = new object[] { search }, Expand = "ForwarderAgent,Provider,ShipmentForwarderAgentMethod" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddPurchaseOrder>("Add PurchaseOrder", null);
            await grid0.Reload();
        }

        protected async Task EditRow(Models.AldebaranDb.PurchaseOrder args)
        {
            await DialogService.OpenAsync<EditPurchaseOrder>("Edit PurchaseOrder", new Dictionary<string, object> { { "PURCHASE_ORDER_ID", args.PURCHASE_ORDER_ID } });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.PurchaseOrder purchaseOrder)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeletePurchaseOrder(purchaseOrder.PURCHASE_ORDER_ID);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete PurchaseOrder"
                });
            }
        }

        protected Models.AldebaranDb.PurchaseOrder purchaseOrder;
        protected async Task GetChildData(Models.AldebaranDb.PurchaseOrder args)
        {
            purchaseOrder = args;
            var PurchaseOrderActivitiesResult = await AldebaranDbService.GetPurchaseOrderActivities(new Query { Filter = $@"i => i.PURCHASE_ORDER_ID == {args.PURCHASE_ORDER_ID}", Expand = "PurchaseOrder" });
            if (PurchaseOrderActivitiesResult != null)
            {
                args.PurchaseOrderActivities = PurchaseOrderActivitiesResult.ToList();
            }
            var PurchaseOrderDetailsResult = await AldebaranDbService.GetPurchaseOrderDetails(new Query { Filter = $@"i => i.PURCHASE_ORDER_ID == {args.PURCHASE_ORDER_ID}", Expand = "PurchaseOrder,ItemReference,Warehouse" });
            if (PurchaseOrderDetailsResult != null)
            {
                args.PurchaseOrderDetails = PurchaseOrderDetailsResult.ToList();
            }
        }

        protected RadzenDataGrid<Models.AldebaranDb.PurchaseOrderActivity> PurchaseOrderActivitiesDataGrid;

        protected async Task PurchaseOrderActivitiesAddButtonClick(MouseEventArgs args, Models.AldebaranDb.PurchaseOrder data)
        {
            var dialogResult = await DialogService.OpenAsync<AddPurchaseOrderActivity>("Add PurchaseOrderActivities", new Dictionary<string, object> { { "PURCHASE_ORDER_ID", data.PURCHASE_ORDER_ID } });
            await GetChildData(data);
            await PurchaseOrderActivitiesDataGrid.Reload();
        }

        protected async Task PurchaseOrderActivitiesRowSelect(Models.AldebaranDb.PurchaseOrderActivity args, Models.AldebaranDb.PurchaseOrder data)
        {
            var dialogResult = await DialogService.OpenAsync<EditPurchaseOrderActivity>("Edit PurchaseOrderActivities", new Dictionary<string, object> { { "PURCHASE_ORDER_ACTIVITY_ID", args.PURCHASE_ORDER_ACTIVITY_ID } });
            await GetChildData(data);
            await PurchaseOrderActivitiesDataGrid.Reload();
        }

        protected async Task PurchaseOrderActivitiesDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.PurchaseOrderActivity purchaseOrderActivity)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeletePurchaseOrderActivity(purchaseOrderActivity.PURCHASE_ORDER_ACTIVITY_ID);

                    await GetChildData(purchaseOrder);

                    if (deleteResult != null)
                    {
                        await PurchaseOrderActivitiesDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete PurchaseOrderActivity"
                });
            }
        }

        protected RadzenDataGrid<Models.AldebaranDb.PurchaseOrderDetail> PurchaseOrderDetailsDataGrid;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task PurchaseOrderDetailsAddButtonClick(MouseEventArgs args, Models.AldebaranDb.PurchaseOrder data)
        {
            var dialogResult = await DialogService.OpenAsync<AddPurchaseOrderDetail>("Add PurchaseOrderDetails", new Dictionary<string, object> { { "PURCHASE_ORDER_ID", data.PURCHASE_ORDER_ID } });
            await GetChildData(data);
            await PurchaseOrderDetailsDataGrid.Reload();
        }

        protected async Task PurchaseOrderDetailsRowSelect(Models.AldebaranDb.PurchaseOrderDetail args, Models.AldebaranDb.PurchaseOrder data)
        {
            var dialogResult = await DialogService.OpenAsync<EditPurchaseOrderDetail>("Edit PurchaseOrderDetails", new Dictionary<string, object> { { "PURCHASE_ORDER_DETAIL_ID", args.PURCHASE_ORDER_DETAIL_ID } });
            await GetChildData(data);
            await PurchaseOrderDetailsDataGrid.Reload();
        }

        protected async Task PurchaseOrderDetailsDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.PurchaseOrderDetail purchaseOrderDetail)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeletePurchaseOrderDetail(purchaseOrderDetail.PURCHASE_ORDER_DETAIL_ID);

                    await GetChildData(purchaseOrder);

                    if (deleteResult != null)
                    {
                        await PurchaseOrderDetailsDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete PurchaseOrderDetail"
                });
            }
        }
    }
}