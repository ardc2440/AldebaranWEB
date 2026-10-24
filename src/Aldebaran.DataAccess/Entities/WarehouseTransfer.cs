using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class WarehouseTransfer : ITrackeable
    {
        public int WarehouseTransferId { get; set; }
        public DateTime TransferDate { get; set; }
        public int EmployeeId { get; set; }
        public short OriginWarehouseId { get; set; }
        public short DestinationWarehouseId { get; set; }
        public string Notes { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string Nationalization { get; set; } = null!;
        public short StatusDocumentTypeId { get; set; }
        public Warehouse OriginWarehouse { get; set; } = null!;
        public Warehouse DestinationWarehouse { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public StatusDocumentType StatusDocumentType { get; set; } = null!;
        public ICollection<WarehouseTransferDetail> WarehouseTransferDetails { get; set; } = null!;
    }
}
