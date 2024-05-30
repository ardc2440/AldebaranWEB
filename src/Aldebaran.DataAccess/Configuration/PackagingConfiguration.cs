using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class PackagingConfiguration : IEntityTypeConfiguration<Packaging>
    {
        public void Configure(EntityTypeBuilder<Packaging> builder)
        {
            builder.ToTable("packaging", "dbo");
            builder.HasKey(x => x.PackagingId).HasName("PK_PACKAGING").IsClustered();
            builder.Property(x => x.PackagingId).HasColumnName(@"PACKAGING_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ItemId).HasColumnName(@"ITEM_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.Weight).HasColumnName(@"WEIGHT").HasColumnType("float").HasPrecision(53).IsRequired(false);
            builder.Property(x => x.Height).HasColumnName(@"HEIGHT").HasColumnType("float").HasPrecision(53).IsRequired(false);
            builder.Property(x => x.Width).HasColumnName(@"WIDTH").HasColumnType("float").HasPrecision(53).IsRequired(false);
            builder.Property(x => x.Length).HasColumnName(@"LENGTH").HasColumnType("float").HasPrecision(53).IsRequired(false);
            builder.Property(x => x.Quantity).HasColumnName(@"QUANTITY").HasColumnType("int").IsRequired(false);
            // Foreign keys
            builder.HasOne(a => a.Item).WithMany(b => b.Packagings).HasForeignKey(c => c.ItemId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PACKAGING_ITEMS");
            builder.ToTable(tb => tb.HasTrigger("TRGINSERTREMBALAJES"));
        }
    }
}
