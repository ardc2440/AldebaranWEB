using Aldebaran.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedPurchaseOrderTransitAlarmRepository
    {
        Task AddAsync(VisualizedPurchaseOrderTransitAlarm item, CancellationToken ct = default);
    }
}
