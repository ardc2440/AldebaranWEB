using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Reports
{
    public class CustomerOrderActivityReportService : ICustomerOrderActivityReportService
    {
        private readonly ICustomerOrderActivityReportRepository _repository;
        private readonly IMapper _mapper;

        public CustomerOrderActivityReportService(ICustomerOrderActivityReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderActivityReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task<IEnumerable<CustomerOrderActivityReport>> GetCustomerOrderActivityReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            var data = await _repository.GetCustomerOrderActivityReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<CustomerOrderActivityReport>>(data);
        }
    }
}
