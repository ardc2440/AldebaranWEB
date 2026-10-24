namespace Aldebaran.Application.Services.Models.Reports
{
    public class PurchaseOrderReport
    {
        public int OrderId { get; set; }
        public string StatusDocumentTypeName { get; set; } = null!;
        public string OrderNumber { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ExpectedReceiptDate { get; set; }

        public string ProviderName { get; set; } = null!;
        public string ImportNumber { get; set; } = null!;
        public string ShipmentMethodName { get; set; } = null!;
        public string EmbarkationPort { get; set; } = null!;
        public string ProformaNumber { get; set; } = null!;

        public string ForwarderName { get; set; } = null!;
        public string ForwarderPhone { get; set; } = null!;
        public string ForwarderFax { get; set; } = null!;
        public string ForwarderEmail { get; set; } = null!;

        public string ForwarderAgentName { get; set; } = null!;
        public string AgentPhone { get; set; } = null!;
        public string AgentFax { get; set; } = null!;
        public string AgentEmail { get; set; } = null!;

        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;

        public short LineId { get; set; }
        public string LineCode { get; set; } = null!;
        public string LineName { get; set; } = null!;

        public int ItemId { get; set; }
        public string InternalReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;

        public string ReferenceCode { get; set; } = null!;
        public string ReferenceName { get; set; } = null!;
        public int Amount { get; set; }
        public double Volume { get; set; }
        public double Weight { get; set; }
        public int RequestedAmount { get; set; }
    }
}
