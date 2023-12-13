using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderActivityConfiguration : IEntityTypeConfiguration<CustomerOrderActivity>
    {
        public void Configure(EntityTypeBuilder<CustomerOrderActivity> builder)
        {
            builder.ToTable("customer_order_activities", "dbo");
            builder.HasKey(x => x.CustomerOrderActivityId).HasName("PK_CUSTOMER_ORDER_ACTIVITY").IsClustered();
            builder.Property(x => x.CustomerOrderActivityId).HasColumnName(@"CUSTOMER_ORDER_ACTIVITY_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.AreaId).HasColumnName(@"AREA_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ActivityDate).HasColumnName(@"ACTIVITY_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(255)").IsRequired(false).IsUnicode(false).HasMaxLength(255);
            // Foreign keys
            builder.HasOne(a => a.Area).WithMany(b => b.CustomerOrderActivities).HasForeignKey(c => c.AreaId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_ACTIVITY_AREA");
            builder.HasOne(a => a.CustomerOrder).WithMany(b => b.CustomerOrderActivities).HasForeignKey(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_ACTIVITY_CUSTOMER_ORDER");
            builder.HasOne(a => a.Employee).WithMany(b => b.CustomerOrderActivities).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_ACTIVITY_EMPLOYEE");
            builder.HasIndex(x => new { x.CustomerOrderId, x.ActivityDate }).HasDatabaseName("IND_CUSTOMER_ORDER_ACTIVITIES");
        }
    }
}
