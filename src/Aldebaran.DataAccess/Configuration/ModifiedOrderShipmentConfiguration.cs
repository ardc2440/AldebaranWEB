using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedOrderShipmentConfiguration : IEntityTypeConfiguration<ModifiedOrderShipment>
    {
        public void Configure(EntityTypeBuilder<ModifiedOrderShipment> builder)
        {
            builder.ToTable("MODIFIED_ORDER_SHIPMENT", "dbo");
            builder.HasKey(x => x.ModifiedCustomerShipmentId).HasName("PK_MODIFIED_CUSTOMER_ORDER_SHIPMENT").IsClustered();
            builder.Property(x => x.ModifiedCustomerShipmentId).HasColumnName(@"MODIFIED_CUSTOMER_SHIPMENT_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderShipmentId).HasColumnName(@"CUSTOMER_ORDER_SHIPMENT_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationReasonId).HasColumnName(@"MODIFICATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationDate).HasColumnName(@"MODIFICATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CustomerOrderShipment).WithMany(b => b.ModifiedOrderShipments).HasForeignKey(c => c.CustomerOrderShipmentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_ORDER_SHIPMENT_CUSTOMER_ORDER_IN_PROCESS");
            builder.HasOne(a => a.Employee).WithMany(b => b.ModifiedOrderShipments).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_ORDER_SHIPMENT_EMPLOYEE");
            builder.HasOne(a => a.ModificationReason).WithMany(b => b.ModifiedOrderShipments).HasForeignKey(c => c.ModificationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_ORDER_SHIPMENT_MODIFICATION_REASON");
        }
    }
}
