using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ModifiedOrderShipmentService : IModifiedOrderShipmentService
    {
        private readonly IModifiedOrderShipmentRepository _repository;
        private readonly IMapper _mapper;
        public ModifiedOrderShipmentService(IModifiedOrderShipmentRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IModifiedOrderShipmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
