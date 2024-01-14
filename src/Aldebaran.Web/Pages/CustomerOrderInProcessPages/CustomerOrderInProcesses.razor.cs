using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
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

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await DocumentTypeService.FindByCodeAsync("P");

                isLoadingInProgress = true;

                await Task.Yield();

                await GetCustomerOrderInProcessAsync();
                await DialogResultResolver();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        #endregion

        #region Events

        async Task GetCustomerOrderInProcessAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            var orders = string.IsNullOrEmpty(searchKey) ? await CustomerOrderService.GetAsync(ct) : await CustomerOrderService.GetAsync(searchKey, ct);
            customerOrders = orders.Where(x => x.StatusDocumentType.EditMode);
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task DialogResultResolver(CancellationToken ct = default)
        {
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

            customerOrders = (await CustomerOrderService.GetAsync(search)).Where(x => x.StatusDocumentType.EditMode).ToList();
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
                    REFERENCE_DESCRIPTION = $"({item.ItemReference.Item.Line.LineName}) {item.ItemReference.Item.ItemName} - {item.ItemReference.ReferenceName}",
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
            return Security.IsInRole("Admin", "Customer Order In Process Editor") && customerOrderinProcess.StatusDocumentType.EditMode;
        }

        protected async Task CancelCustomerOrderProcess(MouseEventArgs args, CustomerOrdersInProcess customerOrderInProcess)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Est� seguro que desea cancelar este traslado a proceso?") == true)
                {
                    customerOrderInProcess.StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync((await DocumentTypeService.FindByCodeAsync("T")).DocumentTypeId, 2);
                    customerOrderInProcess.StatusDocumentTypeId = customerOrderInProcess.StatusDocumentType.StatusDocumentTypeId;

                    var customerOrderInProcessDetail = await CustomerOrderInProcessDetailService.GetByCustomerOrderInProcessIdAsync(customerOrderInProcess.CustomerOrderInProcessId);
                    customerOrderInProcess.CustomerOrderInProcessDetails = customerOrderInProcessDetail.ToList();

                    await CustomerOrdersInProcessService.UpdateAsync(customerOrderInProcess.CustomerOrderInProcessId, customerOrderInProcess);
                    await GetCustomerOrderInProcessAsync(search);

                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Traslado a proceso",
                        Severity = NotificationSeverity.Success,
                        Detail = $"El traslado a proceso ha sido cancelado correctamente."
                    });

                    await CustomerOrderInProcessesDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cancelar el traslado. \n\r {ex.InnerException.Message}"
                });
            }
        }

        #endregion
    }
}