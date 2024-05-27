using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderShipmentPages
{
    public partial class EditCustomerOrderShipment
    {
        #region Injections

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected ICustomerOrderShipmentService CustomerOrderShipmentService { get; set; }

        [Inject]
        protected ICustomerOrderShipmentDetailService CustomerOrderShipmentDetailService { get; set; }

        [Inject]
        protected IShippingMethodService ShippingMethodService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        #endregion

        #region Params

        [Parameter]
        public string CustomerOrderShipmentId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

        protected CustomerOrder customerOrder;
        protected CustomerOrderShipment customerOrderShipment;
        protected DocumentType documentType;
        protected ICollection<DetailInProcess> detailsInProcess;
        protected LocalizedDataGrid<DetailInProcess> customerOrderDetailGrid;
        protected IEnumerable<Employee> employeesFOREMPLOYEEID;
        protected IEnumerable<ShippingMethod> shippingMethodsFORSHIPPINGMETHODID;
        protected bool isLoadingInProgress;
        protected string title;
        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected string Error;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(CustomerOrderShipmentId, out var customerOrderShipmentId))
                    throw new Exception("El Id de despacho recibido no es valido");

                documentType = await DocumentTypeService.FindByCodeAsync("T");

                customerOrderShipment = await CustomerOrderShipmentService.FindAsync(customerOrderShipmentId);

                customerOrder = await CustomerOrderService.FindAsync(customerOrderShipment.CustomerOrderId);

                detailsInProcess = await GetDetailsInProcess(customerOrderShipment);

                shippingMethodsFORSHIPPINGMETHODID = await ShippingMethodService.GetAsync();
                employeesFOREMPLOYEEID = await EmployeeService.GetAsync();

                title = $"Modificación del despacho para el Pedido No. {customerOrder.OrderNumber}";
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task<List<DetailInProcess>> GetDetailsInProcess(CustomerOrderShipment customerOrderShipment)
        {
            return (from item in await CustomerOrderShipmentDetailService.GetByCustomerOrderShipmentIdAsync(customerOrderShipment.CustomerOrderShipmentId) ?? throw new ArgumentException("The references of Customer Order Shipment, could not be obtained.")
                    let viewOrderDetail = new DetailInProcess()
                    {
                        REFERENCE_ID = item.CustomerOrderDetail.ReferenceId,
                        CUSTOMER_ORDER_DETAIL_ID = item.CustomerOrderDetailId,
                        REFERENCE_DESCRIPTION = $"[{item.CustomerOrderDetail.ItemReference.Item.InternalReference}] ({item.CustomerOrderDetail.ItemReference.Item.Line.LineName}) {item.CustomerOrderDetail.ItemReference.Item.ItemName} - {item.CustomerOrderDetail.ItemReference.ReferenceName}",
                        PENDING_QUANTITY = item.CustomerOrderDetail.RequestedQuantity - item.CustomerOrderDetail.ProcessedQuantity - item.CustomerOrderDetail.DeliveredQuantity,
                        PROCESSED_QUANTITY = item.CustomerOrderDetail.ProcessedQuantity,
                        DELIVERED_QUANTITY = item.CustomerOrderDetail.DeliveredQuantity,
                        THIS_QUANTITY = item.DeliveredQuantity,
                        ItemReference = item.CustomerOrderDetail.ItemReference,
                        CustomerOrderInProcessDetailId = item.CustomerOrderShipmentDetailId
                    }
                    select viewOrderDetail).ToList();
        }

        protected async Task SendToShipment(DetailInProcess args)
        {
            if (await DialogService.OpenAsync<SetQuantityShipment>("Cantidad a Despachar", new Dictionary<string, object> { { "DetailInProcess", args } }) != null)
                await customerOrderDetailGrid.Reload();
        }

        protected async Task CancelToShipment(DetailInProcess args)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") != true)
            {
                return;
            }

            args.PROCESSED_QUANTITY += args.THIS_QUANTITY;
            args.DELIVERED_QUANTITY -= args.THIS_QUANTITY;

            args.THIS_QUANTITY = 0;

            await customerOrderDetailGrid.Reload();
        }

        protected bool CanCancel(DetailInProcess detailInProcess) => detailInProcess.THIS_QUANTITY > 0 && Security.IsInRole("Administrador", "Modificación de despachos");

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;

                if (!detailsInProcess.Any(x => x.THIS_QUANTITY > 0))
                    throw new Exception("No ha ingresado ninguna cantidad a despachar");

                var reasonResult = await DialogService.OpenAsync<ModificationReasonDialog>("Confirmar modificación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "D" }, { "TITLE", "Está seguro que desea actualizar este despacho?" } });
                if (reasonResult == null)
                    return;
                var reason = (Reason)reasonResult;

                customerOrderShipment.CustomerOrderShipmentDetails = await MapDetailsInProcess(detailsInProcess);
                await CustomerOrderShipmentService.UpdateAsync(customerOrderShipment.CustomerOrderShipmentId, customerOrderShipment, reason);
                NavigationManager.NavigateTo($"shipment-customer-orders/edit/{customerOrderShipment.CustomerOrderShipmentId}");
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally { IsSubmitInProgress = false; }
        }

        protected async Task<ICollection<CustomerOrderShipmentDetail>> MapDetailsInProcess(ICollection<DetailInProcess> detailsInProcess)
        {
            return (from item in detailsInProcess.Where(i => i.THIS_QUANTITY > 0)
                    let orderInProcessDetail = new CustomerOrderShipmentDetail()
                    {
                        CustomerOrderDetailId = item.CUSTOMER_ORDER_DETAIL_ID,
                        DeliveredQuantity = item.THIS_QUANTITY,
                        CustomerOrderShipmentId = customerOrderShipment.CustomerOrderShipmentId,
                        CustomerOrderShipmentDetailId = item.CustomerOrderInProcessDetailId
                    }
                    select orderInProcessDetail).ToList();
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que desea cancelar la modificación del despacho??", "Confirmar") == true)
                NavigationManager.NavigateTo("shipment-customer-orders");
        }

        #endregion
    }
}