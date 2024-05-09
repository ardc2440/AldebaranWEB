

using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class VisualizedPurchaseOrderTransitAlarmConfiguration : IEntityTypeConfiguration<VisualizedPurchaseOrderTransitAlarm>
    {
        public void Configure(EntityTypeBuilder<VisualizedPurchaseOrderTransitAlarm> builder)
        {
            builder.ToTable("visualized_purchase_order_transit_alarms", "dbo");
            builder.HasKey(x => new { x.PurchaseOrderTransitAlarmId, x.EmployeeId }).HasName("PK_VISUALIZED_PURCHASE_ORDER_TRANSIT_ALARM").IsClustered();
            builder.Property(x => x.PurchaseOrderTransitAlarmId).HasColumnName(@"PURCHASE_ORDER_TRANSIT_ALARM_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            // Foreign keys
            builder.HasOne(a => a.PurchaseOrderTransitAlarm).WithMany(b => b.VisualizedAlarms).HasForeignKey(c => c.PurchaseOrderTransitAlarmId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_PURCHASE_ORDER_TRANSIT_ALARM_PURCHASE_ORDER_TRANSIT_ALARM");
            builder.HasOne(a => a.Employee).WithMany(b => b.VisualizedPurchaseOrderTransitAlarms).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_PURCHASE_ORDER_TRANSIT_ALARM_EMPLOYEE");
        }
    }
}
