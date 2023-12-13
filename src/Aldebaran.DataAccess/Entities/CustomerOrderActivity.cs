namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderActivity
    {
        public int CustomerOrderActivityId { get; set; }
        public int CustomerOrderId { get; set; }
        public short AreaId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Notes { get; set; }
        // Reverse navigation
        public ICollection<CustomerOrderActivityDetail> CustomerOrderActivityDetails { get; set; }
        public Area Area { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public Employee Employee { get; set; }
        public CustomerOrderActivity()
        {
            ActivityDate = DateTime.Now;
            CustomerOrderActivityDetails = new List<CustomerOrderActivityDetail>();
        }
    }
}
