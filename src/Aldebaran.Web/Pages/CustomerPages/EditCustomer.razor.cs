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
    public partial class EditCustomer
    {
        #region Injections
        [Inject]
        protected ILogger<EditCustomer> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected IIdentityTypeService IdentityTypeService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public int CUSTOMER_ID { get; set; }

        #endregion

        #region Variables

        protected bool IsErrorVisible;
        protected Customer Customer;
        protected IEnumerable<IdentityType> IdentityTypesForSelection = new List<IdentityType>();
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
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
                Customer = await CustomerService.FindAsync(CUSTOMER_ID);
                IdentityTypesForSelection = await IdentityTypeService.GetAsync();
                emails = Customer.Email1.Split(";").ToList();
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
                Customer.Email1 = string.Join(";", emails.ToArray());
                await CustomerService.UpdateAsync(CUSTOMER_ID, Customer);
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