namespace Aldebaran.DataAccess.Entities.Reports
{
    /// <summary>Parámetros de SP_GET_WAREHOUSE_TRANSFER_REPORT (reporte de traslados entre bodegas). Null o vacío = sin filtro.</summary>
    public class WarehouseTransferReportFilter
    {
        public short? TargetWarehouseId { get; set; }
        public short? SourceWarehouseId { get; set; }
        public DateTime? AdjustmentDateFrom { get; set; }
        public DateTime? AdjustmentDateTo { get; set; }
        public string? NationalizationNumber { get; set; }
        public short? StatusId { get; set; }
        public string? ReferenceIds { get; set; } // ids separados por coma; vacío = todas
    }
}
