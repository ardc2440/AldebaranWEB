using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrdersInProcessConfiguration : IEntityTypeConfiguration<CustomerOrdersInProcess>
    {
        public void Configure(EntityTypeBuilder<CustomerOrdersInProcess> builder)
        {
            builder.ToTable("customer_orders_in_process", "dbo");
            builder.HasKey(x => x.CustomerOrderInProcessId).HasName("PK_CUSTOMER_ORDER_IN_PROCESS").IsClustered();
            builder.Property(x => x.CustomerOrderInProcessId).HasColumnName(@"CUSTOMER_ORDER_IN_PROCESS_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ProcessDate).HasColumnName(@"PROCESS_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.ProcessSatelliteId).HasColumnName(@"PROCESS_SATELLITE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.TransferDatetime).HasColumnName(@"TRANSFER_DATETIME").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.EmployeeRecipientId).HasColumnName(@"EMPLOYEE_RECIPIENT_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CREATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CustomerOrder).WithMany(b => b.CustomerOrdersInProcesses).HasForeignKey(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_IN_PROCESS_CUSTOMER_ORDER");
            builder.HasOne(a => a.Employee).WithMany(b => b.CustomerOrdersInProcesses).HasForeignKey(c => c.EmployeeRecipientId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_IN_PROCESS_EMPLOYEE_RECIPIENT");
            builder.HasOne(a => a.ProcessSatellite).WithMany(b => b.CustomerOrdersInProcesses).HasForeignKey(c => c.ProcessSatelliteId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_IN_PROCESS_PROCESS_SATELLITE");
            builder.HasIndex(x => new { x.CustomerOrderId, x.ProcessDate }).HasDatabaseName("IND_CUSTOMER_ORDERS_IN_PROCESS_PROCESS_DATE");
        }
    }
}
