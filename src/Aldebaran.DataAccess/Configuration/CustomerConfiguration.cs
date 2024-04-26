using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("customers", "dbo");
            builder.HasKey(x => x.CustomerId).HasName("PK_CUSTOMER").IsClustered();
            builder.Property(x => x.CustomerId).HasColumnName(@"CUSTOMER_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.IdentityTypeId).HasColumnName(@"IDENTITY_TYPE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.IdentityNumber).HasColumnName(@"IDENTITY_NUMBER").HasColumnType("varchar(15)").IsRequired().IsUnicode(false).HasMaxLength(15);
            builder.Property(x => x.CustomerName).HasColumnName(@"CUSTOMER_NAME").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.Phone1).HasColumnName(@"PHONE1").HasColumnType("varchar(20)").IsRequired().IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Phone2).HasColumnName(@"PHONE2").HasColumnType("varchar(20)").IsRequired(false).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Fax).HasColumnName(@"FAX").HasColumnType("varchar(22)").IsRequired(false).IsUnicode(false).HasMaxLength(22);
            builder.Property(x => x.CustomerAddress).HasColumnName(@"CUSTOMER_ADDRESS").HasColumnType("varchar(52)").IsRequired().IsUnicode(false).HasMaxLength(52);
            builder.Property(x => x.CellPhone).HasColumnName(@"CELL_PHONE").HasColumnType("varchar(15)").IsRequired(false).IsUnicode(false).HasMaxLength(15);
            builder.Property(x => x.Email1).HasColumnName(@"EMAIL1").HasColumnType("varchar(252)").IsRequired(false).IsUnicode(false).HasMaxLength(252);
            builder.Property(x => x.Email2).HasColumnName(@"EMAIL2").HasColumnType("varchar(252)").IsRequired(false).IsUnicode(false).HasMaxLength(252);
            builder.Property(x => x.CityId).HasColumnName(@"CITY_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.Email3).HasColumnName(@"EMAIL3").HasColumnType("varchar(252)").IsRequired(false).IsUnicode(false).HasMaxLength(252);
            // Foreign keys
            builder.HasOne(a => a.City).WithMany(b => b.Customers).HasForeignKey(c => c.CityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_CITY");
            builder.HasOne(a => a.IdentityType).WithMany(b => b.Customers).HasForeignKey(c => c.IdentityTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_IDENTITY_TYPE");
            builder.HasIndex(x => new { x.CityId, x.CustomerId }).HasDatabaseName("IND_CUSTOMER_CITY");
            builder.HasIndex(x => new { x.CityId, x.CustomerName }).HasDatabaseName("IND_CUSTOMER_CITY_CUSTOMER_NAME");
            builder.HasIndex(x => new { x.CityId, x.IdentityNumber }).HasDatabaseName("IND_CUSTOMER_CITY_IDENTITY_NUMBER");
            builder.HasIndex(x => x.IdentityNumber).HasDatabaseName("UQ_CUSTOMER_IDENTIFICATION").IsUnique();
            builder.HasIndex(x => x.CustomerName).HasDatabaseName("UQ_CUSTOMER_NAME").IsUnique();
        }
    }
}
