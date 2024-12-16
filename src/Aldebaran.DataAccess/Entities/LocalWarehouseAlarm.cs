using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class LocalWarehouseAlarm: ITrackeable
    {
        public int LocalWarehouseAlarmId { get; set; }
        public short DocumentTypeId { get; set; }
        public int DocumentNumber { get; set; }
        public string ReferenceList { get; set; } = string.Empty;
        public string? CustomerOrderList { get; set; } 
        public DocumentType? DocumentType { get; set; } 
        public DateTime AlarmDate { get; set; }
        public ICollection<VisualizedLocalWarehouseAlarm> VisualizedLocalWarehouseAlarms { get; set; } = new List<VisualizedLocalWarehouseAlarm>();
    }
}
