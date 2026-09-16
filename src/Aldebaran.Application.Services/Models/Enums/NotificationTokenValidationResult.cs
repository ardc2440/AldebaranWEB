using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Models.Enums
{
    public enum NotificationTokenValidationResult
    {
        Valid,
        NotFound,
        Expired,
        Consumed,
        Success,
        Failed
    }
}
