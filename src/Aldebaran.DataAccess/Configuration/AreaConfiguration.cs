using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class AreaConfiguration : IEntityTypeConfiguration<Area>
    {
        public void Configure(EntityTypeBuilder<Area> builder)
        {
            builder.ToTable("areas", "dbo");
            builder.HasKey(x => x.AreaId).IsClustered();
            builder.Property(x => x.AreaId).HasColumnName(@"AREA_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.AreaCode).HasColumnName(@"AREA_CODE").HasColumnType("varchar(10)").IsRequired().IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.AreaName).HasColumnName(@"AREA_NAME").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.Description).HasColumnName(@"DESCRIPTION").HasColumnType("varchar(200)").IsRequired(false).IsUnicode(false).HasMaxLength(200);
            builder.HasIndex(x => x.AreaCode).IsUnique();
            builder.HasIndex(x => x.AreaName).IsUnique();
        }
    }
}
