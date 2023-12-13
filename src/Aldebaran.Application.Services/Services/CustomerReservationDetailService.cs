using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerReservationDetailService : ICustomerReservationDetailService
    {
        private readonly ICustomerReservationDetailRepository _repository;
        private readonly IMapper _mapper;
        public CustomerReservationDetailService(ICustomerReservationDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerReservationDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
