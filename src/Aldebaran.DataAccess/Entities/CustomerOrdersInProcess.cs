namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrdersInProcess
    {
        public int CustomerOrderInProcessId { get; set; }
        public int CustomerOrderId { get; set; }
        public DateTime ProcessDate { get; set; }
        public string Notes { get; set; }
        public int ProcessSatelliteId { get; set; }
        public DateTime TransferDatetime { get; set; }
        public int EmployeeRecipientId { get; set; }
        public DateTime CreationDate { get; set; }
        // Reverse navigation
        public CanceledOrdersInProcess CanceledOrdersInProcess { get; set; }
        public ICollection<CustomerOrderInProcessDetail> CustomerOrderInProcessDetails { get; set; }
        public ICollection<ModifiedOrdersInProcess> ModifiedOrdersInProcesses { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public Employee Employee { get; set; }
        public ProcessSatellite ProcessSatellite { get; set; }
        public CustomerOrdersInProcess()
        {
            ProcessDate = DateTime.Now;
            TransferDatetime = DateTime.Now;
            CreationDate = DateTime.Now;
            CustomerOrderInProcessDetails = new List<CustomerOrderInProcessDetail>();
            ModifiedOrdersInProcesses = new List<ModifiedOrdersInProcess>();
        }
    }
}
