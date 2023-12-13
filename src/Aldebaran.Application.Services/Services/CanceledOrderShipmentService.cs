using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CanceledOrderShipmentService : ICanceledOrderShipmentService
    {
        private readonly ICanceledOrderShipmentRepository _repository;
        private readonly IMapper _mapper;
        public CanceledOrderShipmentService(ICanceledOrderShipmentRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICanceledOrderShipmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
