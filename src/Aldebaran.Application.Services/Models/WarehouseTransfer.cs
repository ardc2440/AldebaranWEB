namespace Aldebaran.Application.Services.Models
{
    public class WarehouseTransfer
    {
        public int WarhouseTransferId { get; set; }
        public DateTime TransferDate { get; set; }
        public int EmployeeId { get; set; }
        public short OrigenWarehouseId { get; set; }
        public short DestinationWarehouseId { get; set; }
        public string Notes { get; set; }
        public DateTime CreationDate { get; set; }
        public string Nationalization { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public Warehouse OrigenWarehouse { get; set; }
        public Warehouse DestinationWarehouse { get; set; }
        public Employee Employee { get; set; }
        public StatusDocumentType StatusDocumentType { get; set; }
        public ICollection<WarehouseTransferDetail> WarehouseTransferDetails { get; set; }
    }
}
