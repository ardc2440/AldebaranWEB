using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class VisualizedAlarmConfiguration : IEntityTypeConfiguration<VisualizedAlarm>
    {
        public void Configure(EntityTypeBuilder<VisualizedAlarm> builder)
        {
            builder.ToTable("visualized_alarms", "dbo");
            builder.HasKey(x => new { x.AlarmId, x.EmployeeId }).HasName("PK_VISUALIZED_ALARM").IsClustered();
            builder.Property(x => x.AlarmId).HasColumnName(@"ALARM_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.VisualizedDate).HasColumnName(@"VISUALIZED_DATE").HasColumnType("date").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Alarm).WithMany(b => b.VisualizedAlarms).HasForeignKey(c => c.AlarmId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_ALARM_ALARMS");
            builder.HasOne(a => a.Employee).WithMany(b => b.VisualizedAlarms).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_ALARM_EMPLOYEE");
        }
    }
}
