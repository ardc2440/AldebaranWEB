using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderInProcessPages
{
    public partial class AddCustomerOrderInProcess
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
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }

        [Inject]
        protected IProcessSatelliteService ProcessSatelliteService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected ICustomerOrdersInProcessService CustomerOrdersInProcessService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CustomerOrderId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

        protected CustomerOrder customerOrder;
        protected CustomerOrdersInProcess customerOrderInProcess;
        protected DocumentType documentType;
        protected DialogResult dialogResult;
        protected ICollection<DetailInProcess> detailsInProcess;
        protected LocalizedDataGrid<DetailInProcess> customerOrderDetailGrid;
        protected bool isLoadingInProgress;
        protected string title;
        protected IEnumerable<Employee> employeesFOREMPLOYEEID;
        protected IEnumerable<ProcessSatellite> processSatellitesFORPROCESSSATELLITEID;
        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected string Error;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (!int.TryParse(CustomerOrderId, out var customerOrderId))
                    throw new Exception("El Id de pedido recibido no es valido");

                isLoadingInProgress = true;

                await Task.Yield();

                documentType = await DocumentTypeService.FindByCodeAsync("T");

                var customerOrderInProgressStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);

                customerOrder = await CustomerOrderService.FindAsync(customerOrderId);

                customerOrder.CustomerOrderDetails = (await CustomerOrderDetailService.GetByCustomerOrderIdAsync(customerOrderId)).ToList();

                detailsInProcess = await GetDetailsInProcess(customerOrder);

                processSatellitesFORPROCESSSATELLITEID = await ProcessSatelliteService.GetAsync();
                employeesFOREMPLOYEEID = await EmployeeService.GetAsync();

                customerOrderInProcess = new CustomerOrdersInProcess()
                {
                    CreationDate = DateTime.Now,
                    CustomerOrderId = customerOrder.CustomerOrderId,
                    Employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id),
                    ProcessDate = DateTime.Today,
                    TransferDatetime = DateTime.Today,
                    StatusDocumentTypeId = customerOrderInProgressStatusDocumentType.StatusDocumentTypeId
                };

                customerOrderInProcess.EmployeeId = customerOrderInProcess.Employee.EmployeeId;

                title = $"Traslado a proceso para el pedido No. {customerOrder.OrderNumber}";
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

        protected async Task<List<DetailInProcess>> GetDetailsInProcess(CustomerOrder customerOrder)
        {
            return (from item in customerOrder.CustomerOrderDetails ?? throw new ArgumentException($"The references of Customer Order {customerOrder.OrderNumber}, could not be obtained.")
                    let viewOrderDetail = new DetailInProcess()
                    {
                        REFERENCE_ID = item.ReferenceId,
                        CUSTOMER_ORDER_DETAIL_ID = item.CustomerOrderDetailId,
                        REFERENCE_DESCRIPTION = $"({item.ItemReference.Item.InternalReference}) {item.ItemReference.Item.ItemName} - {item.ItemReference.ReferenceName}",
                        PENDING_QUANTITY = item.RequestedQuantity - item.ProcessedQuantity - item.DeliveredQuantity,
                        PROCESSED_QUANTITY = item.ProcessedQuantity,
                        DELIVERED_QUANTITY = item.DeliveredQuantity,
                        BRAND = item.Brand,
                        THIS_QUANTITY = 0,
                        ItemReference = item.ItemReference
                    }
                    select viewOrderDetail).ToList();
        }

        protected async Task SendToProcess(DetailInProcess args)
        {
            if (await DialogService.OpenAsync<SetQuantityInProcess>("Cantidad a trasladar", new Dictionary<string, object> { { "DetailInProcess", args } }) != null)
                await customerOrderDetailGrid.Reload();
        }

        protected async Task CancelToProcess(DetailInProcess args)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") != true)
            {
                return;
            }

            args.PENDING_QUANTITY += args.THIS_QUANTITY;
            args.PROCESSED_QUANTITY -= args.THIS_QUANTITY;
            args.WAREHOUSE_ID = 0;
            args.THIS_QUANTITY = 0;

            await customerOrderDetailGrid.Reload();
        }

        protected bool CanSend(DetailInProcess detailInProcess) => detailInProcess.PENDING_QUANTITY > 0 && Security.IsInRole("Admin", "Customer Order In Process Editor");

        protected bool CanCancel(DetailInProcess detailInProcess) => detailInProcess.THIS_QUANTITY > 0 && Security.IsInRole("Admin", "Customer Order In Process Editor");

        protected async Task FormSubmit()
        {
            try
            {
                dialogResult = null;

                IsSubmitInProgress = true;

                if (!detailsInProcess.Any(x => x.THIS_QUANTITY > 0))
                    throw new Exception("No ha ingresado ninguna cantidad a trasladar");

                customerOrderInProcess.CustomerOrderInProcessDetails = await MapDetailsInProcess(detailsInProcess);

                var result = await CustomerOrdersInProcessService.AddAsync(customerOrderInProcess);

                NavigationManager.NavigateTo($"process-customer-orders/{result.CustomerOrderInProcessId}");
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
            var customerOrderInProcessDetails = new List<CustomerOrderInProcessDetail>();
            foreach (var details in detailsInProcess.Where(i => i.THIS_QUANTITY > 0))
            {
                customerOrderInProcessDetails.Add(new CustomerOrderInProcessDetail()
                {
                    Brand = details.BRAND,
                    CustomerOrderDetailId = details.CUSTOMER_ORDER_DETAIL_ID,
                    ProcessedQuantity = details.THIS_QUANTITY,
                    WarehouseId = details.WAREHOUSE_ID
                });
            }

            return customerOrderInProcessDetails;
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que desea cancelar la creación del traslado??", "Confirmar") == true)
                NavigationManager.NavigateTo("process-customer-orders");
        }

        #endregion
    }
}