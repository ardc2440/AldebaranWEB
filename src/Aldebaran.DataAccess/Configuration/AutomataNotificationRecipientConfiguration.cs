using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class AutomataNotificationRecipientConfiguration : IEntityTypeConfiguration<AutomataNotificationRecipient>
    {
        public void Configure(EntityTypeBuilder<AutomataNotificationRecipient> builder)
        {
            builder.ToTable("AUTOMATA_NOTIFICATION_RECIPIENTS");
            builder.HasKey(e => e.Id).HasName("PK_AUTOMATA_NOTIFICATION_RECIPIENTS");
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("INT").IsRequired();
            builder.Property(e => e.Email).HasColumnName("Email").HasColumnType("NVARCHAR(256)").IsRequired();
            builder.Property(e => e.NotificationType).HasColumnName("NotificationType").HasColumnType("NVARCHAR(100)").IsRequired();
            builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnType("BIT").IsRequired(false).HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("DATETIME2(3)").IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("DATETIME2(3)").IsRequired(false);
            builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("NVARCHAR(1000)").IsRequired(false);
            builder.HasIndex(e => e.NotificationType).HasDatabaseName("IX_AUTOMATA_NOTIFICATION_RECIPIENTS_NotificationType");
        }
    }
}
