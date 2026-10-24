namespace Aldebaran.DataAccess.Entities.Reports
{
    public class ReferenceMovementReport
    {
        public short LineId { get; set; }
        public string LineCode { get; set; } = null!;
        public string LineName { get; set; } = null!;

        public int ItemId { get; set; }
        public string InternalReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;

        public int ReferenceId { get; set; }
        public string ReferenceCode { get; set; } = null!;
        public string ReferenceName { get; set; } = null!;
        public int ReservedQuantity { get; set; }
        public int RequestedQuantity { get; set; }

        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public int Amount { get; set; }

        public short? TitleId { get; set; }
        public string? Title { get; set; }
                
        public string? Code { get; set; }
        public DateTime? Date { get; set; }
        public string? Owner { get; set; }
        public int? MovementAmount { get; set; }
        public string? Status { get; set; }
        public short? Operator { get; set; }
    }
}
