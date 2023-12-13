using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("cities", "dbo");
            builder.HasKey(x => x.CityId).HasName("PK_CITY").IsClustered();
            builder.Property(x => x.CityId).HasColumnName(@"CITY_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CityName).HasColumnName(@"CITY_NAME").HasColumnType("varchar(40)").IsRequired().IsUnicode(false).HasMaxLength(40);
            builder.Property(x => x.DepartmentId).HasColumnName(@"DEPARTMENT_ID").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Department).WithMany(b => b.Cities).HasForeignKey(c => c.DepartmentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CITY_DEPARTAMENT");
            builder.HasIndex(x => new { x.CityName, x.DepartmentId }).HasDatabaseName("UQ_CITY_NAME").IsUnique();
        }
    }
}
