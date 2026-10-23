namespace Aldebaran.Application.Services.Models.PurchaseOrderLogs
{
    /// <summary>Solicitud de aprobación de ajustes de una orden de compra y su resolución.</summary>
    /// <param name="Result">En aprobación, Aprobada o Devuelta.</param>
    public sealed record PurchaseOrderAdjustmentRequestItem(int RequestId, DateTime RequestDate, int PurchaseOrderId, string OrderNumber,
        string ProviderName, string RequestedBy, string Result, DateTime? ResolutionDate, string? ResolvedBy, string? Reason);
}
