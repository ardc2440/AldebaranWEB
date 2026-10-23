namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de SP_GET_APPROVAL_RANGE_LOG: cambio de una tolerancia de ajustes de OC (valor anterior y nuevo). Keyless.</summary>
    public class ApprovalRangeLogEntry
    {
        public int LogId { get; set; }
        public DateTime EventDate { get; set; }
        public int PreviousQuantityFrom { get; set; }
        public int PreviousQuantityTo { get; set; }
        public decimal PreviousPercent { get; set; }
        public bool PreviousIsActive { get; set; }
        public int QuantityFrom { get; set; }
        public int QuantityTo { get; set; }
        public decimal Percent { get; set; }
        public bool IsActive { get; set; }
        public string ChangeReason { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }
}
