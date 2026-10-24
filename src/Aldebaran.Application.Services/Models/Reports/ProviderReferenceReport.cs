namespace Aldebaran.Application.Services.Models.Reports
{
    public class ProviderReferenceReport
    {
        public int ProviderId { get; set; }
        public string ProviderCode { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string ProviderAddress { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ContactPerson { get; set; } = null!;

        public short LineId { get; set; }
        public string LineCode { get; set; } = null!;
        public string LineName { get; set; } = null!;

        public int ItemId { get; set; }
        public string InternalReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;

        public int ReferenceId { get; set; }
        public string ReferenceCode { get; set; } = null!;
        public string ReferenceName { get; set; } = null!;
        public string ProviderReferenceName { get; set; } = null!;
        public int ConfirmedAmount { get; set; }
        public int ReservedAmount { get; set; }
        public int AvailableAmount { get; set; }

        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public int Amount { get; set; }
    }
}

