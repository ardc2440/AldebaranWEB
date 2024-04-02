using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Shared
{
    public partial class EmployeePicker
    {
        #region Injections
        [Inject]
        public IAreaService AreaService { get; set; }
        [Inject]
        protected IEmployeeService EmployeeService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int? EMPLOYEE_ID { get; set; }

        [Parameter]
        public EventCallback<ServiceModel.Employee> OnChange { get; set; }
        #endregion

        #region Variables
        protected bool CollapsedPanel { get; set; } = true;
        public short? AREA_ID { get; set; }
        protected IEnumerable<ServiceModel.Area> Areas = new List<ServiceModel.Area>();
        protected IEnumerable<ServiceModel.Employee> Employees = new List<ServiceModel.Employee>();
        protected ServiceModel.Employee Employee;
        bool IsSetParametersEnabled = true;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            Areas = await AreaService.GetAsync();
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (!IsSetParametersEnabled) return;
            if (EMPLOYEE_ID == null)
                return;
            var selectedEmployee = await EmployeeService.FindAsync(EMPLOYEE_ID.Value);
            if (selectedEmployee == null)
                return;
            AREA_ID = selectedEmployee.AreaId;
            await OnAreaChange(selectedEmployee.AreaId);
            await OnEmployeeChange(selectedEmployee.EmployeeId);
        }
        #endregion

        #region Events
        protected async Task OnAreaChange(object areaId)
        {
            EMPLOYEE_ID = null;
            await OnEmployeeChange(null);
            if (areaId == null)
            {
                Employee = null;
                Employees = new List<ServiceModel.Employee>();
                await OnChange.InvokeAsync(null);
                return;
            }
            Employees = await EmployeeService.GetByAreaAsync((short)areaId);
        }
        protected async Task OnEmployeeChange(object employeeId)
        {
            if (employeeId == null)
            {
                Employee = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            Employee = Employees.Single(s => s.EmployeeId == (int)employeeId);
            CollapsedPanel = true;
            IsSetParametersEnabled = false;
            await OnChange.InvokeAsync(Employee);
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
        #endregion

    }
}