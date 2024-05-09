using Aldebaran.Application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderTransitAlarmService
    {
        Task AddAsync(PurchaseOrderTransitAlarm item, CancellationToken ct = default);
    }
}
