using Aldebaran.Application.Services;
using Aldebaran.Web.Models.ViewModels;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class PurchaseOrders
    {

        #region Injections
        [Inject]
        protected ILogger<PurchaseOrders> Logger { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected IPurchaseOrderService PurchaseOrderService { get; set; }

        [Inject]
        protected IPurchaseOrderActivityService PurchaseOrderActivityService { get; set; }
        [Inject]
        protected IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int? PURCHASE_ORDER_ID { get; set; } = null;
        #endregion

        #region Variables
        protected ServiceModel.PurchaseOrder PurchaseOrder;
        protected IEnumerable<ServiceModel.PurchaseOrder> PurchaseOrdersList;
        protected RadzenDataGrid<ServiceModel.PurchaseOrder> PurchaseOrderGrid;
        protected string search = "";
        protected bool IsLoadingInProgress;
        protected DialogResult DialogResult { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoadingInProgress = true;
                await GetPurchaseOrdersAsync();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (PURCHASE_ORDER_ID == null)
                return;
            var purchaseOrder = await PurchaseOrderService.FindAsync(PURCHASE_ORDER_ID.Value);
            if (purchaseOrder != null)
                DialogResult = new DialogResult { Success = true, Message = $"Orden de compra {purchaseOrder.OrderNumber} ha sido creada correctamente." };
        }
        #endregion

        #region Events
        #region PurchaseOrder
        async Task GetPurchaseOrdersAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            PurchaseOrdersList = string.IsNullOrEmpty(searchKey) ? await PurchaseOrderService.GetAsync(ct) : await PurchaseOrderService.GetAsync(searchKey, ct);
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await PurchaseOrderGrid.GoToPage(0);
            await GetPurchaseOrdersAsync(search);
        }
        protected async Task AddPurchaseOrder(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-purchase-order");
        }
        protected async Task GetChildData(ServiceModel.PurchaseOrder args)
        {
            PurchaseOrder = args;
            try
            {
                IsLoadingInProgress = true;
                await Task.Yield();
                var details = await PurchaseOrderDetailService.GetByPurchaseOrderIdAsync(args.PurchaseOrderId);
                args.PurchaseOrderDetails = details.ToList();
                var activities = await PurchaseOrderActivityService.GetByPurchaseOrderIdAsync(args.PurchaseOrderId);
                args.PurchaseOrderActivities = activities.ToList();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }

        protected async Task DeletePurchaseOrder(MouseEventArgs args, ServiceModel.PurchaseOrder purchaseOrder)
        {
            //try
            //{
            //    DialogResult = null;
            //    if (await DialogService.Confirm("Está seguro que desea eliminar esta orden de compra?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            //    {
            //        await PurchaseOrderService.DeleteAsync(purchaseOrder.PURCHASE_ORDER_ID);
            //        await GetPurchaseOrdersAsync();
            //        DialogResult = new DialogResult { Success = true, Message = "Orden de compra ha sido eliminada correctamente." };
            //        await PurchaseOrderGrid.Reload();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogError(ex, nameof(DeletePurchaseOrder));
            //    NotificationService.Notify(new NotificationMessage
            //    {
            //        Severity = NotificationSeverity.Error,
            //        Summary = $"Error",
            //        Detail = $"No se ha podido eliminar la orden de compra"
            //    });
            //}
        }
        #endregion

        #region PurchaseOrderActivity
        protected RadzenDataGrid<ServiceModel.PurchaseOrderActivity> PurchaseOrderActivitiesDataGrid;
        protected async Task AddPurchaseOrderActivity(MouseEventArgs args, ServiceModel.PurchaseOrder data)
        {
            //var result = await DialogService.OpenAsync<AddPurchaseOrderActivity>("Nueva actividad");
            //if (result == null)
            //    return;
            //var activity = (PurchaseOrderActivity)result;
            //activity.PURCHASE_ORDER_ID = data.PURCHASE_ORDER_ID;
            //activity.CREATION_DATE = DateTime.UtcNow;
            //await AldebaranDbService.CreatePurchaseOrderActivity(activity);
            //await GetChildData(data);
            //await PurchaseOrderActivitiesDataGrid.Reload();
            //DialogResult = new DialogResult { Success = true, Message = $"Actividad ha sido creada correctamente." };
        }
        protected async Task EditPurchaseOrderActivity(ServiceModel.PurchaseOrderActivity args, ServiceModel.PurchaseOrder data)
        {
            //var result = await DialogService.OpenAsync<EditPurchaseOrderActivity>("Actualizar actividad", new Dictionary<string, object> { { "PURCHASE_ORDER_ACTIVITY_ID", args.PURCHASE_ORDER_ACTIVITY_ID } });
            //if (result == null)
            //    return;
            //var activity = (PurchaseOrderActivity)result;
            //await AldebaranDbService.UpdatePurchaseOrderActivity(activity.PURCHASE_ORDER_ACTIVITY_ID, activity);
            //await GetChildData(data);
            //await PurchaseOrderActivitiesDataGrid.Reload();
            //dialogResult = new DialogResult { Success = true, Message = $"Actividad ha sido actualizada correctamente." };
        }
        protected async Task DeletePurchaseOrderActivity(MouseEventArgs args, ServiceModel.PurchaseOrderActivity purchaseOrderActivity)
        {
            //try
            //{
            //    if (await DialogService.Confirm("Está seguro que desea eliminar esta actividad?") == true)
            //    {
            //        var deleteResult = await AldebaranDbService.DeletePurchaseOrderActivity(purchaseOrderActivity.PURCHASE_ORDER_ACTIVITY_ID);
            //        await GetChildData(PurchaseOrder);
            //        if (deleteResult != null)
            //        {
            //            DialogResult = new DialogResult { Success = true, Message = "Actividad ha sido eliminada correctamente." };
            //            await PurchaseOrderActivitiesDataGrid.Reload();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    NotificationService.Notify(new NotificationMessage
            //    {
            //        Severity = NotificationSeverity.Error,
            //        Summary = $"Error",
            //        Detail = $"No se ha podido eliminar la actividad"
            //    });
            //}
        }
        #endregion

        #region PurchaseOrderDetail
        protected RadzenDataGrid<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetailsDataGrid;
        protected async Task AddPurchaseOrderDetail(MouseEventArgs args, ServiceModel.PurchaseOrder data)
        {
            //var providerReferences = await AldebaranDbService.GetProviderReferences(new Query { Filter = $"@i => i.PROVIDER_ID == @0", FilterParameters = new object[] { data.PROVIDER_ID } });
            //var providerReferencesIds = new List<int>();
            ////Solo las referencias del proveedor, excepto las que ya estan agregadas
            //foreach (var reference in providerReferences)
            //{
            //    if (!data.PurchaseOrderDetails.Any(a => a.REFERENCE_ID == reference.REFERENCE_ID))
            //        providerReferencesIds.Add(reference.REFERENCE_ID);
            //}
            //var itemReferences = await AldebaranDbService.GetItemReferences(new Query { Filter = "i => @0.Contains(i.REFERENCE_ID)", FilterParameters = new object[] { providerReferencesIds }, Expand = "Item.Line" });
            //var result = await DialogService.OpenAsync<AddPurchaseOrderDetail>("Nueva referencia", new Dictionary<string, object> { { "ProviderItemReferences", itemReferences.ToList() } });
            //if (result == null)
            //    return;
            //var detail = (PurchaseOrderDetail)result;
            //detail.Warehouse = null;
            //detail.PURCHASE_ORDER_ID = data.PURCHASE_ORDER_ID;
            //await AldebaranDbService.CreatePurchaseOrderDetail(detail);
            //await GetChildData(data);
            //await PurchaseOrderDetailsDataGrid.Reload();
            //DialogResult = new DialogResult { Success = true, Message = $"Referencia agregada correctamente a la orden {data.ORDER_NUMBER}." };
        }
        protected async Task EditPurchaseOrderDetail(ServiceModel.PurchaseOrderDetail args, ServiceModel.PurchaseOrder data)
        {
            //var providerReferences = await AldebaranDbService.GetProviderReferences(new Query { Filter = $"@i => i.PROVIDER_ID == @0", FilterParameters = new object[] { data.PROVIDER_ID } });
            //var providerReferencesIds = providerReferences.Select(s => s.REFERENCE_ID).ToList();
            //var itemReferences = await AldebaranDbService.GetItemReferences(new Query { Filter = "i => @0.Contains(i.REFERENCE_ID)", FilterParameters = new object[] { providerReferencesIds }, Expand = "Item.Line" });
            //var result = await DialogService.OpenAsync<EditPurchaseOrderDetail>("Modificar detalle de la orden de compra", new Dictionary<string, object> { { "PURCHASE_ORDER_DETAIL_ID", args.PURCHASE_ORDER_DETAIL_ID }, { "ProviderItemReferences", itemReferences.ToList() } });
            //if (result == null)
            //    return;
            //var detail = (PurchaseOrderDetail)result;
            //await AldebaranDbService.UpdatePurchaseOrderDetail(detail.PURCHASE_ORDER_DETAIL_ID, detail);
            //await GetChildData(data);
            //await PurchaseOrderDetailsDataGrid.Reload();
            //dialogResult = new DialogResult { Success = true, Message = $"Referencia actualizada correctamente." };
        }
        protected async Task DeletePurchaseOrderDetail(MouseEventArgs args, ServiceModel.PurchaseOrderDetail purchaseOrderDetail)
        {
            //try
            //{
            //    var details = await AldebaranDbService.GetPurchaseOrderDetails(new Query { Filter = "i=>i.PURCHASE_ORDER_ID == @0", FilterParameters = new object[] { purchaseOrderDetail.PURCHASE_ORDER_ID } });
            //    if (details.Count() == 1)
            //    {
            //        DialogResult = new DialogResult { Success = false, Message = "La orden de compra debe contener al menos una referencia." };
            //        return;
            //    }
            //    if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?") == true)
            //    {
            //        var deleteResult = await AldebaranDbService.DeletePurchaseOrderDetail(purchaseOrderDetail.PURCHASE_ORDER_DETAIL_ID);
            //        await GetChildData(PurchaseOrder);

            //        if (deleteResult != null)
            //        {
            //            DialogResult = new DialogResult { Success = true, Message = "Referencia ha sido eliminada correctamente." };
            //            await PurchaseOrderDetailsDataGrid.Reload();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    NotificationService.Notify(new NotificationMessage
            //    {
            //        Severity = NotificationSeverity.Error,
            //        Summary = $"Error",
            //        Detail = $"No se ha podido eliminar la referencia"
            //    });
            //}
        }
        #endregion
        #endregion
    }
}