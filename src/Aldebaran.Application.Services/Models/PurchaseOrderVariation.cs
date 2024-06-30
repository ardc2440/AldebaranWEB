namespace Aldebaran.Application.Services.Models
{
    public class PurchaseOrderVariation
    {
        public bool IsValid { get; set; }
        public int Average { get; set; }
        public int MinimumRange { get; set; }
        public int MaximumRange { get; set; }
    }
}
