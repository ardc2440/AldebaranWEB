using Aldebaran.Application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderReportService
    {
        Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(CancellationToken ct = default);
    }
}
