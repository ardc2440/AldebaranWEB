using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderShipmentConfiguration : IEntityTypeConfiguration<CustomerOrderShipment>
    {
        public void Configure(EntityTypeBuilder<CustomerOrderShipment> builder)
        {
            builder.ToTable("customer_order_shipments", "dbo");
            builder.HasKey(x => x.CustomerOrderShipmentId).HasName("PK_CUSTOMER_ORDER_SHIPMENT").IsClustered();
            builder.Property(x => x.CustomerOrderShipmentId).HasColumnName(@"CUSTOMER_ORDER_SHIPMENT_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ShippingMethodId).HasColumnName(@"SHIPPING_METHOD_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.TrackingNumber).HasColumnName(@"TRACKING_NUMBER").HasColumnType("varchar(15)").IsRequired(false).IsUnicode(false).HasMaxLength(15);
            builder.Property(x => x.DeliveryNote).HasColumnName(@"DELIVERY_NOTE").HasColumnType("varchar(15)").IsRequired().IsUnicode(false).HasMaxLength(15);
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ShippingDate).HasColumnName(@"SHIPPING_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CREATION_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.StatusDocumentTypeId).HasColumnName(@"STATUS_DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CustomerOrder).WithMany(b => b.CustomerOrderShipments).HasForeignKey(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_SHIPMENT_CUSTOMER_ORDER");
            builder.HasOne(a => a.Employee).WithMany(b => b.CustomerOrderShipments).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_SHIPMENT_EMPLOYEE");
            builder.HasOne(a => a.ShippingMethod).WithMany(b => b.CustomerOrderShipments).HasForeignKey(c => c.ShippingMethodId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_SHIPMENT_SHIPPING_METHOD");
            builder.HasOne(a => a.StatusDocumentType).WithMany(b => b.CustomerOrderShipments).HasForeignKey(c => c.StatusDocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_SHIPMENT_STATUS_DOCUMENT_TYPE");

        }
    }
}
