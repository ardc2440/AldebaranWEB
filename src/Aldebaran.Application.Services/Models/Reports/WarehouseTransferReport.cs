namespace Aldebaran.Application.Services.Models.Reports
{
    public class WarehouseTransferReport
    {
        public int TransferId { get; set; }
        public DateTime Date { get; set; } /*Transfer Date*/
        public string SourceWarehouseName { get; set; }
        public string TargetWarehouseName { get; set; }
        public DateTime RegistrationDate { get; set; } /* Creation Date */
        public string NationalizationNumber { get; set; }
        public string TransferStatus { get; set; }

        public int ReferenceId { get; set; }
        public string ItemReference { get; set; }
        public string ItemName { get; set; }
        public string ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public int Amount { get; set; }
    }
}
