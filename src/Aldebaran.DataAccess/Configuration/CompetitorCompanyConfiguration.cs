using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    /// <summary>
    /// Mapeo de dbo.COMPETITOR_COMPANIES (script 02 del RQM Homologados Competencia).
    /// Las relaciones con país/departamento/ciudad no agregan navegación inversa: no se modifican las entidades existentes.
    /// </summary>
    public class CompetitorCompanyConfiguration : IEntityTypeConfiguration<CompetitorCompany>
    {
        public void Configure(EntityTypeBuilder<CompetitorCompany> builder)
        {
            builder.ToTable("COMPETITOR_COMPANIES", "dbo");
            builder.HasKey(x => x.CompetitorCompanyId).HasName("PK_COMPETITOR_COMPANIES");
            builder.Property(x => x.CompetitorCompanyId).HasColumnName("COMPETITOR_COMPANY_ID").HasColumnType("int").ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CompanyName).HasColumnName("COMPANY_NAME").HasColumnType("varchar(100)").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.Website).HasColumnName("WEBSITE").HasColumnType("varchar(250)").IsRequired().IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.ContactPerson).HasColumnName("CONTACT_PERSON").HasColumnType("varchar(50)").IsRequired(false).IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.Phone).HasColumnName("PHONE").HasColumnType("varchar(20)").IsRequired(false).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.CountryId).HasColumnName("COUNTRY_ID").HasColumnType("int").IsRequired(false);
            builder.Property(x => x.DepartmentId).HasColumnName("DEPARTMENT_ID").HasColumnType("int").IsRequired(false);
            builder.Property(x => x.CityId).HasColumnName("CITY_ID").HasColumnType("int").IsRequired(false);
            builder.Property(x => x.IsActive).HasColumnName("IS_ACTIVE").HasColumnType("bit").IsRequired(); // El valor inicial (true) lo da la entidad; el DEFAULT de la tabla queda solo para inserts directos en BD.
            builder.Property(x => x.CreationDate).HasColumnName("CREATION_DATE").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();

            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_COMPETITOR_COMPANIES_COUNTRY");
            builder.HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_COMPETITOR_COMPANIES_DEPARTMENT");
            builder.HasOne(x => x.City).WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_COMPETITOR_COMPANIES_CITY");

            builder.HasIndex(x => x.CompanyName).HasDatabaseName("UQ_COMPETITOR_COMPANIES_NAME_ACTIVE").IsUnique().HasFilter("[IS_ACTIVE] = 1");
        }
    }
}
