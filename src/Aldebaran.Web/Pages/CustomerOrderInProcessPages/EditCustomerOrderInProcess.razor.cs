using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderInProcessPages
{
    public partial class EditCustomerOrderInProcess
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
        protected CustomerOrder customerOrder;
        protected CustomerOrderInProcess customerOrderInProcess;
        protected DocumentType documentType;
        protected ICollection<DetailInProcess> detailsInProcess;
        protected RadzenDataGrid<DetailInProcess> customerOrderDetailGrid;

        protected IEnumerable<Employee> employeesFOREMPLOYEEID;
        protected IEnumerable<ProcessSatellite> processSatellitesFORPROCESSSATELLITEID;

        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string title;

        [Parameter]
        public string pCustomerOrderInProcessId { get; set; } = "NoParamInput";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                if (!int.TryParse(pCustomerOrderInProcessId, out var customerOrderInProcessId))
                    throw new Exception("El Id de Traslado recibido no es valido");

                documentType = await AldebaranDbService.GetDocumentTypeByCode("T");

                customerOrderInProcess = await AldebaranDbService.GetCustomerOrderInProcessByCustomerOrderInProcessId(customerOrderInProcessId);

                customerOrder = await GetCustomerOrder(customerOrderInProcess.CUSTOMER_ORDER_ID);

                detailsInProcess = await GetDetailsInProcess(customerOrderInProcess);

                processSatellitesFORPROCESSSATELLITEID = await AldebaranDbService.GetProcessSatellites();
                employeesFOREMPLOYEEID = await AldebaranDbService.GetEmployees();

                title = $"Modificación del Traslado a Proceso para el Pedido No. {customerOrder.ORDER_NUMBER}";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        protected async Task<CustomerOrder> GetCustomerOrder(int customerOrderId) => (await AldebaranDbService.GetCustomerOrders(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID.Equals(@0)", FilterParameters = new object[] { customerOrderId }, Expand = "Customer.City.Department.Country" }) ?? throw new ArgumentException($"The customer order with the ID {customerOrderId}, isn't more available")).FirstOrDefault();

        protected async Task<List<DetailInProcess>> GetDetailsInProcess(CustomerOrderInProcess customerOrderInProcess) => (from item in await AldebaranDbService.GetCustomerOrderInProcessDetails(new Query { Filter = $"i=>i.CUSTOMER_ORDER_IN_PROCESS_ID.Equals({customerOrderInProcess.CUSTOMER_ORDER_IN_PROCESS_ID})", Expand = "CustomerOrderDetail.ItemReference.Item, Warehouse" }) ?? throw new ArgumentException("The references of Customer Order In Process, could not be obtained.")
                                                                                                                           let viewOrderDetail = new DetailInProcess()
                                                                                                                           {
                                                                                                                               REFERENCE_ID = item.CustomerOrderDetail.REFERENCE_ID,
                                                                                                                               CUSTOMER_ORDER_DETAIL_ID = item.CUSTOMER_ORDER_DETAIL_ID,
                                                                                                                               REFERENCE_DESCRIPTION = $"({item.CustomerOrderDetail.ItemReference.Item.INTERNAL_REFERENCE}) {item.CustomerOrderDetail.ItemReference.Item.ITEM_NAME} - {item.CustomerOrderDetail.ItemReference.REFERENCE_NAME}",
                                                                                                                               PENDING_QUANTITY = item.CustomerOrderDetail.REQUESTED_QUANTITY - item.CustomerOrderDetail.PROCESSED_QUANTITY - item.CustomerOrderDetail.DELIVERED_QUANTITY,
                                                                                                                               PROCESSED_QUANTITY = item.CustomerOrderDetail.PROCESSED_QUANTITY,
                                                                                                                               DELIVERED_QUANTITY = item.CustomerOrderDetail.DELIVERED_QUANTITY,
                                                                                                                               BRAND = item.BRAND,
                                                                                                                               WAREHOUSE_ID = item.WAREHOUSE_ID,
                                                                                                                               THIS_QUANTITY = item.PROCESSED_QUANTITY
                                                                                                                           }
                                                                                                                           select viewOrderDetail).ToList();

        protected async Task SendToProcess(Models.ViewModels.DetailInProcess args)
        {
            if (await DialogService.OpenAsync<SetQuantityInProcess>("Cantidad a Trasladar", new Dictionary<string, object> { { "pDetailInProcess", args } }) != null)
                await customerOrderDetailGrid.Reload();
        }

        protected async Task CancelToProcess(Models.ViewModels.DetailInProcess args)
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

                var result = await AldebaranDbService.UpdateCustomerOrderInProcess(customerOrderInProcess, detailsInProcess);

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

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que cancelar la modificación del Traslado??", "Confirmar") == true)
                NavigationManager.NavigateTo("process-customer-orders");
        }

        protected async Task AddCustomerOrderDetailButtonClick(MouseEventArgs args)
        {
            /*var result = await DialogService.OpenAsync<AddCustomerOrderDetail>("Nueva referencia", new Dictionary<string, object> { { "customerOrderDetails", customerOrderDetails } });

            if (result == null)
                return;

            var detail = (CustomerOrderDetail)result;

            customerOrderDetails.Add(detail);

            await customerOrderDetailGrid.Reload();*/
        }

        protected async Task DeleteCustomerOrderDetailButtonClick(MouseEventArgs args, CustomerOrderDetail item)
        {
            /*if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                customerOrderDetails.Remove(item);

                await customerOrderDetailGrid.Reload();
            }*/
        }

        protected async Task EditRow(CustomerOrderDetail args)
        {
            /*var result = await DialogService.OpenAsync<EditCustomerOrderDetail>("Actualizar referencia", new Dictionary<string, object> { { "customerOrderDetail", args } });
            if (result == null)
                return;
            var detail = (CustomerOrderDetail)result;

            customerOrderDetails.Remove(args);
            customerOrderDetails.Add(detail);

            await customerOrderDetailGrid.Reload();*/
        }
    }
}