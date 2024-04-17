using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Shared
{
    public partial class MainLayout
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
        protected IEmployeeService EmployeeService { get; set; }

        private bool sidebarExpanded = true;
        public Employee LoggedEmployee { get; set; }
        [Inject]
        protected SecurityService Security { get; set; }

        void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        protected override async Task OnInitializedAsync()
        {
            LoggedEmployee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
        }
        protected void ProfileMenuClick(RadzenProfileMenuItem args)
        {
            if (args.Value == "Logout")
            {
                Security.Logout();
            }
        }
    }
}
