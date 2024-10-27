using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class AddCustomerOrderFromReservation
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

        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CustomerReservationId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

        protected bool IsErrorVisible;
        protected string Error;
        protected CustomerReservation customerReservation;
        protected CustomerOrder customerOrder;
        protected DocumentType documentType;
        protected ICollection<CustomerOrderDetail> customerOrderDetails;
        protected LocalizedDataGrid<CustomerOrderDetail> customerOrderDetailGrid;

        protected IEnumerable<Customer> customersForCUSTOMERID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool Submitted = false;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                customersForCUSTOMERID = (await CustomerService.GetAsync()).Customers;

                if (!int.TryParse(CustomerReservationId, out var customerReservationId))
                    throw new Exception("El Id de referencia recibido no es valido");

                customerReservation = await CustomerReservationService.FindAsync(customerReservationId) ?? throw new Exception("No ha seleccionado una Reservacion Valida");

                if (!customerReservation.CustomerReservationDetails.Any(cd => cd.SendToCustomerOrder))
                    throw new Exception("La reserva seleccionada no posee artículos para incluir en el pedido");

                await ChargeCustomerOrderModel();
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

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

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
                Submitted = true;
                IsSubmitInProgress = true;

                if (!customerOrderDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                customerOrder.CustomerOrderDetails = customerOrderDetails;

                customerOrder = await CustomerOrderService.AddAsync(customerOrder);

                if (customerOrder.CustomerOrderId > 0)
                {
                    customerReservation.CustomerOrderId = customerOrder.CustomerOrderId;
                    customerReservation.StatusDocumentTypeId = (await StatusDocumentTypeService.FindByDocumentAndOrderAsync((await DocumentTypeService.FindByCodeAsync("R")).DocumentTypeId, 2)).StatusDocumentTypeId;

                    await CustomerReservationService.UpdateAsync(customerReservation.CustomerReservationId, customerReservation, null);

                    var result = await DialogService.OpenAsync<CustomerOrderSummary>(null, new Dictionary<string, object> { { "Id", customerOrder.CustomerOrderId }, { "NotificationTemplateName", "Customer:Order:New" } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
                    NavigationManager.NavigateTo($"customer-reservations/create-order/{customerReservation.CustomerReservationId}");
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
            try
            {
                if (await DialogService.Confirm("Está seguro que desea cancelar la creación del Pedido?", "Confirmar") == true)
                {
                    if (!int.TryParse(CustomerReservationId, out var customerReservationId))
                        throw new Exception("El Id de referencia recibido no es valido");

                    var customerReservation = await CustomerReservationService.FindAsync(customerReservationId) ?? throw new Exception("No ha seleccionado una reservacion valida");

                    foreach (var item in customerReservation.CustomerReservationDetails.Where(d => d.SendToCustomerOrder).ToList())
                    {
                        item.SendToCustomerOrder = false;
                        await CustomerReservationDetailService.UpdateAsync(item.CustomerReservationDetailId, item);
                    }

                    NavigationManager.NavigateTo($"customer-reservations/cancel-order/{customerReservation.CustomerReservationId}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
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

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion
    }
}