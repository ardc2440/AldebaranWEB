using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderShipmentPages
{
    public partial class CustomerOrdersShipment
    {
        #region Injections

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected ICustomerOrderShipmentService CustomerOrderShipmentService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }

        [Inject]
        protected ICustomerOrderShipmentDetailService CustomerOrderShipmentDetailService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        #endregion

        #region Global Variables

        protected DialogResult dialogResult;
        protected DocumentType documentType;
        protected IEnumerable<CustomerOrder> customerOrders;
        protected IEnumerable<CustomerOrderShipment> customerOrderShipments;
        protected IEnumerable<CustomerOrderShipmentDetail> customerOrderShipmentDetails;
        protected IEnumerable<DetailInProcess> detailInProcesses;
        protected LocalizedDataGrid<DetailInProcess> CustomerOrderDetailsDataGrid;
        protected LocalizedDataGrid<CustomerOrderShipment> CustomerOrderShipmentDataGrid;
        protected LocalizedDataGrid<CustomerOrderShipmentDetail> CustomerOrderShipmentDetailDataGrid;
        protected LocalizedDataGrid<CustomerOrder> CustomerOrdersGrid;
        protected string search = "";
        protected bool isLoadingInProgress;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await DocumentTypeService.FindByCodeAsync("P");

                isLoadingInProgress = true;

                await Task.Yield();

                customerOrders = (await CustomerOrderService.GetAsync(search)).Where(x => x.StatusDocumentType.StatusOrder == 2 || x.StatusDocumentType.StatusOrder == 3).ToList();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        #endregion

        #region Events

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await CustomerOrdersGrid.GoToPage(0);

            customerOrders = (await CustomerOrderService.GetAsync(search)).Where(x => x.StatusDocumentType.StatusOrder == 2 || x.StatusDocumentType.StatusOrder == 3).ToList();
        }

        protected async Task GetOrderDetails(CustomerOrder args)
        {
            var customerOrderDetailsResult = await CustomerOrderDetailService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
            if (customerOrderDetailsResult == null)
                return;

            var detailInProcesses = new List<DetailInProcess>();

            foreach (var item in customerOrderDetailsResult)
            {
                var viewOrderDetail = new DetailInProcess()
                {
                    REFERENCE_ID = item.ReferenceId,
                    CUSTOMER_ORDER_DETAIL_ID = item.CustomerOrderDetailId,
                    REFERENCE_DESCRIPTION = $"({item.ItemReference.Item.InternalReference}) {item.ItemReference.Item.ItemName} - {item.ItemReference.ReferenceName}",
                    PENDING_QUANTITY = item.RequestedQuantity - item.ProcessedQuantity - item.DeliveredQuantity,
                    PROCESSED_QUANTITY = item.ProcessedQuantity,
                    DELIVERED_QUANTITY = item.DeliveredQuantity
                };
                detailInProcesses.Add(viewOrderDetail);
            }

            this.detailInProcesses = detailInProcesses;
        }

        protected async Task GetChildData(CustomerOrder args)
        {
            await GetOrderDetails(args);
            customerOrderShipments = await CustomerOrderShipmentService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
        }

        protected async Task GetChildShipmentData(CustomerOrderShipment args)
        {
            customerOrderShipmentDetails = await CustomerOrderShipmentDetailService.GetByCustomerOrderShipmentIdAsync(args.CustomerOrderShipmentId);
        }

        protected async Task SendToShipment(CustomerOrder args)
        {
            NavigationManager.NavigateTo("add-customer-order-shipment/" + args.CustomerOrderId);
        }

        protected async Task EditProcessRow(CustomerOrderShipment args)
        {
            NavigationManager.NavigateTo("edit-customer-order-shipment/" + args.CustomerOrderShipmentId);
        }

        protected async Task<bool> CanEditProcess(CustomerOrderShipment customerOrderShipment)
        {
            return Security.IsInRole("Admin", "Customer Order Shipment Editor") && customerOrderShipment.StatusDocumentType.EditMode;
        }

        protected async Task CancelCustomerOrderShipment(MouseEventArgs args, CustomerOrderShipment customerOrderShipment)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea cancelar este Traslado a Proceso?") == true)
                {
                    customerOrderShipment.StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync((await DocumentTypeService.FindByCodeAsync("D")).DocumentTypeId, 2);
                    customerOrderShipment.StatusDocumentTypeId = customerOrderShipment.StatusDocumentType.StatusDocumentTypeId;

                    var customerOrderShipmentDetail = await CustomerOrderShipmentDetailService.GetByCustomerOrderShipmentIdAsync(customerOrderShipment.CustomerOrderShipmentId);
                    customerOrderShipment.CustomerOrderShipmentDetails = customerOrderShipmentDetail.ToList();

                    await CustomerOrderShipmentService.UpdateAsync(customerOrderShipment.CustomerOrderShipmentId, customerOrderShipment);

                    await DialogService.Alert($"Despacho de Pedido cancelado correctamente", "Información");

                    await Search(new ChangeEventArgs() { });
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cancelar el despacho. \n\r {ex.InnerException.Message}\n\r{ex.StackTrace}"
                });
            }
        }

        public static implicit operator CustomerOrdersShipment(CustomerOrderShipment v)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}