using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Shared
{
    public partial class LocalizationPicker
    {
        #region Injections
        [Inject]
        protected ICountryService CountryService { get; set; }
        [Inject]
        protected IDepartmentService DepartmentService { get; set; }
        [Inject]
        protected ICityService CityService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int? CITY_ID { get; set; }
        [Parameter]
        public EventCallback<ServiceModel.City> OnChange { get; set; }
        #endregion

        #region Variables
        public int? COUNTRY_ID { get; set; }
        public int? DEPARTMENT_ID { get; set; }

        protected IEnumerable<ServiceModel.Country> Countries;
        protected ServiceModel.Country SelectedCountry;
        protected IEnumerable<ServiceModel.Department> Departments;
        protected ServiceModel.Department SelectedDepartment;
        protected IEnumerable<ServiceModel.City> Cities;
        protected ServiceModel.City SelectedCity;
        protected bool CollapsedPanel { get; set; } = true;
        bool IsSetParametersEnabled = true;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            Countries = await CountryService.GetAsync();
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (!IsSetParametersEnabled) return;
            if (CITY_ID == null)
                return;
            var city = await CityService.FindAsync(CITY_ID.Value);
            if (city == null)
                return;
            COUNTRY_ID = city.Department.CountryId;
            await OnCountryChange(COUNTRY_ID);
            DEPARTMENT_ID = city.DepartmentId;
            await OnDepartmentChange(DEPARTMENT_ID);
            await OnCityChange(CITY_ID);
        }
        #endregion

        #region Events
        protected async Task OnCountryChange(object countryId)
        {
            if (countryId == null)
            {
                SelectedCountry = null;
                CleanDepartments();
                await CleanCities();
                return;
            }
            SelectedCountry = Countries.Single(s => s.CountryId == (int)countryId);
            Departments = await DepartmentService.GetGetByCountryIdAsyncAsync((int)countryId);
        }
        protected async Task OnDepartmentChange(object departmentId)
        {
            if (departmentId == null)
            {
                SelectedDepartment = null;
                await CleanCities();
                return;
            }
            SelectedDepartment = Departments.Single(s => s.DepartmentId == (int)departmentId);
            Cities = await CityService.GetByDepartmentIdAsync((int)departmentId);
        }
        protected async Task OnCityChange(object cityId)
        {
            if (cityId == null)
            {
                SelectedCity = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            SelectedCity = Cities.Single(s => s.CityId == (int)cityId);
            CollapsedPanel = true;
            IsSetParametersEnabled = false;
            await OnChange.InvokeAsync(SelectedCity);
        }
        protected async Task PanelCollapseToggle(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            CollapsedPanel = !CollapsedPanel;
        }
        void PanelCollapseChange(string Command)
        {
            if (Command == "Expand")
                CollapsedPanel = false;
            if (Command == "Collapse")
                CollapsedPanel = true;
        }
        void CleanDepartments()
        {
            SelectedDepartment = null;
            Departments = null;
        }
        async Task CleanCities()
        {
            CITY_ID = null;
            SelectedCity = null;
            Cities = null;
            await OnChange.InvokeAsync(null);
        }
        #endregion
    }
}