using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Linq.Dynamic.Core;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class EditCustomerOrderActivity
    {
        #region Injections

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICustomerOrderActivityService CustomerOrderActivityService { get; set; }

        [Inject]
        protected IAreaService AreaService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected ICustomerOrderActivityDetailService CustomerOrderActivityDetailService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CustomerOrderActivityId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

        protected bool errorVisible;
        protected string errorMessage;
        protected ICollection<CustomerOrderActivityDetail> customerOrderActivityDetails;
        protected LocalizedDataGrid<CustomerOrderActivityDetail> customerOrderActivityDetailsGrid;
        protected IEnumerable<Area> areasForAREAID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;
        protected CustomerOrderActivity customerOrderActivity;
        protected CustomerOrder customerOrder;
        protected string title;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(CustomerOrderActivityId, out var customerOrderActivityId))
                    throw new Exception("El Id de Referencia recibido no es valido");

                customerOrderActivity = await CustomerOrderActivityService.FindAsync(customerOrderActivityId);

                customerOrderActivity.Area = await AreaService.FindAsync(customerOrderActivity.AreaId);
                customerOrderActivity.Employee = await EmployeeService.FindAsync(customerOrderActivity.EmployeeId);

                customerOrder = await CustomerOrderService.FindAsync(customerOrderActivity.CustomerOrderId);

                areasForAREAID = await AreaService.GetAsync();
                employeesForEMPLOYEEID = await EmployeeService.GetByAreaAsync(customerOrderActivity.AreaId);

                customerOrderActivityDetails = (await CustomerOrderActivityDetailService.GetByCustomerOrderActivityIdAsync(customerOrderActivityId)).ToList();

                title = $"Modificación de Actividades para el Pedido No. {customerOrder.OrderNumber}";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;

                customerOrderActivity.CustomerOrderActivityDetails = customerOrderActivityDetails;
                await CustomerOrderActivityService.UpdateAsync(customerOrderActivity.CustomerOrderActivityId, customerOrderActivity);

                await DialogService.Alert("Actividad Guardada Satisfactoriamente", "Información");
                NavigationManager.NavigateTo("customer-orders");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que cancelar la creacion de la Actividad??", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-orders");
        }

        protected async Task AddCustomerOrderActivityDetail(MouseEventArgs args)
        {
            try
            {
                if (customerOrderActivity.AreaId == 0)
                    throw new Exception("No ha seleccionado el Area para la Actividad");

                var result = await DialogService.OpenAsync<AddCustomerOrderActivityDetail>("Nuevo Tipo de Actividad", new Dictionary<string, object> { { "CustomerOrderActivityDetails", customerOrderActivityDetails }, { "CustomerOrderActivityAreaId", customerOrderActivity.AreaId } });

                if (result == null)
                    return;

                var detail = (CustomerOrderActivityDetail)result;

                customerOrderActivityDetails.Add(detail);

                await customerOrderActivityDetailsGrid.Reload();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        protected async Task DeleteCustomerOrderActivityDetail(MouseEventArgs args, CustomerOrderActivityDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar este Tipo de Actividad?", "Confirmar") == true)
            {
                customerOrderActivityDetails.Remove(item);

                await customerOrderActivityDetailsGrid.Reload();
            }
        }

        protected async Task EditCustomerOrderActivityDetail(CustomerOrderActivityDetail args)
        {
            var result = await DialogService.OpenAsync<EditCustomerOrderActivityDetail>("Actualizar Tipo e Actividad", new Dictionary<string, object> { { "CustomerOrderActivityDetail", args }, { "CustomerOrderActivityAreaId", customerOrderActivity.AreaId }, { "CustomerOrderActivityDetails", customerOrderActivityDetails } });
            if (result == null)
                return;

            await customerOrderActivityDetailsGrid.Reload();
        }

        protected async Task OnAreaChange(object areaId)
        {

            if ((customerOrderActivityDetails.Any()) && (!await DialogService.Confirm("Esta seguro que desea cambiar el área, se borrara el detalle de Tipos de Actividad asociado a esta actividad?") == true))
                return;

            if (areaId == null)
            {
                employeesForEMPLOYEEID = new List<Employee>();
                return;
            }

            customerOrderActivityDetails.Clear();
            await customerOrderActivityDetailsGrid.Reload();
            employeesForEMPLOYEEID = await EmployeeService.GetByAreaAsync((short)areaId);
        }

        #endregion
    }
}