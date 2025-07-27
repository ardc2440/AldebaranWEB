namespace Aldebaran.Web.Pages.ReportPages.MinimumWarehouseStock.ViewModel
{
    public class MinimumWarehouseStockViewModel
    {
        public List<Warehouse> Warehouses { get; set; } = new List<Warehouse>();

        public class Warehouse
        {
            public short WarehouseId { get; set; }
            public string WarehouseName { get; set; } = string.Empty;
            public List<Line> Lines { get; set; } = new List<Line>();
        }

        public class Line
        {
            public string LineName { get; set; } = string.Empty;
            public string LineCode { get; set; } = string.Empty;
            public List<Item> Items { get; set; } = new List<Item>();
        }

        public class Item
        {
            public string InternalReference { get; set; } = string.Empty;
            public string ItemName { get; set; } = string.Empty;
            public List<Reference> References { get; set; } = new List<Reference>();
        }

        public class Reference
        {
            public string ReferenceName { get; set; } = string.Empty;
            public string ReferenceCode { get; set; } = string.Empty;
            public string ProviderReferenceName { get; set; } = string.Empty;
            public int AvailableAmount { get; set; }
            public int MinimumQuantity { get; set; }
            public int AlarmMinimumQuantity { get; set; }
            public decimal MinimumQuantityPercent { get; set; }
            public bool IsBelowMinimum { get; set; }
            public int DeficitAmount { get; set; } // Cantidad faltante para llegar al mínimo
        }
    }
}