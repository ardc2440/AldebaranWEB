using Aldebaran.DataAccess.Entities.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IWarehouseTransferReportRepository
    {
        Task<IEnumerable<WarehouseTransferReport>> GetWarehouseTransferReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
