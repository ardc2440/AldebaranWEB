using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderShipmentPages
{
    public partial class AddCustomerOrderShipment
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
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }

        [Inject]
        protected IShippingMethodService ShippingMethodService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected ICustomerOrderShipmentService CustomerOrderShipmentService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CustomerOrderId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

        protected CustomerOrder customerOrder;
        protected CustomerOrderShipment customerOrderShipment;
        protected DocumentType documentType;
        protected DialogResult dialogResult;
        protected ICollection<DetailInProcess> detailsInProcess;
        protected LocalizedDataGrid<DetailInProcess> customerOrderDetailGrid;
        protected bool isLoadingInProgress;
        protected string title;
        protected IEnumerable<Employee> employeesFOREMPLOYEEID;
        protected IEnumerable<ShippingMethod> shippingMethodsFORSHIPPINGMETHODID;
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
                if (!int.TryParse(CustomerOrderId, out var customerOrderId))
                    throw new Exception("El Id de pedido recibido no es valido");

                isLoadingInProgress = true;

                await Task.Yield();

                documentType = await DocumentTypeService.FindByCodeAsync("D");

                var customerOrderShipmentStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);

                customerOrder = await CustomerOrderService.FindAsync(customerOrderId);

                customerOrder.CustomerOrderDetails = (await CustomerOrderDetailService.GetByCustomerOrderIdAsync(customerOrderId)).ToList();

                detailsInProcess = await GetDetailsInProcess(customerOrder);

                shippingMethodsFORSHIPPINGMETHODID = await ShippingMethodService.GetAsync();
                employeesFOREMPLOYEEID = await EmployeeService.GetAsync();

                customerOrderShipment = new CustomerOrderShipment()
                {
                    CreationDate = DateTime.Now,
                    ShippingDate = DateTime.Now,
                    CustomerOrderId = customerOrder.CustomerOrderId,
                    StatusDocumentTypeId = customerOrderShipmentStatusDocumentType.StatusDocumentTypeId
                };

                title = $"Despacho de artículos para el pedido No. {customerOrder.OrderNumber}";
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

        protected async Task<List<DetailInProcess>> GetDetailsInProcess(CustomerOrder customerOrder)
        {
            return (from item in customerOrder.CustomerOrderDetails.Where(i => i.ProcessedQuantity > 0) ?? throw new ArgumentException($"The references of Customer Order {customerOrder.OrderNumber}, could not be obtained.")
                    let viewOrderDetail = new DetailInProcess()
                    {
                        REFERENCE_ID = item.ReferenceId,
                        CUSTOMER_ORDER_DETAIL_ID = item.CustomerOrderDetailId,
                        REFERENCE_DESCRIPTION = $"[{item.ItemReference.Item.InternalReference}] {item.ItemReference.Item.ItemName} - {item.ItemReference.ReferenceName}",
                        PENDING_QUANTITY = item.RequestedQuantity - item.ProcessedQuantity - item.DeliveredQuantity,
                        PROCESSED_QUANTITY = item.ProcessedQuantity,
                        DELIVERED_QUANTITY = item.DeliveredQuantity,
                        BRAND = item.Brand,
                        THIS_QUANTITY = 0,
                        ItemReference = item.ItemReference
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

        protected bool CanSend(DetailInProcess detailInProcess) => detailInProcess.PROCESSED_QUANTITY > 0 && Security.IsInRole("Administrador", "Modificación de despachos");

        protected bool CanCancel(DetailInProcess detailInProcess) => detailInProcess.THIS_QUANTITY > 0 && Security.IsInRole("Administrador", "Modificación de despachos");

        protected async Task FormSubmit()
        {
            try
            {
                dialogResult = null;

                IsSubmitInProgress = true;

                if (!detailsInProcess.Any(x => x.THIS_QUANTITY > 0))
                    throw new Exception("No ha ingresado ninguna cantidad a trasladar");

                customerOrderShipment.CustomerOrderShipmentDetails = await MapDetailsInProcess(detailsInProcess);

                await GetNewCustomerOrderStatus();

                customerOrderShipment.CustomerOrder = customerOrder;

                var result = await CustomerOrderShipmentService.AddAsync(customerOrderShipment);

                NavigationManager.NavigateTo($"Shipment-customer-orders/{result.CustomerOrderShipmentId}");
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally { IsSubmitInProgress = false; }
        }

        protected async Task GetNewCustomerOrderStatus(CancellationToken ct=default) 
        {
            var orderDetail = await CustomerOrderDetailService.GetByCustomerOrderIdAsync(customerOrder.CustomerOrderId);

            var totalRequestedQuantity = orderDetail.Sum(s => s.RequestedQuantity);
            var totalDeliveryQuantity = orderDetail.Sum(s => s.DeliveredQuantity) + customerOrderShipment.CustomerOrderShipmentDetails.Sum(s => s.DeliveredQuantity);

            var documentType = await DocumentTypeService.FindByCodeAsync("P", ct);

            var newStatus = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, (totalDeliveryQuantity == totalRequestedQuantity ? 4 : 3), ct);
            
            customerOrder.StatusDocumentTypeId = newStatus.StatusDocumentTypeId;
        }

        protected async Task<ICollection<CustomerOrderShipmentDetail>> MapDetailsInProcess(ICollection<DetailInProcess> detailsInProcess)
        {
            var customerOrderShipmentDetails = new List<CustomerOrderShipmentDetail>();
            foreach (var details in detailsInProcess.Where(i => i.THIS_QUANTITY > 0))
            {
                customerOrderShipmentDetails.Add(new CustomerOrderShipmentDetail()
                {
                    CustomerOrderDetailId = details.CUSTOMER_ORDER_DETAIL_ID,
                    DeliveredQuantity = details.THIS_QUANTITY
                });
            }

            return customerOrderShipmentDetails;
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que cancelar la creación del despacho??", "Confirmar") == true)
                NavigationManager.NavigateTo("shipment-customer-orders");
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion
    }
}