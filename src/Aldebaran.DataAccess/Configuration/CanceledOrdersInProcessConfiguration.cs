using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CanceledOrdersInProcessConfiguration : IEntityTypeConfiguration<CanceledOrdersInProcess>
    {
        public void Configure(EntityTypeBuilder<CanceledOrdersInProcess> builder)
        {
            builder.ToTable("canceled_orders_in_process", "dbo");
            builder.HasKey(x => x.CustomerOrderInProcessId).HasName("PK_CANCELED_CUSTOMER_ORDER_IN_PROCESS").IsClustered();
            builder.Property(x => x.CustomerOrderInProcessId).HasColumnName(@"CUSTOMER_ORDER_IN_PROCESS_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.CancellationReasonId).HasColumnName(@"CANCELLATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CancellationDate).HasColumnName(@"CANCELLATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CancellationReason).WithMany(b => b.CanceledOrdersInProcesses).HasForeignKey(c => c.CancellationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_ORDER_IN_PROCESS_CANCELLATION");
            builder.HasOne(a => a.CustomerOrdersInProcess).WithOne(b => b.CanceledOrdersInProcess).HasForeignKey<CanceledOrdersInProcess>(c => c.CustomerOrderInProcessId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_ORDER_IN_PROCESS_CUSTOMER_ORDER_IN_PROCESS");
            builder.HasOne(a => a.Employee).WithMany(b => b.CanceledOrdersInProcesses).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_ORDER_IN_PROCESS_EMPLOYEE");
        }
    }
}
