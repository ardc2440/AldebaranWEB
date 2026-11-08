namespace Aldebaran.DataAccess.Entities.Reports
{
    /// <summary>Parámetros de SP_GET_WAREHOUSE_STOCK_REPORT (reporte de existencias por bodega). Null o vacío = sin filtro.</summary>
    public class WarehouseStockReportFilter
    {
        public short? WarehouseId { get; set; }
        public string? ReferenceIds { get; set; } // ids separados por coma; vacío = todas
    }
}
