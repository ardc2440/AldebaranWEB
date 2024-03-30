using Aldebaran.Application.Services.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Reports
{
    public interface IWarehouseTransferReportService
    {
        Task<IEnumerable<WarehouseTransferReport>> GetWarehouseTransferReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
