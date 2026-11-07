using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models.Competitors;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CompetitorPages
{
    /// <summary>Administración de compañías de la competencia: listado, crear, editar, inactivar y reactivar.</summary>
    public partial class CompetitorCompanies : ComponentBase
    {
        #region Injections
        [Inject]
        protected ILogger<CompetitorCompanies> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected ICompetitorCompanyService CompetitorCompanyService { get; set; }
        #endregion

        #region Variables
        protected RadzenDataGrid<CompetitorCompany> Grid;
        protected IReadOnlyList<CompetitorCompany> Companies = Array.Empty<CompetitorCompany>();
        protected bool IsLoading;
        protected bool IncludeInactive;
        private string _search = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync() => await LoadAsync();

        #region Consulta
        private async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                Companies = await CompetitorCompanyService.GetAsync(IncludeInactive, _search);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ReloadAsync()
        {
            await LoadAsync();
            if (Grid != null)
                await Grid.Reload();
        }

        protected async Task SearchAsync(ChangeEventArgs args)
        {
            _search = $"{args.Value}";
            if (Grid != null)
                await Grid.GoToPage(0);
            await LoadAsync();
        }

        protected async Task OnIncludeInactiveChanged(bool value)
        {
            IncludeInactive = value;
            await ReloadAsync();
        }

        /// <summary>Enlace navegable: si el sitio no trae esquema se abre con https.</summary>
        protected static string WebLink(string website) =>
            website.Contains("://", StringComparison.Ordinal) ? website : "https://" + website;
        #endregion

        #region Acciones
        protected async Task AddAsync()
        {
            if (await DialogService.OpenAsync<CompetitorCompanyDialog>("Nueva compañía de la competencia", options: new DialogOptions { Width = "800px" }) == true)
                Notify(NotificationSeverity.Success, "Compañía creada correctamente.");
            await ReloadAsync();
        }

        protected async Task EditAsync(CompetitorCompany company)
        {
            var parameters = new Dictionary<string, object> { { nameof(CompetitorCompanyDialog.CompetitorCompanyId), company.CompetitorCompanyId } };
            if (await DialogService.OpenAsync<CompetitorCompanyDialog>("Editar compañía de la competencia", parameters, new DialogOptions { Width = "800px" }) == true)
                Notify(NotificationSeverity.Success, "Compañía actualizada correctamente.");
            await ReloadAsync();
        }

        protected async Task InactivateAsync(CompetitorCompany company)
        {
            if (await DialogService.Confirm($"¿Desea inactivar la compañía \"{company.CompanyName}\"? Sus homologaciones dejarán de poder crearse con ella.",
                    "Confirmar inactivación", new ConfirmOptions { OkButtonText = "Sí", CancelButtonText = "No" }) != true)
                return;
            await RunAsync(() => CompetitorCompanyService.InactivateAsync(company.CompetitorCompanyId), "Compañía inactivada correctamente.");
        }

        protected async Task ActivateAsync(CompetitorCompany company) =>
            await RunAsync(() => CompetitorCompanyService.ActivateAsync(company.CompetitorCompanyId), "Compañía reactivada correctamente.");

        /// <summary>Ejecuta la acción, muestra el resultado y recarga. Los errores de negocio se muestran tal cual al usuario.</summary>
        private async Task RunAsync(Func<Task> action, string successMessage)
        {
            try
            {
                await action();
                Notify(NotificationSeverity.Success, successMessage);
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or KeyNotFoundException)
            {
                Notify(NotificationSeverity.Warning, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error en la administración de compañías de la competencia");
                Notify(NotificationSeverity.Error, "No se pudo completar la operación. Intente de nuevo.");
            }
            await ReloadAsync();
        }

        private void Notify(NotificationSeverity severity, string detail) =>
            NotificationService.Notify(new NotificationMessage { Severity = severity, Summary = "Compañías de la competencia", Detail = detail, Duration = 6000 });
        #endregion
    }
}
