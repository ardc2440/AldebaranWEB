using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class EmployeePreferenceConfiguration : IEntityTypeConfiguration<EmployeePreference>
    {
        public void Configure(EntityTypeBuilder<EmployeePreference> builder)
        {
            builder.ToTable("employee_preferences");

            builder.HasKey(x => x.EmployeeId);

            builder.Property(x => x.EmployeeId)
                .HasColumnName("EMPLOYEE_ID");

            builder.Property(x => x.EnableNotifications)
                .HasColumnName("ENABLE_NOTIFICATIONS")
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .HasColumnName("CREATED_DATE")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(x => x.UpdatedDate)
                .HasColumnName("UPDATED_DATE")
                .HasColumnType("datetime")
                .IsRequired();

            builder.HasOne(x => x.Employee)
                .WithMany(b => b.EmployeePreferences)
                .HasForeignKey(x => x.EmployeeId)
                .HasConstraintName("FK_EMPLOYEE_PREFERENCE_EMPLOYEE");
        }
    }
}