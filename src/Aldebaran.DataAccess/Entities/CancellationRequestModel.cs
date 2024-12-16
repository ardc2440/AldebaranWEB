namespace Aldebaran.DataAccess.Entities
{
    public class CancellationRequestModel 
    {
        public int CancellationRequestId { get; set; }
        public string DocumentTypeName { get; set; }
        public int DocumentNumber { get; set; }
        public string RequestBy { get; set; }
        public string? ResponseBy { get; set; }
        public string StatusDocumentTypeName { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string? ResponseReason { get; set; }
        public string OrderNumber { get; set; }
        public string ThirdPart { get; set; }
        public string IdentityNumber { get; set; }
        public string DocumentTypeCode { get; set; }
    }
}
