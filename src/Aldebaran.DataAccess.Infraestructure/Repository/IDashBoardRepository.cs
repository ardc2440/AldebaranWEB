using Aldebaran.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IDashBoardRepository
    {
        Task<IEnumerable<PurchaseOrderDetail>> GetByReferenceIdAndStatusOrderAsync(int statusOrder, int? referenceId = null, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantityAsync(CancellationToken ct = default);
        Task<IEnumerable<CustomerReservation>> GetExpiredReservationsAsync(CancellationToken ct = default);
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
        Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default);
        Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default);
        Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderTransitAlarm>> GetAllTransitAlarmAsync(int employeeId, CancellationToken ct = default);
    }
}
