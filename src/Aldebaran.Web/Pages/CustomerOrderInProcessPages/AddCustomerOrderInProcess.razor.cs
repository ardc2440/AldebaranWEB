using Aldebaran.Web.Models;
using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderInProcessPages
{
    public partial class AddCustomerOrderInProcess
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
        protected DialogResult dialogResult;
        protected ICollection<DetailInProcess> detailsInProcess;
        protected RadzenDataGrid<DetailInProcess> customerOrderDetailGrid;
        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string title;
        protected IEnumerable<Employee> employeesFOREMPLOYEEID;
        protected IEnumerable<ProcessSatellite> processSatellitesFORPROCESSSATELLITEID;

        [Parameter]
        public string pCustomerOrderId { get; set; } = "NoParamInput";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (!int.TryParse(pCustomerOrderId, out var customerOrderId))
                    throw new Exception("El Id de Pedido recibido no es valido");

                isLoadingInProgress = true;

                await Task.Yield();

                documentType = await AldebaranDbService.GetDocumentTypeByCode("T");

                var customerOrderInProgressStatusDocumentType = await AldebaranDbService.GetStatusDocumentTypeByDocumentAndOrder(documentType, 1);

                customerOrder = await GetCustomerOrder(customerOrderId);

                detailsInProcess = await GetDetailsInProcess(customerOrder);

                processSatellitesFORPROCESSSATELLITEID = await AldebaranDbService.GetProcessSatellites();
                employeesFOREMPLOYEEID = await AldebaranDbService.GetEmployees();

                customerOrderInProcess = new CustomerOrderInProcess()
                {
                    CREATION_DATE = DateTime.Now,
                    CUSTOMER_ORDER_ID = customerOrder.CUSTOMER_ORDER_ID,
                    PROCESS_DATE = DateTime.Today,
                    TRANSFER_DATETIME = DateTime.Today,
                    STATUS_DOCUMENT_TYPE_ID = customerOrderInProgressStatusDocumentType.STATUS_DOCUMENT_TYPE_ID
                };

                title = $"Traslado a Proceso para el Pedido No. {customerOrder.ORDER_NUMBER}";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        protected async Task<CustomerOrder> GetCustomerOrder(int customerOrderId) => (await AldebaranDbService.GetCustomerOrders(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID.Equals(@0)", FilterParameters = new object[] { customerOrderId }, Expand = "Customer.City.Department.Country" }) ?? throw new ArgumentException($"The customer order with the ID {customerOrderId}, isn't more available")).FirstOrDefault();

        protected async Task<List<CustomerOrderDetail>> GetOrderDetails(Models.AldebaranDb.CustomerOrder args) => await AldebaranDbService.GetCustomerOrderDetails(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID == {args.CUSTOMER_ORDER_ID}", Expand = "CustomerOrder,ItemReference,ItemReference.Item" }) == null ? new List<CustomerOrderDetail>() : (await AldebaranDbService.GetCustomerOrderDetails(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID == {args.CUSTOMER_ORDER_ID}", Expand = "CustomerOrder,ItemReference,ItemReference.Item" })).ToList();

        protected async Task<List<DetailInProcess>> GetDetailsInProcess(CustomerOrder customerOrder) => (from item in await GetOrderDetails(customerOrder) ?? throw new ArgumentException($"The references of Customer Order {customerOrder.ORDER_NUMBER}, could not be obtained.")
                                                                                                         let viewOrderDetail = new DetailInProcess()
                                                                                                         {
                                                                                                             REFERENCE_ID = item.REFERENCE_ID,
                                                                                                             CUSTOMER_ORDER_DETAIL_ID = item.CUSTOMER_ORDER_DETAIL_ID,
                                                                                                             REFERENCE_DESCRIPTION = $"({item.ItemReference.Item.INTERNAL_REFERENCE}) {item.ItemReference.Item.ITEM_NAME} - {item.ItemReference.REFERENCE_NAME}",
                                                                                                             PENDING_QUANTITY = item.REQUESTED_QUANTITY - item.PROCESSED_QUANTITY - item.DELIVERED_QUANTITY,
                                                                                                             PROCESSED_QUANTITY = item.PROCESSED_QUANTITY,
                                                                                                             DELIVERED_QUANTITY = item.DELIVERED_QUANTITY,
                                                                                                             BRAND = item.BRAND,
                                                                                                             THIS_QUANTITY = 0
                                                                                                         }
                                                                                                         select viewOrderDetail).ToList();

        protected async Task SendToProcess(Models.ViewModels.DetailInProcess args)
        {
            if (await DialogService.OpenAsync<SetQuantityInProcess>("Cantidad a Trasladar", new Dictionary<string, object> { { "detailInProcess", args } }) != null)
                await customerOrderDetailGrid.Reload();
        }

        protected async Task CancelToProcess(Models.ViewModels.DetailInProcess args)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") != true)
            {
                return;
            }

            args.THIS_QUANTITY = 0;

            await customerOrderDetailGrid.Reload();
        }

        protected async Task FormSubmit()
        {
            try
            {
                dialogResult = null;

                isSubmitInProgress = true;

                if (!detailsInProcess.Any(x => x.THIS_QUANTITY > 0))
                    throw new Exception("No ha ingresado ninguna cantidad a trasladar");

                customerOrderInProcess.CustomerOrderInProcessDetails = await MapDetailsInProcess(detailsInProcess);

                var result = await AldebaranDbService.CreateCustomerOrderInProcess(customerOrderInProcess);

                if (result != null)
                {
                    await DialogService.Alert($"Traslado a Proceso Grabado Satisfactoriamente", "Información");
                    NavigationManager.NavigateTo("process-customer-orders");
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
        }

        protected async Task<ICollection<CustomerOrderInProcessDetail>> MapDetailsInProcess(ICollection<DetailInProcess> detailsInProcess)
        {
            var customerOrderInProcessDetails = new List<CustomerOrderInProcessDetail>();
            foreach (var details in detailsInProcess)
            {
                customerOrderInProcessDetails.Add(new CustomerOrderInProcessDetail()
                {
                    BRAND = details.BRAND,
                    CUSTOMER_ORDER_DETAIL_ID = details.CUSTOMER_ORDER_DETAIL_ID,
                    PROCESSED_QUANTITY = details.THIS_QUANTITY,
                    WAREHOUSE_ID = details.WAREHOUSE_ID
                });
            }

            return customerOrderInProcessDetails;
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que cancelar la creacion del Traslado??", "Confirmar") == true)
                NavigationManager.NavigateTo("process-customer-orders");
        }
    }
}