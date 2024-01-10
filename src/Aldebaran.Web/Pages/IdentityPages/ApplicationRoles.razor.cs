using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.IdentityPages
{
    public partial class ApplicationRoles
    {
        #region Injections
        [Inject]
        protected ILogger<ApplicationRoles> Logger { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        #endregion

        #region Variables
        protected IEnumerable<ApplicationRole> ApplicationRolesList;
        protected RadzenDataGrid<ApplicationRole> ApplicationRoleDataGrid;
        protected RadzenDataGrid<ApplicationUser> ApplicationUserDataGrid;
        protected ApplicationRole ApplicationRole;
        protected string Error;
        protected bool IsErrorVisible;
        protected string search = "";
        protected IEnumerable<ApplicationUser> ApplicationUsers;
        protected bool IsLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoadingInProgress = true;
                await GetApplicationRolesAsync();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }
        #endregion

        #region Events
        async Task GetApplicationRolesAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            var roles = await Security.GetRoles();

            ApplicationRolesList = string.IsNullOrEmpty(searchKey) ? roles.OrderBy(o => o.Name) : roles.Where(i => i.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).OrderBy(o => o.Name);
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ApplicationRoleDataGrid.GoToPage(0);
            await GetApplicationRolesAsync(search);
        }
        protected async Task GetApplicationUsers(ApplicationRole args)
        {
            ApplicationRole = args;
            try
            {
                IsLoadingInProgress = true;
                await Task.Yield();
                ApplicationUsers = await Security.GetUsersByRole(args.Id);
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }
        #endregion
    }
}