using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderActivityService : IPurchaseOrderActivityService
    {
        private readonly IPurchaseOrderActivityRepository _repository;
        private readonly IMapper _mapper;
        public PurchaseOrderActivityService(IPurchaseOrderActivityRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderActivityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
