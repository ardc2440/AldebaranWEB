using Aldebaran.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderAdjustmentLog : ITrackeable
    {
        public int PurchaseOrderAdjustmentLogId { get; set; }
        public int PurchaseOrderId { get; set; }
        public short NewStatusDocumentTypeId { get; set; }
        public required string Reason { get; set; } 
        public int EmployeeId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
