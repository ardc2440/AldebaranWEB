using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("countries", "dbo");
            builder.HasKey(x => x.CountryId).HasName("PK_COUNTRY").IsClustered();
            builder.Property(x => x.CountryId).HasColumnName(@"COUNTRY_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CountryName).HasColumnName(@"COUNTRY_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.CountryCode).HasColumnName(@"COUNTRY_CODE").HasColumnType("varchar(5)").IsRequired().IsUnicode(false).HasMaxLength(5);
            builder.HasIndex(x => x.CountryCode).HasDatabaseName("UQ_COUNTRY_CODE").IsUnique();
            builder.HasIndex(x => x.CountryName).HasDatabaseName("UQ_COUNTRY_NAME").IsUnique();
        }
    }
}
