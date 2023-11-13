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
        protected DocumentType documentType;
        protected ICollection<DetailInProcess> detailInProcesses;
        protected RadzenDataGrid<DetailInProcess> customerOrderDetailGrid;

        protected bool isSubmitInProgress;
        protected bool isLoadingInProgress;
        protected DialogResult dialogResult;
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
                    throw new Exception("El Id de Pedido recibido no es valido");

                documentType = await AldebaranDbService.GetDocumentTypeByCode("P");

                var customerOrders = await AldebaranDbService.GetCustomerOrders(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID.Equals(@0)", FilterParameters = new object[] { customerOrderId }, Expand = "Customer.City.Department.Country" });
                customerOrder = customerOrders.FirstOrDefault();

                await GetDetailsInProcess(customerOrder);

                title = $"Traslado a Proceso para el Pedido No. {customerOrder.ORDER_NUMBER}";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isLoadingInProgress = false; }
        }

        protected async Task<List<CustomerOrderDetail>> GetOrderDetails(Models.AldebaranDb.CustomerOrder args)
        {
            var CustomerOrderDetailsResult = await AldebaranDbService.GetCustomerOrderDetails(new Query { Filter = $@"i => i.CUSTOMER_ORDER_ID == {args.CUSTOMER_ORDER_ID}", Expand = "CustomerOrder,ItemReference,ItemReference.Item" });
            if (CustomerOrderDetailsResult != null)
            {
                return CustomerOrderDetailsResult.ToList();
            }

            return new List<CustomerOrderDetail>();
        }

        private async Task GetDetailsInProcess(CustomerOrder customerOrder)
        {
            var orderDetail = await GetOrderDetails(customerOrder);

            var detailInProcesses = new List<DetailInProcess>();

            foreach (var item in orderDetail)
            {
                var viewOrderDetail = new DetailInProcess()
                {
                    REFERENCE_ID = item.REFERENCE_ID,
                    CUSTOMER_ORDER_DETAIL_ID = item.CUSTOMER_ORDER_DETAIL_ID,
                    REFERENCE_DESCRIPTION = $"({item.ItemReference.Item.INTERNAL_REFERENCE}) {item.ItemReference.Item.ITEM_NAME} - {item.ItemReference.REFERENCE_NAME}",
                    PENDING_QUANTITY = item.REQUESTED_QUANTITY - item.PROCESSED_QUANTITY - item.DELIVERED_QUANTITY,
                    PROCESSED_QUANTITY = item.PROCESSED_QUANTITY,
                    DELIVERED_QUANTITY = item.DELIVERED_QUANTITY,
                    BRAND = item.BRAND,
                    THIS_QUANTITY = 0
                };
                detailInProcesses.Add(viewOrderDetail);
            }

            this.detailInProcesses = detailInProcesses;
        }

        protected async Task SendToProcess(Models.ViewModels.DetailInProcess args)
        {
            var result = await DialogService.OpenAsync<SetQuantityInProcess>("Cantidad a Trasladar", new Dictionary<string, object> { { "detailInProcess", args } });

            if (result == null)
                return;

            var detail = (DetailInProcess)result;

            args.THIS_QUANTITY = detail.THIS_QUANTITY;

            await customerOrderDetailGrid.Reload();
        }

        protected async Task CancelToProcess(Models.ViewModels.DetailInProcess args)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                args.THIS_QUANTITY = 0;

                await customerOrderDetailGrid.Reload();
            }
        }

        protected async Task FormSubmit()
        {
            try
            {
                dialogResult = null;

                var inProcessDocumentType = await AldebaranDbService.GetDocumentTypeByCode("T");

                isSubmitInProgress = true;

                if (!detailInProcesses.Any(x => x.THIS_QUANTITY > 0))
                    throw new Exception("No ha ingresado ninguna cantidad a trasladar");

                var cancelStatusDocumentType = await AldebaranDbService.GetStatusDocumentTypeByDocumentAndOrder(inProcessDocumentType, 2);

                //var cancelResult = await AldebaranDbService.CreateCustomerOrderInProcess(customerOrderInProcess);

                //if (cancelResult != null)
                //{
                //    dialogResult = new DialogResult { Success = true, Message = "Traslado a Proceso cancelado correctamente." };
                //    await CustomerOrderInProcessesDataGrid.Reload();
                //}

                await DialogService.Alert($"Traslado a Proceso Grabado Satisfactoriamente", "Información");
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
            if (await DialogService.Confirm("Está seguro que cancelar la creacion del Pedido??", "Confirmar") == true)
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