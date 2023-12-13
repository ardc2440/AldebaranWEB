using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("employees", "dbo");
            builder.HasKey(x => x.EmployeeId).HasName("PK_users").IsClustered();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.AreaId).HasColumnName(@"AREA_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.IdentityTypeId).HasColumnName(@"IDENTITY_TYPE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.IdentityNumber).HasColumnName(@"IDENTITY_NUMBER").HasColumnType("varchar(15)").IsRequired().IsUnicode(false).HasMaxLength(15);
            builder.Property(x => x.DisplayName).HasColumnName(@"DISPLAY_NAME").HasColumnType("nvarchar(50)").IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.FullName).HasColumnName(@"FULL_NAME").HasColumnType("nvarchar(50)").IsRequired().HasMaxLength(50);
            builder.Property(x => x.LoginUserId).HasColumnName(@"LOGIN_USER_ID").HasColumnType("nvarchar(450)").IsRequired(false).HasMaxLength(450);
            builder.Property(x => x.Position).HasColumnName(@"POSITION").HasColumnType("nvarchar(30)").IsRequired(false).HasMaxLength(30);
            // Foreign keys
            builder.HasOne(a => a.Area).WithMany(b => b.Employees).HasForeignKey(c => c.AreaId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_users_areas");
            builder.HasOne(a => a.IdentityType).WithMany(b => b.Employees).HasForeignKey(c => c.IdentityTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_users_identity_types");
        }
    }
}
