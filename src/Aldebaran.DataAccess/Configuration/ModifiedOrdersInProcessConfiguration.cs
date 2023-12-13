using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedOrdersInProcessConfiguration : IEntityTypeConfiguration<ModifiedOrdersInProcess>
    {
        public void Configure(EntityTypeBuilder<ModifiedOrdersInProcess> builder)
        {
            builder.ToTable("MODIFIED_ORDERS_IN_PROCESS", "dbo");
            builder.HasKey(x => x.ModifiedCustomerOrderInProcessId).HasName("PK_MODIFIED_CUSTOMER_ORDER_IN_PROCESS").IsClustered();
            builder.Property(x => x.ModifiedCustomerOrderInProcessId).HasColumnName(@"MODIFIED_CUSTOMER_ORDER_IN_PROCESS_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderInProcessId).HasColumnName(@"CUSTOMER_ORDER_IN_PROCESS_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationReasonId).HasColumnName(@"MODIFICATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationDate).HasColumnName(@"MODIFICATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CustomerOrdersInProcess).WithMany(b => b.ModifiedOrdersInProcesses).HasForeignKey(c => c.CustomerOrderInProcessId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_ORDER_IN_PROCESS_CUSTOMER_ORDER_IN_PROCESS");
            builder.HasOne(a => a.Employee).WithMany(b => b.ModifiedOrdersInProcesses).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_ORDER_IN_PROCESS_EMPLOYEE");
            builder.HasOne(a => a.ModificationReason).WithMany(b => b.ModifiedOrdersInProcesses).HasForeignKey(c => c.ModificationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_ORDER_IN_PROCESS_MODIFICATION_REASON");
        }
    }
}
