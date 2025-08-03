using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public class WarehouseStockValidationService : IWarehouseStockValidationService
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IReferencesWarehouseService _referencesWarehouseService;
        private readonly IItemReferenceService _itemReferenceService;
        private readonly ICustomerOrderDetailService _customerOrderDetailService;

        public WarehouseStockValidationService(
            IWarehouseService warehouseService,
            IReferencesWarehouseService referencesWarehouseService,
            IItemReferenceService itemReferenceService,
            ICustomerOrderDetailService customerOrderDetailService)
        {
            _warehouseService = warehouseService;
            _referencesWarehouseService = referencesWarehouseService;
            _itemReferenceService = itemReferenceService;
            _customerOrderDetailService = customerOrderDetailService;
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

        /// <summary>
        /// Valida stock específicamente para traslados a proceso desde bodega local.
        /// En traslados a proceso, las cantidades comprometidas en pedidos SÍ están disponibles
        /// porque forman parte del flujo normal del proceso.
        /// </summary>
        public async Task<WarehouseStockValidationResult> ValidateLocalWarehouseStockForProcessTransferAsync(
            int referenceId, 
            int requestedQuantity, 
            int originalQuantity = 0, 
            int? excludeCustomerOrderDetailId = null,
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

                // Para traslados a proceso: 
                // - Las cantidades comprometidas en el CustomerOrderDetail ACTUAL SÍ están disponibles para proceso
                // - Solo descontamos las reservas (ReservedQuantity) y otros pedidos diferentes al actual
                result.PhysicalStock = localWarehouseStock.Quantity;
                
                // Calcular cantidades comprometidas excluyendo el CustomerOrderDetail actual
                var otherOrdersCommittedQuantity = await CalculateCommittedQuantityExcludingOrderAsync(
                    referenceId, excludeCustomerOrderDetailId, cancellationToken);
                
                result.CommittedQuantity = itemReference.ReservedQuantity + otherOrdersCommittedQuantity;
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

        /// <summary>
        /// Calcula las cantidades comprometidas en otros pedidos, excluyendo un CustomerOrderDetail específico.
        /// Enfoque simplificado: Total OrderedQuantity - PENDING_QUANTITY del CustomerOrderDetail actual
        /// </summary>
        private async Task<int> CalculateCommittedQuantityExcludingOrderAsync(
            int referenceId, 
            int? excludeCustomerOrderDetailId, 
            CancellationToken cancellationToken)
        {
            try
            {
                // Obtener el total de cantidades comprometidas de la referencia
                var itemRef = await _itemReferenceService.FindAsync(referenceId, cancellationToken);
                var totalOrderedQuantity = itemRef?.OrderedQuantity ?? 0;

                // Si no hay CustomerOrderDetailId a excluir, retornamos toda la cantidad ordenada
                if (!excludeCustomerOrderDetailId.HasValue || totalOrderedQuantity == 0)
                {
                    return totalOrderedQuantity;
                }

                // Obtener directamente el CustomerOrderDetail específico por ID
                var currentOrderDetail = await _customerOrderDetailService.FindAsync(
                    excludeCustomerOrderDetailId.Value, cancellationToken);

                if (currentOrderDetail == null)
                {
                    // Si no encontramos el detalle específico, retornamos todo
                    return totalOrderedQuantity;
                }

                // Calcular el PENDING_QUANTITY del CustomerOrderDetail actual
                // PENDING = RequestedQuantity - ProcessedQuantity - DeliveredQuantity
                var currentOrderPendingQuantity = currentOrderDetail.RequestedQuantity 
                    - currentOrderDetail.ProcessedQuantity 
                    - currentOrderDetail.DeliveredQuantity;

                // Lo que está comprometido en otros pedidos = Total - Pendiente del detalle actual
                var otherOrdersCommittedQuantity = totalOrderedQuantity - Math.Max(0, currentOrderPendingQuantity);

                return Math.Max(0, otherOrdersCommittedQuantity);
            }
            catch (Exception ex)
            {
                // En caso de error, ser conservador y no excluir nada
                var itemRef = await _itemReferenceService.FindAsync(referenceId, cancellationToken);
                return itemRef?.OrderedQuantity ?? 0;
            }
        }
    }
}