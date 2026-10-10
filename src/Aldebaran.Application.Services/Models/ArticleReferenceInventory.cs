namespace Aldebaran.Application.Services.Models
{
    /// <summary>
    /// Caso de uso "Consultar el inventario del artículo de una alarma de cantidades mínimas":
    /// inventario de una referencia activa del artículo.
    /// </summary>
    /// <param name="PhysicalStock">Stock Físico = Bodega Local + Zona Franca.</param>
    /// <param name="Committed">Comprometido = Reservas + Pedidos.</param>
    /// <param name="Available">Disponible = Stock Físico + Tránsito - Comprometido.</param>
    /// <param name="IsAlarmReference">La referencia es la de la alarma consultada (se resalta).</param>
    /// <param name="ProviderName">Proveedor de la última OC Confirmada de la referencia (null si no tiene OC confirmada).</param>
    /// <param name="PurchaseOrderDate">Fecha de esa OC.</param>
    /// <param name="RealReceiptDate">Fecha real de recibo de esa OC.</param>
    /// <param name="RequestedQuantity">Cantidad solicitada de la referencia en esa OC.</param>
    /// <param name="ReceivedQuantity">Cantidad recibida de la referencia en esa OC.</param>
    public sealed record ArticleReferenceInventory(
        int ReferenceId,
        string ReferenceCode,
        string ReferenceName,
        int LocalWarehouse,
        int FreeZone,
        int PhysicalStock,
        int InTransit,
        int Committed,
        int Available,
        bool IsAlarmReference,
        string? ProviderName,
        DateTime? PurchaseOrderDate,
        DateTime? RealReceiptDate,
        int? RequestedQuantity,
        int? ReceivedQuantity);
}
