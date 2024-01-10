using Aldebaran.Application.Services;
using Aldebaran.Web.Models;
using Aldebaran.Web.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages
{
    public partial class Profile
    {
        #region Injections
        [Inject]
        protected ILogger<Profile> Logger { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IMapper Mapper { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Variables
        protected string OldPassword = "";
        protected string NewPassword = "";
        protected string ConfirmPassword = "";
        protected string Error;
        protected bool IsErrorVisible;
        protected DialogResult DialogResult { get; set; }
        protected bool IsSubmitInProgress;

        protected ServiceModel.Employee Employee;
        protected ApplicationUser ApplicationUser;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ApplicationUser = await Security.GetUserById($"{Security.User.Id}");
            Employee = await EmployeeService.FindByLoginUserIdAsync(ApplicationUser.Id);
        }
        #endregion

        #region Events
        protected async Task EmployeeFormSubmit()
        {
            try
            {
                DialogResult = null;
                if (await DialogService.Confirm("Est� seguro que desea actualizar su informaci�n?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualizaci�n") == true)
                {
                    await EmployeeService.UpdateAsync(Employee.EmployeeId, Employee);
                    DialogResult = new DialogResult { Success = true, Message = "Informaci�n actualizada correctamente." };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(EmployeeFormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
        }
        protected async Task ApplicationUserFormSubmit()
        {
            try
            {
                DialogResult = null;
                if (await DialogService.Confirm("Est� seguro que desea actualizar su contrase�a?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualizaci�n") == true)
                {
                    IsErrorVisible = false;
                    await Security.ChangePassword(OldPassword, NewPassword);
                    DialogResult = new DialogResult { Success = true, Message = "Contrase�a actualizada correctamente." };
                    OldPassword = "";
                    NewPassword = "";
                    ConfirmPassword = "";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(ApplicationUserFormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
        }
        #endregion
    }
}