using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class StatusDocumentTypeConfiguration : IEntityTypeConfiguration<StatusDocumentType>
    {
        public void Configure(EntityTypeBuilder<StatusDocumentType> builder)
        {
            builder.ToTable("status_document_types", "dbo");
            builder.HasKey(x => x.StatusDocumentTypeId).HasName("PK_STATUS_DOCUMENT_TYPE").IsClustered();
            builder.Property(x => x.StatusDocumentTypeId).HasColumnName(@"STATUS_DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.StatusDocumentTypeName).HasColumnName(@"STATUS_DOCUMENT_TYPE_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.DocumentTypeId).HasColumnName(@"DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(250)").IsRequired().IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.EditMode).HasColumnName(@"EDIT_MODE").HasColumnType("bit").IsRequired();
            builder.Property(x => x.StatusOrder).HasColumnName(@"STATUS_ORDER").HasColumnType("smallint").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.DocumentType).WithMany(b => b.StatusDocumentTypes).HasForeignKey(c => c.DocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_STATUS_DOCUMENT_TYPE_DOCUMENT_TYPE");
            builder.HasIndex(x => new { x.DocumentTypeId, x.StatusDocumentTypeName }).HasDatabaseName("UQ_STATUS_DOCUMENT_TYPE_NAME").IsUnique();
            builder.HasIndex(x => new { x.DocumentTypeId, x.StatusOrder }).HasDatabaseName("UQ_STATUS_DOCUMENT_TYPE_STATUS_ORDER").IsUnique();
        }
    }
}
