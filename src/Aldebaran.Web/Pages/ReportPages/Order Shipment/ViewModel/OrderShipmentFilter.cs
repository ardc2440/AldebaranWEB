using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared.ViewModel;

namespace Aldebaran.Web.Pages.ReportPages.Order_Shipment.ViewModel
{
    public class OrderShipmentFilter : ICloneable
    {
        public string OrderNumber { get; set; }
        public DateRange CreationDate { get; set; } = new();
        public DateRange RequestDate { get; set; } = new();
        public DateRange ExpectedReceiptDate { get; set; } = new();
        public DateRange RealReceiptDate { get; set; } = new();
        public string ImportNumber { get; set; }
        public string EmbarkationPort { get; set; }
        public string ProformaNumber { get; set; }
        public int? ProviderId { get; set; }
        public Provider Provider { get; set; }
        public int? ForwarderId { get; set; }
        public Forwarder Forwarder { get; set; }
        public int? ForwarderAgentId { get; set; }
        public ForwarderAgent ForwarderAgent { get; set; }
        public short? ShipmentMethodId { get; set; }
        public ShipmentMethod ShipmentMethod { get; set; }
        public short? WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
