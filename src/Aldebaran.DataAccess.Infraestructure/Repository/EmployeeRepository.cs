using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AldebaranDbContext _context;
        public EmployeeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default)
        {
            return await _context.Employees.AsNoTracking().FirstOrDefaultAsync(f => f.LoginUserId == loginUserId, ct);
        }
    }

}
