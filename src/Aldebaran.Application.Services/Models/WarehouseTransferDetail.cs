namespace Aldebaran.Application.Services.Models
{
    public class WarehouseTransferDetail
    {
        public int WarehouseTransferDetailId { get; set; }
        public int WarehouseTransferId { get; set; }
        public int ReferenceId { get; set; }
        public int Quantity { get; set; }
        public WarehouseTransfer WarehouseTransfer { get; set; } = null!;
        public ItemReference ItemReference { get; set; } = null!;
    }
}
