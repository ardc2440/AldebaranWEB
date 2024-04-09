using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class NotificationProviderSettingConfiguration : IEntityTypeConfiguration<NotificationProviderSetting>
    {
        public void Configure(EntityTypeBuilder<NotificationProviderSetting> builder)
        {
            builder.ToTable("notification_provider_settings", "dbo");
            builder.HasKey(x => x.NotificationProviderSettingId).IsClustered();
            builder.Property(x => x.NotificationProviderSettingId).HasColumnName(@"NOTIFICATION_PROVIDER_SETTING_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.Subject).HasColumnName(@"SUBJECT").HasColumnType("varchar(20)").IsRequired().IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Settings).HasColumnName(@"SETTINGS").HasColumnType("ntext").IsRequired().IsUnicode(true);
        }
    }
}
