using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class AdjustmentTypeConfiguration : IEntityTypeConfiguration<AdjustmentType>
    {
        public void Configure(EntityTypeBuilder<AdjustmentType> builder)
        {
            builder.ToTable("adjustment_types", "dbo");
            builder.HasKey(x => x.AdjustmentTypeId).HasName("PK_ADJUSTMENT_TYPE").IsClustered();
            builder.Property(x => x.AdjustmentTypeId).HasColumnName(@"ADJUSTMENT_TYPE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.AdjustmentTypeName).HasColumnName(@"ADJUSTMENT_TYPE_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.Operator).HasColumnName(@"OPERATOR").HasColumnType("smallint").IsRequired();
            builder.HasIndex(x => x.AdjustmentTypeName).HasDatabaseName("UQ_ADJUSTMENT_TYPE_NAME").IsUnique();
        }
    }
}
