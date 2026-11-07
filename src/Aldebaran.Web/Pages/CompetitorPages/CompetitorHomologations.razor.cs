using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models.Competitors;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CompetitorPages
{
    /// <summary>Administración de homologaciones con la competencia: listado filtrable, crear, editar, inactivar y reactivar.</summary>
    public partial class CompetitorHomologations : ComponentBase
    {
        #region Injections
        [Inject]
        protected ILogger<CompetitorHomologations> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected ICompetitorHomologationService CompetitorHomologationService { get; set; }

        [Inject]
        protected ICompetitorCompanyService CompetitorCompanyService { get; set; }
        #endregion

        #region Variables
        protected RadzenDataGrid<CompetitorHomologation> Grid;
        protected IReadOnlyList<CompetitorHomologation> Homologations = Array.Empty<CompetitorHomologation>();
        /// <summary>Todas las compañías (también inactivas) para poder filtrar homologaciones antiguas.</summary>
        protected IReadOnlyList<CompetitorCompany> Companies = Array.Empty<CompetitorCompany>();
        protected int? CompanyFilter;
        protected bool IncludeInactive;
        protected bool IsLoading;
        private string _search = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Companies = await CompetitorCompanyService.GetAsync(includeInactive: true);
            await LoadAsync();
        }

        #region Consulta
        private async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                Homologations = await CompetitorHomologationService.GetAsync(new CompetitorHomologationFilter
                {
                    CompetitorCompanyId = CompanyFilter,
                    IncludeInactive = IncludeInactive,
                    SearchKey = _search
                });
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

        protected async Task OnCompanyFilterChanged(int? companyId)
        {
            CompanyFilter = companyId;
            await ReloadAsync();
        }

        protected async Task OnIncludeInactiveChanged(bool value)
        {
            IncludeInactive = value;
            await ReloadAsync();
        }
        #endregion

        #region Acciones
        protected async Task AddAsync()
        {
            var parameters = new Dictionary<string, object>();
            if (CompanyFilter != null)
                parameters.Add(nameof(CompetitorHomologationDialog.DefaultCompetitorCompanyId), CompanyFilter.Value);
            if (await DialogService.OpenAsync<CompetitorHomologationDialog>("Nueva homologación con la competencia", parameters, new DialogOptions { Width = "900px" }) == true)
                Notify(NotificationSeverity.Success, "Homologación creada correctamente.");
            await ReloadAsync();
        }

        protected async Task EditAsync(CompetitorHomologation homologation)
        {
            var parameters = new Dictionary<string, object> { { nameof(CompetitorHomologationDialog.CompetitorHomologationId), homologation.CompetitorHomologationId } };
            if (await DialogService.OpenAsync<CompetitorHomologationDialog>("Editar homologación con la competencia", parameters, new DialogOptions { Width = "900px" }) == true)
                Notify(NotificationSeverity.Success, "Homologación actualizada correctamente.");
            await ReloadAsync();
        }

        protected async Task InactivateAsync(CompetitorHomologation homologation)
        {
            if (await DialogService.Confirm($"¿Desea inactivar la homologación de {homologation.PromosDescription} con \"{homologation.CompetitorReference}\" de {homologation.CompanyName}?",
                    "Confirmar inactivación", new ConfirmOptions { OkButtonText = "Sí", CancelButtonText = "No" }) != true)
                return;
            await RunAsync(() => CompetitorHomologationService.InactivateAsync(homologation.CompetitorHomologationId), "Homologación inactivada correctamente.");
        }

        protected async Task ActivateAsync(CompetitorHomologation homologation) =>
            await RunAsync(() => CompetitorHomologationService.ActivateAsync(homologation.CompetitorHomologationId), "Homologación reactivada correctamente.");

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
                Logger.LogError(ex, "Error en la administración de homologaciones con la competencia");
                Notify(NotificationSeverity.Error, "No se pudo completar la operación. Intente de nuevo.");
            }
            await ReloadAsync();
        }

        private void Notify(NotificationSeverity severity, string detail) =>
            NotificationService.Notify(new NotificationMessage { Severity = severity, Summary = "Homologaciones con la competencia", Detail = detail, Duration = 6000 });
        #endregion
    }
}
