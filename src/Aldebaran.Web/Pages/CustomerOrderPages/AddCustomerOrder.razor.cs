using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class AddCustomerOrder
    {
        #region Injections

        [Inject]
        protected ILogger<AddCustomerOrder> Logger { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

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
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Global Variables

        protected CustomerOrder customerOrder;
        protected DocumentType documentType;
        protected ICollection<CustomerOrderDetail> customerOrderDetails;
        protected LocalizedDataGrid<CustomerOrderDetail> customerOrderDetailGrid;
        protected IEnumerable<Customer> customersForCUSTOMERID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected string Error;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {

                IsErrorVisible = false;

                await Task.Yield();

                customersForCUSTOMERID = await CustomerService.GetAsync();

                documentType = await DocumentTypeService.FindByCodeAsync("P");

                customerOrderDetails = new List<CustomerOrderDetail>();

                customerOrder = new CustomerOrder()
                {
                    CustomerOrderId = 0,
                    Employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id),
                    StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1),
                    OrderDate = DateTime.Today
                };
                customerOrder.EmployeeId = customerOrder.Employee.EmployeeId;
                customerOrder.StatusDocumentTypeId = customerOrder.StatusDocumentType.StatusDocumentTypeId;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
        }
        #endregion

        #region Events

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
                if (await DialogService.Confirm("Está seguro que desea crear este pedido de artículos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar creación") == true)
                {
                    customerOrder.CustomerOrderDetails = customerOrderDetails;
                    customerOrder = await CustomerOrderService.AddAsync(customerOrder);

                    var result = await DialogService.OpenAsync<CustomerOrderSummary>(null, new Dictionary<string, object> { { "Id", customerOrder.CustomerOrderId }, { "NotificationTemplateName", "Customer:PurchaseOrder:New" } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
                    NavigationManager.NavigateTo($"customer-orders/{customerOrder.CustomerOrderId}");
                }
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
            if (await DialogService.Confirm("Está seguro que desea cancelar la creación del pedido?", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-orders");
        }

        protected async Task AddCustomerOrderDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddCustomerOrderDetail>("Agregar referencia", new Dictionary<string, object> { { "CustomerOrderDetails", customerOrderDetails } });

            if (result == null)
                return;

            var detail = (CustomerOrderDetail)result;

            customerOrderDetails.Add(detail);

            await customerOrderDetailGrid.Reload();
        }

        protected async Task DeleteCustomerOrderDetailButtonClick(MouseEventArgs arg, CustomerOrderDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                customerOrderDetails.Remove(item);

                await customerOrderDetailGrid.Reload();
            }
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