namespace Aldebaran.DataAccess.Entities.Reports
{
    public class WarehouseTransferReport
    {
        public int TransferId { get; set; }
        public DateTime Date { get; set; } /*Transfer Date */
        public string SourceWarehouseName { get; set; } = null!;
        public string TargetWarehouseName { get; set; } = null!;        
        public DateTime RegistrationDate { get; set; } /* Creation Date */
        public string NationalizationNumber { get; set; } = null!;
        public string TransferStatus { get; set; } = null!;

        public int ReferenceId { get; set; }
        public string ItemReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string ReferenceCode { get; set; } = null!;
        public string ReferenceName { get; set; } = null!;
        public int Amount { get; set; }
    }
}
