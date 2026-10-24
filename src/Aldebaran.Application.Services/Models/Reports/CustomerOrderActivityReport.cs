namespace Aldebaran.Application.Services.Models.Reports
{
    public class CustomerOrderActivityReport
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Fax { get; set; }

        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public bool IsSpecialOrder { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string StatusOrder { get; set; } = null!;
        public string? InternalNotes { get; set; }
        public string? CustomerNotes { get; set; }


        public int ReferenceId { get; set; }
        public string ItemReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string ReferenceCode { get; set; } = null!;
        public string ReferenceName { get; set; } = null!;
        public int Amount { get; set; }
        public int DeliveredAmount { get; set; }
        public int InProcessAmount { get; set; }
        public string StatusDetail { get; set; } = null!;


        public int ActivityId { get; set; }
        public DateTime CreationDateActivity { get; set; }
        public string AreaName { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public string? Notes { get; set; }

        public string? ActivityType { get; set; }
        public string? EmployeNameDetail { get; set; }
    }
}
