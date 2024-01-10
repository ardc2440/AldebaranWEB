using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
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
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected ICustomerContactService CustomerContactService { get; set; }

        #endregion

        #region Global Variables

        protected IEnumerable<Customer> CustomersList;
        protected LocalizedDataGrid<Customer> CustomerDataGrid;
        protected string search = "";
        protected Customer Customer;
        protected LocalizedDataGrid<CustomerContact> CustomerContactsDataGrid;
        protected DialogResult DialogResult;
        protected bool IsLoadingInProgress;

        #endregion

        #region Override

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoadingInProgress = true;
                await Task.Yield();
                await GetCustomersAsync();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }

        #endregion

        #region Events

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
            DialogResult = null;
            var result = await DialogService.OpenAsync<AddCustomer>("Nuevo cliente");
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Cliente creado correctamente." };
            }
            await GetCustomersAsync();
            await CustomerDataGrid.Reload();
        }
        protected async Task EditCustomer(Customer args)
        {
            DialogResult = null;
            var result = await DialogService.OpenAsync<EditCustomer>("Actualizar cliente", new Dictionary<string, object> { { "CUSTOMER_ID", args.CustomerId } });
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Cliente actualizado correctamente." };
            }
            await GetCustomersAsync();
            await CustomerDataGrid.Reload();
        }
        protected async Task DeleteCustomer(MouseEventArgs args, Customer customer)
        {
            try
            {
                DialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este cliente?") == true)
                {
                    await CustomerService.DeleteAsync(customer.CustomerId);
                    await GetCustomersAsync();
                    DialogResult = new DialogResult { Success = true, Message = "Cliente eliminado correctamente." };
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
                    Detail = $"No se ha podido eliminar el cliente"
                });
            }
        }

        protected async Task GetCustomerContacts(Customer args)
        {
            Customer = args;
            try
            {
                IsLoadingInProgress = true;
                await Task.Yield();
                var CustomerContactsResult = await CustomerContactService.GetByCustomerIdAsync(args.CustomerId);
                args.CustomerContacts = CustomerContactsResult.ToList();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }

        protected async Task AddCustomerContact(MouseEventArgs args, Customer data)
        {
            DialogResult = null;
            var result = await DialogService.OpenAsync<AddCustomerContact>("Nuevo contacto", new Dictionary<string, object> { { "CUSTOMER_ID", data.CustomerId } });
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Contacto creado correctamente." };
            }
            await GetCustomerContacts(data);
            await CustomerContactsDataGrid.Reload();
        }

        protected async Task EditCustomerContact(CustomerContact args, Customer data)
        {
            DialogResult = null;
            var result = await DialogService.OpenAsync<EditCustomerContact>("Actualizar contacto", new Dictionary<string, object> { { "CUSTOMER_CONTACT_ID", args.CustomerContactId } });
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Contacto actualizado correctamente." };
            }
            await GetCustomerContacts(data);
            await CustomerContactsDataGrid.Reload();
        }

        protected async Task DeleteCustomerContact(MouseEventArgs args, CustomerContact customerContact)
        {
            try
            {
                DialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este contacto?") == true)
                {
                    await CustomerContactService.DeleteAsync(customerContact.CustomerContactId);
                    await GetCustomerContacts(Customer);
                    DialogResult = new DialogResult { Success = true, Message = "Contacto eliminado correctamente." };
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
                    Detail = $"No se ha podido eliminar el contacto"
                });
            }
        }

        #endregion
    }
}