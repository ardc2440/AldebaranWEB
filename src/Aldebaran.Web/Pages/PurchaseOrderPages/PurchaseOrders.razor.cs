using Aldebaran.Web.Models;
using Aldebaran.Web.Models.AldebaranDb;
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
        protected bool isLoadingInProgress;
        protected DialogResult dialogResult { get; set; }
        [Inject]
        protected SecurityService Security { get; set; }
        [Parameter]
        public string OrderNumber { get; set; } = null;
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            purchaseOrders = await AldebaranDbService.GetPurchaseOrders(new Query { Filter = $@"i => i.ORDER_NUMBER.Contains(@0) || i.EMPLOYEE_ID.Contains(@0) || i.IMPORT_NUMBER.Contains(@0) || i.EMBARKATION_PORT.Contains(@0) || i.PROFORMA_NUMBER.Contains(@0)", FilterParameters = new object[] { search }, Expand = "ForwarderAgent,Provider,ShipmentForwarderAgentMethod,StatusDocumentType" });
        }
        protected override async Task OnInitializedAsync()
        {
            purchaseOrders = await AldebaranDbService.GetPurchaseOrders(new Query { Filter = $@"i => i.ORDER_NUMBER.Contains(@0) || i.IMPORT_NUMBER.Contains(@0) || i.EMBARKATION_PORT.Contains(@0) || i.PROFORMA_NUMBER.Contains(@0)", FilterParameters = new object[] { search }, Expand = "ForwarderAgent.Forwarder,Provider,ShipmentForwarderAgentMethod.ShipmentMethod,StatusDocumentType" });
        }
        protected override async void OnParametersSet()
        {
            if (string.IsNullOrEmpty(OrderNumber))
                return;
            var orderByOrderNumber = await AldebaranDbService.GetPurchaseOrders(new Query { Filter = "@i => i.ORDER_NUMBER == @0", FilterParameters = new object[] { OrderNumber } });
            if (orderByOrderNumber.Any())
                dialogResult = new DialogResult { Success = true, Message = $"Orden de compra {OrderNumber} ha sido creada correctamente." };
        }

        protected async Task AddPurchaseOrder(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-purchase-order");
        }

        protected async Task DeletePurchaseOrder(MouseEventArgs args, Models.AldebaranDb.PurchaseOrder purchaseOrder)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar esta orden de compra?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeletePurchaseOrder(purchaseOrder.PURCHASE_ORDER_ID);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Orden de compra ha sido eliminada correctamente." };
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
                    Detail = $"No se ha podido eliminar la orden de compra"
                });
            }
        }

        protected Models.AldebaranDb.PurchaseOrder purchaseOrder;
        protected async Task GetChildData(Models.AldebaranDb.PurchaseOrder args)
        {
            purchaseOrder = args;
            var PurchaseOrderActivitiesResult = await AldebaranDbService.GetPurchaseOrderActivities(new Query { Filter = $@"i => i.PURCHASE_ORDER_ID == {args.PURCHASE_ORDER_ID}", Expand = "PurchaseOrder,ActivityEmployee.Area,Employee.Area" });
            if (PurchaseOrderActivitiesResult != null)
            {
                args.PurchaseOrderActivities = PurchaseOrderActivitiesResult.ToList();
            }
            var PurchaseOrderDetailsResult = await AldebaranDbService.GetPurchaseOrderDetails(new Query { Filter = $@"i => i.PURCHASE_ORDER_ID == {args.PURCHASE_ORDER_ID}", Expand = "PurchaseOrder,ItemReference.Item.Line,Warehouse" });
            if (PurchaseOrderDetailsResult != null)
            {
                args.PurchaseOrderDetails = PurchaseOrderDetailsResult.ToList();
            }
        }

        protected RadzenDataGrid<Models.AldebaranDb.PurchaseOrderActivity> PurchaseOrderActivitiesDataGrid;

        protected async Task AddPurchaseOrderActivity(MouseEventArgs args, Models.AldebaranDb.PurchaseOrder data)
        {
            var result = await DialogService.OpenAsync<AddPurchaseOrderActivity>("Nueva actividad");
            if (result == null)
                return;
            var activity = (PurchaseOrderActivity)result;
            activity.PURCHASE_ORDER_ID = data.PURCHASE_ORDER_ID;
            activity.CREATION_DATE = DateTime.UtcNow;
            await AldebaranDbService.CreatePurchaseOrderActivity(activity);
            await GetChildData(data);
            await PurchaseOrderActivitiesDataGrid.Reload();
            dialogResult = new DialogResult { Success = true, Message = $"Actividad ha sido creada correctamente." };
        }
        protected async Task EditPurchaseOrderActivity(Models.AldebaranDb.PurchaseOrderActivity args, Models.AldebaranDb.PurchaseOrder data)
        {
            var result = await DialogService.OpenAsync<EditPurchaseOrderActivity>("Actualizar actividad", new Dictionary<string, object> { { "PURCHASE_ORDER_ACTIVITY_ID", args.PURCHASE_ORDER_ACTIVITY_ID } });
            if (result == null)
                return;
            var activity = (PurchaseOrderActivity)result;
            await AldebaranDbService.UpdatePurchaseOrderActivity(activity.PURCHASE_ORDER_ACTIVITY_ID, activity);
            await GetChildData(data);
            await PurchaseOrderActivitiesDataGrid.Reload();
            dialogResult = new DialogResult { Success = true, Message = $"Actividad ha sido actualizada correctamente." };
        }
        protected async Task DeletePurchaseOrderActivity(MouseEventArgs args, Models.AldebaranDb.PurchaseOrderActivity purchaseOrderActivity)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar esta actividad?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeletePurchaseOrderActivity(purchaseOrderActivity.PURCHASE_ORDER_ACTIVITY_ID);
                    await GetChildData(purchaseOrder);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Actividad ha sido eliminada correctamente." };
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
                    Detail = $"No se ha podido eliminar la actividad"
                });
            }
        }

        protected RadzenDataGrid<Models.AldebaranDb.PurchaseOrderDetail> PurchaseOrderDetailsDataGrid;

        protected async Task AddPurchaseOrderDetail(MouseEventArgs args, Models.AldebaranDb.PurchaseOrder data)
        {
            var providerReferences = await AldebaranDbService.GetProviderReferences(new Query { Filter = $"@i => i.PROVIDER_ID == @0", FilterParameters = new object[] { data.PROVIDER_ID } });
            var providerReferencesIds = new List<int>();
            //Solo las referencias del proveedor, excepto las que ya estan agregadas
            foreach (var reference in providerReferences)
            {
                if (!data.PurchaseOrderDetails.Any(a => a.REFERENCE_ID == reference.REFERENCE_ID))
                    providerReferencesIds.Add(reference.REFERENCE_ID);
            }
            var itemReferences = await AldebaranDbService.GetItemReferences(new Query { Filter = "i => @0.Contains(i.REFERENCE_ID)", FilterParameters = new object[] { providerReferencesIds }, Expand = "Item.Line" });
            var result = await DialogService.OpenAsync<AddPurchaseOrderDetail>("Nueva referencia", new Dictionary<string, object> { { "ProviderItemReferences", itemReferences.ToList() } });
            if (result == null)
                return;
            var detail = (PurchaseOrderDetail)result;
            detail.Warehouse = null;
            detail.PURCHASE_ORDER_ID = data.PURCHASE_ORDER_ID;
            await AldebaranDbService.CreatePurchaseOrderDetail(detail);
            await GetChildData(data);
            await PurchaseOrderDetailsDataGrid.Reload();
            dialogResult = new DialogResult { Success = true, Message = $"Referencia agregada correctamente a la orden {data.ORDER_NUMBER}." };
        }
        protected async Task EditPurchaseOrderDetail(Models.AldebaranDb.PurchaseOrderDetail args, Models.AldebaranDb.PurchaseOrder data)
        {
            var providerReferences = await AldebaranDbService.GetProviderReferences(new Query { Filter = $"@i => i.PROVIDER_ID == @0", FilterParameters = new object[] { data.PROVIDER_ID } });
            var providerReferencesIds = providerReferences.Select(s => s.REFERENCE_ID).ToList();
            var itemReferences = await AldebaranDbService.GetItemReferences(new Query { Filter = "i => @0.Contains(i.REFERENCE_ID)", FilterParameters = new object[] { providerReferencesIds }, Expand = "Item.Line" });
            var result = await DialogService.OpenAsync<EditPurchaseOrderDetail>("Modificar detalle de la orden de compra", new Dictionary<string, object> { { "PURCHASE_ORDER_DETAIL_ID", args.PURCHASE_ORDER_DETAIL_ID }, { "ProviderItemReferences", itemReferences.ToList() } });
            if (result == null)
                return;
            var detail = (PurchaseOrderDetail)result;
            await AldebaranDbService.UpdatePurchaseOrderDetail(detail.PURCHASE_ORDER_DETAIL_ID, detail);
            await GetChildData(data);
            await PurchaseOrderDetailsDataGrid.Reload();
            dialogResult = new DialogResult { Success = true, Message = $"Referencia actualizada correctamente." };
        }
        protected async Task DeletePurchaseOrderDetail(MouseEventArgs args, Models.AldebaranDb.PurchaseOrderDetail purchaseOrderDetail)
        {
            try
            {
                var details = await AldebaranDbService.GetPurchaseOrderDetails(new Query { Filter = "i=>i.PURCHASE_ORDER_ID == @0", FilterParameters = new object[] { purchaseOrderDetail.PURCHASE_ORDER_ID } });
                if (details.Count() == 1)
                {
                    dialogResult = new DialogResult { Success = false, Message = "La orden de compra debe contener al menos una referencia." };
                    return;
                }
                if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeletePurchaseOrderDetail(purchaseOrderDetail.PURCHASE_ORDER_DETAIL_ID);
                    await GetChildData(purchaseOrder);

                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Referencia ha sido eliminada correctamente." };
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
                    Detail = $"No se ha podido eliminar la referencia"
                });
            }
        }
    }
}