namespace Aldebaran.Application.Services.Models
{
    public class DocumentType
    {
        public short DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentTypeCode { get; set; }
        // Reverse navigation
        public ICollection<AlarmType> AlarmTypes { get; set; }
        public ICollection<CancellationReason> CancellationReasons { get; set; }
        public ICollection<ModificationReason> ModificationReasons { get; set; }
        public ICollection<StatusDocumentType> StatusDocumentTypes { get; set; }
        public DocumentType()
        {
            AlarmTypes = new List<AlarmType>();
            CancellationReasons = new List<CancellationReason>();
            ModificationReasons = new List<ModificationReason>();
            StatusDocumentTypes = new List<StatusDocumentType>();
        }
    }
}
