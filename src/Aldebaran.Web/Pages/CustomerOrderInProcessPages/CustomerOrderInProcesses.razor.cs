using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using DocumentFormat.OpenXml.Drawing.Charts;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderInProcessPages
{
    public partial class CustomerOrderInProcesses
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
        protected ICustomerOrdersInProcessService CustomerOrdersInProcessService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }

        [Inject]
        protected ICustomerOrderInProcessDetailService CustomerOrderInProcessDetailService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CUSTOMER_ORDER_IN_PROCESS_ID { get; set; } = null;
        [Parameter]
        public string Action { get; set; } = null;

        #endregion

        #region Global Variables

        protected DialogResult dialogResult;
        protected DocumentType documentType;
        protected IEnumerable<CustomerOrder> customerOrders;
        protected IEnumerable<CustomerOrdersInProcess> customerOrderInProcesses;
        protected IEnumerable<CustomerOrderInProcessDetail> customerOrderInProcessDetails;
        protected IEnumerable<DetailInProcess> detailInProcesses;
        protected LocalizedDataGrid<DetailInProcess> CustomerOrderDetailsDataGrid;
        protected LocalizedDataGrid<CustomerOrdersInProcess> CustomerOrderInProcessesDataGrid;
        protected LocalizedDataGrid<CustomerOrderInProcessDetail> CustomerOrderInProcessDetailDataGrid;
        protected LocalizedDataGrid<CustomerOrder> CustomerOrdersGrid;
        protected string search = "";
        protected bool isLoadingInProgress;
        protected bool IsErrorVisible = false;
        protected string Error = "";

        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await DocumentTypeService.FindByCodeAsync("P");

                isLoadingInProgress = true;

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
            await GetCustomerOrderInProcessAsync(search);
        }

        async Task GetCustomerOrderInProcessAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            (customerOrders, count) = string.IsNullOrEmpty(searchKey) ? await CustomerOrderService.GetAsync(skip, top, 0, ct) : await CustomerOrderService.GetAsync(skip, top, searchKey, 0, ct);            
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task DialogResultResolver(CancellationToken ct = default)
        {
            IsErrorVisible = false;

            if (CUSTOMER_ORDER_IN_PROCESS_ID == null)
                return;

            var valid = int.TryParse(CUSTOMER_ORDER_IN_PROCESS_ID, out var customerOrderInProcessId);
            if (!valid)
                return;

            var customerOrderInProcess = await CustomerOrdersInProcessService.FindAsync(customerOrderInProcessId, ct);
            if (customerOrderInProcess == null)
                return;

            if (Action == "edit")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Traslado a Proceso",
                    Severity = NotificationSeverity.Success,
                    Detail = $"El traslado a proceso ha sido actualizado correctamente."
                });
                return;
            }

            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Traslado a proceso",
                Severity = NotificationSeverity.Success,
                Detail = $"El traslado a proceso ha sido creado correctamente."
            });
        }

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await CustomerOrdersGrid.GoToPage(0);

            await GetCustomerOrderInProcessAsync(search);            
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
                    REFERENCE_DESCRIPTION = $"[{item.ItemReference.Item.InternalReference}] ({item.ItemReference.Item.Line.LineName}) {item.ItemReference.Item.ItemName} - {item.ItemReference.ReferenceName}",
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
            customerOrderInProcesses = await CustomerOrdersInProcessService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
        }

        protected async Task GetChildInProcessData(CustomerOrdersInProcess args)
        {
            customerOrderInProcessDetails = await CustomerOrderInProcessDetailService.GetByCustomerOrderInProcessIdAsync(args.CustomerOrderInProcessId);
        }

        protected async Task SendToProcess(CustomerOrder args)
        {
            NavigationManager.NavigateTo("add-customer-order-in-process/" + args.CustomerOrderId);
        }

        protected async Task EditProcessRow(CustomerOrdersInProcess args)
        {
            NavigationManager.NavigateTo("edit-customer-order-in-process/" + args.CustomerOrderInProcessId);
        }

        protected async Task<bool> CanEditProcess(CustomerOrdersInProcess customerOrderinProcess)
        {
            return Security.IsInRole("Administrador", "Modificación de pedidos en proceso") && customerOrderinProcess.StatusDocumentType.EditMode;
        }

        protected async Task CancelCustomerOrderProcess(MouseEventArgs args, CustomerOrdersInProcess customerOrderInProcess)
        {
            try
            {
                IsErrorVisible = false;

                dialogResult = null;

                var customerOrderInProcessDetail = await CustomerOrderInProcessDetailService.GetByCustomerOrderInProcessIdAsync(customerOrderInProcess.CustomerOrderInProcessId);

                foreach (var item in customerOrderInProcessDetail)
                {
                    var detail = detailInProcesses.FirstOrDefault(i => i.CUSTOMER_ORDER_DETAIL_ID == item.CustomerOrderDetailId);

                    if (detail.PROCESSED_QUANTITY < item.ProcessedQuantity)
                    {
                        throw new Exception("Este traslado posee artículos despachados al cliente. \n\r" +
                                "Para poder cancelarlo, antes debe cancelar todos los despachos realizados para el pedido.");
                    }
                }

                var reasonResult = await DialogService.OpenAsync<CancellationReasonDialog>("Confirmar cancelación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "T" }, { "TITLE", "Está seguro que desea cancelar este traslado a proceso?" } });
                if (reasonResult == null)
                    return;

                var reason = (Reason)reasonResult;

                var statusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync((await DocumentTypeService.FindByCodeAsync("T")).DocumentTypeId, 2);                
                await CustomerOrdersInProcessService.CancelAsync(customerOrderInProcess.CustomerOrderInProcessId, statusDocumentType.StatusDocumentTypeId, reason);
                await GetCustomerOrderInProcessAsync(search);

                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Traslado a proceso",
                    Severity = NotificationSeverity.Success,
                    Detail = $"El traslado a proceso ha sido cancelado correctamente."
                });

                await CustomerOrderInProcessesDataGrid.Reload();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
        }

        #endregion
    }
}