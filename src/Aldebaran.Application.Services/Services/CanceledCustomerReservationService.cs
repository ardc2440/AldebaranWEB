using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CanceledCustomerReservationService : ICanceledCustomerReservationService
    {
        private readonly ICanceledCustomerReservationRepository _repository;
        private readonly IMapper _mapper;
        public CanceledCustomerReservationService(ICanceledCustomerReservationRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICanceledCustomerReservationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
