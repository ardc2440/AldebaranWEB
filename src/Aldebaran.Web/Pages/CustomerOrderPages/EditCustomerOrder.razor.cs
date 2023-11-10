using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class EditCustomerOrder
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

        protected bool errorVisible;
        protected string errorMessage;
        protected CustomerOrder customerOrder;
        protected DocumentType documentType;
        protected ICollection<CustomerOrderDetail> customerOrderDetails;
        protected RadzenDataGrid<CustomerOrderDetail> customerOrderDetailGrid;

        protected IEnumerable<Customer> customersForCUSTOMERID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string title;

        [Parameter]
        public string pCustomerOrderId { get; set; } = "NoParamInput";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(pCustomerOrderId, out var customerOrderId))
                    throw new Exception("El Id de Referencia recibido no es valido");

                customersForCUSTOMERID = await AldebaranDbService.GetCustomers(new Query { Expand = "City.Department.Country" });

                documentType = await AldebaranDbService.GetDocumentTypeByCode("P");

                customerOrder = await AldebaranDbService.GetCustomerOrdersById(customerOrderId);

                var customerOrderDetails = await AldebaranDbService.GetCustomerOrderDetails(new Query { Filter = $@"i=>i.CUSTOMER_ORDER_ID.Equals(@0)", FilterParameters = new object[] { customerOrder.CUSTOMER_ORDER_ID }, Expand = "CustomerOrder, ItemReference, ItemReference.Item, ItemReference.Item.Line" });
                this.customerOrderDetails = customerOrderDetails.ToList();

                title = $"Modificaciones para el Pedido No. {customerOrder.ORDER_NUMBER}";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;

                if (!customerOrderDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                customerOrder.CustomerOrderDetails = customerOrderDetails;
                await AldebaranDbService.UpdateCustomerOrder(customerOrder);

                await DialogService.Alert($"Pedido de Articulos Modificado Satisfactoriamente", "Información");
                NavigationManager.NavigateTo("customer-orders");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que cancelar la creacion del Pedido??", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-orders");
        }

        protected async Task AddCustomerOrderDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddCustomerOrderDetail>("Nueva referencia", new Dictionary<string, object> { { "customerOrderDetails", customerOrderDetails } });

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
            var result = await DialogService.OpenAsync<EditCustomerOrderDetail>("Actualizar referencia", new Dictionary<string, object> { { "customerOrderDetail", args } });
            if (result == null)
                return;
            var detail = (CustomerOrderDetail)result;

            customerOrderDetails.Remove(args);
            customerOrderDetails.Add(detail);

            await customerOrderDetailGrid.Reload();
        }
    }
}