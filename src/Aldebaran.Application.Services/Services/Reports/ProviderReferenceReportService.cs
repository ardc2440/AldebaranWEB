using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{

    public class ProviderReferenceReportService : IProviderReferenceReportService
    {
        private readonly IProviderReferenceReportRepository _repository;
        private readonly IMapper _mapper;

        public ProviderReferenceReportService(IProviderReferenceReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IProviderReferenceReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetProviderReferenceReportDataAsync(ct);
            return _mapper.Map<IEnumerable<ProviderReferenceReport>>(data);
        }
    }
}
