using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    /// <summary>
    /// Mapeo de dbo.COMPETITOR_HOMOLOGATIONS (script 02 del RQM Homologados Competencia).
    /// Sin navegación inversa en Item / ItemReference: no se modifican las entidades existentes.
    /// </summary>
    public class CompetitorHomologationConfiguration : IEntityTypeConfiguration<CompetitorHomologation>
    {
        public void Configure(EntityTypeBuilder<CompetitorHomologation> builder)
        {
            builder.ToTable("COMPETITOR_HOMOLOGATIONS", "dbo");
            builder.HasKey(x => x.CompetitorHomologationId).HasName("PK_COMPETITOR_HOMOLOGATIONS");
            builder.Property(x => x.CompetitorHomologationId).HasColumnName("COMPETITOR_HOMOLOGATION_ID").HasColumnType("int").ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CompetitorCompanyId).HasColumnName("COMPETITOR_COMPANY_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ItemId).HasColumnName("ITEM_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReferenceId).HasColumnName("REFERENCE_ID").HasColumnType("int").IsRequired(false);
            builder.Property(x => x.CompetitorReference).HasColumnName("COMPETITOR_REFERENCE").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.CompetitorProductName).HasColumnName("COMPETITOR_PRODUCT_NAME").HasColumnType("varchar(150)").IsRequired().IsUnicode(false).HasMaxLength(150);
            builder.Property(x => x.ProductUrl).HasColumnName("PRODUCT_URL").HasColumnType("varchar(500)").IsRequired(false).IsUnicode(false).HasMaxLength(500);
            builder.Property(x => x.CompetitorPrice).HasColumnName("COMPETITOR_PRICE").HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(x => x.ImageFileName).HasColumnName("IMAGE_FILE_NAME").HasColumnType("varchar(100)").IsRequired(false).IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.IsActive).HasColumnName("IS_ACTIVE").HasColumnType("bit").IsRequired().HasDefaultValue(true);
            builder.Property(x => x.CreationDate).HasColumnName("CREATION_DATE").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();

            builder.HasOne(x => x.CompetitorCompany).WithMany().HasForeignKey(x => x.CompetitorCompanyId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_COMPETITOR_HOMOLOGATIONS_COMPANY");
            builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_COMPETITOR_HOMOLOGATIONS_ITEM");
            builder.HasOne(x => x.ItemReference).WithMany().HasForeignKey(x => x.ReferenceId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_COMPETITOR_HOMOLOGATIONS_REFERENCE");

            builder.HasIndex(x => new { x.CompetitorCompanyId, x.ItemId, x.ReferenceId, x.CompetitorReference })
                   .HasDatabaseName("UQ_COMPETITOR_HOMOLOGATIONS").IsUnique().HasFilter("[IS_ACTIVE] = 1");
        }
    }
}
