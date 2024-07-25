using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class EditCustomerOrder
    {
        #region Injections

        [Inject]
        protected ILogger<AddCustomerOrder> Logger { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public string CustomerOrderId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables
        protected bool IsErrorVisible;
        protected string Error;
        protected CustomerOrder customerOrder;
        protected DocumentType documentType;
        protected ICollection<CustomerOrderDetail> customerOrderDetails;
        protected LocalizedDataGrid<CustomerOrderDetail> customerOrderDetailGrid;
        protected IEnumerable<Customer> customersForCUSTOMERID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string title;
        protected bool Submitted = false;

        protected int count = 0;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(CustomerOrderId, out var customerOrderId))
                    throw new Exception("El Id de pedido recibido no es valido");

                var (customers, _count) = await CustomerService.GetAsync(0, 5);
                customersForCUSTOMERID = customers.ToList();
                count = _count;

                documentType = await DocumentTypeService.FindByCodeAsync("P");

                customerOrder = await CustomerOrderService.FindAsync(customerOrderId);

                var customerOrderDetails = await CustomerOrderDetailService.GetByCustomerOrderIdAsync(customerOrder.CustomerOrderId);
                this.customerOrderDetails = customerOrderDetails.ToList();

                title = $"Actualizar el pedido No. {customerOrder.OrderNumber}";
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
        protected async Task LoadData(LoadDataArgs args)
        {
            await Task.Yield();
            var (customers, _count) = string.IsNullOrEmpty(args.Filter) ? await CustomerService.GetAsync(args.Skip.Value, args.Top.Value) : await CustomerService.GetAsync(args.Skip.Value, args.Top.Value, args.Filter);
            customersForCUSTOMERID = customers.ToList();
            count = _count;
        }

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task FormSubmit()
        {
            try
            {
                Submitted = true;
                IsSubmitInProgress = true;
                if (!customerOrderDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                var reasonResult = await DialogService.OpenAsync<ModificationReasonDialog>("Confirmar modificación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "P" }, { "TITLE", "Está seguro que desea actualizar este pedido?" } });
                if (reasonResult == null)
                    return;
                var reason = (Reason)reasonResult;
                customerOrder.CustomerOrderDetails = customerOrderDetails;
                await CustomerOrderService.UpdateAsync(customerOrder.CustomerOrderId, customerOrder, reason);

                var result = await DialogService.OpenAsync<CustomerOrderSummary>(null, new Dictionary<string, object> { { "Id", customerOrder.CustomerOrderId }, { "NotificationTemplateName", "Customer:Order:Update" } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
                NavigationManager.NavigateTo($"customer-orders/edit/{customerOrder.CustomerOrderId}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
            finally { IsSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que desea cancelar la modificación del pedido?", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-orders");
        }

        protected async Task EditRow(CustomerOrderDetail args)
        {
            var result = await DialogService.OpenAsync<EditCustomerOrderDetail>("Actualizar referencia", new Dictionary<string, object> { { "CustomerOrderDetail", args } });

            if (result == null)
                return;

            args.RequestedQuantity = result.RequestedQuantity;
            args.Brand = result.Brand;

            await customerOrderDetailGrid.Reload();
        }
                
        #endregion
    }
}