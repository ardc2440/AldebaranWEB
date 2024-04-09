using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class ConfirmPurchaseOrder
    {
        #region Injections
        [Inject]
        protected ILogger<AddPurchaseOrder> Logger { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IProviderService ProviderService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected IPurchaseOrderService PurchaseOrderService { get; set; }

        [Inject]
        protected IProviderReferenceService ProviderReferenceService { get; set; }

        [Inject]
        protected IShipmentForwarderAgentMethodService ShipmentForwarderAgentMethodService { get; set; }

        [Inject]
        protected IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public string PURCHASE_ORDER_ID { get; set; } = null;
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.PurchaseOrder PurchaseOrder;
        protected IEnumerable<ServiceModel.Provider> Providers;
        protected IEnumerable<ServiceModel.ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods;
        protected RadzenDropDownDataGrid<int> ProviderDropDownDataGrid;

        protected List<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetails = new List<ServiceModel.PurchaseOrderDetail>();
        protected RadzenDataGrid<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetailGrid;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                if (PURCHASE_ORDER_ID == null)
                    NavigationManager.NavigateTo("purchase-orders");
                var valid = int.TryParse(PURCHASE_ORDER_ID, out var purchaseOrderId);
                if (!valid)
                    NavigationManager.NavigateTo("purchase-orders");

                PurchaseOrder = await PurchaseOrderService.FindAsync(purchaseOrderId);
                if (PurchaseOrder == null)
                    NavigationManager.NavigateTo("purchase-orders");
                PurchaseOrder.EmbarkationPort = string.IsNullOrWhiteSpace(PurchaseOrder.EmbarkationPort) ? null : PurchaseOrder.EmbarkationPort;
                PurchaseOrder.ProformaNumber = string.IsNullOrWhiteSpace(PurchaseOrder.ProformaNumber) ? null : PurchaseOrder.ProformaNumber;
                await GetDataAsync(purchaseOrderId);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);
        protected async Task GetDataAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            await Task.Yield();
            var orderDetails = await PurchaseOrderDetailService.GetByPurchaseOrderIdAsync(purchaseOrderId);
            PurchaseOrderDetails = orderDetails.ToList();
            PurchaseOrderDetails.ForEach(f => f.ReceivedQuantity = f.ReceivedQuantity == 0 ? null : f.ReceivedQuantity);
            Providers = await ProviderService.GetAsync();
        }
        #region PurchaseOrder
        private int PROVIDER_ID { get; set; }
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                Submitted = true;
                if (PurchaseOrderDetails.Any(a => a.ReceivedQuantity == null) || detailToUpdate != null)
                    return;
                if (await DialogService.Confirm("Desea confirmar la orden de compra?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar orden de compra") == true)
                {
                    var now = DateTime.UtcNow;
                    // Complementar la orden compra
                    PurchaseOrder.PurchaseOrderDetails = PurchaseOrderDetails.Select(s => new ServiceModel.PurchaseOrderDetail
                    {
                        PurchaseOrderDetailId = s.PurchaseOrderDetailId,
                        ReceivedQuantity = s.ReceivedQuantity,
                    }).ToList();

                    await PurchaseOrderService.ConfirmAsync(PurchaseOrder.PurchaseOrderId, PurchaseOrder);
                    NavigationManager.NavigateTo($"purchase-orders/confirm/{PurchaseOrder.PurchaseOrderId}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected async Task AgentForwarderHandler(ServiceModel.ForwarderAgent agent)
        {
            PurchaseOrder.ForwarderAgentId = agent?.ForwarderAgentId ?? 0;
            if (PurchaseOrder.ForwarderAgentId == 0)
            {
                ShipmentForwarderAgentMethods = new List<ServiceModel.ShipmentForwarderAgentMethod>();
                return;
            }
            ShipmentForwarderAgentMethods = await ShipmentForwarderAgentMethodService.GetByForwarderAgentIdAsync(PurchaseOrder.ForwarderAgentId.Value);
        }
        protected async Task CancelPurchaseOrder(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("purchase-orders");
        }
        protected async Task ProviderSelectionChange(object providerId)
        {
            int id = (int)providerId;
            if (!PurchaseOrderDetails.Any())
            {
                PROVIDER_ID = id;
                return;
            }

            if (await DialogService.Confirm("Al realizar cambio de proveedor, las referencias agregadas serán borradas, esta seguro que desea cambiar de proveedor?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                PurchaseOrderDetails = new List<ServiceModel.PurchaseOrderDetail>();
                PROVIDER_ID = id;
                return;
            }
            PurchaseOrder.ProviderId = PROVIDER_ID;
            var p = Providers.First(p => p.ProviderId == PROVIDER_ID);
            await ProviderDropDownDataGrid.DataGrid.SelectRow(p, false);
        }
        #endregion

        #region PurchaseOrderDetail

        protected async Task<string> GetReferenceHint(ServiceModel.ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task AddPurchaseOrderDetail(MouseEventArgs args)
        {
            if (PurchaseOrder.ProviderId == 0)
                return;
            var providerReferences = await ProviderReferenceService.GetByProviderIdAsync(PurchaseOrder.ProviderId);
            var itemReferences = providerReferences.Select(s => s.ItemReference).ToList();
            var result = await DialogService.OpenAsync<AddPurchaseOrderDetail>("Nueva referencia",
                new Dictionary<string, object> {
                    { "ProviderItemReferences", itemReferences.ToList() },
                    { "PurchaseOrderDetails", PurchaseOrderDetails.ToList() }
                });
            if (result == null)
                return;
            var detail = (ServiceModel.PurchaseOrderDetail)result;
            PurchaseOrderDetails.Add(detail);
            await PurchaseOrderDetailGrid.Reload();
        }
        protected async Task DeletePurchaseOrderDetail(MouseEventArgs args, ServiceModel.PurchaseOrderDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                PurchaseOrderDetails.Remove(item);
                await PurchaseOrderDetailGrid.Reload();
            }
        }
        ServiceModel.PurchaseOrderDetail detailToUpdate;
        protected async Task EditReceivedQuantity(ServiceModel.PurchaseOrderDetail item)
        {
            detailToUpdate = item;
            await PurchaseOrderDetailGrid.EditRow(detailToUpdate);
        }
        protected async Task SaveReceivedQuantity(ServiceModel.PurchaseOrderDetail item)
        {
            if (item.ReceivedQuantity <= item.RequestedQuantity)
            {
                await PurchaseOrderDetailGrid.UpdateRow(item);
                Reset();
                return;
            }

            if (await DialogService.Confirm("La cantidad recibida supera la cantidad solicitada, esta seguro de esta cantidad?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar cantidad") == true)
            {
                await PurchaseOrderDetailGrid.UpdateRow(item);
                Reset();
                return;
            }
            return;
        }
        protected async Task CancelEditReceivedQuantity(ServiceModel.PurchaseOrderDetail item)
        {
            Reset();
            await GetDataAsync(item.PurchaseOrderId);
            PurchaseOrderDetailGrid.CancelEditRow(item);
        }
        void Reset()
        {
            detailToUpdate = null;
        }

        #endregion
        #endregion
    }
}