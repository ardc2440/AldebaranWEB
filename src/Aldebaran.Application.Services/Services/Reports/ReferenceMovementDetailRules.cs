using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// Reglas para obtener las líneas de movimiento de una referencia en el reporte "Movimientos de artículos".
    /// El SP entrega cada línea de movimiento repetida una vez por cada bodega de la referencia (cruce final con
    /// references_warehouse). Se toma una sola copia (la de la primera bodega) para no perder líneas reales del
    /// mismo documento (p. ej. un ajuste que afecta la misma referencia en dos bodegas).
    /// </summary>
    public static class ReferenceMovementDetailRules
    {
        public static IReadOnlyList<ReferenceMovementReport> MovementLines(IEnumerable<ReferenceMovementReport> rows, int referenceId, short titleId)
        {
            var referenceRows = rows.Where(w => w.ReferenceId == referenceId).ToList();
            if (referenceRows.Count == 0)
                return Array.Empty<ReferenceMovementReport>();

            var firstWarehouseId = referenceRows.Min(m => m.WarehouseId);

            return referenceRows
                .Where(w => w.WarehouseId == firstWarehouseId && w.TitleId == titleId)
                .OrderBy(o => o.Date)
                .ThenBy(o => o.Code?.Length ?? 0)
                .ThenBy(o => o.Code, StringComparer.Ordinal)
                .ToList();
        }
    }
}
