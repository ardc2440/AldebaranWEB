using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// Orquestador de los casos de uso del reporte: obtiene las filas del repositorio
    /// y delega la forma del resultado en los builders (árbol o exportación).
    /// </summary>
    public class ItemReferencesReportService : IItemReferencesReportService
    {
        private readonly IItemReferencesReportRepository _repository;
        private readonly IMapper _mapper;

        public ItemReferencesReportService(IItemReferencesReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ItemReferencesReportResult> GetReportAsync(ItemReferencesReportFilter filter, CancellationToken ct = default)
        {
            var rows = await GetRowsAsync(filter, ct);
            return ItemReferencesReportTreeBuilder.Build(rows);
        }

        public async Task<IReadOnlyList<ItemReferencesReportExportRow>> GetExportAsync(ItemReferencesReportFilter filter, CancellationToken ct = default)
        {
            var rows = await GetRowsAsync(filter, ct);
            return ItemReferencesReportExportRowBuilder.Build(rows);
        }

        private Task<IEnumerable<Entities.ItemReferencesReport>> GetRowsAsync(ItemReferencesReportFilter filter, CancellationToken ct)
        {
            var dataFilter = _mapper.Map<Entities.ItemReferencesReportFilter>(filter ?? new ItemReferencesReportFilter());
            return _repository.GetItemReferencesReportDataAsync(dataFilter, ct);
        }
    }
}
