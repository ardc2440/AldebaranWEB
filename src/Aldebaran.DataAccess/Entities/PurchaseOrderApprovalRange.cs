using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderApprovalRange
    {
        public int PurchaseOrderApprovalRangeId { get; set; }

        public int RequestedQuantityFrom { get; set; }

        public int RequestedQuantityTo { get; set; }

        public decimal AllowedDifferencePercent { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public int EmployeeId {  get; set; }

        public Employee? Employee { get; set; }

        public PurchaseOrderApprovalRange()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
    }
}
