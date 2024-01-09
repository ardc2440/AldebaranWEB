using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerPages
{
    public partial class Customers
    {
        #region Injections

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

        protected IEnumerable<Customer> customers;
        protected RadzenDataGrid<Customer> grid0;
        protected string search = "";
        protected Customer customer;
        protected RadzenDataGrid<CustomerContact> CustomerContactsDataGrid;
        protected DialogResult dialogResult;
        protected bool isLoadingInProgress;

        #endregion

        #region Override

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                customers = await CustomerService.GetAsync(search);
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await grid0.GoToPage(0);
            customers = await CustomerService.GetAsync(search);
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddCustomer>("Nuevo cliente");
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Cliente creado correctamente." };
                customers = await CustomerService.GetAsync(search);
                await grid0.Reload();
            }
        }

        protected async Task EditRow(Customer args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditCustomer>("Actualizar cliente", new Dictionary<string, object> { { "CUSTOMER_ID", args.CustomerId } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Cliente actualizado correctamente." };
                customers = await CustomerService.GetAsync(search);
                await grid0.Reload();
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Customer customer)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este cliente?") == true)
                {
                    await CustomerService.DeleteAsync(customer.CustomerId);
                    dialogResult = new DialogResult { Success = true, Message = "Cliente eliminado correctamente." };
                    customers = await CustomerService.GetAsync(search);
                    await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el cliente\n\r{ex.InnerException.Message}\n\r{ex.StackTrace}"
                });
            }
        }

        protected async Task GetChildData(Customer args)
        {
            customer = args;
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();
                var CustomerContactsResult = await CustomerContactService.GetByCustomerIdAsync(args.CustomerId);
                if (CustomerContactsResult != null)
                {
                    args.CustomerContacts = CustomerContactsResult.ToList();
                }
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        protected async Task CustomerContactsAddButtonClick(MouseEventArgs args, Customer data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddCustomerContact>("Nuevo contacto", new Dictionary<string, object> { { "CUSTOMER_ID", data.CustomerId } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Contacto creado correctamente." };
            }
            await GetChildData(data);
            await CustomerContactsDataGrid.Reload();
        }

        protected async Task EditChildRow(CustomerContact args, Customer data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditCustomerContact>("Actualizar contacto", new Dictionary<string, object> { { "CUSTOMER_CONTACT_ID", args.CustomerContactId } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Contacto actualizado correctamente." };
            }
            await GetChildData(data);
            await CustomerContactsDataGrid.Reload();
        }

        protected async Task CustomerContactsDeleteButtonClick(MouseEventArgs args, CustomerContact customerContact)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este contacto?") == true)
                {
                    await CustomerContactService.DeleteAsync(customerContact.CustomerContactId);
                    await GetChildData(customer);

                    dialogResult = new DialogResult { Success = true, Message = "Contacto eliminado correctamente." };
                    await CustomerContactsDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el contacto\n\r{ex.InnerException.Message}\n\r{ex.StackTrace}"
                });
            }
        }

        #endregion
    }
}