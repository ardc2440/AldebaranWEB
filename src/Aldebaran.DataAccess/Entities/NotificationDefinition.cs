using Aldebaran.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class NotificationDefinition : ITrackeable
    {
        public int NotificationDefinitionId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int ValidationIntervalMinutes { get; set; }
        public required string ValidationQuery { get; set; }
        public string? QueryParameters { get; set; }
        public required string NotificationMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? LastValidationDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<NotificationDefinitionRole> NotificationDefinitionRoles { get; set; } = new List<NotificationDefinitionRole>();

    }
}
