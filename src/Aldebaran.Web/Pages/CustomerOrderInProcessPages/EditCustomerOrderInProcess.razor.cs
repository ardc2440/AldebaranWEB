using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
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
        protected TooltipService TooltipService { get; set; }

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

        protected CustomerOrder customerOrder;
        protected CustomerOrdersInProcess customerOrderInProcess;
        protected DocumentType documentType;
        protected ICollection<DetailInProcess> detailsInProcess;
        protected LocalizedDataGrid<DetailInProcess> customerOrderDetailGrid;
        protected IEnumerable<Employee> employeesFOREMPLOYEEID;
        protected IEnumerable<ProcessSatellite> processSatellitesFORPROCESSSATELLITEID;
        protected bool isLoadingInProgress;
        protected string title;
        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected string Error;
        internal const string quantitiesError = "Esta referencia, ya posee artículos despachados al cliente. \n\r" +
                                                "Para poder cancelar su cantidad, debe cancelar todos los despachos realizados para el pedido.";

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(CustomerOrderInProcessId, out var customerOrderInProcessId)) throw new Exception("El Id de traslado recibido no es valido");

                documentType = await DocumentTypeService.FindByCodeAsync("T");

                customerOrderInProcess = await CustomerOrdersInProcessService.FindAsync(customerOrderInProcessId);

                customerOrder = await CustomerOrderService.FindAsync(customerOrderInProcess.CustomerOrderId);

                detailsInProcess = await GetDetailsInProcess(customerOrderInProcess);

                processSatellitesFORPROCESSSATELLITEID = await ProcessSatelliteService.GetAsync();
                employeesFOREMPLOYEEID = await EmployeeService.GetAsync();

                title = $"Modificación del traslado a proceso para el pedido No. {customerOrder.OrderNumber}";
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task<List<DetailInProcess>> GetDetailsInProcess(CustomerOrdersInProcess customerOrderInProcess)
        {
            return (from item in await CustomerOrderInProcessDetailService.GetByCustomerOrderInProcessIdAsync(customerOrderInProcess.CustomerOrderInProcessId) ?? throw new ArgumentException("The references of Customer Order In Process, could not be obtained.")
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
                        THIS_QUANTITY = item.ProcessedQuantity,
                        ItemReference = item.CustomerOrderDetail.ItemReference,
                        CustomerOrderInProcessDetailId = item.CustomerOrderInProcessDetailId
                    }
                    select viewOrderDetail).ToList();
        }

        protected async Task SendToProcess(DetailInProcess args)
        {
            if (args.PROCESSED_QUANTITY < args.THIS_QUANTITY)
            {
                Error = quantitiesError;
                IsErrorVisible = true;
                return;
            }

            if (await DialogService.OpenAsync<SetQuantityInProcess>("Cantidad a Trasladar", new Dictionary<string, object> { { "DetailInProcess", args } }) != null)
                await customerOrderDetailGrid.Reload();
        }

        protected async Task CancelToProcess(DetailInProcess args)
        {
            if (args.PROCESSED_QUANTITY < args.THIS_QUANTITY)
            {
                Error = quantitiesError;
                IsErrorVisible = true;
                return;
            }

            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") != true) return;

            args.PENDING_QUANTITY += args.THIS_QUANTITY;
            args.PROCESSED_QUANTITY -= args.THIS_QUANTITY;
            args.WAREHOUSE_ID = 0;
            args.THIS_QUANTITY = 0;

            await customerOrderDetailGrid.Reload();
        }

        protected bool CanCancel(DetailInProcess detailInProcess) => detailInProcess.THIS_QUANTITY > 0 && Security.IsInRole("Administrador", "Modificación de pedidos en proceso");

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                if (!detailsInProcess.Any(x => x.THIS_QUANTITY > 0)) throw new Exception("No ha ingresado ninguna cantidad a trasladar");

                var reasonResult = await DialogService.OpenAsync<ModificationReasonDialog>("Confirmar modificación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "T" }, { "TITLE", "Está seguro que desea actualizar este traslado?" } });
                if (reasonResult == null)
                    return;

                var reason = (Reason)reasonResult;

                customerOrderInProcess.CustomerOrderInProcessDetails = await MapDetailsInProcess(detailsInProcess);
                await CustomerOrdersInProcessService.UpdateAsync(customerOrderInProcess.CustomerOrderInProcessId, customerOrderInProcess, reason);

                NavigationManager.NavigateTo($"process-customer-orders/edit/{customerOrderInProcess.CustomerOrderInProcessId}");
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally { IsSubmitInProgress = false; }
        }

        protected async Task<ICollection<CustomerOrderInProcessDetail>> MapDetailsInProcess(ICollection<DetailInProcess> detailsInProcess)
        {
            return (from item in detailsInProcess.Where(i => i.THIS_QUANTITY > 0)
                    let orderInProcessDetail = new CustomerOrderInProcessDetail()
                    {
                        Brand = item.BRAND,
                        CustomerOrderDetailId = item.CUSTOMER_ORDER_DETAIL_ID,
                        ProcessedQuantity = item.THIS_QUANTITY,
                        WarehouseId = item.WAREHOUSE_ID,
                        CustomerOrderInProcessId = customerOrderInProcess.CustomerOrderInProcessId,
                        CustomerOrderInProcessDetailId = item.CustomerOrderInProcessDetailId
                    }
                    select orderInProcessDetail).ToList();
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que desea cancelar la modificación del traslado??", "Confirmar") == true)
                NavigationManager.NavigateTo("process-customer-orders");
        }

        #endregion
    }
}