using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class AddCustomerOrderFromReservation
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

        [Parameter]
        public string pCustomerReservationId { get; set; } = "NoParamInput";

        protected bool errorVisible;
        protected string errorMessage;
        protected CustomerReservation customerReservation;
        protected CustomerOrder customerOrder;
        protected DocumentType documentType;
        protected ICollection<CustomerOrderDetail> customerOrderDetails;
        protected RadzenDataGrid<CustomerOrderDetail> customerOrderDetailGrid;

        protected IEnumerable<Customer> customersForCUSTOMERID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                customersForCUSTOMERID = await AldebaranDbService.GetCustomers();

                if (!int.TryParse(pCustomerReservationId, out var customerReservationId))
                    throw new Exception("El Id de Referencia recibido no es valido");

                var customerReservation = await AldebaranDbService.GetCustomerReservationByCustomerReservationId(customerReservationId) ?? throw new Exception("No ha seleccionado una Reservacion Valida");

                if (!customerReservation.CustomerReservationDetails.Any(cd => cd.SEND_TO_CUSTOMER_ORDER))
                    throw new Exception("La reserva seleccionada no posee articulos para incluir en el pedido");

                await ChargeCustomerOrderModel(customerReservation);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        protected async Task ChargeCustomerOrderModel(CustomerReservation customerReservation)
        {

            customerOrder = new CustomerOrder()
            {
                CUSTOMER_NOTES = customerReservation.NOTES,
                ORDER_DATE = DateTime.Today
            };

            customerOrder.Customer = await AldebaranDbService.GetCustomerByCustomerId(customerReservation.CUSTOMER_ID);
            customerOrder.CUSTOMER_ID = customerOrder.Customer.CUSTOMER_ID;

            customerOrder.Employee = await AldebaranDbService.GetLoggedEmployee(Security);
            customerOrder.EMPLOYEE_ID = customerOrder.Employee.EMPLOYEE_ID;

            documentType = await AldebaranDbService.GetDocumentTypeByCode("P");

            customerOrder.StatusDocumentType = await AldebaranDbService.GetStatusDocumentTypeByDocumentAndOrder(documentType, 1);
            customerOrder.STATUS_DOCUMENT_TYPE_ID = customerOrder.StatusDocumentType.STATUS_DOCUMENT_TYPE_ID;

            await ChargeCustomerOrderDetailModel(customerReservation.CustomerReservationDetails);
        }

        protected async Task ChargeCustomerOrderDetailModel(ICollection<CustomerReservationDetail> customerReservationDetails)
        {
            customerOrderDetails = new List<CustomerOrderDetail>();

            foreach (var customerReservationDetail in customerReservationDetails.Where(cd => cd.SEND_TO_CUSTOMER_ORDER))
            {
                customerOrderDetails.Add(new CustomerOrderDetail()
                {
                    REFERENCE_ID = customerReservationDetail.REFERENCE_ID,
                    REQUESTED_QUANTITY = customerReservationDetail.RESERVED_QUANTITY,
                    BRAND = customerReservationDetail.BRAND
                });
            }

            foreach (var customerOrderDetail in customerOrderDetails)
            {
                customerOrderDetail.ItemReference = await AldebaranDbService.GetItemReferenceByReferenceId(customerOrderDetail.REFERENCE_ID);
            }
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;

                if (!customerOrderDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                customerOrder.CustomerOrderDetails = customerOrderDetails;
                await AldebaranDbService.CreateCustomerOrder(customerOrder);

                customerOrder.ORDER_NUMBER = await AldebaranDbService.GenerateDocumentNumber(documentType);
                await AldebaranDbService.UpdateCustomerOrder(customerOrder);

                await DialogService.Alert("Pedido de Reserva de Articulos Guardado Satisfactoriamente", "Información");
                NavigationManager.NavigateTo("customer-reservations");
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
                NavigationManager.NavigateTo("customer-reservations");
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