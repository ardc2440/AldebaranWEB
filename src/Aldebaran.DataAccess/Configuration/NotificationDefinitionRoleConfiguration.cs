using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class NotificationDefinitionRoleConfiguration : IEntityTypeConfiguration<NotificationDefinitionRole>
    {
        public void Configure(EntityTypeBuilder<NotificationDefinitionRole> builder)
        {
            builder.ToTable("notification_definition_roles");

            builder.HasKey(x => x.NotificationDefinitionRoleId);

            builder.Property(x => x.NotificationDefinitionRoleId)
                .HasColumnName("NOTIFICATION_DEFINITION_ROLE_ID")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.NotificationDefinitionId)
                .HasColumnName("NOTIFICATION_DEFINITION_ID")
                .IsRequired();

            builder.Property(x => x.RoleId)
                .HasColumnName("ROLE_ID")
                .HasMaxLength(450)
                .IsRequired();

            builder.HasOne(x => x.NotificationDefinition)
                .WithMany(x => x.NotificationDefinitionRoles)
                .HasForeignKey(x => x.NotificationDefinitionId)
                .HasConstraintName("FK_NDR_NOTIFICATION_DEFINITION");

            builder.HasIndex(x => new { x.NotificationDefinitionId, x.RoleId })
                .IsUnique()
                .HasDatabaseName("UQ_NOTIFICATION_DEFINITION_ROLE");
        }
    }
}