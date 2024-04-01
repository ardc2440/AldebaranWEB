using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class FreezoneVsAvailableReportService : IFreezoneVsAvailableReportService
    {
        private readonly IFreezoneVsAvailableReportRepository _repository;
        private readonly IMapper _mapper;

        public FreezoneVsAvailableReportService(IFreezoneVsAvailableReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IFreezoneVsAvailableReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<FreezoneVsAvailableReport>> GetFreezoneVsAvailableReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            var data = await _repository.GetFreezoneVsAvailableReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<FreezoneVsAvailableReport>>(data);
        }
    }
}
