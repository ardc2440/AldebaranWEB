using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
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
        protected bool IsLoadingInProgress;
        protected string title;
        protected bool Submitted = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(CustomerOrderId, out var customerOrderId))
                    throw new Exception("El Id de pedido recibido no es valido");

                customersForCUSTOMERID = await CustomerService.GetAsync();

                documentType = await DocumentTypeService.FindByCodeAsync("P");

                customerOrder = await CustomerOrderService.FindAsync(customerOrderId);

                var customerOrderDetails = await CustomerOrderDetailService.GetByCustomerOrderIdAsync(customerOrder.CustomerOrderId);
                this.customerOrderDetails = customerOrderDetails.ToList();

                title = $"Actualizar el Pedido No. {customerOrder.OrderNumber}";
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally { IsLoadingInProgress = false; }
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

                customerOrder.CustomerOrderDetails = customerOrderDetails;
                await CustomerOrderService.UpdateAsync(customerOrder.CustomerOrderId, customerOrder);

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

        protected async Task AddCustomerOrderDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddCustomerOrderDetail>("Agregar referencia", new Dictionary<string, object> { { "CustomerOrderDetails", customerOrderDetails } });

            if (result == null)
                return;

            var detail = (CustomerOrderDetail)result;

            customerOrderDetails.Add(detail);

            await customerOrderDetailGrid.Reload();
        }

        protected async Task DeleteCustomerOrderDetailButtonClick(MouseEventArgs args, CustomerOrderDetail item)
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
            var detail = (CustomerOrderDetail)result;

            customerOrderDetails.Remove(args);
            customerOrderDetails.Add(detail);

            await customerOrderDetailGrid.Reload();
        }
        #endregion
    }
}