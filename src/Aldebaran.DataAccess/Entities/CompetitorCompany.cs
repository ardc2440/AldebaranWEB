using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Compañía de la competencia (dbo.COMPETITOR_COMPANIES). Informativa; se inactiva, no se elimina.</summary>
    public class CompetitorCompany : ITrackeable
    {
        public int CompetitorCompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Website { get; set; } = null!;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public int? CountryId { get; set; }
        public int? DepartmentId { get; set; }
        public int? CityId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreationDate { get; set; }

        public Country? Country { get; set; }
        public Department? Department { get; set; }
        public City? City { get; set; }
    }
}
