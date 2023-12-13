using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ShipmentMethodConfiguration : IEntityTypeConfiguration<ShipmentMethod>
    {
        public void Configure(EntityTypeBuilder<ShipmentMethod> builder)
        {
            builder.ToTable("shipment_methods", "dbo");
            builder.HasKey(x => x.ShipmentMethodId).HasName("PK_SHIPMENT_METHOD").IsClustered();
            builder.Property(x => x.ShipmentMethodId).HasColumnName(@"SHIPMENT_METHOD_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ShipmentMethodName).HasColumnName(@"SHIPMENT_METHOD_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.ShipmentMethodNotes).HasColumnName(@"SHIPMENT_METHOD_NOTES").HasColumnType("varchar(150)").IsRequired(false).IsUnicode(false).HasMaxLength(150);
            builder.HasIndex(x => x.ShipmentMethodName).HasDatabaseName("UQ_SHIPMENT_METHOD_NAME").IsUnique();
        }
    }
}
