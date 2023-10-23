using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.IdentityPages
{
    public partial class AddApplicationUser
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
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected IEnumerable<Models.ApplicationRole> roles;
        protected IEnumerable<IdentityType> identityTypesForIDENTITYTYPEID;
        protected IEnumerable<Area> areasForAREAID;
        protected Models.ApplicationUser applicationUser;
        protected Employee user;
        protected IEnumerable<string> userRoles = Enumerable.Empty<string>();
        protected bool errorVisible;
        protected string error;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            user = new Employee();
            applicationUser = new Models.ApplicationUser();
            roles = await Security.GetRoles();
            identityTypesForIDENTITYTYPEID = await AldebaranDbService.GetIdentityTypes();
            areasForAREAID = await AldebaranDbService.GetAreas();
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                // verificar que el username no se encuentre ya en uso
                var users = await Security.GetUsers();
                if (users.Any(s => s.UserName == applicationUser.UserName.Trim()))
                    throw new Exception("Ya existe un usuario con el mismo nombre");

                applicationUser.UserName = applicationUser.UserName.Trim();
                applicationUser.Roles = roles.Where(role => userRoles.Contains(role.Id)).ToList();
                var result = await Security.CreateUser(applicationUser);

                user.LOGIN_USER_ID = result.Id;
                await AldebaranDbService.CreateUser(user);

                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }
    }
}