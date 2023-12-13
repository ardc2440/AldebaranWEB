using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ProcessSatelliteConfiguration : IEntityTypeConfiguration<ProcessSatellite>
    {
        public void Configure(EntityTypeBuilder<ProcessSatellite> builder)
        {
            builder.ToTable("process_satellites", "dbo");
            builder.HasKey(x => x.ProcessSatelliteId).HasName("PK_PROCESS_SATELLITE").IsClustered();
            builder.Property(x => x.ProcessSatelliteId).HasColumnName(@"PROCESS_SATELLITE_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ProcessSatelliteName).HasColumnName(@"PROCESS_SATELLITE_NAME").HasColumnType("varchar(100)").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.ProcessSatelliteAddress).HasColumnName(@"PROCESS_SATELLITE_ADDRESS").HasColumnType("varchar(100)").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.IdentityTypeId).HasColumnName(@"IDENTITY_TYPE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.IdentityNumber).HasColumnName(@"IDENTITY_NUMBER").HasColumnType("varchar(15)").IsRequired().IsUnicode(false).HasMaxLength(15);
            builder.Property(x => x.Phone).HasColumnName(@"PHONE").HasColumnType("varchar(20)").IsRequired().IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Fax).HasColumnName(@"FAX").HasColumnType("varchar(20)").IsRequired(false).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Email).HasColumnName(@"EMAIL").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.CityId).HasColumnName(@"CITY_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.LegalRepresentative).HasColumnName(@"LEGAL_REPRESENTATIVE").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.IsActive).HasColumnName(@"IS_ACTIVE").HasColumnType("bit").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.City).WithMany(b => b.ProcessSatellites).HasForeignKey(c => c.CityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PROCESS_SATELLITE_CITIES");
            builder.HasOne(a => a.IdentityType).WithMany(b => b.ProcessSatellites).HasForeignKey(c => c.IdentityTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PROCESS_SATELLITE_IDENTITY_TYPES");
            builder.HasIndex(x => x.ProcessSatelliteName).HasDatabaseName("UQ_PROCESS_SATELLITE_NAME").IsUnique();
        }
    }
}
