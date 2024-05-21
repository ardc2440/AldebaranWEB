using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Core
{
    public class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.ToTable("track", "log");
            builder.HasKey(x => x.Id).IsClustered();
            builder.Property(x => x.Id).HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(e => e.EntityName).HasColumnName("ENTITY_NAME").HasColumnType("nvarchar(100)").IsRequired().IsUnicode(true).HasMaxLength(100);
            builder.Property(e => e.EntityKey).HasColumnName("ENTITY_KEY").IsRequired(false);
            builder.Property(e => e.Action).HasColumnName("ACTION").HasColumnType("varchar(10)").IsRequired().IsUnicode(false).HasMaxLength(10);
            builder.Property(e => e.DataLog).HasColumnName("DATA_LOG").HasColumnType("text").IsRequired();
            builder.Property(e => e.ModifiedDate).HasColumnName("MODIFIED_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(e => e.ModifierName).HasColumnName("MODIFIER_NAME").HasColumnType("nvarchar(50)").IsRequired().IsUnicode(true).HasMaxLength(50);
        }
    }
}
