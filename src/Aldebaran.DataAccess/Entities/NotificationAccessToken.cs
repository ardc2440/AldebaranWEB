using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class NotificationAccessToken
    {
        public Guid NotificationAccessTokenId { get; set; }
        public short NotificationTemplateId { get; set; }
        public string? ExtraData { get; set; }
        public DateTime GeneratedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsConsumed { get; set; }
        public DateTime? ConsumedDate { get; set; }

        public virtual NotificationTemplate NotificationTemplate { get; set; } = null!;
    }
}
