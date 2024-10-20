using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ReferencesWarehouseConfiguration : IEntityTypeConfiguration<ReferencesWarehouse>
    {
        public void Configure(EntityTypeBuilder<ReferencesWarehouse> builder)
        {
            builder.ToTable("references_warehouse", "dbo");
            builder.HasKey(x => new { x.ReferenceId, x.WarehouseId }).HasName("PK_REFERENCE_WAREHOUSE").IsClustered();
            builder.Property(x => x.ReferenceId).HasColumnName(@"REFERENCE_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.WarehouseId).HasColumnName(@"WAREHOUSE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Quantity).HasColumnName(@"QUANTITY").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.ItemReference).WithMany(b => b.ReferencesWarehouses).HasForeignKey(c => c.ReferenceId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ITEM_ITEM_WAREHOUSE_ITEM_REFERENCE");
            builder.HasOne(a => a.Warehouse).WithMany(b => b.ReferencesWarehouses).HasForeignKey(c => c.WarehouseId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ITEM_WAREHOUSE_WAREHOUSE");
            builder.ToTable(tb => tb.HasTrigger("TRGINSERTREXISTENCIAS"));
            builder.ToTable(tb => tb.HasTrigger("Trg_Minimum_Local_warehouse_Quantity_Alarm"));            
        }
    }
}
