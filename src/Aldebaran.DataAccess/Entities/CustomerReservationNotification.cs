using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class CustomerReservationNotification : NotificationBase
    {
        public int CustomerReservationNotificationId { get; set; }
        public short NotificationTemplateId { get; set; }
        public int CustomerReservationId { get; set; }

        // Reverse navigation
        public CustomerReservation CustomerReservation { get; set; } = null!;
        public NotificationTemplate NotificationTemplate { get; set; } = null!;
    }
}
