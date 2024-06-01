namespace Aldebaran.Web.Pages.ReportPages.Purchase_Orders.ViewModel
{
    public class PurchaseOrderViewModel
    {
        public List<Order> Orders { get; set; }
        public class Order
        {
            public string OrderNumber { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime RequestDate { get; set; }
            public DateTime ExpectedReceiptDate { get; set; }
            public string ProviderName { get; set; }
            public Forwarder Forwarder { get; set; }
            public ForwarderAgent ForwarderAgent { get; set; }
            public string ImportNumber { get; set; }
            public string ShipmentMethodName { get; set; }
            public string EmbarkationPort { get; set; }
            public string ProformaNumber { get; set; }
            public List<Warehouse> Warehouses { get; set; }
            public string StatusDocumentName { get; set; }

        }
        public class Forwarder
        {
            public string ForwarderName { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public string Email { get; set; }
        }
        public class ForwarderAgent
        {
            public string ForwarderAgentName { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public string Email { get; set; }
        }

        public class Warehouse
        {
            public short WarehouseId { get; set; }
            public string WarehouseName { get; set; }
            public List<Line> Lines { get; set; }
        }

        public class Line
        {
            public string LineCode { get; set; }
            public string LineName { get; set; }
            public List<Item> Items { get; set; }
        }
        public class Item
        {
            public string InternalReference { get; set; }
            public string ItemName { get; set; }
            public List<Reference> References { get; set; }
        }
        public class Reference
        {
            public string ReferenceCode { get; set; }
            public string ReferenceName { get; set; }
            public int Amount { get; set; }
            public double Volume { get; set; }
            public double Weight { get; set; }
        }
    }
}
