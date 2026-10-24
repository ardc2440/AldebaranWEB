namespace Aldebaran.DataAccess.Entities
{
    public class CancellationRequestModel 
    {
        public int CancellationRequestId { get; set; }
        public string DocumentTypeName { get; set; } = null!;
        public int DocumentNumber { get; set; }
        public string RequestBy { get; set; } = null!;
        public string? ResponseBy { get; set; }
        public string StatusDocumentTypeName { get; set; } = null!;
        public DateTime RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string? ResponseReason { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string ThirdPart { get; set; } = null!;
        public string IdentityNumber { get; set; } = null!;
        public string DocumentTypeCode { get; set; } = null!;
    }
}
