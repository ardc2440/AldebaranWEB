using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class NotificationDefinitionConfiguration        : IEntityTypeConfiguration<NotificationDefinition>
    {
        public void Configure(            EntityTypeBuilder<NotificationDefinition> builder)
        {
            builder.ToTable("notification_definitions");

            builder.HasKey(x => x.NotificationDefinitionId);

            builder.Property(x => x.NotificationDefinitionId)
                .HasColumnName("NOTIFICATION_DEFINITION_ID")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("NAME")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("DESCRIPTION")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.ValidationIntervalMinutes)
                .HasColumnName("VALIDATION_INTERVAL_MINUTES")
                .IsRequired();

            builder.Property(x => x.ValidationQuery)
                .HasColumnName("VALIDATION_QUERY")
                .IsRequired();

            builder.Property(x => x.QueryParameters)
                .HasColumnName("QUERY_PARAMETERS");

            builder.Property(x => x.NotificationMessage)
                .HasColumnName("NOTIFICATION_MESSAGE")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("IS_ACTIVE")
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .HasColumnName("CREATED_DATE")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(x => x.UpdatedDate)
                .HasColumnName("UPDATED_DATE")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(x => x.LastValidationDate)
                .HasColumnName("LAST_VALIDATION_DATE")
                .HasColumnType("datetime");
        }
    }
}