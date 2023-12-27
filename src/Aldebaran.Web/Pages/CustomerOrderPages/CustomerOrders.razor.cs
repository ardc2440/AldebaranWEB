using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class CustomerOrders
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
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerOrderActivityService CustomerOrderActivityService { get; set; }

        [Inject]
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }

        [Inject]
        protected ICustomerOrderActivityDetailService CustomerOrderActivityDetailService { get; set; }

        #endregion

        #region Global Variables
        protected DialogResult dialogResult;
        protected DocumentType documentType;
        protected IEnumerable<CustomerOrder> customerOrders;
        protected LocalizedDataGrid<CustomerOrder> grid0;
        protected CustomerOrder customerOrder;
        protected CustomerOrderActivity customerOrderActivity;
        protected LocalizedDataGrid<CustomerOrderDetail> CustomerOrderDetailsDataGrid;
        protected LocalizedDataGrid<CustomerOrderActivity> CustomerOrderActivitiesDataGrid;
        protected LocalizedDataGrid<CustomerOrderActivityDetail> CustomerOrderActivityDetailsDataGrid;
        protected string search = "";
        protected bool isLoadingInProgress;
        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await DocumentTypeService.FindByCodeAsync("P");

                isLoadingInProgress = true;

                await Task.Yield();

                customerOrders = await CustomerOrderService.GetAsync(search);
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

            await grid0.GoToPage(0);

            customerOrders = await CustomerOrderService.GetAsync(search);

        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-customer-order");
        }

        protected async Task EditRow(CustomerOrder args)
        {
            NavigationManager.NavigateTo("edit-customer-order/" + args.CustomerOrderId);
        }

        protected async Task AddActivityButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-customer-order-activity/" + customerOrder.CustomerOrderId);
        }

        protected async Task EditActivityRow(CustomerOrderActivity args)
        {
            NavigationManager.NavigateTo("edit-customer-order-activity/" + args.CustomerOrderActivityId);
        }

        protected async Task DeleteActivity(MouseEventArgs arg, CustomerOrderActivity customerOrderActivity)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea eliminar esta actividad??") == true)
                {

                    await CustomerOrderActivityService.DeleteAsync(customerOrderActivity.CustomerOrderActivityId);

                    dialogResult = new DialogResult { Success = true, Message = "Actividad eliminada correctamente." };
                    await CustomerOrderActivitiesDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar la actividad.\n\r{ex.InnerException.Message}\n\r{ex.StackTrace}"
                });
            }
        }

        protected async Task CancelCustomerOrder(MouseEventArgs arg, CustomerOrder customerOrder)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea cancelar este pedido?") == true)
                {
                    var cancelStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 6);

                    await CustomerOrderService.CancelAsync(customerOrder.CustomerOrderId, cancelStatusDocumentType.StatusDocumentTypeId);

                    dialogResult = new DialogResult { Success = true, Message = "Pedido cancelado correctamente." };
                    await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cancelar el pedido.\n\r{ex.InnerException.Message}\n\r{ex.StackTrace}"
                });
            }
        }

        protected async Task GetOrderDetails(CustomerOrder args)
        {
            var CustomerOrderDetailsResult = await CustomerOrderDetailService.GetAsync(args.CustomerOrderId);
            if (CustomerOrderDetailsResult != null)
            {
                args.CustomerOrderDetails = CustomerOrderDetailsResult.ToList();
            }
        }

        protected async Task GetOrderActivities(CustomerOrder args)
        {
            var CustomerOrderActivitiesResult = await CustomerOrderActivityService.GetAsync(args.CustomerOrderId);
            if (CustomerOrderActivitiesResult != null)
            {
                args.CustomerOrderActivities = CustomerOrderActivitiesResult.ToList();
            }
        }

        protected async Task GetChildActivityData(CustomerOrderActivity args)
        {
            customerOrderActivity = args;

            var CustomerOrderActivityDetailsResult = await CustomerOrderActivityDetailService.GetAsync(args.CustomerOrderActivityId);
            if (CustomerOrderActivityDetailsResult != null)
            {
                args.CustomerOrderActivityDetails = CustomerOrderActivityDetailsResult.ToList();
            }
        }

        protected async Task GetChildData(CustomerOrder args)
        {
            customerOrder = args;

            await GetOrderDetails(args);

            await GetOrderActivities(args);
        }

        protected async Task<bool> CanEdit(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Admin", "Customer Order Editor") && customerOrder.StatusDocumentType.EditMode;
        }

        protected async Task<bool> CanCloseCustomerOrder(CustomerOrder customerOrder)
        {
            var documentStatus = StatusDocumentTypeService.GetAsync(documentType.DocumentTypeId).Result.Where(i => i.StatusOrder.Equals(2) || i.StatusOrder.Equals(3));

            return Security.IsInRole("Admin", "Customer Order Editor") && documentStatus.Any(i => i.StatusDocumentTypeId.Equals(customerOrder.StatusDocumentTypeId));
        }

        protected async Task CloseCustomerOrder(CustomerOrder args)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea cerrar este pedido?") == true)
                {
                    var cancelStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 6);

                    await CustomerOrderService.CancelAsync(args.CustomerOrderId, cancelStatusDocumentType.StatusDocumentTypeId);

                    dialogResult = new DialogResult { Success = true, Message = "Pedido cerrado correctamente." };
                    await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cerrar el pedido.\n\r{ex.InnerException.Message}\n\r{ex.StackTrace}"
                });
            }
        }
        #endregion

    }
}