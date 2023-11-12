using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Shared
{
    public partial class EmployeePicker
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

        [Parameter]
        public int? EMPLOYEE_ID { get; set; }

        [Parameter]
        public EventCallback<Models.AldebaranDb.Employee> OnChange { get; set; }

        protected bool CollapsedPanel { get; set; } = true;
        public short? AREA_ID { get; set; }
        protected IEnumerable<Models.AldebaranDb.Area> areas;
        protected IEnumerable<Models.AldebaranDb.Employee> employees;
        protected Models.AldebaranDb.Employee employee;
        protected override async Task OnInitializedAsync()
        {
            areas = await AldebaranDbService.GetAreas();
            EMPLOYEE_ID = null;
        }
        protected async Task OnAreaChange(object areaId)
        {
            EMPLOYEE_ID = null;
            if (areaId == null)
            {
                employee = null;
                employees = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            employees = await AldebaranDbService.GetEmployees(new Query { Filter = $"i=>i.AREA_ID==@0", FilterParameters = new object[] { areaId }, Expand = "Area" });
        }
        protected async Task OnEmployeeChange(object employeeId)
        {
            if (employeeId == null)
            {
                employee = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            employee = employees.Single(s => s.EMPLOYEE_ID == (int)employeeId);
            CollapsedPanel = true;
            await OnChange.InvokeAsync(employee);
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
    }
}