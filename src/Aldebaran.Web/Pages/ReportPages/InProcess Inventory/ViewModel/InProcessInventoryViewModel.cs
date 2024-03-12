namespace Aldebaran.Web.Pages.ReportPages.InProcess_Inventory.ViewModel
{
    public class InProcessInventoryViewModel
    {
        public List<InProcessItem> Items { get; set; }
    }
    public class InProcessItem
    {
        public string InternalReference { get; set; }
        public string ItemName { get; set; }
        public List<InProcessReference> References { get; set; }
    }
    public class InProcessReference
    {
        public string ReferenceName { get; set; }
        public int InProcessAmount { get; set; }
        public List<InProcessWarehouse> Warehouses { get; set; }
    }

    public class InProcessWarehouse
    {
        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Amount { get; set; }
    }

}
