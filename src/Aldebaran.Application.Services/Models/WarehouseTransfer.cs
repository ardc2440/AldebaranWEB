namespace Aldebaran.Application.Services.Models
{
    public class WarehouseTransfer
    {
        public int WarehouseTransferId { get; set; }
        public DateTime TransferDate { get; set; }
        public int EmployeeId { get; set; }
        public short OriginWarehouseId { get; set; }
        public short DestinationWarehouseId { get; set; }
        public string Notes { get; set; }
        public DateTime CreationDate { get; set; }
        public string Nationalization { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public Warehouse OriginWarehouse { get; set; }
        public Warehouse DestinationWarehouse { get; set; }
        public Employee Employee { get; set; }
        public StatusDocumentType StatusDocumentType { get; set; }
        public ICollection<WarehouseTransferDetail> WarehouseTransferDetails { get; set; }
    }
}
