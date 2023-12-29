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

                customerOrders = (await CustomerOrderService.GetAsync(search)).Where(x => x.StatusDocumentType.EditMode).ToList();
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

            customerOrders = (await CustomerOrderService.GetAsync(search)).Where(x => x.StatusDocumentType.EditMode).ToList();
        }

        protected async Task GetOrderDetails(CustomerOrder args)
        {
            var customerOrderDetailsResult = await CustomerOrderDetailService.GetAsync(args.CustomerOrderId);
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
            customerOrderInProcesses = await CustomerOrdersInProcessService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
        }

        protected async Task GetChildInProcessData(CustomerOrdersInProcess args)
        {
            customerOrderInProcessDetails = await CustomerOrderInProcessDetailService.GetAsync(args.CustomerOrderInProcessId);
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

                if (await DialogService.Confirm("Esta seguro que desea cancelar este Traslado a Proceso?") == true)
                {
                    customerOrderInProcess.StatusDocumentTypeId = (await StatusDocumentTypeService.FindByDocumentAndOrderAsync((await DocumentTypeService.FindByCodeAsync("T")).DocumentTypeId, 2)).StatusDocumentTypeId;

                    await CustomerOrdersInProcessService.UpdateAsync(customerOrderInProcess.CustomerOrderInProcessId, customerOrderInProcess);

                    dialogResult = new DialogResult { Success = true, Message = "Traslado a Proceso cancelado correctamente." };
                    await CustomerOrderInProcessesDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cancelar el pedido. \n\r {ex.InnerException.Message}\n\r{ex.StackTrace}"
                });
            }
        }

        #endregion
    }
}