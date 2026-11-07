using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Competitors
{
    public interface ICompetitorCompanyRepository
    {
        /// <summary>Compañías con su ubicación. <paramref name="searchKey"/> busca en nombre, sitio web, contacto y teléfono.</summary>
        Task<IReadOnlyList<CompetitorCompany>> GetAsync(bool includeInactive, string? searchKey, CancellationToken ct = default);
        Task<CompetitorCompany?> FindAsync(int competitorCompanyId, CancellationToken ct = default);
        /// <summary>true si existe otra compañía ACTIVA con el mismo nombre (sin distinguir mayúsculas ni espacios extremos).</summary>
        Task<bool> ExistsActiveByNameAsync(string companyName, int? excludeCompetitorCompanyId, CancellationToken ct = default);
        /// <summary>true si el departamento pertenece al país y la ciudad al departamento (los nulos no se validan).</summary>
        Task<bool> IsValidLocationAsync(int? countryId, int? departmentId, int? cityId, CancellationToken ct = default);
        Task<int> AddAsync(CompetitorCompany company, CancellationToken ct = default);
        Task UpdateAsync(int competitorCompanyId, CompetitorCompany company, CancellationToken ct = default);
        Task SetActiveAsync(int competitorCompanyId, bool isActive, CancellationToken ct = default);
    }
}
