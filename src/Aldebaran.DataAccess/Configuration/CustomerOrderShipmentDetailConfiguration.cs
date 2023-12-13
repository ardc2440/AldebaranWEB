using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderShipmentDetailConfiguration : IEntityTypeConfiguration<CustomerOrderShipmentDetail>
    {
        public void Configure(EntityTypeBuilder<CustomerOrderShipmentDetail> builder)
        {
            builder.ToTable("CUSTOMER_ORDER_SHIPMENT_DETAILS", "dbo");
            builder.HasKey(x => x.CustomerOrderShipmentDetailId).HasName("PK_CUSTMER_ORDER_SHIPMENT_DETAIL").IsClustered();
            builder.Property(x => x.CustomerOrderShipmentDetailId).HasColumnName(@"CUSTOMER_ORDER_SHIPMENT_DETAIL_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderShipmentId).HasColumnName(@"CUSTOMER_ORDER_SHIPMENT_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CustomerOrderDetailId).HasColumnName(@"CUSTOMER_ORDER_DETAIL_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.WarehouseId).HasColumnName(@"WAREHOUSE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.DeliveredQuantity).HasColumnName(@"DELIVERED_QUANTITY").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CustomerOrderDetail).WithMany(b => b.CustomerOrderShipmentDetails).HasForeignKey(c => c.CustomerOrderDetailId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTMER_ORDER_SHIPMENT_DETAIL_CUSTOMER_ORDER_DETAIL");
            builder.HasOne(a => a.CustomerOrderShipment).WithMany(b => b.CustomerOrderShipmentDetails).HasForeignKey(c => c.CustomerOrderShipmentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTMER_ORDER_SHIPMENT_DETAIL_ORDER_IN_PROCESS");
            builder.HasOne(a => a.Warehouse).WithMany(b => b.CustomerOrderShipmentDetails).HasForeignKey(c => c.WarehouseId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTMER_ORDER_SHIPMENT_DETAIL_WAREHOUSES");
            builder.HasIndex(x => new { x.CustomerOrderShipmentId, x.CustomerOrderDetailId }).HasDatabaseName("UQ_CUSTMER_ORDER_SHIPMENT_DETAIL").IsUnique();
        }
    }
}
