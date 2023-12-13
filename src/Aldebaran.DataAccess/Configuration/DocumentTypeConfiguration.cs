using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
    {
        public void Configure(EntityTypeBuilder<DocumentType> builder)
        {
            builder.ToTable("document_types", "dbo");
            builder.HasKey(x => x.DocumentTypeId).HasName("PK_DOCUMENT_TYPE").IsClustered();
            builder.Property(x => x.DocumentTypeId).HasColumnName(@"DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.DocumentTypeName).HasColumnName(@"DOCUMENT_TYPE_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.DocumentTypeCode).HasColumnName(@"DOCUMENT_TYPE_CODE").HasColumnType("char(1)").IsRequired().IsFixedLength().IsUnicode(false).HasMaxLength(1);
            builder.HasIndex(x => x.DocumentTypeCode).HasDatabaseName("UQ_DOCUMENT_TYPE_CODE").IsUnique();
        }
    }
}
