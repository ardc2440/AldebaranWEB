using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class IdentityTypeConfiguration : IEntityTypeConfiguration<IdentityType>
    {
        public void Configure(EntityTypeBuilder<IdentityType> builder)
        {
            builder.ToTable("identity_types", "dbo");
            builder.HasKey(x => x.IdentityTypeId).HasName("PK_IDENTITY_TYPE").IsClustered();
            builder.Property(x => x.IdentityTypeId).HasColumnName(@"IDENTITY_TYPE_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.IdentityTypeCode).HasColumnName(@"IDENTITY_TYPE_CODE").HasColumnType("char(3)").IsRequired().IsFixedLength().IsUnicode(false).HasMaxLength(3);
            builder.Property(x => x.IdentityTypeName).HasColumnName(@"IDENTITY_TYPE_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.HasIndex(x => x.IdentityTypeCode).HasDatabaseName("UQ_IDENTITY_TYPE_CODE").IsUnique();
            builder.HasIndex(x => x.IdentityTypeName).HasDatabaseName("UQ_IDENTITY_TYPE_NAME").IsUnique();
        }
    }
}
