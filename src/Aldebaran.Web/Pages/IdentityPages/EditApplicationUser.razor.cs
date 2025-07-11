using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace Aldebaran.Web.Pages.IdentityPages
{
    public partial class EditApplicationUser
    {
        #region Injections
        [Inject]
        protected ILogger<EditApplicationUser> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        #endregion

        [Parameter]
        public string Id { get; set; }

        protected IEnumerable<Models.ApplicationRole> ApplicationRoles;
        protected Models.ApplicationUser ApplicationUser;
        protected IEnumerable<string> UserRoles = Enumerable.Empty<string>();
        protected string Error;
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                ApplicationUser = await Security.GetUserById($"{Id}");
                UserRoles = ApplicationUser.Roles.Select(role => role.Id);
                ApplicationRoles = (await Security.GetRoles()).OrderBy(o => o.Name);
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
                ApplicationUser.Roles = UserRoles == null ? new List<Models.ApplicationRole>() : ApplicationRoles.Where(role => UserRoles.Contains(role.Id)).ToList();
                var result = await Security.UpdateUser($"{Id}", ApplicationUser);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        bool IsPasswordConfirmationRequired()
        {
            return !(!string.IsNullOrEmpty(ApplicationUser.Password) && string.IsNullOrEmpty(ApplicationUser.ConfirmPassword));
        }
        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }
        #endregion
    }
}