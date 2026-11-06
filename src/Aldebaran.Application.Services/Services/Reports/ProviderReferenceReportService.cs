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
    public class ProviderReferenceReportService : IProviderReferenceReportService
    {
        private readonly IProviderReferenceReportRepository _repository;
        private readonly IMapper _mapper;

        public ProviderReferenceReportService(IProviderReferenceReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ProviderReferenceReportResult> GetReportAsync(ProviderReferenceReportFilter filter, CancellationToken ct = default)
        {
            filter ??= new ProviderReferenceReportFilter();
            var rows = await GetRowsAsync(filter, ct);
            return ProviderReferenceReportTreeBuilder.Build(rows, filter.IncludeInventory);
        }

        public async Task<IReadOnlyList<ProviderReferenceReportExportRow>> GetExportAsync(ProviderReferenceReportFilter filter, CancellationToken ct = default)
        {
            filter ??= new ProviderReferenceReportFilter();
            var rows = await GetRowsAsync(filter, ct);
            return ProviderReferenceReportExportRowBuilder.Build(rows, filter.IncludeInventory);
        }

        [Obsolete("Filtro armado como texto. Se retira en T3 (la pantalla pasa a usar GetReportAsync).")]
        public async Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetProviderReferenceReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<ProviderReferenceReport>>(data);
        }

        private Task<IEnumerable<Entities.ProviderReferenceReport>> GetRowsAsync(ProviderReferenceReportFilter filter, CancellationToken ct)
        {
            var dataFilter = _mapper.Map<Entities.ProviderReferenceReportFilter>(filter);
            return _repository.GetProviderReferenceReportDataAsync(dataFilter, ct);
        }
    }
}
