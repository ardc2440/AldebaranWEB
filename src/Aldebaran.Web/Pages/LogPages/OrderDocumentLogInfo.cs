using Aldebaran.Application.Services.Models.OrderDocumentLogs;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>Datos de presentación de cada log de despachos / traslados a proceso: segmento de la URL, título y columnas.</summary>
    public sealed record OrderDocumentLogInfo(string Route, OrderDocumentLogType LogType, string Title, string DocumentTitle, string DocumentDateTitle, string EventDateTitle)
    {
        public static IReadOnlyList<OrderDocumentLogInfo> All { get; } = new[]
        {
            new OrderDocumentLogInfo("shipment-modification", OrderDocumentLogType.ShipmentModification, "Log de modificación de despachos", "Despacho", "Fecha de despacho", "Fecha de modificación"),
            new OrderDocumentLogInfo("shipment-cancellation", OrderDocumentLogType.ShipmentCancellation, "Log de cancelación de despachos", "Despacho", "Fecha de despacho", "Fecha de cancelación"),
            new OrderDocumentLogInfo("in-process-modification", OrderDocumentLogType.InProcessModification, "Log de modificación de traslados a proceso", "Traslado", "Fecha de traslado", "Fecha de modificación"),
            new OrderDocumentLogInfo("in-process-cancellation", OrderDocumentLogType.InProcessCancellation, "Log de cancelación de traslados a proceso", "Traslado", "Fecha de traslado", "Fecha de cancelación")
        };

        public static OrderDocumentLogInfo FromRoute(string route) =>
            All.FirstOrDefault(c => string.Equals(c.Route, route, StringComparison.OrdinalIgnoreCase));
    }
}
