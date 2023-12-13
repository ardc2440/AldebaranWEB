using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class LineConfiguration : IEntityTypeConfiguration<Line>
    {
        public void Configure(EntityTypeBuilder<Line> builder)
        {
            builder.ToTable("lines", "dbo");
            builder.HasKey(x => x.LineId).HasName("PK_LINE").IsClustered();
            builder.Property(x => x.LineId).HasColumnName(@"LINE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.LineCode).HasColumnName(@"LINE_CODE").HasColumnType("varchar(10)").IsRequired().IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.LineName).HasColumnName(@"LINE_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.IsDemon).HasColumnName(@"IS_DEMON").HasColumnType("bit").IsRequired();
            builder.Property(x => x.IsActive).HasColumnName(@"IS_ACTIVE").HasColumnType("bit").IsRequired();
            builder.HasIndex(x => x.LineCode).HasDatabaseName("UQ_LINE_CODE").IsUnique();
            builder.HasIndex(x => x.LineName).HasDatabaseName("UQ_LINE_NAME").IsUnique();
        }
    }
}
