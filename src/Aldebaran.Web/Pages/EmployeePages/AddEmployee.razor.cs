using Aldebaran.Application.Services;
using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.EmployeePages
{
    public partial class AddEmployee
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

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.Employee Employee;
        protected IEnumerable<ServiceModel.Area> Areas;
        protected IEnumerable<ServiceModel.IdentityType> IdentityTypes;
        protected IEnumerable<ApplicationUser> ApplicationUsers;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress ;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                Employee = new ServiceModel.Employee();
                Areas = await AreaService.GetAsync();
                IdentityTypes = await IdentityTypeService.GetAsync();
                var users = await Security.GetUsers();
                var employees = await EmployeeService.GetAsync();
                var ids = employees.Select(s => s.LoginUserId);
                ApplicationUsers = users.Where(w => !ids.Contains(w.Id));

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
                await EmployeeService.AddAsync(Employee);
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