namespace Aldebaran.DataAccess.Entities.Reports
{
    public class InventoryReport
    {
        public string LineName { get; set; } = null!;
        public short LineId { get; set; }

        public string InternalReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public int ItemId { get; set; }

        public string ReferenceName { get; set; } = null!;
        public int AvailableAmount { get; set; }
        public int LocalWarehouse { get; set; }
        public int FreeZone { get; set; }
        public int ReferenceId { get; set; }

        public DateTime? OrderDate { get; set; }
        public string? Warehouse { get; set; }
        public int? Total { get; set; }
        public int? PurchaseOrderId { get; set; }

        public DateTime? ActivityDate { get; set; }
        public string? Description { get; set; }
        public int? ActivityId { get; set; }
    }
}
