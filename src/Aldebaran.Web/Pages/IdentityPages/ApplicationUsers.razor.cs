using Aldebaran.Web.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.IdentityPages
{
    public partial class ApplicationUsers
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
        protected TooltipService tooltipService { get; set; }

        protected IEnumerable<ApplicationUser> users;
        protected LocalizedDataGrid<ApplicationUser> ApplicationUserDataGrid;
        protected string error;
        protected bool errorVisible;
        protected DialogResult dialogResult { get; set; }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => tooltipService.Open(elementReference, content, options);
        protected override async Task OnInitializedAsync()
        {
            await GetUsers();
        }

        private async Task GetUsers()
        {
            var applicationUsers = await Security.GetUsers();
            users = applicationUsers;
        }

        protected async Task AddClick()
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddApplicationUser>("Nuevo inicio de sesión");
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Inicio de sesión creado correctamente." };
            }
            await GetUsers();
            await ApplicationUserDataGrid.Reload();
        }

        protected async Task EditRow(ApplicationUser user)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditApplicationUser>("Actualizar inicio de sesión", new Dictionary<string, object> { { "Id", user.Id } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Inicio de sesión actualizado correctamente." };
            }
            await GetUsers();
            await ApplicationUserDataGrid.Reload();
        }

        protected async Task DeleteClick(ApplicationUser user)
        {
            dialogResult = null;
            var confirm = await DialogService.Confirm("Está seguro que desea eliminar este inicio de sesión?");
            if (confirm == false)
                return;
            try
            {
                var result = await Security.DeleteUser($"{user.Id}");
                if (result != null)
                {
                    dialogResult = new DialogResult { Success = true, Message = "Inicio de sesión eliminado correctamente." };
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el inicio de sesión"
                });
            }
            finally
            {
                await GetUsers();
                await ApplicationUserDataGrid.Reload();
            }
        }

        protected async Task LockUserClick(ApplicationUser user)
        {
            var confirm = await DialogService.Confirm("Desea bloquear el ingreso de este usuario?");
            if (confirm == false)
                return;
            try
            {
                await Security.LockUser(user.Id);
                dialogResult = new DialogResult { Success = true, Message = "Usuario bloqueado correctamente." };
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido bloquear el usuario"
                });
            }
            finally
            {
                await GetUsers();
                await ApplicationUserDataGrid.Reload();
            }
        }
        protected async Task UnLockUserClick(ApplicationUser user)
        {
            var confirm = await DialogService.Confirm("Desea desbloquear el ingreso de este usuario?");
            if (confirm == false)
                return;
            try
            {
                await Security.UnlockUser(user.Id);
                dialogResult = new DialogResult { Success = true, Message = "Usuario desbloqueado correctamente." };
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido desbloquear el usuario"
                });
            }
            finally
            {
                await GetUsers();
                await ApplicationUserDataGrid.Reload();
            }
        }
    }
}