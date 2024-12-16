using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class VisualizedMinimumQuantityAlarmConfiguration : IEntityTypeConfiguration<VisualizedMinimumQuantityAlarm>
    {
        public void Configure(EntityTypeBuilder<VisualizedMinimumQuantityAlarm> builder)
        {
            builder.ToTable("visualized_minimum_quantity_alarms", "dbo");
            builder.HasKey(x => new { x.MinimumQuantityAlarmId, x.EmployeeId }).HasName("PK_VISUALIZED_MINIMUM_QUANTITY_ALARM").IsClustered();
            builder.Property(x => x.MinimumQuantityAlarmId).HasColumnName(@"MINIMUM_QUANTITY_ALARM_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.VisualizedDate).HasColumnName(@"VIZUALIZED_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Employee).WithMany(b => b.VisualizedMinimumQuantityAlarms).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_MINIMUM_QUANTITY_ALARM_EMPLOYEE");
        }
    }
}
