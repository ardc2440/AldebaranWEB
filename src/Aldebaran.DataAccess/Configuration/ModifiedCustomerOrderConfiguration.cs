using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedCustomerOrderConfiguration : IEntityTypeConfiguration<ModifiedCustomerOrder>
    {
        public void Configure(EntityTypeBuilder<ModifiedCustomerOrder> builder)
        {
            builder.ToTable("MODIFIED_CUSTOMER_ORDERS", "dbo");
            builder.HasKey(x => x.ModifiedCustomerOrderId).HasName("PK_MODIFIED_CUSTOMER_ORDER").IsClustered();
            builder.Property(x => x.ModifiedCustomerOrderId).HasColumnName(@"MODIFIED_CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationReasonId).HasColumnName(@"MODIFICATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationDate).HasColumnName(@"MODIFICATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CustomerOrder).WithMany(b => b.ModifiedCustomerOrders).HasForeignKey(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_ORDER_CUSTOMER_ORDER");
            builder.HasOne(a => a.Employee).WithMany(b => b.ModifiedCustomerOrders).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_ORDER_EMPLOYEE");
            builder.HasOne(a => a.ModificationReason).WithMany(b => b.ModifiedCustomerOrders).HasForeignKey(c => c.ModificationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_ORDER_MODIFICATION_REASON");
        }
    }
}
