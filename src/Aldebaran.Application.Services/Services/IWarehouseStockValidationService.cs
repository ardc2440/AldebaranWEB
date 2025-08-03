using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IWarehouseStockValidationService
    {
        /// <summary>
        /// Valida si la cantidad solicitada está disponible en bodega local considerando
        /// las cantidades comprometidas y la cantidad original del registro que se está editando
        /// </summary>
        /// <param name="referenceId">ID de la referencia</param>
        /// <param name="requestedQuantity">Cantidad solicitada</param>
        /// <param name="originalQuantity">Cantidad original del registro (0 para nuevos, cantidad previa para ediciones)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la validación</returns>
        Task<WarehouseStockValidationResult> ValidateLocalWarehouseStockAsync(
            int referenceId, 
            int requestedQuantity, 
            int originalQuantity = 0, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida stock específicamente para traslados a proceso desde bodega local.
        /// En traslados a proceso, las cantidades comprometidas en pedidos SÍ están disponibles
        /// porque forman parte del flujo normal del proceso.
        /// </summary>
        /// <param name="referenceId">ID de la referencia</param>
        /// <param name="requestedQuantity">Cantidad solicitada</param>
        /// <param name="originalQuantity">Cantidad original del traslado (0 para nuevos, cantidad previa para ediciones)</param>
        /// <param name="excludeCustomerOrderDetailId">ID del CustomerOrderDetail actual a excluir del cálculo de cantidades comprometidas</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la validación</returns>
        Task<WarehouseStockValidationResult> ValidateLocalWarehouseStockForProcessTransferAsync(
            int referenceId, 
            int requestedQuantity, 
            int originalQuantity = 0, 
            int? excludeCustomerOrderDetailId = null,
            CancellationToken cancellationToken = default);
    }

    public class WarehouseStockValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int AvailableStock { get; set; }
        public int RequestedQuantity { get; set; }
        public int PhysicalStock { get; set; }
        public int CommittedQuantity { get; set; }
        public int OriginalQuantity { get; set; }
    }
}