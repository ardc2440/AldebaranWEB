using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class PurchaseOrderTransitAlarmConfiguration : IEntityTypeConfiguration<PurchaseOrderTransitAlarm>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderTransitAlarm> builder)
        {
            builder.ToTable("purchase_order_transit_alarms", "dbo");
            builder.HasKey(x => x.PurchaseOrderTransitAlarmId).HasName("PK_PURCHASE_ORDER_TRANSIT_ALARM").IsClustered();
            builder.Property(x => x.PurchaseOrderTransitAlarmId).HasColumnName(@"PURCHASE_ORDER_TRANSIT_ALARM_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ModifiedPurchaseOrderId).HasColumnName(@"MODIFIED_PURCHASE_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.OldExpectedReceiptDate).HasColumnName(@"OLD_EXPECTED_RECEIPT_DATE").HasColumnType("date").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.ModifiedPurchaseOrder).WithMany(b => b.PurchaseOrderTransitAlarms).HasForeignKey(c => c.ModifiedPurchaseOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_TRANSIT_ALARM_MODIFIED_PURCHASE_ORDER");
        }
    }
}
