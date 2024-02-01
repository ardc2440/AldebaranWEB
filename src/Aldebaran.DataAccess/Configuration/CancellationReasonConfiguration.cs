using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CancellationReasonConfiguration : IEntityTypeConfiguration<CancellationReason>
    {
        public void Configure(EntityTypeBuilder<CancellationReason> builder)
        {
            builder.ToTable("cancellation_reasons", "dbo");
            builder.HasKey(x => x.CancellationReasonId).HasName("PK_CANCELLATION_REASON").IsClustered();
            builder.Property(x => x.CancellationReasonId).HasColumnName(@"CANCELLATION_REASON_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CancellationReasonName).HasColumnName(@"CANCELLATION_REASON_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.DocumentTypeId).HasColumnName(@"DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(200)").IsRequired(false).IsUnicode(false).HasMaxLength(200);
            // Foreign keys
            builder.HasOne(a => a.DocumentType).WithMany(b => b.CancellationReasons).HasForeignKey(c => c.DocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELLATION_REASON_DOCUMENT_TYPE");
            builder.HasIndex(x => new { x.DocumentTypeId, x.CancellationReasonName }).HasDatabaseName("UQ_CANCELLATION_REASON").IsUnique();
        }
    }
}
