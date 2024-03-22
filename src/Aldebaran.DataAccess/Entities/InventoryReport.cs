namespace Aldebaran.DataAccess.Entities
{
    public class InventoryReport
    {
        public string LineName { get; set; }
        public short LineId { get; set; }
        
        public string InternalReference { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        
        public string ReferenceName { get; set; }
        public int AvailableAmount { get; set; }
        public int FreeZone { get; set; }
        public int ReferenceId { get; set; }
        
        public DateTime OrderDate { get; set; }
        public string Warehouse { get; set; }
        public int Total { get; set; }
        public int PurchaseOrderId { get; set; }
        
        public DateTime ActivityDate { get; set; }
        public string Description { get; set; }
    }
}
