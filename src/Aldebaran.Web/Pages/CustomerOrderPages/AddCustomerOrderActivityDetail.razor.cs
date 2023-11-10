using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class AddCustomerOrderActivityDetail
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
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Parameter]
        public ICollection<CustomerOrderActivityDetail> customerOrderActivityDetails { get; set; }

        [Parameter]
        public short customerOrderActivityAreaId { get; set; }

        [Parameter]
        public int CUSTOMER_ORDER_ACTIVITY_ID { get; set; }

        [Parameter]
        public short ACTIVITY_ID { get; set; }

        [Parameter]
        public int ACTIVITY_EMPLOYEE_ID { get; set; }

        [Parameter]
        public int EMPLOYEE_ID { get; set; }

        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;

        protected CustomerOrderActivityDetail customerOrderActivityDetail;

        protected IEnumerable<Employee> employeesForACTIVITYEMPLOYEEID;
        protected IEnumerable<ActivityTypeArea> activityTypesForACTIVITY_ID;

        bool hasCUSTOMER_ORDER_ACTIVITY_IDValue;
        bool hasACTIVITY_TYPE_IDValue;
        bool hasACTIVITY_EMPLOYEE_IDValue;
        bool hasEMPLOYEE_IDValue;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                var area = await AldebaranDbService.GetAreaByAreaId(customerOrderActivityAreaId);

                activityTypesForACTIVITY_ID = await AldebaranDbService.GetActivityTypesArea(new Query { Filter = $"i=>i.AREA_ID==@0", FilterParameters = new object[] { area.AREA_ID }, Expand = "ActivityType" });

                if (!activityTypesForACTIVITY_ID.Any())
                    throw new Exception("No se han definido tipos de actividad para el área seleccionada");

                employeesForACTIVITYEMPLOYEEID = await AldebaranDbService.GetEmployees(new Query { Filter = $"i=>i.AREA_ID==@0", FilterParameters = new object[] { area.AREA_ID } });
            }
            catch (Exception ex)
            {
                alertMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if (customerOrderActivityDetails.Any(ad => ad.ACTIVITY_TYPE_ID.Equals(customerOrderActivityDetail.ACTIVITY_TYPE_ID)))
                    throw new Exception("El tipo de actividad seleccionada, ya existe dentro de esta actividad del pedido.");

                var activityType = await AldebaranDbService.GetActivityTypeById(customerOrderActivityDetail.ACTIVITY_TYPE_ID);
                var employeeActivity = await AldebaranDbService.GetEmployeeByEmployeeId(customerOrderActivityDetail.ACTIVITY_EMPLOYEE_ID);
                var employee = await AldebaranDbService.GetLoggedEmployee(Security);

                customerOrderActivityDetail.ActivityType = activityType;
                customerOrderActivityDetail.EmployeeActivity = employeeActivity;
                customerOrderActivityDetail.Employee = employee;
                customerOrderActivityDetail.EMPLOYEE_ID = employee.EMPLOYEE_ID;

                DialogService.Close(customerOrderActivityDetail);
            }
            catch (Exception ex)
            {
                alertMessage = ex.Message;
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            customerOrderActivityDetail = new CustomerOrderActivityDetail();

            hasCUSTOMER_ORDER_ACTIVITY_IDValue = parameters.TryGetValue<int>("CUSTOMER_ORDER_ACTIVITY_ID", out var hasCUSTOMER_ORDER_ACTIVITY_IDResult);

            if (hasCUSTOMER_ORDER_ACTIVITY_IDValue)
            {
                customerOrderActivityDetail.CUSTOMER_ORDER_ACTIVITY_ID = hasCUSTOMER_ORDER_ACTIVITY_IDResult;
            }

            hasACTIVITY_TYPE_IDValue = parameters.TryGetValue<short>("ACTIVITY_TYPE_ID", out var hasACTIVITY_IDResult);

            if (hasACTIVITY_TYPE_IDValue)
            {
                customerOrderActivityDetail.ACTIVITY_TYPE_ID = hasACTIVITY_IDResult;
            }

            hasACTIVITY_EMPLOYEE_IDValue = parameters.TryGetValue<int>("ACTIVITY_EMPLOYEE_ID", out var hasACTIVITY_EMPLOYEE_IDResult);

            if (hasCUSTOMER_ORDER_ACTIVITY_IDValue)
            {
                customerOrderActivityDetail.ACTIVITY_EMPLOYEE_ID = hasACTIVITY_EMPLOYEE_IDResult;
            }

            hasEMPLOYEE_IDValue = parameters.TryGetValue<int>("EMPLOYEE_ID", out var hasEMPLOYEE_IDResult);

            if (hasEMPLOYEE_IDValue)
            {
                customerOrderActivityDetail.EMPLOYEE_ID = hasEMPLOYEE_IDResult;
            }

            await base.SetParametersAsync(parameters);
        }

    }
}