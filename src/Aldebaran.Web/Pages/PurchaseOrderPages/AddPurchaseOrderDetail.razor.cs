using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Entities;
using Aldebaran.Web.Models;
using Aldebaran.Web.Pages.ItemPages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class AddPurchaseOrderDetail
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        [Inject]
        protected IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }

        [Inject]
        public IOptions<AppSettings> Settings { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }


        #endregion

        #region Parameters
        [Parameter]
        public IEnumerable<ServiceModel.ItemReference> ProviderItemReferences { get; set; } = new List<ServiceModel.ItemReference>();
        [Parameter]
        public IEnumerable<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<ServiceModel.PurchaseOrderDetail>();
        [Parameter]
        public int LastReferenceId { get; set; }
        [Parameter]
        public short LastWarehouseId { get; set; }
        [Parameter]
        public int ProviderId { get; set; }
        [Parameter]
        public int PurchaseOrderId { get; set; } = -1;
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.PurchaseOrderDetail PurchaseOrderDetail;
        protected IEnumerable<ServiceModel.Warehouse> Warehouses;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        protected ServiceModel.ItemReference ItemReference;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                PurchaseOrderDetail = new ServiceModel.PurchaseOrderDetail();
                Warehouses = await WarehouseService.GetAsync();

                if (LastWarehouseId != 0)
                {
                    PurchaseOrderDetail.WarehouseId = LastWarehouseId;
                }
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                // Un detalle de orden de compra es unico por referencia y bodega
                if (PurchaseOrderDetails.Any(a => a.ReferenceId == PurchaseOrderDetail.ReferenceId && a.WarehouseId == PurchaseOrderDetail.WarehouseId))
                {
                    IsErrorVisible = true;
                    Error = "Ya existe una referencia para la misma bodega adicionada a esta orden de compra";
                    return;
                }

                var purchaseOrderVariation = (await PurchaseOrderDetailService.IsValidPurchaseOrderVariation(ProviderId, PurchaseOrderDetail.ReferenceId, PurchaseOrderDetail.RequestedQuantity, Settings.Value.VariationMonthNumber, PurchaseOrderId)).FirstOrDefault();

                if (!purchaseOrderVariation.IsValid)
                    if (await DialogService.Confirm($"Ha ingresado una cantidad fuera del rango entre {purchaseOrderVariation.MinimumRange} y {purchaseOrderVariation.MaximumRange}, de {purchaseOrderVariation.Average} unidades solicitadas en promedio durante los ultimos {Settings.Value.VariationMonthNumber} meses, " +
                            "en las ordenes de compra del proveedor seleccionado.<br><br>Desea continuar con el proceso?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar cantidad") == false) return;

                PurchaseOrderDetail.Warehouse = Warehouses.Single(s => s.WarehouseId == PurchaseOrderDetail.WarehouseId);

                await ValidateReferenceConfiguration(PurchaseOrderDetail.ReferenceId);

                DialogService.Close(PurchaseOrderDetail);
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected async Task ItemReferenceHandler(ServiceModel.ItemReference reference)
        {
            PurchaseOrderDetail.ReferenceId = reference?.ReferenceId ?? 0;
            PurchaseOrderDetail.ItemReference = PurchaseOrderDetail.ReferenceId == 0 ? null : ProviderItemReferences.Single(s => s.ReferenceId == PurchaseOrderDetail.ReferenceId);
            ItemReference = reference;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        protected async Task ValidateReferenceConfiguration(int referenceId)
        {
            var itemReference = await ItemReferenceService.FindAsync(referenceId);
            var message = "";

            if ((!itemReference.Item.IsDomesticProduct &&
                 !itemReference.Item.IsSpecialImport &&
                 !itemReference.Item.IsSaleOff &&
                  itemReference.MinimumQuantityPercent <= 0 && itemReference.AlarmMinimumQuantity <= 0) ||
                itemReference.PurchaseOrderVariation <= 0)
            {
                if (!itemReference.Item.IsDomesticProduct &&
                    !itemReference.Item.IsSpecialImport &&
                    !itemReference.Item.IsSaleOff && itemReference.AlarmMinimumQuantity <= 0 && 
                    (itemReference.MinimumQuantityPercent <= 0 || (itemReference.MinimumQuantityPercent > 0 && itemReference.HavePurchaseOrderDetail)))
                    message += message + "la cantidad mínima";

                if (!itemReference.Item.IsDomesticProduct &&
                    !itemReference.Item.IsSpecialImport &&
                    !itemReference.Item.IsSaleOff && itemReference.MinimumQuantityPercent <= 0 && !itemReference.HavePurchaseOrderDetail)
                    message += (message != "" ? " o " : "") + "el % de cantidad mínima";

                if (itemReference.PurchaseOrderVariation <= 0)
                    message += (message != "" ? " y " : "") + "el % Variación en orden de compra";

                if (await DialogService.Confirm($"Desea configurar {message} para esta referencia?",
                        options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" },
                        title: "Cantidad mínima") == true)
                {
                    var result = await DialogService.OpenAsync<EditItemReference>("Actualizar referencia",
                                    new Dictionary<string, object> {
                                        { "REFERENCE_ID", itemReference.ReferenceId },

                                        { "MINIMUM_QUANTITY", !itemReference.Item.IsDomesticProduct &&
                                                                     !itemReference.Item.IsSpecialImport &&
                                                                     !itemReference.Item.IsSaleOff && itemReference.AlarmMinimumQuantity <= 0 &&
                                                                      (itemReference.MinimumQuantityPercent <= 0 || (itemReference.MinimumQuantityPercent > 0 && itemReference.HavePurchaseOrderDetail))},

                                        { "PURCHASE_ORDER_VARIATION", itemReference.PurchaseOrderVariation <= 0 },

                                        { "MINIMUM_QUANTITY_PERCENT",!itemReference.Item.IsDomesticProduct &&
                                                                     !itemReference.Item.IsSpecialImport &&
                                                                     !itemReference.Item.IsSaleOff && itemReference.MinimumQuantityPercent <= 0 && !itemReference.HavePurchaseOrderDetail } });
                    if (result == true)
                    {
                        NotificationService.Notify(new NotificationMessage
                        {
                            Summary = "Referencia",
                            Severity = NotificationSeverity.Success,
                            Detail = $"Referencia actualizada correctamente."
                        });
                    }
                }
            }
        }

        #endregion
    }
}