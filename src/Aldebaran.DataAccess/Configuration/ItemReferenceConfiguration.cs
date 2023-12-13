using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ItemReferenceConfiguration : IEntityTypeConfiguration<ItemReference>
    {
        public void Configure(EntityTypeBuilder<ItemReference> builder)
        {
            builder.ToTable("item_references", "dbo");
            builder.HasKey(x => x.ReferenceId).HasName("PK_REFERENCE").IsClustered();
            builder.Property(x => x.ReferenceId).HasColumnName(@"REFERENCE_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ItemId).HasColumnName(@"ITEM_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReferenceCode).HasColumnName(@"REFERENCE_CODE").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.ProviderReferenceCode).HasColumnName(@"PROVIDER_REFERENCE_CODE").HasColumnType("varchar(10)").IsRequired(false).IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.ReferenceName).HasColumnName(@"REFERENCE_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.ProviderReferenceName).HasColumnName(@"PROVIDER_REFERENCE_NAME").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.InventoryQuantity).HasColumnName(@"INVENTORY_QUANTITY").HasColumnType("int").IsRequired();
            builder.Property(x => x.OrderedQuantity).HasColumnName(@"ORDERED_QUANTITY").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReservedQuantity).HasColumnName(@"RESERVED_QUANTITY").HasColumnType("int").IsRequired();
            builder.Property(x => x.WorkInProcessQuantity).HasColumnName(@"WORK_IN_PROCESS_QUANTITY").HasColumnType("int").IsRequired();
            builder.Property(x => x.IsActive).HasColumnName(@"IS_ACTIVE").HasColumnType("bit").IsRequired();
            builder.Property(x => x.IsSoldOut).HasColumnName(@"IS_SOLD_OUT").HasColumnType("bit").IsRequired();
            builder.Property(x => x.AlarmMinimumQuantity).HasColumnName(@"ALARM_MINIMUM_QUANTITY").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Item).WithMany(b => b.ItemReferences).HasForeignKey(c => c.ItemId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_REFERENCE_ITEM");
            builder.HasIndex(x => x.ProviderReferenceCode).HasDatabaseName("IND_REFERENCES_INTERNAL_REFERENCE_CODE");
            builder.HasIndex(x => x.ItemId).HasDatabaseName("IND_REFERENCES_ITEM_ID");
            builder.HasIndex(x => x.ReferenceName).HasDatabaseName("IND_REFERENCES_REFERENCE_NAME");
            builder.HasIndex(x => new { x.ItemId, x.ReferenceCode }).HasDatabaseName("UQ_REFERENCE_CODE").IsUnique();
            builder.HasIndex(x => new { x.ItemId, x.ReferenceName }).HasDatabaseName("UQ_REFERENCE_NAME").IsUnique();
        }
    }
}
