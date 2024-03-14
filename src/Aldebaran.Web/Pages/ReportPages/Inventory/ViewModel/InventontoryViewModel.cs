namespace Aldebaran.Web.Pages.ReportPages.Inventory.ViewModel
{
    public class InventontoryViewModel
    {
        public List<InventoryLine> Lines { get; set; }
    }
    public class InventoryLine
    {
        public string LineName { get; set; }
        public List<InventoryItem> Items { get; set; }
    }
    public class InventoryItem
    {
        public string InternalReference { get; set; }
        public string ItemName { get; set; }
        public List<InventoryReference> InventoryReferences { get; set; }        
    }
    public class InventoryReference
    {
        public string ReferenceName { get; set; }
        public int AvailableAmount { get; set; }
        public int FreeZone { get; set; }
        public List<InventoryPurchaseOrder> PurchaseOrders { get; set; }
    }
    public class InventoryPurchaseOrder
    {
        public DateTime Date { get; set; }
        public string Warehouse { get; set; }
        public int Total { get; set; }
        public List<InventoryActivity> Activity { get; set; }
    }
    public class InventoryActivity
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
