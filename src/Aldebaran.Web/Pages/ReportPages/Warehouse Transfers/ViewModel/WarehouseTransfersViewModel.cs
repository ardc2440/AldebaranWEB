namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.ViewModel
{
    public class WarehouseTransfersViewModel
    {
        public List<WarehouseTransfer> WarehouseTransfers { get; set; }
        public class WarehouseTransfer
        {
            public DateTime Date { get; set; }
            public string SourceWarehouseName { get; set; }
            public string TargetWarehouseName { get; set; }
            public string Reason { get; set; }
            public DateTime RegistrationDate { get; set; }
            public string NationalizationNumber { get; set; }
            public List<Reference> References { get; set; }
            public int Total => References?.Sum(w => w.Amount) ?? 0;
        }
        public class Reference
        {
            public string ItemReference { get; set; }
            public string ItemName { get; set; }
            public string ReferenceCode { get; set; }
            public string ReferenceName { get; set; }
            public int Amount { get; set; }
        }
    }
}
