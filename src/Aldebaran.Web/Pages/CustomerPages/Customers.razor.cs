using Aldebaran.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerPages
{
    public partial class Customers
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

        protected IEnumerable<Models.AldebaranDb.Customer> customers;
        protected RadzenDataGrid<Models.AldebaranDb.Customer> grid0;
        protected string search = "";
        protected Models.AldebaranDb.Customer customer;
        protected RadzenDataGrid<Models.AldebaranDb.CustomerContact> CustomerContactsDataGrid;
        protected DialogResult dialogResult { get; set; }
        protected bool isLoadingInProgress;

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await grid0.GoToPage(0);
            customers = await AldebaranDbService.GetCustomers(new Query { Filter = $@"i => i.IDENTITY_NUMBER.Contains(@0) || i.CUSTOMER_NAME.Contains(@0) || i.PHONE1.Contains(@0) || i.PHONE2.Contains(@0) || i.FAX.Contains(@0) || i.CUSTOMER_ADDRESS.Contains(@0) || i.CELL_PHONE.Contains(@0) || i.EMAIL1.Contains(@0) || i.EMAIL2.Contains(@0) || i.EMAIL3.Contains(@0)", FilterParameters = new object[] { search }, Expand = "City.Department.Country,IdentityType" });
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                customers = await AldebaranDbService.GetCustomers(new Query { Filter = $@"i => i.IDENTITY_NUMBER.Contains(@0) || i.CUSTOMER_NAME.Contains(@0) || i.PHONE1.Contains(@0) || i.PHONE2.Contains(@0) || i.FAX.Contains(@0) || i.CUSTOMER_ADDRESS.Contains(@0) || i.CELL_PHONE.Contains(@0) || i.EMAIL1.Contains(@0) || i.EMAIL2.Contains(@0) || i.EMAIL3.Contains(@0)", FilterParameters = new object[] { search }, Expand = "City.Department.Country,IdentityType" });
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddCustomer>("Nuevo cliente");
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Cliente creado correctamente." };
            }
            await grid0.Reload();
        }

        protected async Task EditRow(Models.AldebaranDb.Customer args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditCustomer>("Actualizar cliente", new Dictionary<string, object> { { "CUSTOMER_ID", args.CUSTOMER_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Cliente actualizado correctamente." };
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.Customer customer)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este cliente?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteCustomer(customer.CUSTOMER_ID);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Cliente eliminado correctamente." };
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el cliente"
                });
            }
        }

        protected async Task GetChildData(Models.AldebaranDb.Customer args)
        {
            customer = args;
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();
                var CustomerContactsResult = await AldebaranDbService.GetCustomerContacts(new Query { Filter = $@"i => i.CUSTOMER_ID == {args.CUSTOMER_ID}", Expand = "Customer" });
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

        protected async Task CustomerContactsAddButtonClick(MouseEventArgs args, Models.AldebaranDb.Customer data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddCustomerContact>("Nuevo contacto", new Dictionary<string, object> { { "CUSTOMER_ID", data.CUSTOMER_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Contacto creado correctamente." };
            }
            await GetChildData(data);
            await CustomerContactsDataGrid.Reload();
        }

        protected async Task EditChildRow(Models.AldebaranDb.CustomerContact args, Models.AldebaranDb.Customer data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditCustomerContact>("Actualizar contacto", new Dictionary<string, object> { { "CUSTOMER_CONTACT_ID", args.CUSTOMER_CONTACT_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Contacto actualizado correctamente." };
            }
            await GetChildData(data);
            await CustomerContactsDataGrid.Reload();
        }

        protected async Task CustomerContactsDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.CustomerContact customerContact)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este contacto?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteCustomerContact(customerContact.CUSTOMER_CONTACT_ID);
                    await GetChildData(customer);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Contacto eliminado correctamente." };
                        await CustomerContactsDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el contacto"
                });
            }
        }
    }
}