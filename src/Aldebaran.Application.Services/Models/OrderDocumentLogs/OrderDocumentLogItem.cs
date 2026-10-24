namespace Aldebaran.Application.Services.Models.OrderDocumentLogs
{
    /// <summary>Modificación o cancelación de un despacho o traslado a proceso.</summary>
    /// <param name="EventDate">Fecha de la modificación o cancelación.</param>
    /// <param name="DocumentId">Número del despacho o del traslado a proceso.</param>
    /// <param name="DocumentDate">Fecha de despacho o de traslado del documento.</param>
    public sealed record OrderDocumentLogItem(string LogKey, DateTime EventDate, int DocumentId, DateTime? DocumentDate,
        int CustomerOrderId, string OrderNumber, bool IsSpecialOrder, string CustomerName, string Reason, string EmployeeName);
}
