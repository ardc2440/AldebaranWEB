using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerReservationService : ICustomerReservationService
    {
        private readonly ICustomerReservationRepository _repository;
        private readonly IMapper _mapper;
        public CustomerReservationService(ICustomerReservationRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerReservationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
