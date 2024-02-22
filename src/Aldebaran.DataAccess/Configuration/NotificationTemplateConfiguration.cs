using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
        {
            builder.ToTable("notification_templates", "dbo");
            builder.HasKey(x => x.NotificationTemplateId).IsClustered();
            builder.Property(x => x.NotificationTemplateId).HasColumnName(@"NOTIFICATION_TEMPLATE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.Name).HasColumnName(@"NAME").HasColumnType("varchar(100)").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.Subject).HasColumnName(@"SUBJECT").HasColumnType("varchar(100)").IsRequired(true).IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.Message).HasColumnName(@"MESSAGE").HasColumnType("ntext").IsRequired().IsUnicode(true);
        }
    }
}
