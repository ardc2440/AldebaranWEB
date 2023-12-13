using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class AdjustmentReasonConfiguration : IEntityTypeConfiguration<AdjustmentReason>
    {
        public void Configure(EntityTypeBuilder<AdjustmentReason> builder)
        {
            builder.ToTable("adjustment_reasons", "dbo");
            builder.HasKey(x => x.AdjustmentReasonId).HasName("PK_ADJUSTMENT_REASON").IsClustered();
            builder.Property(x => x.AdjustmentReasonId).HasColumnName(@"ADJUSTMENT_REASON_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.AdjustmentReasonName).HasColumnName(@"ADJUSTMENT_REASON_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.AdjustmentReasonNotes).HasColumnName(@"ADJUSTMENT_REASON_NOTES").HasColumnType("varchar(150)").IsRequired(false).IsUnicode(false).HasMaxLength(150);
            builder.HasIndex(x => x.AdjustmentReasonName).HasDatabaseName("UQ_ADJUSTMENT_REASON_NAME").IsUnique();
        }
    }
}
