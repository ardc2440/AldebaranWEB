using Aldebaran.Web.Models;
using Aldebaran.Web.Models.ViewModel;
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
        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        protected IEnumerable<ApplicationRole> roles;
        protected RadzenDataGrid<ApplicationRole> grid0;
        protected RadzenDataGrid<UserViewModel> ApplicationUserDataGrid;
        protected ApplicationRole role;
        protected string error;
        protected bool errorVisible;
        protected string search = "";
        protected IEnumerable<UserViewModel> users;

        protected override async Task OnInitializedAsync()
        {
            roles = await Security.GetRoles();
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await grid0.GoToPage(0);
            var r = await Security.GetRoles();
            roles = r.Where(i => i.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));
        }
        protected async Task GetChildData(ApplicationRole args)
        {
            role = args;
            users = new List<UserViewModel>();
            var applicationUsers = await Security.GetUsersByRole(args.Id);
            var allUsers = await AldebaranDbService.GetUsers(new Query { Expand = "IdentityType,Area" });
            if (applicationUsers == null || !applicationUsers.Any())
            {
                users = new List<UserViewModel>();
                return;
            }
            users = applicationUsers.Select(appUser =>
            {
                var user = allUsers.Single(s => s.LOGIN_USER_ID == appUser.Id);
                return new UserViewModel
                {
                    Id = appUser.Id,
                    Email = appUser.Email,
                    LockoutEnabled = appUser.LockoutEnabled,
                    UserName = appUser.Name,
                    IdentificationNumber = $"{user.IdentityType.IDENTITY_TYPE_CODE.Trim()}. {user.IDENTITY_NUMBER.Trim()}",
                    FullName = user.FULL_NAME,
                    Position = user.POSITION,
                    Area = $"{user.Area.AREA_CODE.Trim()} - {user.Area.AREA_NAME.Trim()}"
                };
            });
        }
        protected async Task DeleteClick(ApplicationRole role)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this role?") == true)
                {
                    await Security.DeleteRole($"{role.Id}");

                    roles = await Security.GetRoles();
                }
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }
    }
}