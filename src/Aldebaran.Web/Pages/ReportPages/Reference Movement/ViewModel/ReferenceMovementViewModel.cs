namespace Aldebaran.Web.Pages.ReportPages.Reference_Movement.ViewModel
{
    public class ReferenceMovementViewModel
    {
        public List<ReferenceMovementLine> Lines { get; set; }

    }
    public class ReferenceMovementLine
    {
        public string LineCode { get; set; }
        public string LineName { get; set; }
        public List<ReferenceMovementItem> Items { get; set; }
    }
    public class ReferenceMovementItem
    {
        public string InternalReference { get; set; }
        public string ItemName { get; set; }
        public List<ReferenceMovementReference> References { get; set; }

    }

    public class ReferenceMovementReference
    {
        public string ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public int ReservedQuantity { get; set; }
        public int RequestedQuantity { get; set; }
        public List<ReferenceMovementWarehouse> Warehouses { get; set; }
    }

    public class ReferenceMovementWarehouse
    {
        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Amount { get; set; }
    }
}
