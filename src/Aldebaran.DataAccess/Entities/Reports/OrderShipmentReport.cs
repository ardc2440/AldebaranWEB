namespace Aldebaran.DataAccess.Entities.Reports
{
    public class OrderShipmentReport
    {
        public int OrderId { get; set; }
        public string StatusDocumentTypeName { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ExpectedReceiptDate { get; set; }
        public string ProviderName { get; set; }
        public string? ImportNumber { get; set; }
        public string? ShipmentMethodName { get; set; }
        public string EmbarkationPort { get; set; }
        public string ProformaNumber { get; set; }

        public string? ForwarderName { get; set; }
        public string? ForwarderPhone { get; set; }
        public string? ForwarderFax { get; set; }
        public string? ForwarderEmail { get; set; }

        public string? ForwarderAgentName { get; set; }
        public string? AgentPhone { get; set; }
        public string? AgentFax { get; set; }
        public string? AgentEmail { get; set; }

        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public short LineId { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }

        public int ItemId { get; set; }
        public string InternalReference { get; set; }
        public string ItemName { get; set; }

        public string ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public int Amount { get; set; }
        public double Volume { get; set; }
        public double Weight { get; set; }
    }
}
