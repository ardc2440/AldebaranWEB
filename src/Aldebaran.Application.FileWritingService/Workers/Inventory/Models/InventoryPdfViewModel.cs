namespace Aldebaran.Application.FileWritingService.Workers.Inventory.Models
{
    public class InventoryPdfViewModel
    {
        public DateTime Now => DateTime.Now;
        public List<Line> Lines { get; set; }

        public class Line
        {
            public string LineName { get; set; }
            public List<Item> Items { get; set; }
        }
        public class Item
        {
            public string InternalReference { get; set; }
            public string ItemName { get; set; }
            public List<Reference> References { get; set; }
        }
        public class Reference
        {
            public string ReferenceName { get; set; }
            public int AvailableAmount { get; set; }
            public int FreeZone { get; set; }
            public List<PurchaseOrder> PurchaseOrders { get; set; }
        }
        public class PurchaseOrder
        {
            public DateTime? Date { get; set; }
            public string? Warehouse { get; set; }
            public int Total { get; set; }
            public List<Activity> Activities { get; set; }
        }
        public class Activity
        {
            public DateTime? Date { get; set; }
            public string? Description { get; set; }
        }
    }
}
