using Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.ViewModel
{
    public class InventoryAdjustmentsFilter
    {
        public int? AdjustmentId { get; set; }
        public DateTime? AdjustmentDateFrom { get; set; }
        public DateTime AdjustmentDateTo { get; set; } = DateTime.Now;
        public DateTime? CreationDateFrom { get; set; }
        public DateTime CreationDateTo { get; set; } = DateTime.Now;
        public short? AdjustmentTypeId { get; set; }
        public AdjustmentType AdjustmentType { get; set; }
        public short? AdjustmentReasonId { get; set; }
        public AdjustmentReason AdjustmentReason { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();

    }
}
