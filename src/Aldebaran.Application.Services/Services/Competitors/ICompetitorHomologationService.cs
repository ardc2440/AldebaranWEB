using Aldebaran.Application.Services.Models.Competitors;

namespace Aldebaran.Application.Services.Competitors
{
    /// <summary>Casos de uso de las homologaciones con la competencia (rol "Catálogo de la competencia").</summary>
    public interface ICompetitorHomologationService
    {
        Task<IReadOnlyList<CompetitorHomologation>> GetAsync(CompetitorHomologationFilter filter, CancellationToken ct = default);
        Task<CompetitorHomologation?> FindAsync(int competitorHomologationId, CancellationToken ct = default);

        /// <summary>Crea la homologación. ArgumentException con errores de captura; InvalidOperationException si ya existe una activa igual.</summary>
        Task<int> AddAsync(CompetitorHomologationInput input, CancellationToken ct = default);
        Task UpdateAsync(int competitorHomologationId, CompetitorHomologationInput input, CancellationToken ct = default);
        Task InactivateAsync(int competitorHomologationId, CancellationToken ct = default);
        /// <summary>Reactiva; InvalidOperationException si ya hay otra activa igual o la compañía está inactiva.</summary>
        Task ActivateAsync(int competitorHomologationId, CancellationToken ct = default);
    }
}
