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
        bool IsAlarmReference);
}
