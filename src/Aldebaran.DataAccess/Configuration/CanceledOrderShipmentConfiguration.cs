using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CanceledOrderShipmentConfiguration : IEntityTypeConfiguration<CanceledOrderShipment>
    {
        public void Configure(EntityTypeBuilder<CanceledOrderShipment> builder)
        {
            builder.ToTable("canceled_order_shipment", "dbo");
            builder.HasKey(x => x.CustomerOrderShipmentId).HasName("PK_CANCELED_CUSTOMER_ORDER_SHIPMENT").IsClustered();
            builder.Property(x => x.CustomerOrderShipmentId).HasColumnName(@"CUSTOMER_ORDER_SHIPMENT_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.CancellationReasonId).HasColumnName(@"CANCELLATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CancellationDate).HasColumnName(@"CANCELLATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CancellationReason).WithMany(b => b.CanceledOrderShipments).HasForeignKey(c => c.CancellationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_ORDER_SHIPMENT_CANCELLATION");
            builder.HasOne(a => a.CustomerOrderShipment).WithOne(b => b.CanceledOrderShipment).HasForeignKey<CanceledOrderShipment>(c => c.CustomerOrderShipmentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_ORDER_SHIPMENT_CUSTOMER_ORDER_SHIPMENT");
            builder.HasOne(a => a.Employee).WithMany(b => b.CanceledOrderShipments).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_ORDER_SHIPMENT_EMPLOYEE");
        }
    }
}
