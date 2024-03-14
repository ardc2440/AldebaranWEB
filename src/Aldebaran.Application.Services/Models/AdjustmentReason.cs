namespace Aldebaran.Application.Services.Models
{
    public class AdjustmentReason
    {
        public short AdjustmentReasonId { get; set; }
        public string AdjustmentReasonName { get; set; } = null!;
        public string AdjustmentReasonNotes { get; set; } = null!;
        // Reverse navigation
        public ICollection<Adjustment> Adjustments { get; set; }
        public AdjustmentReason()
        {
            Adjustments = new List<Adjustment>();
        }

    }
}
