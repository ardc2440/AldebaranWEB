using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    // ****************************************************************************************************
    // This is not a commercial licence, therefore only a few tables/views/stored procedures are generated.
    // ****************************************************************************************************
    public class ActivityTypeConfiguration : IEntityTypeConfiguration<ActivityType>
    {
        public void Configure(EntityTypeBuilder<ActivityType> builder)
        {
            builder.ToTable("activity_types", "dbo");
            builder.HasKey(x => x.ActivityTypeId).HasName("PK_ACTIVITY_TYPE").IsClustered();
            builder.Property(x => x.ActivityTypeId).HasColumnName(@"ACTIVITY_TYPE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ActivityTypeName).HasColumnName(@"ACTIVITY_TYPE_NAME").HasColumnType("varchar(80)").IsRequired().IsUnicode(false).HasMaxLength(80);
        }
    }
}
