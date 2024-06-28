using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class NotificationWithError
    {
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string? Reason { get; set; }
        public string NotifiedMailList { get; set; }
        public DateTime NotificationDate { get; set; }
        public string? NotificationSendingErrorMessage { get; set; }

        public NotificationWithError()
        {
            Description = "";
            CustomerName = "";
            Reason = "";
            NotificationDate = DateTime.Now;
            NotificationSendingErrorMessage = "";
            NotifiedMailList = "";
        }
    }
}
