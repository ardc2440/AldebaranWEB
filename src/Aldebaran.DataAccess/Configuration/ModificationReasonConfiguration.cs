using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ModificationReasonConfiguration : IEntityTypeConfiguration<ModificationReason>
    {
        public void Configure(EntityTypeBuilder<ModificationReason> builder)
        {
            builder.ToTable("MODIFICATION_REASONS", "dbo");
            builder.HasKey(x => x.ModificationReasonId).HasName("PK_MODIFICATION_REASON").IsClustered();
            builder.Property(x => x.ModificationReasonId).HasColumnName(@"MODIFICATION_REASON_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ModificationReasonName).HasColumnName(@"MODIFICATION_REASON_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.DocumentTypeId).HasColumnName(@"DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(200)").IsRequired(false).IsUnicode(false).HasMaxLength(200);
            // Foreign keys
            builder.HasOne(a => a.DocumentType).WithMany(b => b.ModificationReasons).HasForeignKey(c => c.DocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFICATION_REASON_DOCUMENT_TYPE");
            builder.HasIndex(x => new { x.DocumentTypeId, x.ModificationReasonName }).HasDatabaseName("UQ_MODIFICATION_REASON").IsUnique();
        }
    }
}
