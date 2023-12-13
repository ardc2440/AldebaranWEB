namespace Aldebaran.Application.Services.Models
{
    public class CanceledOrderShipment
    {
        public int CustomerOrderShipmentId { get; set; }
        public short CancellationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CancellationDate { get; set; }
        public CancellationReason CancellationReason { get; set; }
        public CustomerOrderShipment CustomerOrderShipment { get; set; }
        public Employee Employee { get; set; }
        public CanceledOrderShipment()
        {
            CancellationDate = DateTime.Now;
        }

    }
}
