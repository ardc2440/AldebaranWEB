using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public class WarehouseStockValidationService : IWarehouseStockValidationService
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IReferencesWarehouseService _referencesWarehouseService;
        private readonly IItemReferenceService _itemReferenceService;

        public WarehouseStockValidationService(
            IWarehouseService warehouseService,
            IReferencesWarehouseService referencesWarehouseService,
            IItemReferenceService itemReferenceService)
        {
            _warehouseService = warehouseService;
            _referencesWarehouseService = referencesWarehouseService;
            _itemReferenceService = itemReferenceService;
        }

        public async Task<WarehouseStockValidationResult> ValidateLocalWarehouseStockAsync(
            int referenceId, 
            int requestedQuantity, 
            int originalQuantity = 0, 
            CancellationToken cancellationToken = default)
        {
            var result = new WarehouseStockValidationResult
            {
                RequestedQuantity = requestedQuantity,
                OriginalQuantity = originalQuantity
            };

            try
            {
                // Obtener la referencia para verificar si requiere validación de stock
                var itemReference = await _itemReferenceService.FindAsync(referenceId, cancellationToken);
                if (itemReference == null)
                {
                    result.ErrorMessage = "La referencia especificada no existe.";
                    return result;
                }

                // Solo validar stock para productos que no son importación especial ni productos domésticos
                if (itemReference.Item.IsSpecialImport || itemReference.Item.IsDomesticProduct)
                {
                    result.IsValid = true;
                    return result;
                }

                // Obtener bodega local (código 1)
                var localWarehouse = await _warehouseService.FindByCodeAsync(1, cancellationToken);
                if (localWarehouse == null)
                {
                    result.ErrorMessage = "No se pudo encontrar la bodega local.";
                    return result;
                }

                // Obtener stock en bodega local
                var localWarehouseStock = await _referencesWarehouseService.GetByReferenceAndWarehouseIdAsync(
                    referenceId, localWarehouse.WarehouseId, cancellationToken);

                if (localWarehouseStock == null)
                {
                    result.ErrorMessage = "No se encontró información de stock para esta referencia en bodega local.";
                    return result;
                }

                // Calcular stock disponible:
                // Stock físico - cantidades comprometidas (ordenadas + reservadas) + cantidad original liberada
                result.PhysicalStock = localWarehouseStock.Quantity;
                result.CommittedQuantity = itemReference.OrderedQuantity + itemReference.ReservedQuantity;
                result.AvailableStock = result.PhysicalStock - result.CommittedQuantity + originalQuantity;

                // Validar si hay suficiente stock disponible
                if (requestedQuantity > result.AvailableStock)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "La cantidad ingresada supera la existencia en bodega local. Verifique disponibilidad de la referencia.";
                }
                else
                {
                    result.IsValid = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Error al validar stock: {ex.Message}";
                return result;
            }
        }
    }
}