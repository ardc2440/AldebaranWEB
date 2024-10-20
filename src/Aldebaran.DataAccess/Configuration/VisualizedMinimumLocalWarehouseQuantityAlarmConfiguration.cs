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
    public class VisualizedMinimumLocalWarehouseQuantityAlarmConfiguration : IEntityTypeConfiguration<VisualizedMinimumLocalWarehouseQuantityAlarm>
    {
        public void Configure(EntityTypeBuilder<VisualizedMinimumLocalWarehouseQuantityAlarm> builder)
        {
            builder.ToTable("visualized_minimum_local_warehouse_quantity_alarms", "dbo");
            builder.HasKey(x => new { x.AlarmId, x.EmployeeId }).HasName("PK_VISUALIZED_MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM").IsClustered();
            builder.Property(x => x.AlarmId).HasColumnName(@"MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.VisualizedDate).HasColumnName(@"VIZUALIZED_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Employee).WithMany(b => b.VisualizedMinimumLocalWarehouseQuantityAlarms).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_EMPLOYEE");
        }
    }
}
