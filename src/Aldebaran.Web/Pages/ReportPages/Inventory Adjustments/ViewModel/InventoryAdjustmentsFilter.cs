using Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.ViewModel
{
    public class InventoryAdjustmentsFilter
    {
        public int? AdjustmentId { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public short? AdjustmentTypeId { get; set; }
        public AdjustmentType AdjustmentType { get; set; }
        public short? AdjustmentReasonId { get; set; }
        public AdjustmentReason AdjustmentReason { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();

    }
}
