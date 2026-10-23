namespace Aldebaran.Application.Services.Models.TrackLogs
{
    /// <summary>
    /// Filtros de la consulta de Track. Rango de fechas obligatorio y, al menos, la entidad o el objeto:
    /// - Solo entidad: movimientos de esa entidad.
    /// - Solo objeto: movimientos de todas las entidades que tocaron el objeto.
    /// - Entidad y objeto: movimientos de esa entidad que tocaron el objeto.
    /// </summary>
    public sealed record TrackLogFilter(DateTime DateFrom, DateTime DateTo, string? EntityName = null, TrackObjectRef? Object = null, TrackAction? Action = null, string? ModifierName = null);
}
