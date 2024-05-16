using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Configuration
{
    public class CustomerReservationNotificationConfiguration : IEntityTypeConfiguration<CustomerReservationNotification>
    {
        public void Configure(EntityTypeBuilder<CustomerReservationNotification> builder)
        {
            builder.ToTable("purchase_order_notifications", "dbo");
            builder.HasKey(x => x.CustomerReservationNotificationId).HasName("PK_CUSTOMER_RESERVATION_NOTIFICATIONS").IsClustered();
            builder.Property(x => x.CustomerReservationNotificationId).HasColumnName(@"CUSTOMER_RESERVATION_NOTIFICATION_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.NotificationTemplateId).HasColumnName(@"NOTIFICATION_TEMPLATE_ID").HasColumnType("SMALLINT").IsRequired();
            builder.Property(x => x.NotificationId).HasColumnName(@"NOTIFICATION_ID").HasColumnType("varchar(50)").IsRequired();

            builder.Property(x => x.NotificationDate).HasColumnName(@"NOTIFICATION_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.NotifiedMailList).HasColumnName(@"NOTIFIED_MAIL_LIST").HasColumnType("varchar(max)").IsRequired();
            builder.Property(x => x.CustomerReservationId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.NotificationState).HasColumnName(@"NOTIFICATION_STATE").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.NotificationSendingErrorMessage).HasColumnName(@"NOTIFICATION_SENDING_ERROR_MESSAGE").HasColumnType("varchar(MAX)").IsRequired(false).IsUnicode(false);
            // Foreign keys
            builder.HasOne(a => a.CustomerReservation).WithMany(b => b.CustomerReservationNotifications).HasForeignKey(c => c.CustomerReservationId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_RESERVATION_NOTIFICATIONS_CUSTOMER_ORDER");
            builder.HasOne(a => a.NotificationTemplate).WithMany(b => b.CustomerReservationNotifications).HasForeignKey(c => c.NotificationTemplateId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_RESERVATION_NOTIFICATIONS_NOTIFICATIONS_NOTIFICATION_TEMPLATE");
        }
    }
}
