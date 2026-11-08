namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>Filtros del reporte de existencias por bodega. Null o vacío = sin filtro.</summary>
    public class WarehouseStockReportFilter
    {
        public short? WarehouseId { get; set; }
        public string? ReferenceIds { get; set; } // ids separados por coma; vacío = todas
    }
}
