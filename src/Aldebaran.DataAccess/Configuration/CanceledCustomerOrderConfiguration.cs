using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CanceledCustomerOrderConfiguration : IEntityTypeConfiguration<CanceledCustomerOrder>
    {
        public void Configure(EntityTypeBuilder<CanceledCustomerOrder> builder)
        {
            builder.ToTable("CANCELED_CUSTOMER_ORDERS", "dbo");
            builder.HasKey(x => x.CustomerOrderId).HasName("PK_CANCELED_CUSTOMER_ORDER").IsClustered();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.CancellationReasonId).HasColumnName(@"CANCELLATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CancellationDate).HasColumnName(@"CANCELLATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CancellationReason).WithMany(b => b.CanceledCustomerOrders).HasForeignKey(c => c.CancellationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_ORDER_CANCELLATION");
            builder.HasOne(a => a.CustomerOrder).WithOne(b => b.CanceledCustomerOrder).HasForeignKey<CanceledCustomerOrder>(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_ORDER_CUSTOMER_ORDER");
            builder.HasOne(a => a.Employee).WithMany(b => b.CanceledCustomerOrders).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_ORDER_EMPLOYEE");
        }
    }
}
