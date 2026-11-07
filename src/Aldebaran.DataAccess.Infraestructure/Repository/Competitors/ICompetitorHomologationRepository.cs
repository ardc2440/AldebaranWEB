using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Competitors
{
    public interface ICompetitorHomologationRepository
    {
        /// <summary>Homologaciones con compañía, artículo (y línea) y referencia.</summary>
        Task<IReadOnlyList<CompetitorHomologation>> GetAsync(CompetitorHomologationQuery query, CancellationToken ct = default);
        Task<CompetitorHomologation?> FindAsync(int competitorHomologationId, CancellationToken ct = default);
        /// <summary>true si existe otra homologación ACTIVA igual (compañía + artículo + referencia PROMOS + referencia competencia).</summary>
        Task<bool> ExistsActiveAsync(int competitorCompanyId, int itemId, int? referenceId, string competitorReference, int? excludeCompetitorHomologationId, CancellationToken ct = default);
        Task<bool> IsActiveCompanyAsync(int competitorCompanyId, CancellationToken ct = default);
        Task<bool> ItemExistsAsync(int itemId, CancellationToken ct = default);
        /// <summary>true si la referencia pertenece al artículo.</summary>
        Task<bool> ReferenceBelongsToItemAsync(int referenceId, int itemId, CancellationToken ct = default);
        Task<int> AddAsync(CompetitorHomologation homologation, CancellationToken ct = default);
        Task UpdateAsync(int competitorHomologationId, CompetitorHomologation homologation, CancellationToken ct = default);
        Task SetActiveAsync(int competitorHomologationId, bool isActive, CancellationToken ct = default);
        Task SetImageFileNameAsync(int competitorHomologationId, string? imageFileName, CancellationToken ct = default);
    }
}
