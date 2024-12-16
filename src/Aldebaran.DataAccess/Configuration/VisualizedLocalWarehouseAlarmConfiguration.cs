using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class VisualizedLocalWarehouseAlarmConfiguration : IEntityTypeConfiguration<VisualizedLocalWarehouseAlarm>
    {
        public void Configure(EntityTypeBuilder<VisualizedLocalWarehouseAlarm> builder)
        {
            builder.ToTable("Visualized_Local_Warehouse_Alarms", "dbo");
            builder.HasKey(x => new { x.LocalWarehouseAlarmId, x.EmployeeId }).HasName("PK_VISUALIZED_LOCAL_WAREHOUSE_ALARM").IsClustered();
            builder.Property(x => x.LocalWarehouseAlarmId).HasColumnName(@"LOCAL_WAREHOUSE_ALARM_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.VisualizedDate).HasColumnName(@"VISUALIZED_DATE").HasColumnType("date").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.LocalWarehouseAlarm).WithMany(b => b.VisualizedLocalWarehouseAlarms).HasForeignKey(c => c.LocalWarehouseAlarmId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_ALARM_ALARMS");
            builder.HasOne(a => a.Employee).WithMany(b => b.VisualizedLocalWarehouseAlarms).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_ALARM_EMPLOYEE");
        }
    }
}
