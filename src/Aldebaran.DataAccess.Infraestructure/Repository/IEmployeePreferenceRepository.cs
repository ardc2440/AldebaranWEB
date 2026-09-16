using Aldebaran.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IEmployeePreferenceRepository
    {
        Task<EmployeePreference?> FindAsync(int employeeId, CancellationToken ct = default);
        Task AddAsync(EmployeePreference entity, CancellationToken ct = default);
        Task UpdateAsync(EmployeePreference entity, CancellationToken ct = default);
    }
}
