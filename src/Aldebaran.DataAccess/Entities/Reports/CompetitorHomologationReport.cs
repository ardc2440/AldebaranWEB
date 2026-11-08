namespace Aldebaran.DataAccess.Entities.Reports
{
    /// <summary>Fila de SP_GET_COMPETITOR_HOMOLOGATION_REPORT (una por homologación).</summary>
    public class CompetitorHomologationReport
    {
        public int CompetitorCompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Website { get; set; } = null!;
        public bool IsCompanyActive { get; set; }
        public short LineId { get; set; }
        public string LineName { get; set; } = null!;
        public int ItemId { get; set; }
        public string InternalReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public int? ReferenceId { get; set; }
        public string? ReferenceCode { get; set; }
        public string? ReferenceName { get; set; }
        public int CompetitorHomologationId { get; set; }
        public string CompetitorReference { get; set; } = null!;
        public string CompetitorProductName { get; set; } = null!;
        public string? ProductUrl { get; set; }
        public decimal? CompetitorPrice { get; set; }
        public string? ImageFileName { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>Parámetros de SP_GET_COMPETITOR_HOMOLOGATION_REPORT.</summary>
    public class CompetitorHomologationReportFilter
    {
        public IEnumerable<short> LineIds { get; set; } = Enumerable.Empty<short>();
        public IEnumerable<int> ItemIds { get; set; } = Enumerable.Empty<int>();
        public IEnumerable<int> ReferenceIds { get; set; } = Enumerable.Empty<int>();
        public IEnumerable<int> CompetitorCompanyIds { get; set; } = Enumerable.Empty<int>();
        public string? CompetitorReference { get; set; }
        public string? CompetitorProductName { get; set; }
        public bool IncludeInactive { get; set; }
    }
}
