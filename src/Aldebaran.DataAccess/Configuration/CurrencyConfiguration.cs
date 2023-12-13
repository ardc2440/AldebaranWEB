using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("currencies", "dbo");
            builder.HasKey(x => x.CurrencyId).HasName("PK_CURRENCY").IsClustered();
            builder.Property(x => x.CurrencyId).HasColumnName(@"CURRENCY_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CurrencyName).HasColumnName(@"CURRENCY_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.HasIndex(x => x.CurrencyName).HasDatabaseName("UQ_MONEDAS_NOMBRE").IsUnique();
        }
    }
}
