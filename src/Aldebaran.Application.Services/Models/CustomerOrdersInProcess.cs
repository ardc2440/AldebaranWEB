namespace Aldebaran.Application.Services.Models
{
    public class CustomerOrdersInProcess
    {
        public int CustomerOrderInProcessId { get; set; }
        public int CustomerOrderId { get; set; }
        public DateTime ProcessDate { get; set; }
        public string Notes { get; set; } = null!;
        public int ProcessSatelliteId { get; set; }
        public DateTime TransferDatetime { get; set; }
        public int EmployeeRecipientId { get; set; }
        public DateTime CreationDate { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public int EmployeeId { get; set; }

        // Reverse navigation
        public CanceledOrdersInProcess CanceledOrdersInProcess { get; set; } = null!;
        public ICollection<CustomerOrderInProcessDetail> CustomerOrderInProcessDetails { get; set; }
        public ICollection<ModifiedOrdersInProcess> ModifiedOrdersInProcesses { get; set; }
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public Employee EmployeeRecipient { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public ProcessSatellite ProcessSatellite { get; set; } = null!;
        public StatusDocumentType StatusDocumentType { get; set; } = null!;
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
