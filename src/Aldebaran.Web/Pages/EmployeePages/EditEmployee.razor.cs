using Aldebaran.Application.Services;
using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.EmployeePages
{
    public partial class EditEmployee
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IAreaService AreaService { get; set; }

        [Inject]
        protected IIdentityTypeService IdentityTypeService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int EMPLOYEE_ID { get; set; }
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.Employee Employee;
        protected IEnumerable<ServiceModel.Area> Areas;
        protected IEnumerable<ServiceModel.IdentityType> IdentityTypes;
        protected IEnumerable<ApplicationUser> ApplicationUsers;
        protected bool IsSubmitInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Employee = await EmployeeService.FindAsync(EMPLOYEE_ID);
            Areas = await AreaService.GetAsync();
            IdentityTypes = await IdentityTypeService.GetAsync();
            ApplicationUsers = await Security.GetUsers();
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                await EmployeeService.UpdateAsync(EMPLOYEE_ID, Employee);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}