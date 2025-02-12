using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class AutomaticCustomerOrderDetail
    {
        public required int ReferenceId { get; set; }
        public required string ArticleName { get; set; }
        public int Requested { get; set; }
        public int Assigned { get; set; }
        public int Pending { get; set; }
    }
}
