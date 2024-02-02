using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CloseCustomerOrderReasonService : ICloseCustomerOrderReasonService
    {
        private readonly ICloseCustomerOrderReasonRepository _repository;
        private readonly IMapper _mapper;
        public CloseCustomerOrderReasonService(ICloseCustomerOrderReasonRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICloseCustomerOrderReasonRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<CloseCustomerOrderReason>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<IEnumerable<CloseCustomerOrderReason>>(data);
        }
    }

}
