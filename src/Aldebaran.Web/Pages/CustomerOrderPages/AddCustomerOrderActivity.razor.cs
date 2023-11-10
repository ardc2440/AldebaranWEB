using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class AddCustomerOrderActivity
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

        protected bool errorVisible;
        protected string errorMessage;
        protected ICollection<CustomerOrderActivityDetail> customerOrderActivityDetails;
        protected RadzenDataGrid<CustomerOrderActivityDetail> customerOrderActivityDetailsGrid;

        protected IEnumerable<Area> areasForAREAID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;

        protected CustomerOrderActivity customerOrderActivity;
        protected CustomerOrder customerOrder;
        protected string title;

        [Parameter]
        public string pCustomerOrderId { get; set; } = "NoParamInput";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(pCustomerOrderId, out var customerOrderId))
                    throw new Exception("El Id de recibido no es valido");

                customerOrder = await AldebaranDbService.GetCustomerOrdersById(customerOrderId);

                customerOrderActivity = new CustomerOrderActivity()
                {
                    CUSTOMER_ORDER_ID = customerOrderId,
                };

                areasForAREAID = await AldebaranDbService.GetAreas();
                employeesForEMPLOYEEID = new List<Employee>();

                customerOrderActivityDetails = new List<CustomerOrderActivityDetail>();

                title = $"Actividades para el Pedido No. {customerOrder.ORDER_NUMBER}";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;

                customerOrderActivity.CustomerOrderActivityDetails = customerOrderActivityDetails;
                await AldebaranDbService.CreateCustomerOrderActivity(customerOrderActivity);

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
                if (customerOrderActivity.AREA_ID == 0)
                    throw new Exception("No ha seleccionado el Area para la Actividad");

                var result = await DialogService.OpenAsync<AddCustomerOrderActivityDetail>("Nuevo Tipo de Actividad", new Dictionary<string, object> { { "customerOrderActivityDetails", customerOrderActivityDetails }, { "customerOrderActivityAreaId", customerOrderActivity.AREA_ID } });

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
            var result = await DialogService.OpenAsync<EditCustomerOrderActivityDetail>("Actualizar Tipo e Actividad", new Dictionary<string, object> { { "customerOrderActivityDetail", args }, { "customerOrderActivityAreaId", customerOrderActivity.AREA_ID }, { "customerOrderActivityDetails", customerOrderActivityDetails } });
            if (result == null)
                return;
            var detail = (CustomerOrderActivityDetail)result;

            customerOrderActivityDetails.Remove(args);
            customerOrderActivityDetails.Add(detail);

            await customerOrderActivityDetailsGrid.Reload();
        }

        protected async Task OnAreaChange(object areaId)
        {

            if ((customerOrderActivityDetails.Any()) && (!await DialogService.Confirm("Esta seguro que desea cambiar el área, se borrara el detalle de Tipos de Actividad asociado a esta actividad?") == true))
            {
                areaId = customerOrderActivity.AREA_ID;
                return;
            }

            if (areaId == null)
            {
                employeesForEMPLOYEEID = new List<Employee>();
                return;
            }

            customerOrderActivityDetails.Clear();
            await customerOrderActivityDetailsGrid.Reload();
            employeesForEMPLOYEEID = await AldebaranDbService.GetEmployees(new Query { Filter = $"i=>i.AREA_ID==@0", FilterParameters = new object[] { areaId } });
        }
    }
}