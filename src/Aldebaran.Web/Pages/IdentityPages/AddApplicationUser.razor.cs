using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.IdentityPages
{
    public partial class AddApplicationUser
    {
        #region Injections
        [Inject]
        protected ILogger<AddApplicationUser> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        #endregion

        #region Variables
        protected IEnumerable<Models.ApplicationRole> ApplicationRoles;
        protected Models.ApplicationUser ApplicationUser;
        protected IEnumerable<string> UserRoles = Enumerable.Empty<string>();
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                ApplicationUser = new Models.ApplicationUser();
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
                // verificar que el username no se encuentre ya en uso
                var users = await Security.GetUsers();
                if (users.Any(s => string.Compare(s.UserName.Trim(), ApplicationUser.UserName.TrimEnd(), true) == 0))
                    throw new Exception("Ya existe un usuario con el mismo nombre.");
                ApplicationUser.LockoutEnabled = false;
                ApplicationUser.UserName = ApplicationUser.UserName.Trim();
                ApplicationUser.Roles = ApplicationRoles.Where(role => UserRoles.Contains(role.Id)).ToList();
                var result = await Security.CreateUser(ApplicationUser);
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

        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }
        #endregion
    }
}