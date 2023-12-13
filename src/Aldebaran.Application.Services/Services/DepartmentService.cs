using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;

        public DepartmentService(IDepartmentRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IDepartmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<Department?> FindAsync(int departmentId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(departmentId, ct);
            return _mapper.Map<Department?>(data);
        }

        public async Task<IEnumerable<Department>> GetAsync(int countryId, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(countryId, ct);
            return _mapper.Map<List<Department>>(data);
        }
    }
}
