namespace Aldebaran.Application.Services.Models
{
    public class AdjustmentType
    {
        public short AdjustmentTypeId { get; set; }
        public string AdjustmentTypeName { get; set; } = null!;
        public short Operator { get; set; }
        // Reverse navigation
        public ICollection<Adjustment> Adjustments { get; set; }
        public AdjustmentType()
        {
            Adjustments = new List<Adjustment>();
        }

    }
}
