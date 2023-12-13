using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class UsersAlarmTypeConfiguration : IEntityTypeConfiguration<UsersAlarmType>
    {
        public void Configure(EntityTypeBuilder<UsersAlarmType> builder)
        {
            builder.ToTable("USERS_ALARM_TYPE", "dbo");
            builder.HasKey(x => new { x.AlarmTypeId, x.EmployeeId }).HasName("PK_USER_ALARM_TYPE").IsClustered();
            builder.Property(x => x.AlarmTypeId).HasColumnName(@"ALARM_TYPE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Visualize).HasColumnName(@"VISUALIZE").HasColumnType("bit").IsRequired();
            builder.Property(x => x.Deactivates).HasColumnName(@"DEACTIVATES").HasColumnType("bit").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.AlarmType).WithMany(b => b.UsersAlarmTypes).HasForeignKey(c => c.AlarmTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_USER_ALARM_TYPE_ALARM_TYPE");
            builder.HasOne(a => a.Employee).WithMany(b => b.UsersAlarmTypes).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_USER_ALARM_TYPE_EMPLOYEE");
        }
    }
}
