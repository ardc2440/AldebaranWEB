namespace Aldebaran.Application.Services.Models.Reports
{
    public class FreezoneVsAvailableReport
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
        public int AvailableAmount { get; set; }
        public int FreeZone { get; set; }
    }
}
