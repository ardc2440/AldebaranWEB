namespace Aldebaran.Web.Pages.ReportPages.MinimumWarehouseStock.ViewModel
{
    public class MinimumWarehouseStockViewModel
    {
        public List<ItemWithReferences> Items { get; set; } = new List<ItemWithReferences>();
    }

    public class ItemWithReferences
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string InternalReference { get; set; } = string.Empty; // Referencia interna del artículo
        public string LineName { get; set; } = string.Empty;
        public List<ReferenceWithStock> References { get; set; } = new List<ReferenceWithStock>();
    }

    public class ReferenceWithStock
    {
        public int ReferenceId { get; set; }
        public string ReferenceName { get; set; } = string.Empty; // Nombre de la referencia
        public string ReferenceCode { get; set; } = string.Empty; // Código de la referencia
        public int TotalPhysicalStock { get; set; } // Existencia física total sumando todas las bodegas
        public int CommittedQuantity { get; set; } // Cantidad comprometida (pedidos + reservas)
        public int AvailableQuantity { get; set; } // Cantidad disponible (física - comprometida)
        public List<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
    }

    public class WarehouseStock
    {
        public short WarehouseId { get; set; } // Cambiado de int a short para coincidir con smallint
        public string WarehouseName { get; set; } = string.Empty;
        public int PhysicalStock { get; set; } // Existencia física en esta bodega específica
    }
}