namespace Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.ViewModel
{
    public class InventoryAdjustmentsViewModel
    {
        public List<Adjustment> Adjustments { get; set; }
        public class Adjustment
        {
            public int AdjustmentId { get; set; }
            public DateTime AdjustmentDate { get; set; }
            public DateTime CreationDate { get; set; }
            public string AdjustmentType { get; set; }
            public string AdjustmentReason { get; set; }
            public string Employee { get; set; }
            public string Notes { get; set; }
            public List<Warehouse> Warehouses { get; set; }
        }

        public class Warehouse
        {
            public short WarehouseId { get; set; }
            public string WarehouseName { get; set; }
            public List<Line> Lines { get; set; }
        }

        public class Line
        {
            public string LineCode { get; set; }
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
            public string ReferenceCode { get; set; }
            public string ReferenceName { get; set; }
            public int AvailableAmount { get; set; }

        }
    }
}
