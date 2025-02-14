using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities.Reports
{
    public class AutomaticCustomerOrderAssigmentReport
    {
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string ProviderIdentity { get; set; }
        public string ProviderName { get; set; }
        public string ProformaNumber { get; set; }
        public string ImportNumber { get; set; }
        public DateTime ReceipDate { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public int CustomerOrderId { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerIdentity { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string StatusOrderName { get; set; }
        public int ItemId { get; set; }
        public string InternalReference { get; set; }
        public string ItemName { get; set; }
        public string ReferenceName { get; set; }
        public int Requested { get; set; }
        public int Assigned { get; set; }
        public int Pending { get; set; }
    }
}
