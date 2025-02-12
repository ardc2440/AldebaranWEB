namespace Aldebaran.Application.Services.Models
{
    public class AutomaticCustomerOrderDetail
    {
        public required int ReferenceId { get; set; }
        public required string ArticleName { get; set; }
        public int Requested { get; set; }
        public int Assigned { get; set; }
        public int Pending { get; set; }
    }
}
