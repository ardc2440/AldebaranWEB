using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class EditCustomerOrderActivityDetail
    {
        #region Injections

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        public IActivityTypesAreaService ActivityTypesAreaService { get; set; }

        [Inject]
        public IEmployeeService EmployeeService { get; set; }

        [Inject]
        public IAreaService AreaService { get; set; }

        [Inject]
        public IActivityTypeService ActivityTypeService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public ICollection<CustomerOrderActivityDetail> CustomerOrderActivityDetails { get; set; }

        [Parameter]
        public short CustomerOrderActivityAreaId { get; set; }

        [Parameter]
        public CustomerOrderActivityDetail CustomerOrderActivityDetail { get; set; }

        #endregion

        #region Global Variables

        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;
        protected IEnumerable<Employee> employeesForACTIVITYEMPLOYEEID;
        protected IEnumerable<ActivityTypesArea> activityTypesForACTIVITY_ID;
        bool hasCUSTOMER_ORDER_ACTIVITY_IDValue;
        bool hasACTIVITY_TYPE_IDValue;
        bool hasACTIVITY_EMPLOYEE_IDValue;
        bool hasEMPLOYEE_IDValue;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                var area = await AreaService.FindAsync(CustomerOrderActivityAreaId);

                activityTypesForACTIVITY_ID = await ActivityTypesAreaService.GetByAreaAsync(area.AreaId);

                if (!activityTypesForACTIVITY_ID.Any())
                    throw new Exception("No se han definido tipos de actividad para el área seleccionada");

                employeesForACTIVITYEMPLOYEEID = await EmployeeService.GetByAreaAsync(area.AreaId);
            }
            catch (Exception ex)
            {
                alertMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            CustomerOrderActivityDetail = new CustomerOrderActivityDetail();

            hasCUSTOMER_ORDER_ACTIVITY_IDValue = parameters.TryGetValue<int>("CUSTOMER_ORDER_ACTIVITY_ID", out var hasCUSTOMER_ORDER_ACTIVITY_IDResult);

            if (hasCUSTOMER_ORDER_ACTIVITY_IDValue)
            {
                CustomerOrderActivityDetail.CustomerOrderActivityId = hasCUSTOMER_ORDER_ACTIVITY_IDResult;
            }

            hasACTIVITY_TYPE_IDValue = parameters.TryGetValue<short>("ACTIVITY_TYPE_ID", out var hasACTIVITY_IDResult);

            if (hasACTIVITY_TYPE_IDValue)
            {
                CustomerOrderActivityDetail.ActivityTypeId = hasACTIVITY_IDResult;
            }

            hasACTIVITY_EMPLOYEE_IDValue = parameters.TryGetValue<int>("ACTIVITY_EMPLOYEE_ID", out var hasACTIVITY_EMPLOYEE_IDResult);

            if (hasCUSTOMER_ORDER_ACTIVITY_IDValue)
            {
                CustomerOrderActivityDetail.ActivityEmployeeId = hasACTIVITY_EMPLOYEE_IDResult;
            }

            hasEMPLOYEE_IDValue = parameters.TryGetValue<int>("EMPLOYEE_ID", out var hasEMPLOYEE_IDResult);

            if (hasEMPLOYEE_IDValue)
            {
                CustomerOrderActivityDetail.EmployeeId = hasEMPLOYEE_IDResult;
            }

            await base.SetParametersAsync(parameters);
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if (CustomerOrderActivityDetails.Any(ad => ad.ActivityTypeId.Equals(CustomerOrderActivityDetail.ActivityTypeId) && !ad.ActivityEmployeeId.Equals(CustomerOrderActivityDetail.ActivityEmployeeId)))
                    throw new Exception("El tipo de actividad seleccionada, ya existe dentro de esta actividad.");

                var activityType = await ActivityTypeService.FindAsync(CustomerOrderActivityDetail.ActivityTypeId);
                CustomerOrderActivityDetail.ActivityType = activityType;
                DialogService.Close(CustomerOrderActivityDetail);
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

        #endregion
    }
}