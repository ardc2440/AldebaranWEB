using Aldebaran.Application.Services;
using Aldebaran.Web.Models.AldebaranDb;
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

        protected ICollection<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetails = new List<ServiceModel.PurchaseOrderDetail>();
        protected RadzenDataGrid<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetailGrid;

        protected ICollection<ServiceModel.PurchaseOrderActivity> PurchaseOrderActivities = new List<ServiceModel.PurchaseOrderActivity>();
        protected RadzenDataGrid<ServiceModel.PurchaseOrderActivity> PurchaseOrderActivityGrid;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected string Error;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            PurchaseOrder = new ServiceModel.PurchaseOrder { RequestDate = DateTime.Now };
            Providers = await ProviderService.GetAsync();
        }
        #endregion

        #region Events
        #region PurchaseOrderDetail
        protected async Task AddPurchaseOrderDetail(MouseEventArgs args)
        {
            if (PROVIDER_ID == null)
                return;
            var providerReferences = await ProviderReferenceService.GetByProviderIdAsync(PROVIDER_ID.Value);
            var itemReferences = providerReferences.Select(s => s.ItemReference).ToList();
            var result = await DialogService.OpenAsync<AddPurchaseOrderDetail>("Nueva referencia", new Dictionary<string, object> { { "ProviderItemReferences", itemReferences.ToList() } });
            if (result == null)
                return;
            var detail = (ServiceModel.PurchaseOrderDetail)result;
            // Un detalle de orden de compra es unico por referencia y bodega
            if (PurchaseOrderDetails.Any(a => a.ReferenceId == detail.ReferenceId && a.WarehouseId == detail.WarehouseId))
            {
                IsErrorVisible = true;
                Error = "Ya existe una referencia para la misma bodega adicionada a esta orden de compra";
                return;
            }
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
        #endregion

        #region PurchaseOrderActivity
        protected async Task AddPurchaseOrderActivity(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddPurchaseOrderActivity>("Nueva actividad");
            if (result == null)
                return;
            var detail = (ServiceModel.PurchaseOrderActivity)result;
            PurchaseOrderActivities.Add(detail);
            await PurchaseOrderActivityGrid.Reload();
        }
        protected async Task DeletePurchaseOrderActivity(MouseEventArgs args, ServiceModel.PurchaseOrderActivity item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta actividad?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                PurchaseOrderActivities.Remove(item);
                await PurchaseOrderActivityGrid.Reload();
            }
        }
        #endregion

        #region PurchaseOrder
        private int? PROVIDER_ID { get; set; }
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
                PurchaseOrder.PurchaseOrderActivities = PurchaseOrderActivities.Select(s => new ServiceModel.PurchaseOrderActivity
                {
                    ExecutionDate = s.ExecutionDate,
                    ActivityDescription = s.ActivityDescription,
                    CreationDate = now,
                    EmployeeId = s.EmployeeId,
                    ActivityEmployeeId = s.ActivityEmployeeId
                }).ToList();
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
            PROVIDER_ID = (int)providerId == 0 ? null : (int)providerId;
        }
        #endregion
        #endregion
    }
}