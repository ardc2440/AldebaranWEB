namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderActivity
    {
        public int PurchaseOrderActivityId { get; set; }
        public int PurchaseOrderId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime CreationDate { get; set; }
        public int EmployeeId { get; set; }
        public int ActivityEmployeeId { get; set; }
        public Employee ActivityEmployee { get; set; }
        public Employee Employee { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public PurchaseOrderActivity()
        {
            ExecutionDate = DateTime.Now;
            CreationDate = DateTime.Now;
        }
    }
}
