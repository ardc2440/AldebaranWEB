using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ShippingMethodConfiguration : IEntityTypeConfiguration<ShippingMethod>
    {
        public void Configure(EntityTypeBuilder<ShippingMethod> builder)
        {
            builder.ToTable("shipping_methods", "dbo");
            builder.HasKey(x => x.ShippingMethodId).HasName("PK_SHIPPING_METHOD").IsClustered();
            builder.Property(x => x.ShippingMethodId).HasColumnName(@"SHIPPING_METHOD_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ShippingMethodName).HasColumnName(@"SHIPPING_METHOD_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.ShippingMethodNotes).HasColumnName(@"SHIPPING_METHOD_NOTES").HasColumnType("varchar(150)").IsRequired(false).IsUnicode(false).HasMaxLength(150);
            builder.HasIndex(x => x.ShippingMethodName).HasDatabaseName("IND_SHIPPING_METHODS_NAME");
            builder.HasIndex(x => x.ShippingMethodName).HasDatabaseName("UQ_SHIPPING_METHOD_NAME").IsUnique();
        }
    }
}
