namespace Aldebaran.DataAccess.Entities.Reports
{
    public class MinimumWarehouseStockReport
    {
        // Información del artículo
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string InternalReference { get; set; } = string.Empty; // Referencia interna del artículo
        public string LineName { get; set; } = string.Empty;
        
        // Información de la referencia
        public int ReferenceId { get; set; }
        public string ReferenceName { get; set; } = string.Empty; // Nombre de la referencia
        public string ReferenceCode { get; set; } = string.Empty; // Código de la referencia
        
        // Cantidades agregadas por referencia
        public int TotalPhysicalStock { get; set; } // Existencia física total sumando todas las bodegas
        public int CommittedQuantity { get; set; } // Cantidad comprometida (pedidos + reservas)
        public int AvailableQuantity { get; set; } // Cantidad disponible (física - comprometida)
        
        // Información específica por bodega
        public short? WarehouseId { get; set; } // Cambiado de int? a short? para coincidir con smallint
        public string? WarehouseName { get; set; }
        public int? PhysicalStock { get; set; } // Existencia física en esta bodega específica
    }
}