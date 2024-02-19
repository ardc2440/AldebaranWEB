using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderDetailConfiguration : IEntityTypeConfiguration<PurchaseOrderDetail>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderDetail> builder)
        {
            builder.ToTable("purchase_order_details", "dbo");
            builder.HasKey(x => x.PurchaseOrderDetailId).HasName("PK_PURCHASE_ORDER_DETAILS").IsClustered();
            builder.Property(x => x.PurchaseOrderDetailId).HasColumnName(@"PURCHASE_ORDER_DETAIL_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.PurchaseOrderId).HasColumnName(@"PURCHASE_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReferenceId).HasColumnName(@"REFERENCE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.WarehouseId).HasColumnName(@"WAREHOUSE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.ReceivedQuantity).HasColumnName(@"RECEIVED_QUANTITY").HasColumnType("int").IsRequired(false);
            builder.Property(x => x.RequestedQuantity).HasColumnName(@"REQUESTED_QUANTITY").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.ItemReference).WithMany(b => b.PurchaseOrderDetails).HasForeignKey(c => c.ReferenceId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_DETAILS_ITEM_REFERENCE");
            builder.HasOne(a => a.PurchaseOrder).WithMany(b => b.PurchaseOrderDetails).HasForeignKey(c => c.PurchaseOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_DETAILS_PURCHASE_ORDER");
            builder.HasOne(a => a.Warehouse).WithMany(b => b.PurchaseOrderDetails).HasForeignKey(c => c.WarehouseId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_DETAILS_ITEM_WAREHOUSE");
            builder.HasIndex(x => new { x.PurchaseOrderId, x.ReferenceId, x.WarehouseId }).HasDatabaseName("UQIND_PURCHASE_ORDER_DETAILS").IsUnique();
            builder.ToTable(tb => tb.HasTrigger("TRGINSERTSTRANSITO_DETAILS"));
        }
    }
}
