namespace Aldebaran.Application.Services.Models
{
    public class ReferencesWarehouse
    {
        public int ReferenceId { get; set; }
        public short WarehouseId { get; set; }
        public int Quantity { get; set; }
        public ItemReference ItemReference { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
    }
}
