using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class AddCustomerReservation
    {
        #region Injections
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerReservationService CustomerReservationService { get; set; }

        #endregion

        #region Global Variables

        protected CustomerReservation customerReservation;
        protected IEnumerable<Customer> customersForCUSTOMERID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected ICollection<CustomerReservationDetail> customerReservationDetails;
        protected LocalizedDataGrid<CustomerReservationDetail> customerReservationDetailGrid;
        protected DocumentType documentType;
        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        protected int lastReferenceId = 0;

        protected int count = 0;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                var (customers, _count) = await CustomerService.GetAsync(0, 5);
                customersForCUSTOMERID = customers.ToList();
                count = _count;

                documentType = await DocumentTypeService.FindByCodeAsync("R");

                customerReservationDetails = new List<CustomerReservationDetail>();

                customerReservation = new CustomerReservation()
                {
                    CustomerReservationId = 0,
                    Employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id),
                    StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1),
                    ReservationDate = DateTime.Today,
                    CreationDate = DateTime.Today,
                    ReservationNumber = "0"
                };
                customerReservation.StatusDocumentTypeId = customerReservation.StatusDocumentType.StatusDocumentTypeId;
                customerReservation.EmployeeId = customerReservation.Employee.EmployeeId;
            }
            finally
            {
                isLoadingInProgress = false;
            }            
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

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                if (!customerReservationDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                if (await DialogService.Confirm("Está seguro que desea crear esta reserva de artículos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar creación") == true)
                {
                    customerReservation.CustomerReservationDetails = customerReservationDetails;
                    customerReservation = await CustomerReservationService.AddAsync(customerReservation);

                    var result = await DialogService.OpenAsync<CustomerReservationSummary>(null, new Dictionary<string, object> { { "Id", customerReservation.CustomerReservationId }, { "NotificationTemplateName", "Customer:Reservation:New" } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
                    NavigationManager.NavigateTo($"customer-reservations/{customerReservation.CustomerReservationId}");
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally { IsSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que desea cancelar la creación de la Reserva?", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-reservations");
        }

        protected async Task AddCustomerReservationDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddCustomerReservationDetail>("Agregar referencia", new Dictionary<string, object> { { "CustomerReservationDetails", customerReservationDetails }, { "LastReferenceId", lastReferenceId} });

            if (result == null)
                return;

            var detail = (CustomerReservationDetail)result;

            customerReservationDetails.Add(detail);

            lastReferenceId = detail.ReferenceId;

            await customerReservationDetailGrid.Reload();
        }

        protected async Task DeleteCustomerReservationDetailButtonClick(CustomerReservationDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                customerReservationDetails.Remove(item);

                await customerReservationDetailGrid.Reload();
            }
        }

        protected async Task EditRow(CustomerReservationDetail args)
        {
            var result = await DialogService.OpenAsync<EditCustomerReservationDetail>("Actualizar referencia", new Dictionary<string, object> { { "CustomerReservationDetail", args } });
            if (result == null)
                return;

            args.ReservedQuantity = result.ReservedQuantity;
            args.Brand = result.Brand;

            await customerReservationDetailGrid.Reload();
        }
        #endregion
    }
}