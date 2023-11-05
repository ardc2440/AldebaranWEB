using Aldebaran.Web.Models;
using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class CustomerOrders
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
        protected RadzenDataGrid<Models.AldebaranDb.CustomerOrder> grid0;
        protected Models.AldebaranDb.CustomerOrder customerOrder;
        protected Models.AldebaranDb.CustomerOrderActivity customerOrderActivity;

        protected RadzenDataGrid<Models.AldebaranDb.CustomerOrderDetail> CustomerOrderDetailsDataGrid;
        protected RadzenDataGrid<Models.AldebaranDb.CustomerOrderActivity> CustomerOrderActivitiesDataGrid;
        protected RadzenDataGrid<Models.AldebaranDb.CustomerOrderActivityDetail> CustomerOrderActivityDetailsDataGrid;
        protected string search = "";
        protected bool isLoadingInProgress;

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await grid0.GoToPage(0);

            customerOrders = await AldebaranDbService.GetCustomerOrders(new Query { Filter = $@"i => (i.StatusDocumentType.DOCUMENT_TYPE_ID.Equals(@1) && i.StatusDocumentType.STATUS_DOCUMENT_TYPE_NAME.Contains(@0)) || i.Customer.CUSTOMER_NAME.Contains(@0) || i.ORDER_NUMBER.Contains(@0) || i.INTERNAL_NOTES.Contains(@0) || i.CUSTOMER_NOTES.Contains(@0)", FilterParameters = new object[] { search, documentType.DOCUMENT_TYPE_ID }, Expand = "Customer,StatusDocumentType,Employee" });

        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await AldebaranDbService.GetDocumentTypeByCode("P");

                isLoadingInProgress = true;

                await Task.Yield();

                customerOrders = await AldebaranDbService.GetCustomerOrders(new Query { Filter = $@"i => (i.StatusDocumentType.DOCUMENT_TYPE_ID.Equals(@1) && i.StatusDocumentType.STATUS_DOCUMENT_TYPE_NAME.Contains(@0)) || i.Customer.CUSTOMER_NAME.Contains(@0) || i.ORDER_NUMBER.Contains(@0) || i.INTERNAL_NOTES.Contains(@0) || i.CUSTOMER_NOTES.Contains(@0)", FilterParameters = new object[] { search, documentType.DOCUMENT_TYPE_ID }, Expand = "Customer,StatusDocumentType,Employee" });
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-customer-order");
        }

        protected async Task EditRow(Models.AldebaranDb.CustomerOrder args)
        {
            NavigationManager.NavigateTo("edit-customer-order/" + args.CUSTOMER_ORDER_ID);
        }

        protected async Task GridCancelButtonClick(MouseEventArgs args, Models.AldebaranDb.CustomerOrder customerOrder)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea cancelar este pedido?") == true)
                {
                    var cancelStatusDocumentType = await AldebaranDbService.GetStatusDocumentTypeByDocumentAndOrder(documentType, 6);

                    var cancelResult = await AldebaranDbService.UpdateCustomerOrderStatus(customerOrder, cancelStatusDocumentType.STATUS_DOCUMENT_TYPE_ID);

                    if (cancelResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Pedido cancelado correctamente." };
                        await grid0.Reload();
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

        protected async Task GetOrderDetails(Models.AldebaranDb.CustomerOrder args)
        {
            var CustomerOrderDetailsResult = await AldebaranDbService.GetCustomerOrderDetails(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID == {args.CUSTOMER_ORDER_ID}", Expand = "CustomerOrder,ItemReference,ItemReference.Item" });
            if (CustomerOrderDetailsResult != null)
            {
                args.CustomerOrderDetails = CustomerOrderDetailsResult.ToList();
            }
        }

        protected async Task GetOrderActivities(Models.AldebaranDb.CustomerOrder args)
        {
            var CustomerOrderActivitiesResult = await AldebaranDbService.GetCustomerOrderActivities(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID == {args.CUSTOMER_ORDER_ID}", Expand = "CustomerOrder,Area,Employee" });
            if (CustomerOrderActivitiesResult != null)
            {
                args.CustomerOrderActivities = CustomerOrderActivitiesResult.ToList();
            }
        }

        protected async Task GetChildActivityData(Models.AldebaranDb.CustomerOrderActivity args)
        {
            customerOrderActivity = args;

            var CustomerOrderActivityDetailsResult = await AldebaranDbService.GetCustomerOrderActivityDetails(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ACTIVITY_ID == {args.CUSTOMER_ORDER_ACTIVITY_ID}", Expand = "CustomerOrderActivity,ActivityType,Employee,EmployeeActivity" });
            if (CustomerOrderActivityDetailsResult != null)
            {
                args.CustomerOrderActivityDetails = CustomerOrderActivityDetailsResult.ToList();
            }
        }

        protected async Task GetChildData(Models.AldebaranDb.CustomerOrder args)
        {
            customerOrder = args;

            await GetOrderDetails(args);

            await GetOrderActivities(args);
        }

        protected async Task<bool> CanEdit(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Admin", "Customer Order Editor") && customerOrder.StatusDocumentType.EDIT_MODE;
        }
        protected async Task<bool> CanCloseCustomerOrder(CustomerOrder customerOrder)
        {
            var documentStatus = await AldebaranDbService.GetStatusDocumentTypes(new Query { Filter = $@"i => i.STATUS_ORDER.Equals({2})|| i.STATUS_ORDER.Equals({3})" });

            return Security.IsInRole("Admin", "Customer Order Editor") && documentStatus.Any(i => i.STATUS_DOCUMENT_TYPE_ID.Equals(customerOrder.STATUS_DOCUMENT_TYPE_ID));
        }

        protected async Task CloseCustomerOrder(Models.AldebaranDb.CustomerOrder args)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea cerrar este pedido?") == true)
                {
                    var cancelStatusDocumentType = await AldebaranDbService.GetStatusDocumentTypeByDocumentAndOrder(documentType, 6);

                    var cancelResult = await AldebaranDbService.UpdateCustomerOrderStatus(customerOrder, cancelStatusDocumentType.STATUS_DOCUMENT_TYPE_ID);

                    if (cancelResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Pedido cerrado correctamente." };
                        await grid0.Reload();
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

        protected async Task AddActivityButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-customer-order-activity");
        }
    }
}