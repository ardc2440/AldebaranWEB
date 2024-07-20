using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.WarehouseTransferPages
{
    public partial class WarehouseTransfers
    {
        #region Injections

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        protected TooltipService TooltipService { get; set; }
        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }
        [Inject]
        protected IWarehouseTransferService WarehouseTransferService { get; set; }
        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }
        [Inject]
        protected IWarehouseTransferDetailService WarehouseTransferDetailService { get; set; }
        [Inject]
        protected SecurityService Security { get; set; }

        #endregion

        #region Global Variables

        protected IEnumerable<WarehouseTransfer> warehouseTransfers;
        protected IEnumerable<WarehouseTransferDetail> warehouseTransferDetails;

        protected LocalizedDataGrid<WarehouseTransfer> WarehouseTransfersGrid;
        protected DialogResult DialogResult { get; set; }
        protected string search = "";
        protected bool isLoadingInProgress;
        protected DocumentType documentType;
        protected WarehouseTransfer warehouseTransfer;

        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;

        #endregion

        #region Parameters

        [Parameter]
        public string WAREHOUSE_TRANSFER_ID { get; set; } = null;
        [Parameter]
        public string Action { get; set; } = null;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await Task.Yield();
                documentType = await DocumentTypeService.FindByCodeAsync("B");
                
                await DialogResultResolver();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events

        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip.Value;
            top = args.Top.Value; 
            await GetWarehousetransferssAsync(search);
        }
        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        async Task DialogResultResolver(CancellationToken ct = default)
        {
            if (WAREHOUSE_TRANSFER_ID == null)
                return;

            var valid = int.TryParse(WAREHOUSE_TRANSFER_ID, out var warehouseTransferId);
            if (!valid)
                return;

            var warehouseTransfer = await WarehouseTransferService.FindAsync(warehouseTransferId, ct);
            if (warehouseTransfer == null)
                return;

            if (Action == "edit")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Traslado entre bodegas",
                    Severity = NotificationSeverity.Success,
                    Detail = $"El traslado ha sido actualizado correctamente con el consecutivo {warehouseTransferId}."
                });
                return;
            }

            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Traslado entre bodegas",
                Severity = NotificationSeverity.Success,
                Detail = $"El traslado ha sido creada correctamente."
            });
        }

        async Task GetWarehousetransferssAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            (warehouseTransfers, count) = string.IsNullOrEmpty(searchKey) ? await WarehouseTransferService.GetAsync(skip, top, ct) : await WarehouseTransferService.GetAsync(skip, top, searchKey, ct);
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await WarehouseTransfersGrid.GoToPage(0);
            await GetWarehousetransferssAsync(search);
        }

        protected async Task AddWarehouseTransferClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-warehouse-transfer");
        }

        protected async Task GetChildData(WarehouseTransfer args)
        {
            warehouseTransfer = args;

            var result = await WarehouseTransferDetailService.GetByWarehouseTransferIdAsync(args.WarehouseTransferId);

            if (result != null)
            {
                warehouseTransferDetails = result.ToList();

                args.WarehouseTransferDetails = result.ToList();
            }
        }

        #endregion

    }
}