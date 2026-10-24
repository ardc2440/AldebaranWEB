namespace Aldebaran.Application.Services.Models
{
    public class CustomerOrderActivity
    {
        public int CustomerOrderActivityId { get; set; }
        public int CustomerOrderId { get; set; }
        public short AreaId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Notes { get; set; } = null!;
        // Reverse navigation
        public ICollection<CustomerOrderActivityDetail> CustomerOrderActivityDetails { get; set; }
        public Area Area { get; set; } = null!;
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public CustomerOrderActivity()
        {
            ActivityDate = DateTime.Now;
            CustomerOrderActivityDetails = new List<CustomerOrderActivityDetail>();
        }
    }
}
