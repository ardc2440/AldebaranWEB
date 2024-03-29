using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Models.Reports
{
    public class CustomerOrderActivityReport
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string? Fax { get; set; }

        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string StatusOrder { get; set; }
        public string? InternalNotes { get; set; }
        public string? CustomerNotes { get; set; }


        public string ItemReference { get; set; }
        public string ItemName { get; set; }
        public string ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public int Amount { get; set; }
        public int DeliveredAmount { get; set; }
        public int InProcessAmount { get; set; }
        public string StatusDetail { get; set; }


        public int ActivityId { get; set; }
        public DateTime CreationDateActivity { get; set; }
        public string AreaName { get; set; }
        public string EmployeeName { get; set; }
        public string? Notes { get; set; }
        public string StatusActivity { get; set; }

        public string? ActivityType { get; set; }
        public string? EmployeNameDetail { get; set; }
    }
}
