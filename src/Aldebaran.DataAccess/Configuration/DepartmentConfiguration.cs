using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("departments", "dbo");
            builder.HasKey(x => x.DepartmentId).HasName("PK_DEPARTMENT").IsClustered();
            builder.Property(x => x.DepartmentId).HasColumnName(@"DEPARTMENT_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.DepartmentName).HasColumnName(@"DEPARTMENT_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.CountryId).HasColumnName(@"COUNTRY_ID").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Country).WithMany(b => b.Departments).HasForeignKey(c => c.CountryId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_DEPARTMENT_COUNTRY");
            builder.HasIndex(x => new { x.DepartmentName, x.CountryId }).HasDatabaseName("UQ_DEPARTMENT_NAME").IsUnique();
        }
    }
}
