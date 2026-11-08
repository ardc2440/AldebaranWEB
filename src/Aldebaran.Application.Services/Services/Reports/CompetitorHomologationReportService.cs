using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>Orquestador del reporte de homologaciones: obtiene las filas y delega la forma en los builders.</summary>
    public class CompetitorHomologationReportService : ICompetitorHomologationReportService
    {
        private readonly ICompetitorHomologationReportRepository _repository;

        public CompetitorHomologationReportService(ICompetitorHomologationReportRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<CompetitorHomologationReportResult> GetReportAsync(CompetitorHomologationReportFilter filter, CancellationToken ct = default) =>
            CompetitorHomologationReportTreeBuilder.Build(await GetRowsAsync(filter, ct));

        public async Task<IReadOnlyList<CompetitorHomologationReportExportRow>> GetExportAsync(CompetitorHomologationReportFilter filter, CancellationToken ct = default) =>
            CompetitorHomologationReportExportRowBuilder.Build(await GetRowsAsync(filter, ct));

        public async Task<IReadOnlyList<ItemReference>> FilterHomologatedAsync(IEnumerable<ItemReference> catalog, CancellationToken ct = default)
        {
            var (itemIds, referenceIds) = await _repository.GetHomologatedScopeAsync(ct);
            return HomologatedScope.Apply(catalog, itemIds, referenceIds);
        }

        private Task<IEnumerable<Entities.CompetitorHomologationReport>> GetRowsAsync(CompetitorHomologationReportFilter filter, CancellationToken ct)
        {
            filter ??= new CompetitorHomologationReportFilter();
            return _repository.GetReportDataAsync(new Entities.CompetitorHomologationReportFilter
            {
                LineIds = filter.LineIds,
                ItemIds = filter.ItemIds,
                ReferenceIds = filter.ReferenceIds,
                CompetitorCompanyIds = filter.CompetitorCompanyIds,
                CompetitorReference = filter.CompetitorReference,
                CompetitorProductName = filter.CompetitorProductName
            }, ct);
        }
    }

    /// <summary>Regla del alcance homologado aplicada sobre el catálogo del selector (pura, sin BD).</summary>
    public static class HomologatedScope
    {
        public static IReadOnlyList<ItemReference> Apply(IEnumerable<ItemReference> catalog, IEnumerable<int> itemIds, IEnumerable<int> referenceIds)
        {
            var items = itemIds.ToHashSet();
            var references = referenceIds.ToHashSet();
            return (catalog ?? Enumerable.Empty<ItemReference>())
                   .Where(r => items.Contains(r.ItemId) || references.Contains(r.ReferenceId))
                   .ToList();
        }
    }
}
