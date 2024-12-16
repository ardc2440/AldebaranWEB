using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.WarehouseTransferPages
{
    public partial class AddWarehouseTransfer
    {
        #region Injections

        [Inject]
        protected ILogger<AddWarehouseTransfer> Logger { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }
        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }
        [Inject]
        protected IEmployeeService EmployeeService { get; set; }
        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        protected IWarehouseService WarehouseService { get; set; }
        [Inject]
        protected IWarehouseTransferService WarehouseTransferService { get; set; }
        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters

        #endregion

        #region Properties
        protected IEnumerable<Warehouse> WarehousesForWarehouseId { get; set; }
        protected ICollection<WarehouseTransferDetail> WarehouseTransferDetails { get; set; }

        #endregion

        #region Global Variables

        protected WarehouseTransfer warehouseTransfer;
        protected LocalizedDataGrid<WarehouseTransferDetail> warehouseTransferDetailGrid;
        protected DocumentType documentType;
        protected bool IsErrorVisible;
        private bool Submitted;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        protected string Title;
        protected int lastReferenceId = 0;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                WarehousesForWarehouseId = await WarehouseService.GetAsync();
                WarehouseTransferDetails = new List<WarehouseTransferDetail>();
                warehouseTransfer = new WarehouseTransfer();
                documentType = await DocumentTypeService.FindByCodeAsync("B");
                warehouseTransfer.StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
                warehouseTransfer.StatusDocumentTypeId = warehouseTransfer.StatusDocumentType.StatusDocumentTypeId;
                warehouseTransfer.Employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
                warehouseTransfer.EmployeeId = warehouseTransfer.Employee.EmployeeId;
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"[{reference.Item.InternalReference}] {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task FormSubmit()
        {
            try
            {
                Title = "";
                Submitted = true;
                IsSubmitInProgress = true;
                if (!WarehouseTransferDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");
                if (warehouseTransfer.DestinationWarehouseId == warehouseTransfer.OriginWarehouseId)
                    throw new Exception("Las bodegas de origen y destino deben ser diferentes");
                warehouseTransfer.WarehouseTransferDetails = WarehouseTransferDetails;
                var result = await WarehouseTransferService.AddAsync(warehouseTransfer);
                NavigationManager.NavigateTo($"warehouse-transfers/{result.WarehouseTransferId}");
            }
            catch (Exception ex)
            {
                Title = "No se ha podido grabar el traslado entre bodegas";
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
            finally { IsSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que desea cancelar la creación del traslado?", "Confirmar") == true)
                NavigationManager.NavigateTo("warehouse-transfers");
        }

        protected async Task AddWarehouseTransferDetail(MouseEventArgs args)
        {
            try
            {
                if (warehouseTransfer.OriginWarehouseId <= 0)
                    throw new Exception("No ha seleccionado la bodega de origen");

                var result = await DialogService.OpenAsync<AddWarehouseTransferDetail>("Agregar referencia", new Dictionary<string, object> {
                { "WarehouseTransferDetails", WarehouseTransferDetails },
                { "warehouseTransfer", warehouseTransfer },
                { "LastReferenceId", lastReferenceId }, });

                if (result == null)
                    return;

                var detail = (WarehouseTransferDetail)result;
                WarehouseTransferDetails.Add(detail);
                lastReferenceId = detail.ReferenceId;
                await warehouseTransferDetailGrid.Reload();
            }
            catch (Exception ex)
            {
                Title = "";
                IsErrorVisible = true;
                Error = ex.Message;
            }
        }

        protected async Task DeleteWarehouseTransferDetail(WarehouseTransferDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                WarehouseTransferDetails.Remove(item);
                await warehouseTransferDetailGrid.Reload();
            }
        }

        protected async Task EditWarehouseTransferDetail(WarehouseTransferDetail args)
        {
            var result = await DialogService.OpenAsync<EditWarehouseTransferDetail>("Actualizar referencia", new Dictionary<string, object> { { "WarehouseTransferDetail", args }, { "warehouseTransfer", warehouseTransfer } });

            if (result == null)
                return;

            args.Quantity = result.Quantity;

            await warehouseTransferDetailGrid.Reload();
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion
    }
}