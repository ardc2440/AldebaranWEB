using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        public EmployeeService(IEmployeeRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(Employee employee, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Employee>(employee) ?? throw new ArgumentNullException("Empleado no puede ser nulo.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(int employeeId, CancellationToken ct = default)
        {
            await _repository.DeleteAsync(employeeId, ct);
        }

        public async Task<Employee?> FindAsync(int employeeId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(employeeId, ct);
            return _mapper.Map<Employee?>(data);
        }

        public async Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default)
        {
            var data = await _repository.FindByLoginUserIdAsync(loginUserId, ct);
            return _mapper.Map<Employee?>(data);
        }

        public async Task<IEnumerable<Employee>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<Employee>>(data);
        }

        public async Task<IEnumerable<Employee>> GetByAreaAsync(short areaId, CancellationToken ct = default)
        {
            var data = await _repository.GetByAreaAsync(areaId, ct);
            return _mapper.Map<List<Employee>>(data);
        }

        public async Task<IEnumerable<Employee>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<List<Employee>>(data);
        }

        public async Task UpdateAsync(int employeeId, Employee employee, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Employee>(employee) ?? throw new ArgumentNullException("Empleado no puede ser nulo.");
            await _repository.UpdateAsync(employeeId, entity, ct);
        }

        public async Task<IEnumerable<Employee>> GetByAlarmTypeAsync(short alarmTypeId, CancellationToken ct = default)
        {
            var data = await _repository.GetByAlarmTypeAsync(alarmTypeId, ct);
            return _mapper.Map<List<Employee>>(data);
        }
    }
}
