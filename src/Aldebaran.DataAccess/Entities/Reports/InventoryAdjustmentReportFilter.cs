namespace Aldebaran.DataAccess.Entities.Reports
{
    /// <summary>Parámetros de SP_GET_INVENTORY_ADJUSTMENT_REPORT (reporte de ajustes de inventario). Null o vacío = sin filtro.</summary>
    public class InventoryAdjustmentReportFilter
    {
        public int? AdjustmentId { get; set; }
        public DateTime? AdjustmentDateFrom { get; set; }
        public DateTime? AdjustmentDateTo { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }
        public short? AdjustmentTypeId { get; set; }
        public short? AdjustmentReasonId { get; set; }
        public int? EmployeeId { get; set; }
        public string? ReferenceIds { get; set; } // ids separados por coma; vacío = todas
    }
}
