using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.IdentityPages
{
    public partial class EditApplicationUser
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

        [Parameter]
        public string Id { get; set; }

        protected IEnumerable<Models.ApplicationRole> roles;
        protected Models.ApplicationUser applicationUser;
        protected IEnumerable<string> userRoles;
        protected string error;
        protected bool errorVisible;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            applicationUser = await Security.GetUserById($"{Id}");
            userRoles = applicationUser.Roles.Select(role => role.Id);
            roles = await Security.GetRoles();
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                applicationUser.Roles = roles.Where(role => userRoles.Contains(role.Id)).ToList();
                var result = await Security.UpdateUser($"{Id}", applicationUser);
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