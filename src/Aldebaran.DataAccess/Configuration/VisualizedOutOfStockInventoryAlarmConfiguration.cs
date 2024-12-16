using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class VisualizedOutOfStockInventoryAlarmConfiguration : IEntityTypeConfiguration<VisualizedOutOfStockInventoryAlarm>
    {
        public void Configure(EntityTypeBuilder<VisualizedOutOfStockInventoryAlarm> builder)
        {
            builder.ToTable("visualized_out_of_stock_inventory_alarms", "dbo");
            builder.HasKey(x => new { x.OutOfStockInventoryAlarmId, x.EmployeeId }).HasName("PK_VISUALIZED_OUT_OF_STOCK_INVENTORY_ALARM").IsClustered();
            builder.Property(x => x.OutOfStockInventoryAlarmId).HasColumnName(@"OUT_OF_STOCK_INVENTORY_ALARM_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.VisualizedDate).HasColumnName(@"VIZUALIZED_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Employee).WithMany(b => b.VisualizedOutOfStockInventoryAlarms).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_OUT_OF_STOCK_INVENTORY_ALARM_EMPLOYEE");
        }
    }
}
