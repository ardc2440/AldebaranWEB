using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("items", "dbo");
            builder.HasKey(x => x.ItemId).HasName("PK_ITEMS").IsClustered();
            builder.Property(x => x.ItemId).HasColumnName(@"ITEM_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.LineId).HasColumnName(@"LINE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.InternalReference).HasColumnName(@"INTERNAL_REFERENCE").HasColumnType("varchar(10)").IsRequired().IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.ItemName).HasColumnName(@"ITEM_NAME").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.ProviderReference).HasColumnName(@"PROVIDER_REFERENCE").HasColumnType("varchar(27)").IsRequired().IsUnicode(false).HasMaxLength(27);
            builder.Property(x => x.ProviderItemName).HasColumnName(@"PROVIDER_ITEM_NAME").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.FobCost).HasColumnName(@"FOB_COST").HasColumnType("float").HasPrecision(53).IsRequired();
            builder.Property(x => x.CurrencyId).HasColumnName(@"CURRENCY_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.IsExternalInventory).HasColumnName(@"IS_EXTERNAL_INVENTORY").HasColumnType("bit").IsRequired();
            builder.Property(x => x.CifCost).HasColumnName(@"CIF_COST").HasColumnType("float").HasPrecision(53).IsRequired();
            builder.Property(x => x.Volume).HasColumnName(@"VOLUME").HasColumnType("float").HasPrecision(53).IsRequired();
            builder.Property(x => x.Weight).HasColumnName(@"WEIGHT").HasColumnType("float").HasPrecision(53).IsRequired();
            builder.Property(x => x.FobMeasureUnitId).HasColumnName(@"FOB_MEASURE_UNIT_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.CifMeasureUnitId).HasColumnName(@"CIF_MEASURE_UNIT_ID").HasColumnType("smallint").IsRequired(false);
            builder.Property(x => x.IsDomesticProduct).HasColumnName(@"IS_DOMESTIC_PRODUCT").HasColumnType("bit").IsRequired();
            builder.Property(x => x.IsActive).HasColumnName(@"IS_ACTIVE").HasColumnType("bit").IsRequired();
            builder.Property(x => x.IsCatalogVisible).HasColumnName(@"IS_CATALOG_VISIBLE").HasColumnType("bit").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CifMeasureUnit).WithMany(b => b.Items_CifMeasureUnitId).HasForeignKey(c => c.CifMeasureUnitId).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ITEM_MEASURE_UNIT_CIF");
            builder.HasOne(a => a.Currency).WithMany(b => b.Items).HasForeignKey(c => c.CurrencyId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ITEM_CURRENCY");
            builder.HasOne(a => a.FobMeasureUnit).WithMany(b => b.Items_FobMeasureUnitId).HasForeignKey(c => c.FobMeasureUnitId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ITEMS_MEASURE_UNIT_FOB");
            builder.HasOne(a => a.Line).WithMany(b => b.Items).HasForeignKey(c => c.LineId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ITEM_LINE");
            builder.HasIndex(x => x.InternalReference).HasDatabaseName("IND_ITEMS_INTERNAL_REFERENCE");
            builder.HasIndex(x => x.LineId).HasDatabaseName("IND_ITEMS_LINE");
            builder.HasIndex(x => x.ItemName).HasDatabaseName("IND_ITEMS_NAME");
            builder.HasIndex(x => x.InternalReference).HasDatabaseName("UQ_INTERNAL_REF").IsUnique();
            builder.HasIndex(x => x.ItemName).HasDatabaseName("UQ_ITEM_NAME").IsUnique();
        }
    }
}
