using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class CustomerOrderNotificationConfiguration : IEntityTypeConfiguration<CustomerOrderNotification>
    {
        public void Configure(EntityTypeBuilder<CustomerOrderNotification> builder)
        {
            builder.ToTable("customer_order_notifications", "dbo");
            builder.HasKey(x => x.CustomerOrderNotificationId).HasName("PK_CUSTOMER_ORDER_NOTIFICATIONS").IsClustered();
            builder.Property(x => x.CustomerOrderNotificationId).HasColumnName(@"CUSTOMER_ORDER_NOTIFICATION_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.NotificationTemplateId).HasColumnName(@"NOTIFICATION_TEMPLATE_ID").HasColumnType("SMALLINT").IsRequired();
            builder.Property(x => x.NotificationId).HasColumnName(@"NOTIFICATION_ID").HasColumnType("varchar(50)").IsRequired();

            builder.Property(x => x.NotificationDate).HasColumnName(@"NOTIFICATION_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.NotifiedMailList).HasColumnName(@"NOTIFIED_MAIL_LIST").HasColumnType("varchar(max)").IsRequired();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.NotificationState).HasColumnName(@"NOTIFICATION_STATE").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.NotificationSendingErrorMessage).HasColumnName(@"NOTIFICATION_SENDING_ERROR_MESSAGE").HasColumnType("varchar(MAX)").IsRequired(false).IsUnicode(false);
            // Foreign keys
            builder.HasOne(a => a.CustomerOrder).WithMany(b => b.CustomerOrderNotifications).HasForeignKey(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_NOTIFICATIONS_CUSTOMER_ORDER");
            builder.HasOne(a => a.NotificationTemplate).WithMany(b => b.CustomerOrderNotifications).HasForeignKey(c => c.NotificationTemplateId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_NOTIFICATIONS_NOTIFICATION_TEMPLATE");
        }
    }
}
