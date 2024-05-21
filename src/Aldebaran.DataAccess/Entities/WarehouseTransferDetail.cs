using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class WarehouseTransferDetail : ITrackeable
    {
        public int WarehouseTransferDetailId { get; set; }
        public int WarehouseTransferId { get; set; }
        public int ReferenceId { get; set; }
        public int Quantity { get; set; }
        public WarehouseTransfer WarehouseTransfer { get; set; }
        public ItemReference ItemReference { get; set; }
    }
}
