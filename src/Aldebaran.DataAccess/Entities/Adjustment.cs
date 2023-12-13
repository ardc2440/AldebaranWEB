namespace Aldebaran.DataAccess.Entities
{
    public class Adjustment
    {
        public int AdjustmentId { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public short AdjustmentReasonId { get; set; }
        public short AdjustmentTypeId { get; set; }
        public int EmployeeId { get; set; }
        public string Notes { get; set; }
        public DateTime CreationDate { get; set; }
        // Reverse navigation
        public ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }
        public AdjustmentReason AdjustmentReason { get; set; }
        public AdjustmentType AdjustmentType { get; set; }
        public Employee Employee { get; set; }
        public Adjustment()
        {
            AdjustmentDate = DateTime.Now;
            CreationDate = DateTime.Now;
            AdjustmentDetails = new List<AdjustmentDetail>();
        }
    }
}
