using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class WarehouseTransferDetailConfiguration : IEntityTypeConfiguration<WarehouseTransferDetail>
    {
        public void Configure(EntityTypeBuilder<WarehouseTransferDetail> builder)
        {
            builder.ToTable("warehouse_transfer_details", "dbo");
            builder.HasKey(x => x.WarehouseTransferDetailId).HasName("PK_WAREHOUSE_TRANSFER_DETAIL").IsClustered();
            builder.Property(x => x.WarehouseTransferDetailId).HasColumnName(@"WAREHOUSE_TRANSFER__DETAIL_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.WarehouseTransferId).HasColumnName(@"WAREHOUSE_TRANSFER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReferenceId).HasColumnName(@"REFERENCE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName(@"QUANTITY").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.WarehouseTransfer).WithMany(b => b.WarehouseTransferDetails).HasForeignKey(c => c.WarehouseTransferId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WAREHOUSE_TRANSFER_DETAIL_WAREHOUSE_TRANSFER");
            builder.HasOne(a => a.ItemReference).WithMany(b => b.WarehouseTransferDetails).HasForeignKey(c => c.ReferenceId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WAREHOUSE_TRANSFER_DETAIL_ITEM_REFERENCE");
            builder.HasIndex(x => new { x.WarehouseTransferId, x.ReferenceId }).HasDatabaseName("IND_WAREHOUSE_TRANSFER_DETAIL_REFERENCE");
        }
    }
}