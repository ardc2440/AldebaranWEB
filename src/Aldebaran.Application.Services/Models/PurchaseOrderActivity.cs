namespace Aldebaran.Application.Services.Models
{
    public class PurchaseOrderActivity
    {
        public int PurchaseOrderActivityId { get; set; }
        public int PurchaseOrderId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string ActivityDescription { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public int EmployeeId { get; set; }
        public int ActivityEmployeeId { get; set; }
        public Employee ActivityEmployee { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public PurchaseOrderActivity()
        {
            ExecutionDate = DateTime.Now;
            CreationDate = DateTime.Now;
        }
    }
}
