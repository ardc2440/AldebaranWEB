using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen.Blazor;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Automatic_Customer_Order_In_Process_Creation
{
    public partial class AutomaticCustomerOrderInProcessReport
    {
        #region Injections
        [Inject]
        protected ILogger<AutomaticCustomerOrderInProcessReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IAutomaticPurchaseOrderAssigmentReportService AutomaticPurchaseOrderAssigmentReportService { get; set; }
        #endregion

        #region Variables
        protected ViewModel.AutomaticAssigmentFilter Filter;
        protected ViewModel.AutomaticAssigmentViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        private IEnumerable<Application.Services.Models.Reports.AutomaticCustomerOrderAssigmentReport> DataReport { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Reset();
            }
        }
        #endregion

        #region Events
        async Task Reset()
        {
            Filter = null;
            ViewModel = null;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            await OpenFilters();
        }
        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {

            try
            {
                IsLoadingData = true;

                DataReport = await AutomaticPurchaseOrderAssigmentReportService.GetAutomaticCustomerOrderAssigmentReportDataAsync(filter, ct);

                ViewModel = new ViewModel.AutomaticAssigmentViewModel()
                {
                    Documents = await GetDocumentsAsync(ct)
                };
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }
        async Task<string> SetReportFilterAsync(ViewModel.AutomaticAssigmentFilter filter, CancellationToken ct = default)
        {
            var filterResult = string.Empty;

            if (filter.ReceiptDate.StartDate.HasValue)
                filterResult += $"@ReceiptDateFrom = '{(DateTime)filter.ReceiptDate.StartDate:yyyyMMdd}', @ReceiptDateTo = '{(DateTime)filter.ReceiptDate.EndDate:yyyyMMdd}'";

            if (filter.ConfirmedDate.StartDate.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ConfirmedDateFrom = '{(DateTime)filter.ConfirmedDate.StartDate:yyyyMMdd}', @ConfirmedDateTo = '{(DateTime)filter.ConfirmedDate.EndDate:yyyyMMdd}'";

            if (filter.OrderDate.StartDate.HasValue)
                filterResult += $"@OrderDateFrom = '{(DateTime)filter.OrderDate.StartDate:yyyyMMdd}', @OrderDateTo = '{(DateTime)filter.OrderDate.EndDate:yyyyMMdd}'";

            if (filter.EstimatedDeliveryDate.StartDate.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@EstimatedDeliveryDateFrom = '{(DateTime)filter.EstimatedDeliveryDate.StartDate:yyyyMMdd}', @EstimatedDeliveryDateFrom = '{(DateTime)filter.EstimatedDeliveryDate.EndDate:yyyyMMdd}'";

            if (!filter.DocumentType.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@DocumentType = '{filter.DocumentType}'";

            if (!filter.DocumentNumber.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@DocumentNumber = '{filter.DocumentNumber}'";

            if (!filter.CustomerOrderNumber.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@CustomerOrderNumber = '{filter.CustomerOrderNumber}'";

            if (!filter.ImportNumber.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ImportNumber = '{filter.ImportNumber}'";

            if (filter.ItemReferences.Count > 0)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIds = '{String.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId))}'";

            if (!filter.ProformaNumber.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProformaNumber = '{filter.ProformaNumber}'";

            if (filter.ProviderId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ProviderId = {filter.ProviderId}";

            if (filter.CustomerId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@CustomerId = {filter.CustomerId}";

            if (filter.StatusDocumentTypeId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@StatusDocumentTypeId = {filter.StatusDocumentTypeId}";

            return filterResult;
        }
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<Components.AutomaticAssigmentReportFilter>("Filtrar reporte de asignación automática de referencias a pedidos", parameters: new Dictionary<string, object> { { "Filter", (ViewModel.AutomaticAssigmentFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (ViewModel.AutomaticAssigmentFilter)result;

            await RedrawReportAsync(await SetReportFilterAsync(Filter));

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }
        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                await Reset();
            }
        }
        async Task Save(RadzenSplitButtonItem args)
        {
            if (args?.Value == null)
                return;
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "automatic-assigment-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Asignación automática de referencias a pedidos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "automatic-assigment-report-container");
            }
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        async Task ToggleReadMorePage()
        {
            await JSRuntime.InvokeVoidAsync("readMoreTogglePage", "toggleLinkPage");
        }
        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion

        #region Fill Data Report

        async Task<List<ViewModel.AutomaticAssigmentViewModel.Document>> GetDocumentsAsync(CancellationToken ct = default)
        {
            var orders = new List<ViewModel.AutomaticAssigmentViewModel.Document>();

            foreach (var order in DataReport.Select(s => new { s.DocumentType, s.DocumentId, s.DocumentNumber, s.ProviderIdentity, s.ProviderName, s.ProformaNumber, s.ImportNumber, s.ReceipDate, s.ConfirmationDate })
                                    .DistinctBy(d => new { d.DocumentType, d.DocumentId })
                                    .OrderBy(o => o.DocumentType)
                                    .ThenBy(o => o.DocumentNumber))
            {
                orders.Add(new ViewModel.AutomaticAssigmentViewModel.Document
                {
                    DocumentType = order.DocumentType,
                    DocumentId = order.DocumentId,
                    DocumentNumber = order.DocumentNumber,
                    ProviderIdentity = order.ProviderIdentity,
                    ProviderName = order.ProviderName,
                    ProformaNumber = order.ProformaNumber,
                    ImportNumber = order.ImportNumber,
                    ReceiptDate = order.ReceipDate,
                    ConfirmationDate = order.ConfirmationDate,
                    CustomerOrders = await GetCustomerOrders(order.DocumentType, order.DocumentId, ct)
                });
            }

            return orders;
        }
        async Task<List<ViewModel.AutomaticAssigmentViewModel.CustomerOrder>> GetCustomerOrders(string documentType, int documentId, CancellationToken ct = default)
        {
            var customerOrders = new List<ViewModel.AutomaticAssigmentViewModel.CustomerOrder>();

            foreach (var customerOrder in DataReport.Where(w => w.DocumentType == documentType && w.DocumentId == documentId)
                                            .Select(s => new { s.CustomerOrderId, s.CustomerOrderNumber, s.CustomerIdentity, s.CustomerName, s.OrderDate, s.EstimatedDeliveryDate, s.StatusOrderName })
                                            .DistinctBy(d => d.CustomerOrderId)
                                            .OrderBy(o => o.CustomerOrderNumber))
            {
                customerOrders.Add(new ViewModel.AutomaticAssigmentViewModel.CustomerOrder
                {
                    CustomerOrderId = customerOrder.CustomerOrderId,
                    CustomerOrderNumber = customerOrder.CustomerOrderNumber,
                    CustomerIdentity = customerOrder.CustomerIdentity,
                    CustomerName = customerOrder.CustomerName,
                    OrderDate = customerOrder.OrderDate,
                    EstimatedDeliveryDate = customerOrder.EstimatedDeliveryDate,
                    StatusOrderName = customerOrder.StatusOrderName,
                    CustomerOrderArticles = await GetCustomerOrderArticles(documentType, documentId, customerOrder.CustomerOrderId, ct)
                });
            }

            return customerOrders;
        }
        async Task<List<ViewModel.AutomaticAssigmentViewModel.CustomerOrderArticle>> GetCustomerOrderArticles(string documentType, int documentId, int customerOrderId, CancellationToken ct = default)
        {
            var customerOrderArticles = new List<ViewModel.AutomaticAssigmentViewModel.CustomerOrderArticle>();

            foreach (var customerOrderArticle in DataReport.Where(w => w.DocumentType == documentType && 
                                                                       w.DocumentId == documentId &&
                                                                       w.CustomerOrderId == customerOrderId)
                                                    .Select(s => new { s.ItemId, s.InternalReference, s.ItemName })
                                                    .DistinctBy(d => d.ItemId)
                                                    .OrderBy(o => o.InternalReference))
            {
                customerOrderArticles.Add(new ViewModel.AutomaticAssigmentViewModel.CustomerOrderArticle
                {
                    ItemId = customerOrderArticle.ItemId,
                    ItemName = customerOrderArticle.ItemName,
                    InternalReference = customerOrderArticle.InternalReference,
                    CustomerOrderDetails = await GetCustomerOrderDetails(documentType, documentId, customerOrderId, customerOrderArticle.ItemId, ct)
                });
            }

            return customerOrderArticles;
        }
        async Task<List<ViewModel.AutomaticAssigmentViewModel.CustomerOrderDetail>> GetCustomerOrderDetails(string documentType, int documentId, int customerOrderId, int itemId, CancellationToken ct = default)
        {
            var customerOrderDetails = new List<ViewModel.AutomaticAssigmentViewModel.CustomerOrderDetail>();

            foreach (var customerOrderArticle in DataReport.Where(w => w.DocumentType == documentType && 
                                                                       w.DocumentId == documentId &&
                                                                       w.CustomerOrderId == customerOrderId &&
                                                                       w.ItemId == itemId)
                                                    .Select(s => new { s.ReferenceName, s.Requested, s.Assigned, s.Pending })
                                                    .OrderBy(o => o.ReferenceName))
            {
                customerOrderDetails.Add(new ViewModel.AutomaticAssigmentViewModel.CustomerOrderDetail
                {
                    ReferenceName = customerOrderArticle.ReferenceName,
                    Requested = customerOrderArticle.Requested,
                    Pending = customerOrderArticle.Pending,
                    Assigned = customerOrderArticle.Assigned
                });
            }

            return customerOrderDetails;
        }
        #endregion
    }
}
