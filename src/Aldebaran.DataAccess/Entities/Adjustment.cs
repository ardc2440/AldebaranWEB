using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class Adjustment: ITrackeable
    {
        public int AdjustmentId { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public short AdjustmentReasonId { get; set; }
        public short AdjustmentTypeId { get; set; }
        public int EmployeeId { get; set; }
        public string Notes { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public short StatusDocumentTypeId { get; set; }

        // Reverse navigation
        public ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }
        public AdjustmentReason AdjustmentReason { get; set; } = null!;
        public AdjustmentType AdjustmentType { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public StatusDocumentType StatusDocumentType { get; set; } = null!;

        public Adjustment()
        {
            AdjustmentDate = DateTime.Now;
            CreationDate = DateTime.Now;
            AdjustmentDetails = new List<AdjustmentDetail>();
        }
    }
}
