using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class VisualizedAutomaticInProcessConfiguration : IEntityTypeConfiguration<VisualizedAutomaticInProcess>
    {
        public void Configure(EntityTypeBuilder<VisualizedAutomaticInProcess> builder)
        {
            builder.ToTable("Visualized_Automatic_In_Process", "dbo");
            builder.HasKey(x => new { x.AlarmId, x.EmployeeId }).HasName("PK_VISUALIZED_AUTOMATIC_IN_PROCESS").IsClustered();
            builder.Property(x => x.AlarmId).HasColumnName(@"AUTOMATIC_IN_PROCESS_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.VisualizedDate).HasColumnName(@"VISUALIZED_DATE").HasColumnType("date").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Employee).WithMany(b => b.VisualizedAutomaticInProcesses).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VISUALIZED_ALARM_EMPLOYEE");
        }
    }
}
