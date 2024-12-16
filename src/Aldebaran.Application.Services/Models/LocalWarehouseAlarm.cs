namespace Aldebaran.Application.Services.Models
{
    public class LocalWarehouseAlarm
    {
        public int LocalWarehouseAlarmId { get; set; }
        public short DocumentTypeId { get; set; }
        public int DocumentNumber { get; set; }
        public string ReferenceList { get; set; } = string.Empty;
        public string? CustomerOrderList { get; set; } 
        public DocumentType? DocumentType { get; set; } 
        public DateTime AlarmDate { get; set; }
        public ICollection<VisualizedLocalWarehouseAlarm> VisualizedLocalWarehouseAlarms { get; set; } = new List<VisualizedLocalWarehouseAlarm>();
        public ICollection<AlarmCustomerOrder> AlarmCustomerOrders { get; set; } = new List<AlarmCustomerOrder>();
        public ICollection<AlarmReference> AlarmReferences { get; set; } = new List<AlarmReference>();
    }

    public class AlarmCustomerOrder
    {
        public CustomerOrder? CustomerOrder {  get; set; }    
    }

    public class AlarmReference
    {
        public ItemReference? ItemReference { get; set; }
    }
}
