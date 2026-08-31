using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Models
{
    public class PurchaseOrderApprovalResult
    {
        public bool RequiresApproval { get; set; }
        public int RequestedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public int Difference { get; set; }
        public int MaximumAllowedDifference { get; set; }
        public decimal AllowedDifferencePercent { get; set; }
        public int PurchaseOrderApprovalRangeId { get; set; }
    }
}
