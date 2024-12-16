using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CancellationRequest : ITrackeable
    {
        public int RequestId { get; set; }
        public short DocumentTypeId { get; set; }    
        public int DocumentNumber { get; set; }
        public int RequestEmployeeId { get; set; }
        public int? ResponseEmployeeId { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string? ResponseReason { get; set; }
        public Employee RequestEmployee { get; set; }
        public Employee? ResponseEmployee { get; set; }
        public DocumentType DocumentType { get; set; }
        public StatusDocumentType StatusDocumentType { get; set; }


        public CancellationRequest() 
        {
            RequestDate = DateTime.Now;
            ResponseDate = DateTime.Now;
            ResponseEmployeeId = null;
        } 
    }
}
