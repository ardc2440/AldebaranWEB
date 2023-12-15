using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

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

        public async Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default)
        {
            var data = await _repository.FindByLoginUserIdAsync(loginUserId, ct);
            return _mapper.Map<Employee?>(data);
        }
    }

}
