using Aldebaran.Application.Services;
using Aldebaran.Web.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.IdentityPages
{
    public partial class ApplicationUsers
    {
        #region Injections
        [Inject]
        protected ILogger<ApplicationUsers> Logger { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }
        #endregion

        #region Variables
        protected IEnumerable<ApplicationUser> ApplicationUser;
        protected LocalizedDataGrid<ApplicationUser> ApplicationUserDataGrid;
        protected bool IsErrorVisible;
        protected bool isLoadingInProgress;
        protected string search = "";
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await GetUsers();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ApplicationUserDataGrid.GoToPage(0);
            await GetUsers(search);
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);
        private async Task GetUsers(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            ApplicationUser = string.IsNullOrEmpty(searchKey) ? await Security.GetUsers(ct) : await Security.GetUsers(searchKey,ct) ;
        }
        protected async Task AddApplicationUser()
        {
            var result = await DialogService.OpenAsync<AddApplicationUser>("Nuevo inicio de sesión");
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Inicio de sesión",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Inicio de sesión creado correctamente"
                });
            }
            await GetUsers(search);
            await ApplicationUserDataGrid.Reload();
        }
        protected async Task EditApplicationUser(ApplicationUser user)
        {
            var result = await DialogService.OpenAsync<EditApplicationUser>("Actualizar inicio de sesión", new Dictionary<string, object> { { "Id", user.Id } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Inicio de sesión",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Inicio de sesión actualizado correctamente"
                });
            }
            await GetUsers(search);
            await ApplicationUserDataGrid.Reload();
        }
        protected async Task DeleteApplicationUser(ApplicationUser user)
        {
            try
            {
                var confirm = await DialogService.Confirm("Está seguro que desea eliminar este inicio de sesión?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación");
                if (confirm == false)
                    return;

                // Verificar que el inicio de sesion no esta asociado a un funcionario
                var employee = await EmployeeService.FindByLoginUserIdAsync(user.Id);
                if (employee != null)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Inicio de sesión",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Inicio de sesión pertenece a un funcionario, elimine primero el funcionario \"({employee.Position}) - {employee.FullName}\" asociado al inicio de sesión y posteriormente proceda con la eliminación del inicio de sesión."
                    });
                    return;
                }

                var result = await Security.DeleteUser($"{user.Id}");
                if (result != null)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Inicio de sesión",
                        Severity = NotificationSeverity.Success,
                        Detail = "Inicio de sesión eliminado correctamente."
                    });
                }
                await GetUsers(search);
                await ApplicationUserDataGrid.Reload();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteApplicationUser));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el inicio de sesión."
                });
            }
        }
        protected async Task LockApplicationUser(ApplicationUser user)
        {
            try
            {
                var confirm = await DialogService.Confirm("Desea bloquear el ingreso de este usuario?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación");
                if (confirm == false)
                    return;

                await Security.LockUser(user.Id);
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Inicio de sesión",
                    Severity = NotificationSeverity.Success,
                    Detail = "Usuario bloqueado correctamente."
                });
                await GetUsers(search);
                await ApplicationUserDataGrid.Reload();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(LockApplicationUser));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido bloquear el usuario."
                });
            }
        }
        protected async Task UnLockApplicationUser(ApplicationUser user)
        {
            try
            {
                var confirm = await DialogService.Confirm("Desea desbloquear el ingreso de este usuario?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación");
                if (confirm == false)
                    return;

                await Security.UnlockUser(user.Id);
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Inicio de sesión",
                    Severity = NotificationSeverity.Success,
                    Detail = "Usuario desbloqueado correctamente."
                });
                await GetUsers(search);
                await ApplicationUserDataGrid.Reload();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UnLockApplicationUser));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido desbloquear el usuario."
                });
            }
        }
        #endregion
    }
}