using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;


namespace Aldebaran.Application.Services
{
    public class ReferenceMovementReportService : IReferenceMovementReportService
    {
        private readonly IReferenceMovementReportRepository _repository;
        private readonly IMapper _mapper;

        public ReferenceMovementReportService(IReferenceMovementReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IProviderReferenceReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<ReferenceMovementReport>> GetReferenceMovementReportDataAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetReferenceMovementReportDataAsync(ct);
            return _mapper.Map<IEnumerable<ReferenceMovementReport>>(data);
        }
    }
}
