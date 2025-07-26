namespace Aldebaran.DataAccess.Entities
{
    public class AutomaticCustomerOrderInProcessModification
    {
        public int Id { get; set; }
        public int CUSTOMER_ORDER_IN_PROCESS_ID { get; set; }
        public int CUSTOMER_ORDER_ID { get; set; }
        public required string ActionType { get; set; }
        public DateTime ActionDate { get; set; } 
        public required string ActionReason { get; set; } 
        public required string EmployeeName { get; set; } 
        public required string Order_Number { get; set; }        
        public required string Customer_Name { get; set; }  
    }
}

