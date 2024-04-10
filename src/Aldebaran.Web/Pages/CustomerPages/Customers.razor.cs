using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.AreaPages;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerPages
{
    public partial class Customers
    {
        #region Injections
        [Inject]
        protected ILogger<Areas> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected ICustomerContactService CustomerContactService { get; set; }

        #endregion

        #region Variables

        protected IEnumerable<Customer> CustomersList;
        protected LocalizedDataGrid<Customer> CustomerDataGrid;
        protected string search = "";
        protected Customer Customer;
        protected LocalizedDataGrid<CustomerContact> CustomerContactsDataGrid;
        protected bool isLoadingInProgress;

        #endregion

        #region Override

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await GetCustomersAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task GetCustomersAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            CustomersList = string.IsNullOrEmpty(searchKey) ? await CustomerService.GetAsync(ct) : await CustomerService.GetAsync(searchKey, ct);
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await CustomerDataGrid.GoToPage(0);
            await GetCustomersAsync(search);
        }
        protected async Task AddCustomer(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddCustomer>("Nuevo cliente");
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Cliente",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Cliente creado correctamente."
                });
            }
            await GetCustomersAsync();
            await CustomerDataGrid.Reload();
        }
        protected async Task EditCustomer(Customer args)
        {
            var result = await DialogService.OpenAsync<EditCustomer>("Actualizar cliente", new Dictionary<string, object> { { "CUSTOMER_ID", args.CustomerId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Cliente",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Cliente actualizado correctamente."
                });
            }
            await GetCustomersAsync();
            await CustomerDataGrid.Reload();
        }
        protected async Task DeleteCustomer(MouseEventArgs args, Customer customer)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este cliente?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await CustomerService.DeleteAsync(customer.CustomerId);
                    await GetCustomersAsync();
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Cliente",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Cliente eliminado correctamente."
                    });
                    await CustomerDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteCustomer));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el cliente."
                });
            }
        }

        protected async Task GetCustomerContacts(Customer args)
        {
            Customer = args;
            await Task.Yield();
            var CustomerContactsResult = await CustomerContactService.GetByCustomerIdAsync(args.CustomerId);
            args.CustomerContacts = CustomerContactsResult.ToList();
        }

        protected async Task AddCustomerContact(MouseEventArgs args, Customer data)
        {
            var result = await DialogService.OpenAsync<AddCustomerContact>("Nuevo contacto", new Dictionary<string, object> { { "CUSTOMER_ID", data.CustomerId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Contacto",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Contacto creado correctamente."
                });
            }
            await GetCustomerContacts(data);
            await CustomerContactsDataGrid.Reload();
        }

        protected async Task EditCustomerContact(CustomerContact args, Customer data)
        {
            var result = await DialogService.OpenAsync<EditCustomerContact>("Actualizar contacto", new Dictionary<string, object> { { "CUSTOMER_CONTACT_ID", args.CustomerContactId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Contacto",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Contacto actualizado correctamente."
                });
            }
            await GetCustomerContacts(data);
            await CustomerContactsDataGrid.Reload();
        }

        protected async Task DeleteCustomerContact(MouseEventArgs args, CustomerContact customerContact)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este contacto?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await CustomerContactService.DeleteAsync(customerContact.CustomerContactId);
                    await GetCustomerContacts(Customer);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Contacto",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Contacto eliminado correctamente."
                    });
                    await CustomerContactsDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteCustomerContact));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el contacto."
                });
            }
        }

        #endregion
    }
}