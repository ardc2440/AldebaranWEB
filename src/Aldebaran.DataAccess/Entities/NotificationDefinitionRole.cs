using Aldebaran.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class NotificationDefinitionRole
    {
        public int NotificationDefinitionRoleId { get; set; }
        public int NotificationDefinitionId { get; set; }
        public required String RoleId { get; set; }

        public NotificationDefinition NotificationDefinition { get; set; } = null!;

    }
}
