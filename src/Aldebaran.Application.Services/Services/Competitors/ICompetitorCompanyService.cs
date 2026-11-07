using Aldebaran.Application.Services.Models.Competitors;

namespace Aldebaran.Application.Services.Competitors
{
    /// <summary>Casos de uso de las compañías de la competencia (rol "Catálogo de la competencia").</summary>
    public interface ICompetitorCompanyService
    {
        Task<IReadOnlyList<CompetitorCompany>> GetAsync(bool includeInactive = false, string? searchKey = null, CancellationToken ct = default);
        Task<CompetitorCompany?> FindAsync(int competitorCompanyId, CancellationToken ct = default);

        /// <summary>Crea la compañía. Lanza ArgumentException con los errores de captura o InvalidOperationException si el nombre ya existe entre las activas.</summary>
        Task<int> AddAsync(CompetitorCompanyInput input, CancellationToken ct = default);
        Task UpdateAsync(int competitorCompanyId, CompetitorCompanyInput input, CancellationToken ct = default);
        Task InactivateAsync(int competitorCompanyId, CancellationToken ct = default);
        /// <summary>Reactiva la compañía; InvalidOperationException si ya hay otra activa con el mismo nombre.</summary>
        Task ActivateAsync(int competitorCompanyId, CancellationToken ct = default);
    }
}
