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

        public Employee LoggedEmployee { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IJSRuntime JS { get; set; } = null!;

        private static bool sidebarExpanded = true;
        private bool EnableNotifications;

        static void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        protected override async Task OnInitializedAsync()
        {
            LoggedEmployee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);

            var preference = await EmployeeService.FindPreferenceAsync(LoggedEmployee.EmployeeId);

            EnableNotifications = preference?.EnableNotifications ?? false;
        }

        protected void ProfileMenuClick(RadzenProfileMenuItem args)
        {
            if (args.Value == "Logout")
            {
                Security.Logout();
            }
        }

        private async Task OnEnableNotificationsChanged(bool enabled)
        {
            var preference = await EmployeeService.FindPreferenceAsync(LoggedEmployee.EmployeeId);

            if (!enabled)
            {
                EnableNotifications = false;

                if (preference == null)
                    await EmployeeService.CreatePreferenceAsync(LoggedEmployee.EmployeeId, false);
                else
                    await EmployeeService.UpdatePreferenceAsync(LoggedEmployee.EmployeeId, false);

                return;
            }

            var permission = await JS.InvokeAsync<bool>("aldebaranNotifications.requestPermission");

            if (permission)
            {
                EnableNotifications = true;

                if (preference == null)
                    await EmployeeService.CreatePreferenceAsync(LoggedEmployee.EmployeeId, true);
                else
                    await EmployeeService.UpdatePreferenceAsync(LoggedEmployee.EmployeeId, true);
            }
            else
            {
                EnableNotifications = false;

                if (preference == null)
                    await EmployeeService.CreatePreferenceAsync(LoggedEmployee.EmployeeId, false);
                else
                    await EmployeeService.UpdatePreferenceAsync(LoggedEmployee.EmployeeId, false);

                StateHasChanged();
            }
        }
    }
}
