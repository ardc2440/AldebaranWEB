using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

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

        public async Task AddRangeAsync(IEnumerable<UsersAlarmType> items, CancellationToken ct = default)
        {
            var entities = _mapper.Map<List<Entities.UsersAlarmType>>(items) ?? throw new ArgumentNullException("Alarma del usuario no puede ser nula.");
            await _repository.AddRangeAsync(entities, ct);
        }

        public async Task DeleteAsync(short alarmTypeId, int employeeId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(alarmTypeId, employeeId, ct);
        }
    }
}
