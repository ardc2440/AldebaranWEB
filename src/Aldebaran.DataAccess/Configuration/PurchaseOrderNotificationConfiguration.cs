using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderNotificationConfiguration : IEntityTypeConfiguration<PurchaseOrderNotification>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderNotification> builder)
        {
            builder.ToTable("purchase_order_notifications", "dbo");
            builder.HasKey(x => x.PurchaseOrderNotificationId).HasName("PK_PURCHASE_ORDER_NOTIFICATIONS").IsClustered();
            builder.Property(x => x.PurchaseOrderNotificationId).HasColumnName(@"PURCHASE_ORDER_NOTIFICATION_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ModifiedPurchaseOrderId).HasColumnName(@"MODIFIED_PURCHASE_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.NotificationId).HasColumnName(@"NOTIFICATION_ID").HasColumnType("varchar(50)").IsRequired();

            builder.Property(x => x.NotificationDate).HasColumnName(@"NOTIFICATION_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.NotifiedMailList).HasColumnName(@"NOTIFIED_MAIL_LIST").HasColumnType("varchar(max)").IsRequired();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.NotificationState).HasColumnName(@"NOTIFICATION_STATE").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.NotificationSendingErrorMessage).HasColumnName(@"NOTIFICATION_SENDING_ERROR_MESSAGE").HasColumnType("varchar(MAX)").IsRequired(false).IsUnicode(false);
            // Foreign keys
            builder.HasOne(a => a.CustomerOrder).WithMany(b => b.PurchaseOrderNotifications).HasForeignKey(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_NOTIFICATIONS_CUSTOMER_ORDER");
            builder.HasOne(a => a.ModifiedPurchaseOrder).WithMany(b => b.PurchaseOrderNotifications).HasForeignKey(c => c.ModifiedPurchaseOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_NOTIFICATIONS_MODIFIED_PURCHASE_ORDER");
        }
    }
}
