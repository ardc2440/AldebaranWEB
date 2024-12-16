using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderPages.CustomerOrderActivityPages
{
    public partial class AddCustomerOrderActivity
    {
        #region Injections

        [Inject]
        protected ILogger<AddCustomerOrderActivity> Logger { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected IAreaService AreaService { get; set; }

        [Inject]
        protected ICustomerOrderActivityService CustomerOrderActivityService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public string CustomerOrderId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

        protected ICollection<CustomerOrderActivityDetail> customerOrderActivityDetails;
        protected LocalizedDataGrid<CustomerOrderActivityDetail> customerOrderActivityDetailsGrid;
        protected IEnumerable<Area> areasForAREAID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected CustomerOrderActivity customerOrderActivity;
        protected CustomerOrder customerOrder;
        protected string title;
        protected RadzenDropDownDataGrid<short> areasGrid;
        protected short AreaId { get; set; }
        protected bool IsErrorVisible;
        private bool Submitted;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                IsErrorVisible = false;

                await Task.Yield();

                if (!int.TryParse(CustomerOrderId, out var customerOrderId))
                    throw new Exception("El Id de recibido no es valido");

                customerOrder = await CustomerOrderService.FindAsync(customerOrderId);

                customerOrderActivity = new CustomerOrderActivity()
                {
                    CustomerOrderId = customerOrderId,
                };

                areasForAREAID = await AreaService.GetAsync();

                AreaId = 0;

                employeesForEMPLOYEEID = new List<Employee>();

                customerOrderActivityDetails = new List<CustomerOrderActivityDetail>();

                title = $"Actividades para el pedido No. {customerOrder.OrderNumber}";
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
                Error = ex.Message;
            }
            finally { isLoadingInProgress = false; }
        }
        #endregion

        #region Events

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task FormSubmit()
        {
            try
            {
                Submitted = true;
                IsSubmitInProgress = true;

                customerOrderActivity.CustomerOrderActivityDetails = customerOrderActivityDetails;
                await CustomerOrderActivityService.AddAsync(customerOrderActivity);

                NavigationManager.NavigateTo($"customer-orders/edit-activity/{customerOrderActivity.CustomerOrderId}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
            finally { IsSubmitInProgress = false; }
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
                Error = ex.Message;
                IsErrorVisible = true;
            }
        }

        protected async Task DeleteCustomerOrderActivityDetail(CustomerOrderActivityDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar este detalle?", "Confirmar") == true)
            {
                customerOrderActivityDetails.Remove(item);

                await customerOrderActivityDetailsGrid.Reload();
            }
        }

        protected async Task EditCustomerOrderActivityDetail(CustomerOrderActivityDetail item)
        {
            var result = await DialogService.OpenAsync<EditCustomerOrderActivityDetail>("Actualizar detalle", new Dictionary<string, object> { { "CustomerOrderActivityDetail", item }, { "CustomerOrderActivityAreaId", customerOrderActivity.AreaId }, { "CustomerOrderActivityDetails", customerOrderActivityDetails } });
            if (result == null)
                return;
            var detail = (CustomerOrderActivityDetail)result;

            customerOrderActivityDetails.Remove(item);
            customerOrderActivityDetails.Add(detail);

            await customerOrderActivityDetailsGrid.Reload();
        }

        protected async Task OnAreaChange(object areaId)
        {
            short id = (short)areaId;
            try
            {
                if (!customerOrderActivityDetails.Any())
                {
                    AreaId = id;
                    return;
                }

                if (await DialogService.Confirm("Está seguro que desea cambiar el área?. Se borrará los detalles asociados a esta actividad.") == true)
                {
                    employeesForEMPLOYEEID = new List<Employee>();

                    customerOrderActivityDetails.Clear();
                    await customerOrderActivityDetailsGrid.Reload();

                    AreaId = id;

                    return;
                }

                customerOrderActivity.AreaId = AreaId;
                var area = areasForAREAID.First(i => i.AreaId == AreaId);
                await areasGrid.DataGrid.SelectRow(area, false);
            }
            finally
            {
                employeesForEMPLOYEEID = await EmployeeService.GetByAreaAsync(AreaId);
            }
        }
        #endregion
    }
}