using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
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
        protected RadzenDropDownDataGrid<short> areasGrid;

        protected short AreaId { get; set; }

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(CustomerOrderActivityId, out var customerOrderActivityId))
                    throw new Exception("El Id de actividad recibido no es valido");

                customerOrderActivity = await CustomerOrderActivityService.FindAsync(customerOrderActivityId);

                customerOrderActivity.Area = await AreaService.FindAsync(customerOrderActivity.AreaId);
                customerOrderActivity.Employee = await EmployeeService.FindAsync(customerOrderActivity.EmployeeId);

                AreaId = customerOrderActivity.AreaId;

                customerOrder = await CustomerOrderService.FindAsync(customerOrderActivity.CustomerOrderId);

                areasForAREAID = await AreaService.GetAsync();
                employeesForEMPLOYEEID = await EmployeeService.GetByAreaAsync(customerOrderActivity.AreaId);

                customerOrderActivityDetails = (await CustomerOrderActivityDetailService.GetByCustomerOrderActivityIdAsync(customerOrderActivityId)).ToList();

                title = $"Modificación de actividades para el Pedido No. {customerOrder.OrderNumber}";
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

                await DialogService.Alert("Actividad guardada satisfactoriamente", "Información");
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
            if (await DialogService.Confirm("Está seguro que desea cancelar la creación de la actividad?", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-orders");
        }

        protected async Task AddCustomerOrderActivityDetail(MouseEventArgs args)
        {
            try
            {
                if (customerOrderActivity.AreaId == 0)
                    throw new Exception("No ha seleccionado el área para la actividad");

                var result = await DialogService.OpenAsync<AddCustomerOrderActivityDetail>("Agregar detalle", new Dictionary<string, object> { { "CustomerOrderActivityDetails", customerOrderActivityDetails }, { "CustomerOrderActivityAreaId", customerOrderActivity.AreaId } });

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
            if (await DialogService.Confirm("Está seguro que desea eliminar este detalle?", "Confirmar") == true)
            {
                customerOrderActivityDetails.Remove(item);

                await customerOrderActivityDetailsGrid.Reload();
            }
        }

        protected async Task EditCustomerOrderActivityDetail(MouseEventArgs args, CustomerOrderActivityDetail item)
        {
            var result = await DialogService.OpenAsync<EditCustomerOrderActivityDetail>("Modificar detalle", new Dictionary<string, object> { { "CustomerOrderActivityDetail", item }, { "CustomerOrderActivityAreaId", customerOrderActivity.AreaId }, { "CustomerOrderActivityDetails", customerOrderActivityDetails } });
            if (result == null)
                return;

            await customerOrderActivityDetailsGrid.Reload();
        }

        protected async Task OnAreaChange(object areaId)
        {
            short id = (short)areaId;

            if (!customerOrderActivityDetails.Any())
            {
                AreaId = id;
                return;
            }

            if (await DialogService.Confirm("Esta seguro que desea cambiar el área?. se borraran los detalles asociados a esta actividad.") == true)
            {
                employeesForEMPLOYEEID = new List<Employee>();

                AreaId = id;

                return;
            }

            customerOrderActivity.AreaId = AreaId;
            var area = areasForAREAID.First(i => i.AreaId == AreaId);
            await areasGrid.DataGrid.SelectRow(area, false);

            employeesForEMPLOYEEID = await EmployeeService.GetByAreaAsync(AreaId);
        }

        #endregion
    }
}