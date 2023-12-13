using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class AdjustmentDetailConfiguration : IEntityTypeConfiguration<AdjustmentDetail>
    {
        public void Configure(EntityTypeBuilder<AdjustmentDetail> builder)
        {
            builder.ToTable("adjustment_details", "dbo");
            builder.HasKey(x => x.AdjustmentDetailId).HasName("PK_ADJUSTMENT_DETAIL").IsClustered();
            builder.Property(x => x.AdjustmentDetailId).HasColumnName(@"ADJUSTMENT_DETAIL_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.AdjustmentId).HasColumnName(@"ADJUSTMENT_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReferenceId).HasColumnName(@"REFERENCE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.WarehouseId).HasColumnName(@"WAREHOUSE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName(@"QUANTITY").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Adjustment).WithMany(b => b.AdjustmentDetails).HasForeignKey(c => c.AdjustmentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ADJUSTMENT_DETAIL_ADJUSTMENT");
            builder.HasOne(a => a.ItemReference).WithMany(b => b.AdjustmentDetails).HasForeignKey(c => c.ReferenceId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ADJUSTMENT_DETAIL_ITEM_REFERENCES");
            builder.HasOne(a => a.Warehouse).WithMany(b => b.AdjustmentDetails).HasForeignKey(c => c.WarehouseId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ADJUSTMENT_DETAIL_WAREHOUSE");
            builder.HasIndex(x => new { x.AdjustmentId, x.ReferenceId, x.WarehouseId }).HasDatabaseName("UQ_ADJUSTMENT_DETAIL").IsUnique();
        }
    }
}
