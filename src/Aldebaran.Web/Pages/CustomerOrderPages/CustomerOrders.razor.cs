using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
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

        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CUSTOMER_ORDER_ID { get; set; } = null;
        [Parameter]
        public string Action { get; set; } = null;

        #endregion

        #region Global Variables
        protected DialogResult dialogResult;
        protected DocumentType documentType;
        protected IEnumerable<CustomerOrder> customerOrders;
        protected LocalizedDataGrid<CustomerOrder> CustomerOrdersGrid;
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

                await GetCustomerOrdersAsync();
                await DialogResultResolver();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        async Task DialogResultResolver(CancellationToken ct = default)
        {
            if (CUSTOMER_ORDER_ID == null)
                return;

            var valid = int.TryParse(CUSTOMER_ORDER_ID, out var customerOrderId);
            if (!valid)
                return;

            var customerOrder = await CustomerOrderService.FindAsync(customerOrderId, ct);
            if (customerOrder == null)
                return;

            if (Action == "edit")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos",
                    Severity = NotificationSeverity.Success,
                    Detail = $"El pedido {customerOrder.OrderNumber} ha sido actualizado correctamente."
                });
                return;
            }

            if (Action == "create-activity")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos",
                    Severity = NotificationSeverity.Success,
                    Detail = $"La actividad para el pedido {customerOrder.OrderNumber} ha sido creada correctamente."
                });
                return;
            }

            if (Action == "edit-activity")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos",
                    Severity = NotificationSeverity.Success,
                    Detail = $"La actividad para el pedido {customerOrder.OrderNumber} ha sido actualizada correctamente."
                });
                return;
            }

            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Pedido de artículos",
                Severity = NotificationSeverity.Success,
                Detail = $"El pedido ha sido creado correctamente, con el consecutivo {customerOrder.OrderNumber}."
            });
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task GetCustomerOrdersAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            customerOrders = string.IsNullOrEmpty(searchKey) ? await CustomerOrderService.GetAsync(ct) : await CustomerOrderService.GetAsync(searchKey, ct);
        }

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await CustomerOrdersGrid.GoToPage(0);

            await GetCustomerOrdersAsync(search);
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

                if (await DialogService.Confirm("Está seguro que desea eliminar esta actividad?") == true)
                {
                    await CustomerOrderActivityService.DeleteAsync(customerOrderActivity.CustomerOrderActivityId);
                    var customerOrder = await CustomerOrderService.FindAsync(customerOrderActivity.CustomerOrderId);
                    await GetCustomerOrderActivitiesAsync(customerOrder);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Actividad de pedido",
                        Severity = NotificationSeverity.Success,
                        Detail = $"La Actividad del pedido ha sido eliminada correctamente."
                    });
                    await CustomerOrderActivitiesDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar la actividad.\n {ex.Message}"
                });
            }
        }

        protected async Task CancelCustomerOrder(MouseEventArgs arg, CustomerOrder customerOrder)
        {
            try
            {
                dialogResult = null;
                var reasonResult = await DialogService.OpenAsync<CancellationReasonDialog>("Confirmar cancelación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "P" }, { "TITLE", "Está seguro que desea cancelar este pedido?" } });
                if (reasonResult == null)
                    return;
                var reason = (Reason)reasonResult;

                var cancelStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 6);
                await CustomerOrderService.CancelAsync(customerOrder.CustomerOrderId, cancelStatusDocumentType.StatusDocumentTypeId, reason);
                await GetCustomerOrdersAsync();
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos",
                    Severity = NotificationSeverity.Success,
                    Detail = $"El pedido ha sido cancelado correctamente."
                });
                await CustomerOrdersGrid.Reload();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cancelar el pedido.\n {ex.Message}"
                });
            }
        }

        protected async Task GetOrderDetails(CustomerOrder args)
        {
            var CustomerOrderDetailsResult = await CustomerOrderDetailService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
            if (CustomerOrderDetailsResult != null)
            {
                args.CustomerOrderDetails = CustomerOrderDetailsResult.ToList();
            }
        }

        protected async Task GetCustomerOrderActivitiesAsync(CustomerOrder args)
        {
            var CustomerOrderActivitiesResult = await CustomerOrderActivityService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
            if (CustomerOrderActivitiesResult != null)
            {
                args.CustomerOrderActivities = CustomerOrderActivitiesResult.ToList();
            }
        }

        protected async Task GetChildActivityData(CustomerOrderActivity args)
        {
            customerOrderActivity = args;

            var CustomerOrderActivityDetailsResult = await CustomerOrderActivityDetailService.GetByCustomerOrderActivityIdAsync(args.CustomerOrderActivityId);
            if (CustomerOrderActivityDetailsResult != null)
            {
                args.CustomerOrderActivityDetails = CustomerOrderActivityDetailsResult.ToList();
            }
        }

        protected async Task GetChildData(CustomerOrder args)
        {
            customerOrder = args;

            await GetOrderDetails(args);

            await GetCustomerOrderActivitiesAsync(args);
        }

        protected async Task<bool> CanEdit(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Admin", "Customer Order Editor") && customerOrder.StatusDocumentType.EditMode;
        }

        protected async Task<bool> CanCloseCustomerOrder(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Admin", "Customer Order Editor") && (customerOrder.StatusDocumentType.StatusOrder == 2 || customerOrder.StatusDocumentType.StatusOrder == 3);
        }

        protected async Task<bool> CanCancel(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Admin", "Customer Order Editor") && (customerOrder.StatusDocumentType.StatusOrder == 1);
        }

        protected async Task CloseCustomerOrder(CustomerOrder args)
        {
            try
            {
                dialogResult = null;
                var reasonResult = await DialogService.OpenAsync<CancellationReasonDialog>("Confirmar cancelación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "P" }, { "TITLE", "Está seguro que desea cerrar este pedido?" } });
                if (reasonResult == null)
                    return;
                var reason = (Reason)reasonResult;

                var cancelStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 6);
                await CustomerOrderService.CancelAsync(args.CustomerOrderId, cancelStatusDocumentType.StatusDocumentTypeId, reason);
                await GetCustomerOrdersAsync();
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Pedido de artículos",
                    Detail = $"El pedido No. {args.OrderNumber} ha sido cerrado correctamente."
                });
                await CustomerOrdersGrid.Reload();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cerrar el pedido.\n {ex.Message}"
                });
            }
        }

        protected async Task<bool> CanEditActivities(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Admin", "Customer Order Activities Editor") && customerOrder.StatusDocumentType.EditMode;
        }

        #endregion

    }
}