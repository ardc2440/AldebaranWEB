using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ItemsAreaConfiguration : IEntityTypeConfiguration<ItemsArea>
    {
        public void Configure(EntityTypeBuilder<ItemsArea> builder)
        {
            builder.ToTable("items_area", "dbo");
            builder.HasKey(x => new { x.ItemId, x.AreaId }).HasName("PK_ITEM_AREA").IsClustered();
            builder.Property(x => x.ItemId).HasColumnName(@"ITEM_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.AreaId).HasColumnName(@"AREA_ID").HasColumnType("smallint").IsRequired().ValueGeneratedNever();
            // Foreign keys
            builder.HasOne(a => a.Area).WithMany(b => b.ItemsAreas).HasForeignKey(c => c.AreaId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ITEM_AREA_AREA");
            builder.HasOne(a => a.Item).WithMany(b => b.ItemsAreas).HasForeignKey(c => c.ItemId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ITEM_AREA_ITEM");
        }
    }
}
