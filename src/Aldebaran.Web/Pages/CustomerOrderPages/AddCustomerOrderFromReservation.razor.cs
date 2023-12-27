using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class AddCustomerOrderFromReservation
    {
        #region Injections

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected ICustomerReservationService CustomerReservationService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected ICustomerReservationDetailService CustomerReservationDetailService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CustomerReservationId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

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

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                customersForCUSTOMERID = await CustomerService.GetAsync();

                if (!int.TryParse(CustomerReservationId, out var customerReservationId))
                    throw new Exception("El Id de Referencia recibido no es valido");

                customerReservation = await CustomerReservationService.FindAsync(customerReservationId) ?? throw new Exception("No ha seleccionado una Reservacion Valida");

                if (!customerReservation.CustomerReservationDetails.Any(cd => cd.SendToCustomerOrder))
                    throw new Exception("La reserva seleccionada no posee articulos para incluir en el pedido");

                await ChargeCustomerOrderModel();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        #endregion

        #region Events

        protected async Task ChargeCustomerOrderModel()
        {

            customerOrder = new CustomerOrder()
            {
                CustomerNotes = customerReservation.Notes,
                OrderDate = DateTime.Today
            };

            customerOrder.Customer = await CustomerService.FindAsync(customerReservation.CustomerId);
            customerOrder.CustomerId = customerOrder.Customer.CustomerId;

            customerOrder.Employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
            customerOrder.EmployeeId = customerOrder.Employee.EmployeeId;

            documentType = await DocumentTypeService.FindByCodeAsync("P");

            customerOrder.StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
            customerOrder.StatusDocumentTypeId = customerOrder.StatusDocumentType.StatusDocumentTypeId;

            await ChargeCustomerOrderDetailModel(customerReservation.CustomerReservationDetails);
        }

        protected async Task ChargeCustomerOrderDetailModel(ICollection<CustomerReservationDetail> customerReservationDetails)
        {
            customerOrderDetails = new List<CustomerOrderDetail>();

            foreach (var customerReservationDetail in customerReservationDetails.Where(cd => cd.SendToCustomerOrder))
            {
                customerOrderDetails.Add(new CustomerOrderDetail()
                {
                    ReferenceId = customerReservationDetail.ReferenceId,
                    RequestedQuantity = customerReservationDetail.ReservedQuantity,
                    Brand = customerReservationDetail.Brand
                });
            }

            foreach (var customerOrderDetail in customerOrderDetails)
            {
                customerOrderDetail.ItemReference = await ItemReferenceService.FindAsync(customerOrderDetail.ReferenceId);
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

                customerOrder = await CustomerOrderService.AddAsync(customerOrder);

                if (customerOrder.CustomerOrderId > 0)
                {
                    customerReservation.CustomerOrderId = customerOrder.CustomerOrderId;
                    customerReservation.StatusDocumentTypeId = (await StatusDocumentTypeService.FindByDocumentAndOrderAsync((await DocumentTypeService.FindByCodeAsync("R")).DocumentTypeId, 2)).StatusDocumentTypeId;

                    await CustomerReservationService.UpdateAsync(customerReservation.CustomerReservationId, customerReservation);

                    await DialogService.Alert($"Pedido de Reserva de Articulos Guardado Satisfactoriamente con el consecutivo {customerOrder.OrderNumber}", "Información");
                    NavigationManager.NavigateTo("customer-reservations");
                }
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
            if (await DialogService.Confirm("Está seguro que cancelar la creación del Pedido??", "Confirmar") == true)
            {
                if (!int.TryParse(CustomerReservationId, out var customerReservationId))
                    throw new Exception("El Id de Referencia recibido no es valido");

                var customerReservation = await CustomerReservationService.FindAsync(customerReservationId) ?? throw new Exception("No ha seleccionado una Reservacion Valida");

                foreach (var item in customerReservation.CustomerReservationDetails.Where(d => d.SendToCustomerOrder).ToList())
                {
                    item.SendToCustomerOrder = false;
                    await CustomerReservationDetailService.UpdateAsync(item.CustomerReservationDetailId, item);
                }

                NavigationManager.NavigateTo("customer-reservations");
            }
        }

        protected async Task AddCustomerOrderDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddCustomerOrderDetail>("Nueva referencia", new Dictionary<string, object> { { "CustomerOrderDetails", customerOrderDetails } });

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