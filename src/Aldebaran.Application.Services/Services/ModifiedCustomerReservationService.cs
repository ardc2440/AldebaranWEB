using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ModifiedCustomerReservationService : IModifiedCustomerReservationService
    {
        private readonly IModifiedCustomerReservationRepository _repository;
        private readonly IMapper _mapper;
        public ModifiedCustomerReservationService(IModifiedCustomerReservationRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IModifiedCustomerReservationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
