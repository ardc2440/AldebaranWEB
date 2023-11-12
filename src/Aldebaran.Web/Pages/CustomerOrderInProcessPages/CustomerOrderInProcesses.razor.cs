using Aldebaran.Web.Models;
using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderInProcessPages
{
    public partial class CustomerOrderInProcesses
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected DialogResult dialogResult { get; set; }

        protected DocumentType documentType { get; set; }

        protected IEnumerable<Models.AldebaranDb.CustomerOrder> customerOrders;
        protected IEnumerable<Models.AldebaranDb.CustomerOrderInProcess> customerOrderInProcesses;
        protected IEnumerable<Models.AldebaranDb.CustomerOrderInProcessDetail> customerOrderInProcessDetails;
        protected IEnumerable<Models.ViewModels.DetailInProcess> detailInProcesses;

        protected RadzenDataGrid<Models.ViewModels.DetailInProcess> CustomerOrderDetailsDataGrid;
        protected RadzenDataGrid<Models.AldebaranDb.CustomerOrderInProcess> CustomerOrderInProcessesDataGrid;
        protected RadzenDataGrid<Models.AldebaranDb.CustomerOrderInProcessDetail> CustomerOrderInProcessDetailDataGrid;
        protected RadzenDataGrid<Models.AldebaranDb.CustomerOrder> CustomerOrdersGrid;

        protected string search = "";
        protected bool isLoadingInProgress;

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await CustomerOrdersGrid.GoToPage(0);

            customerOrders = await AldebaranDbService.GetCustomerOrders(new Query { Filter = $@"i => (i.StatusDocumentType.EDIT_MODE.Equals(1) && i.StatusDocumentType.DOCUMENT_TYPE_ID.Equals(@1) && i.StatusDocumentType.STATUS_DOCUMENT_TYPE_NAME.Contains(@0)) || i.Customer.CUSTOMER_NAME.Contains(@0) || i.ORDER_NUMBER.Contains(@0) || i.INTERNAL_NOTES.Contains(@0) || i.CUSTOMER_NOTES.Contains(@0)", FilterParameters = new object[] { search, documentType.DOCUMENT_TYPE_ID }, Expand = "Customer,StatusDocumentType,Employee" });

        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await AldebaranDbService.GetDocumentTypeByCode("P");

                isLoadingInProgress = true;

                await Task.Yield();

                customerOrders = await AldebaranDbService.GetCustomerOrders(new Query { Filter = $@"i => (i.StatusDocumentType.EDIT_MODE.Equals(1) && i.StatusDocumentType.DOCUMENT_TYPE_ID.Equals(@1) && i.StatusDocumentType.STATUS_DOCUMENT_TYPE_NAME.Contains(@0)) || i.Customer.CUSTOMER_NAME.Contains(@0) || i.ORDER_NUMBER.Contains(@0) || i.INTERNAL_NOTES.Contains(@0) || i.CUSTOMER_NOTES.Contains(@0)", FilterParameters = new object[] { search, documentType.DOCUMENT_TYPE_ID }, Expand = "Customer,StatusDocumentType,Employee" });
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        protected async Task GetOrderDetails(Models.AldebaranDb.CustomerOrder args)
        {
            var customerOrderDetailsResult = await AldebaranDbService.GetCustomerOrderDetails(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID == {args.CUSTOMER_ORDER_ID}", Expand = "CustomerOrder,ItemReference,ItemReference.Item" });
            if (customerOrderDetailsResult == null)
                return;

            var detailInProcesses = new List<DetailInProcess>();

            foreach (var item in customerOrderDetailsResult)
            {
                var viewOrderDetail = new DetailInProcess()
                {
                    REFERENCE_ID = item.REFERENCE_ID,
                    CUSTOMER_ORDER_DETAIL_ID = item.CUSTOMER_ORDER_DETAIL_ID,
                    REFERENCE_DESCRIPTION = $"({item.ItemReference.Item.INTERNAL_REFERENCE}) {item.ItemReference.Item.ITEM_NAME} - {item.ItemReference.REFERENCE_NAME}",
                    PENDING_QUANTITY = item.REQUESTED_QUANTITY - item.PROCESSED_QUANTITY - item.DELIVERED_QUANTITY,
                    PROCESSED_QUANTITY = item.PROCESSED_QUANTITY,
                    DELIVERED_QUANTITY = item.DELIVERED_QUANTITY
                };
                detailInProcesses.Add(viewOrderDetail);
            }

            this.detailInProcesses = detailInProcesses;
        }

        protected async Task GetChildData(Models.AldebaranDb.CustomerOrder args)
        {
            await GetOrderDetails(args);
            await GetCustomerOrderInProcesses(args);
        }

        protected async Task GetCustomerOrderInProcesses(CustomerOrder args)
        {
            var customerOrderInProcesesResult = await AldebaranDbService.GetCustomerOrderInProcesses(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID == {args.CUSTOMER_ORDER_ID}" });
            if (customerOrderInProcesesResult == null)
                return;

            customerOrderInProcesses = customerOrderInProcesesResult;
        }

        protected async Task GetChildInProcessData(Models.AldebaranDb.CustomerOrderInProcess args)
        {
            var customerOrderInProcesDetailsResult = await AldebaranDbService.GetCustomerOrderInProcessDetails(new Query { Filter = $@"i => i.CUSTOMER_ORDER_IN_PROCESS_ID == {args.CUSTOMER_ORDER_ID}", Expand = "CustomerOrderDetail.ItemReference.Item" });
            if (customerOrderInProcesDetailsResult == null)
                return;

            customerOrderInProcessDetails = customerOrderInProcesDetailsResult;
        }

        protected async Task SendToProcess(Models.AldebaranDb.CustomerOrder args)
        {
            NavigationManager.NavigateTo("add-customer-order-in-process/" + args.CUSTOMER_ORDER_ID);
        }

        protected async Task EditProcessRow(Models.AldebaranDb.CustomerOrderInProcess args)
        {
            NavigationManager.NavigateTo("edit-customer-order-in-process/" + args.CUSTOMER_ORDER_IN_PROCESS_ID);
        }

        protected async Task<bool> CanEditProcess(CustomerOrderInProcess customerOrderinProcess)
        {
            return Security.IsInRole("Admin", "Customer Order In Process Editor") && customerOrderinProcess.StatusDocumentType.EDIT_MODE;
        }

        protected async Task CancelCustomerOrderProcess(MouseEventArgs args, Models.AldebaranDb.CustomerOrderInProcess customerOrder)
        {
            try
            {
                dialogResult = null;

                var inProcessDocumentType = await AldebaranDbService.GetDocumentTypeByCode("T");

                if (await DialogService.Confirm("Esta seguro que desea cancelar este Traslado a Proceso?") == true)
                {
                    var cancelStatusDocumentType = await AldebaranDbService.GetStatusDocumentTypeByDocumentAndOrder(inProcessDocumentType, 2);

                    var cancelResult = await AldebaranDbService.UpdateCustomerOrderInProgressStatus(customerOrder, cancelStatusDocumentType.STATUS_DOCUMENT_TYPE_ID);

                    if (cancelResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Traslado a Proceso cancelado correctamente." };
                        await CustomerOrderInProcessesDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cancelar el pedido"
                });
            }
        }
    }
}