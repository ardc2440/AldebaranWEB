using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderApprovalRangeLog
    {
        public int PurchaseOrderApprovalRangeLogId { get; set; }

        public int PurchaseOrderApprovalRangeId { get; set; }

        public int PreviousRequestedQuantityFrom { get; set; }

        public int PreviousRequestedQuantityTo { get; set; }

        public decimal PreviousAllowedDifferencePercent { get; set; }

        public bool PreviousIsActive { get; set; }

        public string ChangeReason { get; set; } = string.Empty;

        public int ChangedByEmployeeId { get; set; }

        public DateTime ChangedDate { get; set; }

        public virtual Employee? ChangedByEmployee { get; set; }
    }
}