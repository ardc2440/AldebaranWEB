using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Competitors;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.CompetitorPages
{
    /// <summary>
    /// Crear / editar compañía de la competencia. Ubicación opcional en cascada País -> Departamento -> Ciudad:
    /// al cambiar un nivel se limpian los inferiores. Devuelve true al guardar.
    /// </summary>
    public partial class CompetitorCompanyDialog : ComponentBase
    {
        #region Injections
        [Inject]
        protected ILogger<CompetitorCompanyDialog> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICompetitorCompanyService CompetitorCompanyService { get; set; }

        [Inject]
        protected ICountryService CountryService { get; set; }

        [Inject]
        protected IDepartmentService DepartmentService { get; set; }

        [Inject]
        protected ICityService CityService { get; set; }
        #endregion

        #region Parameters
        /// <summary>null = nueva compañía.</summary>
        [Parameter]
        public int? CompetitorCompanyId { get; set; }
        #endregion

        #region Variables
        protected CompetitorCompanyInput Input = new();
        protected IEnumerable<Country> Countries = Array.Empty<Country>();
        protected IEnumerable<Department> Departments = Array.Empty<Department>();
        protected IEnumerable<City> Cities = Array.Empty<City>();
        protected List<string> Errors = new();
        protected bool IsLoading = true;
        protected bool IsSaving;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Countries = (await CountryService.GetAsync()).OrderBy(c => c.CountryName).ToList();
                if (CompetitorCompanyId != null)
                    await LoadCompanyAsync(CompetitorCompanyId.Value);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadCompanyAsync(int competitorCompanyId)
        {
            var company = await CompetitorCompanyService.FindAsync(competitorCompanyId)
                          ?? throw new KeyNotFoundException($"Compañía de la competencia con id {competitorCompanyId} no existe.");
            Input = new CompetitorCompanyInput
            {
                CompanyName = company.CompanyName,
                Website = company.Website,
                ContactPerson = company.ContactPerson,
                Phone = company.Phone,
                CountryId = company.CountryId,
                DepartmentId = company.DepartmentId,
                CityId = company.CityId
            };
            await LoadDepartmentsAsync();
            await LoadCitiesAsync();
        }

        #region Ubicación
        protected async Task OnCountryChangedAsync(int? countryId)
        {
            Input.CountryId = countryId;
            Input.DepartmentId = null;
            Input.CityId = null;
            await LoadDepartmentsAsync();
            Cities = Array.Empty<City>();
        }

        protected async Task OnDepartmentChangedAsync(int? departmentId)
        {
            Input.DepartmentId = departmentId;
            Input.CityId = null;
            await LoadCitiesAsync();
        }

        private async Task LoadDepartmentsAsync() =>
            Departments = Input.CountryId == null
                ? Array.Empty<Department>()
                : (await DepartmentService.GetGetByCountryIdAsyncAsync(Input.CountryId.Value)).OrderBy(d => d.DepartmentName).ToList();

        private async Task LoadCitiesAsync() =>
            Cities = Input.DepartmentId == null
                ? Array.Empty<City>()
                : (await CityService.GetByDepartmentIdAsync(Input.DepartmentId.Value)).OrderBy(c => c.CityName).ToList();
        #endregion

        #region Guardar
        protected async Task SaveAsync()
        {
            Errors = new List<string>();
            try
            {
                IsSaving = true;
                if (CompetitorCompanyId == null)
                    await CompetitorCompanyService.AddAsync(Input);
                else
                    await CompetitorCompanyService.UpdateAsync(CompetitorCompanyId.Value, Input);
                DialogService.Close(true);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or KeyNotFoundException)
            {
                Errors = ex.Message.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error guardando la compañía de la competencia");
                Errors = new List<string> { "No se pudo guardar la compañía. Intente de nuevo." };
            }
            finally
            {
                IsSaving = false;
            }
        }

        protected void Cancel() => DialogService.Close(null);
        #endregion
    }
}
