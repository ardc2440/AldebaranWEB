namespace Aldebaran.Application.Services.Models
{
    public class CustomerOrderShipmentDetail
    {
        public int CustomerOrderShipmentDetailId { get; set; }
        public int CustomerOrderShipmentId { get; set; }
        public int CustomerOrderDetailId { get; set; }
        public short WarehouseId { get; set; }
        public int DeliveredQuantity { get; set; }
        public CustomerOrderDetail CustomerOrderDetail { get; set; }
        public CustomerOrderShipment CustomerOrderShipment { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
