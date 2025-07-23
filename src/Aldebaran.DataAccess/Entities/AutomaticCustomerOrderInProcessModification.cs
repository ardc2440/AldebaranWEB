using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class AutomaticCustomerOrderInProcessModification
    {
        public int Id { get; set; }
        public int CUSTOMER_ORDER_IN_PROCESS_ID { get; set; }
        public required string ActionType { get; set; }
        public DateTime ActionDate { get; set; } 
        public required string ActionReason { get; set; } 
        public required string EmployeeName { get; set; } 
        public required string Order_Number { get; set; }        
        public required string Customer_Name { get; set; }  
    }
}

