using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ActivityTypesAreaConfiguration : IEntityTypeConfiguration<ActivityTypesArea>
    {
        public void Configure(EntityTypeBuilder<ActivityTypesArea> builder)
        {
            builder.ToTable("activity_types_area", "dbo");
            builder.HasKey(x => new { x.ActivityTypeId, x.AreaId }).HasName("PK_ACTIVITY_TYPE_AREA").IsClustered();
            builder.Property(x => x.ActivityTypeId).HasColumnName(@"ACTIVITY_TYPE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.AreaId).HasColumnName(@"AREA_ID").HasColumnType("smallint").IsRequired().ValueGeneratedNever();
            // Foreign keys
            builder.HasOne(a => a.ActivityType).WithMany(b => b.ActivityTypesAreas).HasForeignKey(c => c.ActivityTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ACTIVITY_TYPE_AREA_ACTIVITY_TYPE");
            builder.HasOne(a => a.Area).WithMany(b => b.ActivityTypesAreas).HasForeignKey(c => c.AreaId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ACTIVITY_TYPE_AREA_AREA");
        }
    }
}
