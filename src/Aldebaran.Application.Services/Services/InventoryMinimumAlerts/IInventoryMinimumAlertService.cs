using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.InventoryMinimumAlerts
{
    public interface IInventoryMinimumAlertService
    {
        Task ExecuteAsync(string imagePath, CancellationToken cancellationToken = default);
    }
}
