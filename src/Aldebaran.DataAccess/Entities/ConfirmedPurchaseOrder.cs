using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class ConfirmedPurchaseOrder
    {
        public int AlarmId { get; set; }
        public int PurchaseOrderId { get; set; }
        public required string OrderNumber { get; set; }
        public required string IdentityNumber { get; set; }
        public required string ProviderName { get; set; }
        public required string ProformaNumber { get; set; }
        public required string ImportNumber { get; set; }
        public DateTime ReceptionDate { get; set; }
        public DateTime ConfirmationDate { get; set; }
    }
}
