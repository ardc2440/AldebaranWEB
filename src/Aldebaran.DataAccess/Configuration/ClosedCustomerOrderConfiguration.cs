using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ClosedCustomerOrderConfiguration : IEntityTypeConfiguration<ClosedCustomerOrder>
    {
        public void Configure(EntityTypeBuilder<ClosedCustomerOrder> builder)
        {
            builder.ToTable("closed_customer_orders", "dbo");
            builder.HasKey(x => x.ClosedCustomerOrderId).HasName("PK_CLOSED_CUSTOMER_ORDER").IsClustered();
            builder.Property(x => x.ClosedCustomerOrderId).HasColumnName(@"CLOSED_CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CloseCustomerOrderReasonId).HasColumnName(@"CLOSE_CUSTOMER_ORDER_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CloseDate).HasColumnName(@"CLOSE_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CloseCustomerOrderReason).WithMany(b => b.ClosedCustomerOrders).HasForeignKey(c => c.CloseCustomerOrderReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CLOSED_CUSTOMER_ORDER_CLOSE_CUSTOMER_ORDER_REASON");
            builder.HasOne(a => a.CustomerOrder).WithMany(b => b.ClosedCustomerOrders).HasForeignKey(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CLOSED_CUSTOMER_ORDER_CUSTOMER_ORDER");
            builder.HasOne(a => a.Employee).WithMany(b => b.ClosedCustomerOrders).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CLOSED_CUSTOMER_ORDER_EMPLOYEE");
        }
    }
}
