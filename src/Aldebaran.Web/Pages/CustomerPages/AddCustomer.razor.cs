using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using System.ComponentModel.DataAnnotations;

namespace Aldebaran.Web.Pages.CustomerPages
{
    public partial class AddCustomer
    {
        #region Injections
        [Inject]
        protected ILogger<AddCustomer> Logger { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IIdentityTypeService IdentityTypeService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        #endregion

        #region Variables

        protected bool IsErrorVisible;
        protected Customer Customer;
        protected IEnumerable<IdentityType> IdentityTypesForSelection = new List<IdentityType>();
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected List<string> ValidationErrors;
        public RadzenTemplateForm<Customer> customerForm;
        List<string> emails = new List<string>();
        List<string> EmailValidationError = new List<string>();
        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Customer = new Customer();
                IdentityTypesForSelection = await IdentityTypeService.GetAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                ValidationErrors = new List<string>();
                var customerNamelreadyExists = await CustomerService.ExistsByName(Customer.CustomerName);
                if (customerNamelreadyExists)
                {
                    ValidationErrors.Add("Ya existe un cliente con el mismo nombre.");
                }
                var identityNumberAlreadyExists = await CustomerService.ExistsByIdentificationNumber(Customer.IdentityNumber);
                if (identityNumberAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe un cliente con el mismo número de identificación.");
                }
                if (ValidationErrors.Any())
                {
                    IsErrorVisible = true;
                    return;
                }
                Customer.Email = string.Join(";", emails.Select(s => s.Trim()));
                await CustomerService.AddAsync(Customer);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected async Task LocalizationHandler(City city)
        {
            Customer.CityId = city?.CityId ?? 0;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        private void OnEmailChipValidation(ChipValidationArgs chipValidationArgs)
        {
            var value = chipValidationArgs.CurrentChip;
            EmailValidationError = new List<string>(chipValidationArgs.ValidationErrors);
            var isValid = (new EmailAddressAttribute()).IsValid(value);
            if (!isValid)
            {
                chipValidationArgs.ValidationErrors.Add("Correo electrónico es inválido");
                EmailValidationError.Add("Correo electrónico inválido");
            }
            if (chipValidationArgs.Chips.Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                chipValidationArgs.ValidationErrors.Add("Correo electrónico ya está incluido en la lista");
                EmailValidationError.Add("Correo electrónico ya está incluido en la lista");
            }
            StateHasChanged();
        }
        private void OnEmailChipChange(List<string> chips)
        {
            emails = new List<string>(chips);
            EmailValidationError = new List<string>();
            customerForm.EditContext.Validate();
        }
        #endregion
    }
}