namespace Aldebaran.DataAccess.Entities
{
    public class ProviderReferenceReport
    {
        public int ProviderId { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ProviderAddress { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }

        public short LineId { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }

        public int ItemId { get; set; }
        public string InternalReference { get; set; }
        public string ItemName { get; set; }

        public int ReferenceId { get; set; }
        public string ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public string ProviderReferenceName { get; set; }
        public int ConfirmedAmount { get; set; }
        public int ReservedAmount { get; set; }
        public int AvailableAmount { get; set; }

        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Amount { get; set; }
    }
}
