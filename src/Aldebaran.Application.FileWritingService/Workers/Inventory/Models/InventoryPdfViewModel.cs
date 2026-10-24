namespace Aldebaran.Application.FileWritingService.Workers.Inventory.Models
{
    public class InventoryPdfViewModel
    {
        public DateTime Now => DateTime.Now;
        public List<Line> Lines { get; set; } = null!;

        public class Line
        {
            public string LineName { get; set; } = null!;
            public List<Item> Items { get; set; } = null!;
        }
        public class Item
        {
            public string InternalReference { get; set; } = null!;
            public string ItemName { get; set; } = null!;
            public List<Reference> References { get; set; } = null!;
        }
        public class Reference
        {
            public string ReferenceName { get; set; } = null!;
            public int AvailableAmount { get; set; }
            public int LocalWarehouse { get; set; }
            public int FreeZone { get; set; }
            public List<PurchaseOrder> PurchaseOrders { get; set; } = null!;
        }
        public class PurchaseOrder
        {
            public DateTime? Date { get; set; }
            public string? Warehouse { get; set; }
            public int Total { get; set; }
            public List<Activity> Activities { get; set; } = null!;
        }
        public class Activity
        {
            public DateTime? Date { get; set; }
            public string? Description { get; set; }
        }
    }
}
