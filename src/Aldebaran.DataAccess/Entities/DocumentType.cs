namespace Aldebaran.DataAccess.Entities
{
    public class DocumentType
    {
        public short DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = null!;
        public string DocumentTypeCode { get; set; } = null!;
        // Reverse navigation
        public ICollection<AlarmType> AlarmTypes { get; set; }
        public ICollection<CancellationReason> CancellationReasons { get; set; }
        public ICollection<ModificationReason> ModificationReasons { get; set; }
        public ICollection<StatusDocumentType> StatusDocumentTypes { get; set; }
        public ICollection<CancellationRequest> CancellationRequests { get; set; } = null!;
        public ICollection<LocalWarehouseAlarm> LocalWarehouseAlarms { get; set; }
        public DocumentType()
        {
            AlarmTypes = new List<AlarmType>();
            CancellationReasons = new List<CancellationReason>();
            ModificationReasons = new List<ModificationReason>();
            StatusDocumentTypes = new List<StatusDocumentType>();
            LocalWarehouseAlarms = new List<LocalWarehouseAlarm>();
        }
    }
}
