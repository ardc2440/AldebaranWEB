using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class UsersAlarmTypeService : IUsersAlarmTypeService
    {
        private readonly IUsersAlarmTypeRepository _repository;
        private readonly IMapper _mapper;
        public UsersAlarmTypeService(IUsersAlarmTypeRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IUsersAlarmTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
