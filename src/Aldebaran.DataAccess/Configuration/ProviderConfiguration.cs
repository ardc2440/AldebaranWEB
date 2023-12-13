using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            builder.ToTable("providers", "dbo");
            builder.HasKey(x => x.ProviderId).HasName("PK_PROVIDER").IsClustered();
            builder.Property(x => x.ProviderId).HasColumnName(@"PROVIDER_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.IdentityTypeId).HasColumnName(@"IDENTITY_TYPE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.IdentityNumber).HasColumnName(@"IDENTITY_NUMBER").HasColumnType("varchar(15)").IsRequired().IsUnicode(false).HasMaxLength(15);
            builder.Property(x => x.ProviderCode).HasColumnName(@"PROVIDER_CODE").HasColumnType("varchar(10)").IsRequired().IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.ProviderName).HasColumnName(@"PROVIDER_NAME").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.ProviderAddress).HasColumnName(@"PROVIDER_ADDRESS").HasColumnType("varchar(80)").IsRequired(false).IsUnicode(false).HasMaxLength(80);
            builder.Property(x => x.Phone).HasColumnName(@"PHONE").HasColumnType("varchar(20)").IsRequired(false).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Fax).HasColumnName(@"FAX").HasColumnType("varchar(22)").IsRequired(false).IsUnicode(false).HasMaxLength(22);
            builder.Property(x => x.Email).HasColumnName(@"EMAIL").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.ContactPerson).HasColumnName(@"CONTACT_PERSON").HasColumnType("varchar(50)").IsRequired(false).IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.CityId).HasColumnName(@"CITY_ID").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.City).WithMany(b => b.Providers).HasForeignKey(c => c.CityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PROVIDER_IDENTITY_CITY");
            builder.HasOne(a => a.IdentityType).WithMany(b => b.Providers).HasForeignKey(c => c.IdentityTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PROVIDER_IDENTITY_TYPE");
            builder.HasIndex(x => x.IdentityNumber).HasDatabaseName("IND_PROVIDER_IDENTITY_NUMBER");
            builder.HasIndex(x => x.ProviderName).HasDatabaseName("IND_PROVIDER_PROVIDER_NAME");
            builder.HasIndex(x => x.IdentityNumber).HasDatabaseName("UQ_PROVIDER_IDENTITY_NUMBER").IsUnique();
            builder.HasIndex(x => x.ProviderCode).HasDatabaseName("UQ_PROVIDER_PROVIDER_CODE").IsUnique();
            builder.HasIndex(x => x.ProviderName).HasDatabaseName("UQ_PROVIDER_PROVIDER_NAME").IsUnique();
        }
    }
}
