using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class NotificationDefinitionRoleViewConfiguration
    : IEntityTypeConfiguration<NotificationDefinitionRoleView>
    {
        public void Configure(
            EntityTypeBuilder<NotificationDefinitionRoleView> builder)
        {
            builder.ToView("vw_notification_definition_roles");

            builder.HasNoKey();

            builder.Property(x => x.NotificationDefinitionId)
                .HasColumnName("NOTIFICATION_DEFINITION_ID");

            builder.Property(x => x.RoleId)
                .HasColumnName("ROLE_ID");

            builder.Property(x => x.RoleName)
                .HasColumnName("ROLE_NAME");
        }
    }
}
