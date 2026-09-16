using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.InventoryMinimumAlerts.Models
{
    internal class TokenExtraDataDto
    {
        public int EmployeeId { get; set; }
        public ICollection<int> AlarmIds { get; set; } = new List<int>();
    }
}
