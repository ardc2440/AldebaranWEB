using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class MeasureUnitConfiguration : IEntityTypeConfiguration<MeasureUnit>
    {
        public void Configure(EntityTypeBuilder<MeasureUnit> builder)
        {
            builder.ToTable("measure_units", "dbo");
            builder.HasKey(x => x.MeasureUnitId).HasName("PK_MEASURE_UNIT").IsClustered();
            builder.Property(x => x.MeasureUnitId).HasColumnName(@"MEASURE_UNIT_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.MeasureUnitName).HasColumnName(@"MEASURE_UNIT_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.HasIndex(x => x.MeasureUnitName).HasDatabaseName("UQ_MEASURE_UNIT_NAME").IsUnique();
            builder.ToTable(tb => tb.HasTrigger("TRGINSERTRUNIDADESMEDIDA"));
        }
    }
}
