using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Security.Cryptography;

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
        protected TooltipService TooltipService { get; set; }

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

        #region Parameters

        [Parameter]
        public string CUSTOMER_ORDER_SHIPMENT_ID { get; set; } = null;
        [Parameter]
        public string Action { get; set; } = null;

        #endregion

        #region Global Variables

        protected DialogResult dialogResult;
        protected DocumentType documentType;
        protected IEnumerable<StatusDocumentType> dispachStatus;
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

        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                documentType = await DocumentTypeService.FindByCodeAsync("P");

                dispachStatus = (await StatusDocumentTypeService.GetByDocumentTypeIdAsync(documentType.DocumentTypeId)).Where(w=>w.StatusOrder == 2 || w.StatusOrder == 3);
                
                await Task.Yield();
                                
                await DialogResultResolver();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events
        
        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip.Value;
            top = args.Top.Value;
            await GetCustomerOrderShipmentAsync(search);
        }

        async Task DialogResultResolver(CancellationToken ct = default)
        {
            if (CUSTOMER_ORDER_SHIPMENT_ID == null)
                return;

            var valid = int.TryParse(CUSTOMER_ORDER_SHIPMENT_ID, out var customerOrderShipmentId);
            if (!valid)
                return;

            var customerOrderShipment = await CustomerOrderShipmentService.FindAsync(customerOrderShipmentId, ct);
            if (customerOrderShipment == null)
                return;

            if (Action == "edit")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Despacho de artículos",
                    Severity = NotificationSeverity.Success,
                    Detail = $"El despacho ha sido actualizado correctamente."
                });
                return;
            }

            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Despacho de artículos",
                Severity = NotificationSeverity.Success,
                Detail = $"El despacho ha sido creado correctamente."
            });
        }

        async Task GetCustomerOrderShipmentAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            (customerOrders, count) = string.IsNullOrEmpty(searchKey) ? await CustomerOrderService.GetAsync(skip, top, 1, ct) : await CustomerOrderService.GetAsync(skip, top, searchKey, 1, ct);            
        }

        async Task<bool> CanDispach(CustomerOrder customerOrder,CancellationToken ct = default)
        {
            var havePendingProcessQuantity = customerOrder.CustomerOrderDetails.Any(a => a.ProcessedQuantity > 0);
            return havePendingProcessQuantity && dispachStatus.Any(a=>a.StatusDocumentTypeId == customerOrder.StatusDocumentTypeId) && Security.IsInRole("Administrador", "Modificación de despachos");
        }
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"[{reference.Item.InternalReference}] {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await CustomerOrdersGrid.GoToPage(0);

            await GetCustomerOrderShipmentAsync(search);
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
                    REFERENCE_DESCRIPTION = $"[{item.ItemReference.Item.InternalReference}] {item.ItemReference.Item.ItemName} - {item.ItemReference.ReferenceName}",
                    PENDING_QUANTITY = item.RequestedQuantity - item.ProcessedQuantity - item.DeliveredQuantity,
                    PROCESSED_QUANTITY = item.ProcessedQuantity,
                    DELIVERED_QUANTITY = item.DeliveredQuantity,
                    ItemReference = item.ItemReference
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
            return Security.IsInRole("Administrador", "Modificación de despachos") && customerOrderShipment.StatusDocumentType.EditMode;
        }

        protected async Task CancelCustomerOrderShipment(MouseEventArgs args, CustomerOrderShipment customerOrderShipment)
        {
            try
            {
                dialogResult = null;

                var reasonResult = await DialogService.OpenAsync<CancellationReasonDialog>("Confirmar cancelación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "D" }, { "TITLE", "Está seguro que desea cancelar este traslado a proceso?" } });
                if (reasonResult == null)
                    return;

                var reason = (Reason)reasonResult;

                var statusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync((await DocumentTypeService.FindByCodeAsync("D")).DocumentTypeId, 2);

                await CustomerOrderShipmentService.CancelAsync(customerOrderShipment.CustomerOrderShipmentId, statusDocumentType.StatusDocumentTypeId, reason);
                await GetCustomerOrderShipmentAsync(search);

                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Traslado a proceso",
                    Severity = NotificationSeverity.Success,
                    Detail = $"El despacho ha sido cancelado correctamente."
                });

                await CustomerOrderShipmentDataGrid.Reload();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cancelar el despacho. \n {ex.Message}"
                });
            }
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion
    }
}