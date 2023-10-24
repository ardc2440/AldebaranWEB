using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.IdentityPages
{
    public partial class ApplicationRoles
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
        protected SecurityService Security { get; set; }

        protected IEnumerable<ApplicationRole> roles;
        protected RadzenDataGrid<ApplicationRole> ApplicationRoleDataGrid;
        protected RadzenDataGrid<ApplicationUser> ApplicationUserDataGrid;
        protected ApplicationRole role;
        protected string error;
        protected bool errorVisible;
        protected string search = "";
        protected IEnumerable<ApplicationUser> users;
        protected bool isLoadingInProgress;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                roles = await Security.GetRoles();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ApplicationRoleDataGrid.GoToPage(0);
            var r = await Security.GetRoles();
            roles = r.Where(i => i.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));
        }
        protected async Task GetChildData(ApplicationRole args)
        {
            role = args;
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                users = await Security.GetUsersByRole(args.Id);
            }
            finally
            {
                isLoadingInProgress = false;
            };
        }
    }
}