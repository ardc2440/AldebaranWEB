using Aldebaran.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface INotificationDefinitionRepository
    {
        Task<IEnumerable<NotificationDefinition>> GetActiveAsync(CancellationToken ct = default);
        Task UpdateLastValidationDateAsync(int notificationDefinitionId, DateTime validationDate, CancellationToken ct = default);
        Task<int> ExecuteValidationQueryAsync(string query, CancellationToken ct = default);
        Task<IEnumerable<string>> GetRolesAsync(int notificationDefinitionId, CancellationToken ct = default);
    }
}
