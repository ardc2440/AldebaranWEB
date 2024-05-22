using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class AddPurchaseOrder
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

        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.PurchaseOrder PurchaseOrder;
        protected IEnumerable<ServiceModel.Provider> Providers;
        protected IEnumerable<ServiceModel.ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods;
        protected RadzenDropDownDataGrid<int> ProviderDropDownDataGrid;

        protected ICollection<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetails = new List<ServiceModel.PurchaseOrderDetail>();
        protected RadzenDataGrid<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetailGrid;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        protected int lastReferenceId = 0;
        protected short lastWarehouseId = 0;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                PurchaseOrder = new ServiceModel.PurchaseOrder { RequestDate = DateTime.Now };
                Providers = await ProviderService.GetAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        #region PurchaseOrder
        private int PROVIDER_ID { get; set; }
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                Submitted = true;
                if (!PurchaseOrderDetails.Any())
                    return;
                var now = DateTime.UtcNow;
                // Complementar la orden compra
                var employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
                var documentType = await DocumentTypeService.FindByCodeAsync("O");
                var statusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
                PurchaseOrder.CreationDate = now;
                PurchaseOrder.EmployeeId = employee.EmployeeId;
                PurchaseOrder.StatusDocumentTypeId = statusDocumentType.StatusDocumentTypeId;
                PurchaseOrder.PurchaseOrderDetails = PurchaseOrderDetails.Select(s => new ServiceModel.PurchaseOrderDetail
                {
                    ReferenceId = s.ReferenceId,
                    WarehouseId = s.WarehouseId,
                    RequestedQuantity = s.RequestedQuantity,
                }).ToList();

                var result = await PurchaseOrderService.AddAsync(PurchaseOrder);
                NavigationManager.NavigateTo($"purchase-orders/{result.PurchaseOrderId}");
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
            var itemReferences = providerReferences.Where(w => w.ItemReference.IsActive && w.ItemReference.Item.IsActive && w.ItemReference.Item.Line.IsActive).Select(s => s.ItemReference).ToList();
            var result = await DialogService.OpenAsync<AddPurchaseOrderDetail>("Nueva referencia",
                new Dictionary<string, object> {
                    { "ProviderItemReferences", itemReferences.ToList() },
                    { "PurchaseOrderDetails", PurchaseOrderDetails.ToList() },                     
                    { "LastReferenceId", lastReferenceId },
                    { "LastWarehouseId", lastWarehouseId}
                });
            if (result == null)
                return;
            var detail = (ServiceModel.PurchaseOrderDetail)result;
            PurchaseOrderDetails.Add(detail);
            lastReferenceId = detail.ReferenceId;
            lastWarehouseId = detail.WarehouseId;
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
        protected async Task EditPurchaseOrderDetail(MouseEventArgs args, ServiceModel.PurchaseOrderDetail item)
        {
            var providerReferences = await ProviderReferenceService.GetByProviderIdAsync(PurchaseOrder.ProviderId);
            var itemReferences = providerReferences.Select(s => s.ItemReference).ToList();
            var result = await DialogService.OpenAsync<EditPurchaseOrderDetail>("Actualizar referencia",
                new Dictionary<string, object> {
                    { "PurchaseOrderDetail", item },
                    { "PurchaseOrderDetails", PurchaseOrderDetails.ToList() }
                });
            if (result == null)
                return;
            await PurchaseOrderDetailGrid.Reload();
        }
        #endregion
        #endregion
    }
}