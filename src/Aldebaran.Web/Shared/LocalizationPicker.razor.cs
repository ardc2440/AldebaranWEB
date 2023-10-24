using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Shared
{
    public partial class LocalizationPicker
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
        public AldebaranDbService AldebaranDbService { get; set; }

        public int? COUNTRY_ID { get; set; }
        public int? DEPARTMENT_ID { get; set; }

        [Parameter]
        public int? CITY_ID { get; set; }
        [Parameter]
        public EventCallback<Models.AldebaranDb.City> OnChange { get; set; }

        protected IEnumerable<Models.AldebaranDb.Country> countries;
        protected Models.AldebaranDb.Country country;
        protected IEnumerable<Models.AldebaranDb.Department> departments;
        protected Models.AldebaranDb.Department department;
        protected IEnumerable<Models.AldebaranDb.City> cities;
        protected Models.AldebaranDb.City city;
        protected override async Task OnInitializedAsync()
        {
            countries = await AldebaranDbService.GetCountries();
        }
        protected bool CollapsedPanel { get; set; } = true;
        protected async Task OnCountryChange(object countryId)
        {
            if (countryId == null)
            {
                country = null;
                CleanDepartments();
                await CleanCities();
                return;
            }
            country = countries.Single(s => s.COUNTRY_ID == (int)countryId);
            departments = await AldebaranDbService.GetDepartments(new Query { Filter = $"i=>i.COUNTRY_ID==@0", FilterParameters = new object[] { countryId } });
        }

        protected async Task OnDepartmentChange(object departmentId)
        {
            if (departmentId == null)
            {
                department = null;
                await CleanCities();
                return;
            }
            department = departments.Single(s => s.DEPARTMENT_ID == (int)departmentId);
            cities = await AldebaranDbService.GetCities(new Query { Filter = $"i=>i.DEPARTMENT_ID==@0", FilterParameters = new object[] { departmentId } });
        }
        protected async Task OnCityChange(object cityId)
        {
            if (cityId == null)
            {
                city = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            city = cities.Single(s => s.CITY_ID == (int)cityId);
            CollapsedPanel = true;
            await OnChange.InvokeAsync(city);
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
            department = null;
            departments = null;
        }
        async Task CleanCities()
        {
            CITY_ID = null;
            city = null;
            cities = null;
            await OnChange.InvokeAsync(null);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (CITY_ID == null)
                return;
            var selectedCity = await AldebaranDbService.GetCities(new Query { Filter = "i=>i.CITY_ID == @0", FilterParameters = new object[] { CITY_ID }, Expand = "Department.Country" });
            if (!selectedCity.Any())
                return;
            city = selectedCity.First();
            COUNTRY_ID = city.Department.COUNTRY_ID;
            await OnCountryChange(COUNTRY_ID);
            country = city.Department.Country;
            DEPARTMENT_ID = city.DEPARTMENT_ID;
            await OnDepartmentChange(DEPARTMENT_ID);
            department = city.Department;
        }
    }
}