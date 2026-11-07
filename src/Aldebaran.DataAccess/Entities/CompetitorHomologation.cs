using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    /// <summary>
    /// Homologación de un artículo (y opcionalmente una referencia) de PROMOS con un producto de una compañía
    /// de la competencia (dbo.COMPETITOR_HOMOLOGATIONS). Informativa; se inactiva, no se elimina.
    /// </summary>
    public class CompetitorHomologation : ITrackeable
    {
        public int CompetitorHomologationId { get; set; }
        public int CompetitorCompanyId { get; set; }
        public int ItemId { get; set; }
        public int? ReferenceId { get; set; }
        public string CompetitorReference { get; set; } = null!;
        public string CompetitorProductName { get; set; } = null!;
        public string? ProductUrl { get; set; }
        public decimal? CompetitorPrice { get; set; }
        public string? ImageFileName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreationDate { get; set; }

        public CompetitorCompany CompetitorCompany { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public ItemReference? ItemReference { get; set; }
    }
}
