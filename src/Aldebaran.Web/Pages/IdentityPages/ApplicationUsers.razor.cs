using Aldebaran.Web.Models;
using Aldebaran.Web.Models.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

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
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService tooltipService { get; set; }

        protected IEnumerable<UserViewModel> users;
        protected RadzenDataGrid<UserViewModel> grid0;
        protected string error;
        protected bool errorVisible;
        protected DialogResult dialogResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetUsers();
        }

        private async Task GetUsers()
        {
            var applicationUsers = await Security.GetUsers();
            var allUsers = await AldebaranDbService.GetUsers(new Query { Expand = "IdentityType,Area" });
            users = applicationUsers
            .Select(appUser =>
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
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => tooltipService.Open(elementReference, content, options);
        protected async Task AddClick()
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddApplicationUser>("Nuevo usuario");
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Usuario creado correctamente." };
            }
            await GetUsers();
            await grid0.Reload();
        }

        protected async Task EditRow(UserViewModel user)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditApplicationUser>("Actualizar Usuario", new Dictionary<string, object> { { "Id", user.Id } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Usuario actualizado correctamente." };
            }
            await GetUsers();
            await grid0.Reload();
        }

        protected async Task DeleteClick(UserViewModel user)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este usuario?") == true)
                {
                    var u = await AldebaranDbService.GetUsers(new Query { Filter = "i=>i.LOGIN_USER_ID==@0", FilterParameters = new object[] { user.Id } });
                    var deleteResult = await AldebaranDbService.DeleteUser(u.Single().USER_ID);
                    var deleteSecurityResult = await Security.DeleteUser($"{user.Id}");
                    if (deleteResult != null && deleteSecurityResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Usuario eliminado correctamente." };
                        await GetUsers();
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el usuario"
                });
            }
        }

        protected async Task LockUserClick(UserViewModel user)
        {
            try
            {
                if (await DialogService.Confirm("Desea bloquear el ingreso de este usuario?") == true)
                {
                    await Security.LockUser(user.Id);
                    dialogResult = new DialogResult { Success = true, Message = "Usuario bloqueado correctamente." };
                    await GetUsers();
                    await grid0.Reload();
                }
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
        }
        protected async Task UnLockUserClick(UserViewModel user)
        {
            try
            {
                if (await DialogService.Confirm("Desea desbloquear el ingreso de este usuario?") == true)
                {
                    await Security.UnlockUser(user.Id);
                    dialogResult = new DialogResult { Success = true, Message = "Usuario desbloqueado correctamente." };
                    await GetUsers();
                    await grid0.Reload();
                }
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
        }
    }
}