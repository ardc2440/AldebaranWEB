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
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }

        #endregion

        #region Global Variables
        protected bool errorVisible;
        protected string errorMessage;
        protected CustomerOrder customerOrder;
        protected DocumentType documentType;
        protected ICollection<CustomerOrderDetail> customerOrderDetails;
        protected LocalizedDataGrid<CustomerOrderDetail> customerOrderDetailGrid;
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
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;

                if (!customerOrderDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                customerOrder.CustomerOrderDetails = customerOrderDetails;

                await CustomerOrderService.AddAsync(customerOrder);

                await DialogService.Alert($"Pedido de Articulos Guardado Satisfactoriamente con el Consecutivo {customerOrder.OrderNumber}", "Información");
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
            var result = await DialogService.OpenAsync<AddCustomerOrderDetail>("Nueva referencia", new Dictionary<string, object> { { "CustomerOrderDetails", customerOrderDetails } });

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
            var detail = (CustomerOrderDetail)result;

            customerOrderDetails.Remove(args);
            customerOrderDetails.Add(detail);

            await customerOrderDetailGrid.Reload();
        }
        #endregion
    }
}