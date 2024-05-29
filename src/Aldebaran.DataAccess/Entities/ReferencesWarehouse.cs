using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class ReferencesWarehouse : ITrackeable
    {
        public int ReferenceId { get; set; }
        public short WarehouseId { get; set; }
        public int Quantity { get; set; }
        public ItemReference ItemReference { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
