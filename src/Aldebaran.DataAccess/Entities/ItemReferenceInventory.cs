namespace Aldebaran.DataAccess.Entities
{
    /// <summary>
    /// Fila de SP_GET_ITEM_REFERENCES_INVENTORY: inventario de una referencia activa del artículo
    /// de una alarma de cantidades mínimas. Entidad keyless (solo lectura).
    /// </summary>
    public class ItemReferenceInventory
    {
        public int ReferenceId { get; set; }
        public string ReferenceCode { get; set; } = string.Empty;
        public string ReferenceName { get; set; } = string.Empty;
        public int LocalWarehouse { get; set; }
        public int FreeZone { get; set; }
        public int PhysicalStock { get; set; }
        public int InTransit { get; set; }
        public int Committed { get; set; }
        public int Available { get; set; }
        public bool IsAlarmReference { get; set; }
    }
}
