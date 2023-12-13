using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("warehouses", "dbo");
            builder.HasKey(x => x.WarehouseId).HasName("PK_WAREHOUSE").IsClustered();
            builder.Property(x => x.WarehouseId).HasColumnName(@"WAREHOUSE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.WarehouseName).HasColumnName(@"WAREHOUSE_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.HasIndex(x => x.WarehouseName).HasDatabaseName("UQ_WAREHOUSE_NAME").IsUnique();
        }
    }
}
