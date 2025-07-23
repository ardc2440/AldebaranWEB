using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class VisualizedAutomaticCustomerOrderInProcessModification
    {
        public int Id {get; set; }
        public required string ActionType { get; set; }
        public int Employee_Id { get; set; }
        public DateTime Visualized_Date { get; set; }
    }
}
