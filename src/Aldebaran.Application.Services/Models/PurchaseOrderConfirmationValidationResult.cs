using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Models
{
    public class PurchaseOrderConfirmationValidationResult
    {
        public bool RequiresApproval { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
