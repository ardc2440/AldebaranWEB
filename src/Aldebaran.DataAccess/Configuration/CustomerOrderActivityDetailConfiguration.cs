using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderActivityDetailConfiguration : IEntityTypeConfiguration<CustomerOrderActivityDetail>
    {
        public void Configure(EntityTypeBuilder<CustomerOrderActivityDetail> builder)
        {
            builder.ToTable("customer_order_activity_details", "dbo");
            builder.HasKey(x => x.CustomerOrderActivityDetailId).HasName("PK_CUSTOMER_ORDER_ACTIVITY_DETAIL").IsClustered();
            builder.Property(x => x.CustomerOrderActivityDetailId).HasColumnName(@"CUSTOMER_ORDER_ACTIVITY_DETAIL_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderActivityId).HasColumnName(@"CUSTOMER_ORDER_ACTIVITY_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ActivityTypeId).HasColumnName(@"ACTIVITY_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ActivityEmployeeId).HasColumnName(@"ACTIVITY_EMPLOYEE_ID").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.ActivityEmployee).WithMany(b => b.CustomerOrderActivityDetails_ActivityEmployeeId).HasForeignKey(c => c.ActivityEmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_ACTIVITY_DETAIL_ACTIVITY_EMPLOYEE");
            builder.HasOne(a => a.ActivityType).WithMany(b => b.CustomerOrderActivityDetails).HasForeignKey(c => c.ActivityTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_ACTIVITY_ACTIVITY_TYPE");
            builder.HasOne(a => a.CustomerOrderActivity).WithMany(b => b.CustomerOrderActivityDetails).HasForeignKey(c => c.CustomerOrderActivityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_ACTIVITY_CUSTOMER_ORDER_ACTIVITY");
            builder.HasOne(a => a.Employee_EmployeeId).WithMany(b => b.CustomerOrderActivityDetails_EmployeeId).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_ACTIVITY_DETAIL_EMPLOYEE");
            builder.HasIndex(x => new { x.CustomerOrderActivityId, x.ActivityTypeId }).HasDatabaseName("UQ_CUSTOMER_ORDER_ACTIVITY").IsUnique();
        }
    }
}
