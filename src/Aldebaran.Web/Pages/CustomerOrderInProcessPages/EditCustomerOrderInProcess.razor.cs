using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderInProcessPages
{
    public partial class EditCustomerOrderInProcess
    {
        #region Injections

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected ICustomerOrdersInProcessService CustomerOrdersInProcessService { get; set; }

        [Inject]
        protected ICustomerOrderInProcessDetailService CustomerOrderInProcessDetailService { get; set; }

        [Inject]
        protected IProcessSatelliteService ProcessSatelliteService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        #endregion

        #region Params

        [Parameter]
        public string CustomerOrderInProcessId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

        protected bool errorVisible;
        protected string errorMessage;
        protected CustomerOrder customerOrder;
        protected CustomerOrdersInProcess customerOrderInProcess;
        protected DocumentType documentType;
        protected ICollection<DetailInProcess> detailsInProcess;
        protected LocalizedDataGrid<DetailInProcess> customerOrderDetailGrid;
        protected IEnumerable<Employee> employeesFOREMPLOYEEID;
        protected IEnumerable<ProcessSatellite> processSatellitesFORPROCESSSATELLITEID;
        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string title;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(CustomerOrderInProcessId, out var customerOrderInProcessId))
                    throw new Exception("El Id de Traslado recibido no es valido");

                documentType = await DocumentTypeService.FindByCodeAsync("T");

                customerOrderInProcess = await CustomerOrdersInProcessService.FindAsync(customerOrderInProcessId);

                customerOrder = await CustomerOrderService.FindAsync(customerOrderInProcess.CustomerOrderId);

                detailsInProcess = await GetDetailsInProcess(customerOrderInProcess);

                processSatellitesFORPROCESSSATELLITEID = await ProcessSatelliteService.GetAsync();
                employeesFOREMPLOYEEID = await EmployeeService.GetAsync();

                title = $"Modificación del Traslado a Proceso para el Pedido No. {customerOrder.OrderNumber}";
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

        protected async Task<List<DetailInProcess>> GetDetailsInProcess(CustomerOrdersInProcess customerOrderInProcess) => (from item in await CustomerOrderInProcessDetailService.GetAsync(customerOrderInProcess.CustomerOrderInProcessId) ?? throw new ArgumentException("The references of Customer Order In Process, could not be obtained.")
                                                                                                                            let viewOrderDetail = new DetailInProcess()
                                                                                                                            {
                                                                                                                                REFERENCE_ID = item.CustomerOrderDetail.ReferenceId,
                                                                                                                                CUSTOMER_ORDER_DETAIL_ID = item.CustomerOrderDetailId,
                                                                                                                                REFERENCE_DESCRIPTION = $"({item.CustomerOrderDetail.ItemReference.Item.InternalReference}) {item.CustomerOrderDetail.ItemReference.Item.ItemName} - {item.CustomerOrderDetail.ItemReference.ReferenceName}",
                                                                                                                                PENDING_QUANTITY = item.CustomerOrderDetail.RequestedQuantity - item.CustomerOrderDetail.ProcessedQuantity - item.CustomerOrderDetail.DeliveredQuantity,
                                                                                                                                PROCESSED_QUANTITY = item.CustomerOrderDetail.ProcessedQuantity,
                                                                                                                                DELIVERED_QUANTITY = item.CustomerOrderDetail.DeliveredQuantity,
                                                                                                                                BRAND = item.Brand,
                                                                                                                                WAREHOUSE_ID = item.WarehouseId,
                                                                                                                                THIS_QUANTITY = item.ProcessedQuantity
                                                                                                                            }
                                                                                                                            select viewOrderDetail).ToList();

        protected async Task SendToProcess(DetailInProcess args)
        {
            if (await DialogService.OpenAsync<SetQuantityInProcess>("Cantidad a Trasladar", new Dictionary<string, object> { { "DetailInProcess", args } }) != null)
                await customerOrderDetailGrid.Reload();
        }

        protected async Task CancelToProcess(DetailInProcess args)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") != true)
            {
                return;
            }

            args.PENDING_QUANTITY = args.PENDING_QUANTITY + args.THIS_QUANTITY;
            args.THIS_QUANTITY = 0;

            await customerOrderDetailGrid.Reload();
        }

        protected bool CanCancel(DetailInProcess detailInProcess) => detailInProcess.THIS_QUANTITY > 0 && Security.IsInRole("Admin", "Customer Order In Process Editor");

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;

                if (!detailsInProcess.Any(x => x.THIS_QUANTITY > 0))
                    throw new Exception("No ha ingresado ninguna cantidad a trasladar");

                customerOrderInProcess.CustomerOrderInProcessDetails = await MapDetailsInProcess(detailsInProcess);

                await CustomerOrdersInProcessService.UpdateAsync(customerOrderInProcess.CustomerOrderInProcessId, customerOrderInProcess);

                await DialogService.Alert($"Pedido de Articulos Modificado Satisfactoriamente", "Información");
                NavigationManager.NavigateTo("process-customer-orders");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
        }

        protected async Task<ICollection<CustomerOrderInProcessDetail>> MapDetailsInProcess(ICollection<DetailInProcess> detailsInProcess) => (from item in detailsInProcess
                                                                                                                                               let orderInProcessDetail = new CustomerOrderInProcessDetail()
                                                                                                                                               {
                                                                                                                                                   Brand = item.BRAND,
                                                                                                                                                   CustomerOrderDetailId = item.CUSTOMER_ORDER_DETAIL_ID,
                                                                                                                                                   ProcessedQuantity = item.THIS_QUANTITY,
                                                                                                                                                   WarehouseId = item.WAREHOUSE_ID
                                                                                                                                               }
                                                                                                                                               select orderInProcessDetail).ToList();

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que cancelar la modificación del Traslado??", "Confirmar") == true)
                NavigationManager.NavigateTo("process-customer-orders");
        }

        #endregion
    }
}