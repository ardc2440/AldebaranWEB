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
                await GetWarehousetransferssAsync();
                await DialogResultResolver();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events

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
                    Detail = $"El traslado ha sido actualizado correctamente."
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
            warehouseTransfers = string.IsNullOrEmpty(searchKey) ? await WarehouseTransferService.GetAsync(ct) : await WarehouseTransferService.GetAsync(searchKey, ct);
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

        protected async Task EditWarehouseTransfer(WarehouseTransfer args)
        {
            NavigationManager.NavigateTo("edit-warehouse-transfer/" + args.WarehouseTransferId);
        }

        protected bool CanEditWarehouseTransfer(WarehouseTransfer args)
        {
            return Security.IsInRole("Admin", "Transfer Warehouses Editor") && args.StatusDocumentType.EditMode;
        }

        protected async Task CancelWhareHouseTransferClick(MouseEventArgs args, WarehouseTransfer warehouseTransfer)
        {
            try
            {
                DialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea cancelar este traslado?") == true)
                {
                    warehouseTransfer.StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 2);
                    warehouseTransfer.StatusDocumentTypeId = warehouseTransfer.StatusDocumentType.StatusDocumentTypeId;
                    warehouseTransfer.WarehouseTransferDetails = (await WarehouseTransferDetailService.GetByWarehouseTransferIdAsync(warehouseTransfer.WarehouseTransferId)).ToList();

                    await WarehouseTransferService.CancelAsync(warehouseTransfer.WarehouseTransferId);
                    await GetWarehousetransferssAsync(search);

                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Traslado entre bodegas",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Traslado ha sido cancelado correctamente."
                    });

                    await WarehouseTransfersGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el traslado \n {ex.Message}"
                });
            }
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