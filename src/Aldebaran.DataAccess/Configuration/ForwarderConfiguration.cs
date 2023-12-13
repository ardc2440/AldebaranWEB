using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ForwarderConfiguration : IEntityTypeConfiguration<Forwarder>
    {
        public void Configure(EntityTypeBuilder<Forwarder> builder)
        {
            builder.ToTable("forwarders", "dbo");
            builder.HasKey(x => x.ForwarderId).HasName("PK_FORWARDER").IsClustered();
            builder.Property(x => x.ForwarderId).HasColumnName(@"FORWARDER_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ForwarderName).HasColumnName(@"FORWARDER_NAME").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.Phone1).HasColumnName(@"PHONE1").HasColumnType("varchar(20)").IsRequired().IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Phone2).HasColumnName(@"PHONE2").HasColumnType("varchar(20)").IsRequired(false).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Fax).HasColumnName(@"FAX").HasColumnType("varchar(22)").IsRequired(false).IsUnicode(false).HasMaxLength(22);
            builder.Property(x => x.ForwarderAddress).HasColumnName(@"FORWARDER_ADDRESS").HasColumnType("varchar(52)").IsRequired().IsUnicode(false).HasMaxLength(52);
            builder.Property(x => x.Mail1).HasColumnName(@"MAIL1").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.Mail2).HasColumnName(@"MAIL2").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.CityId).HasColumnName(@"CITY_ID").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.City).WithMany(b => b.Forwarders).HasForeignKey(c => c.CityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_FORWARDER_CITY");
            builder.HasIndex(x => new { x.CityId, x.ForwarderId }).HasDatabaseName("IND_FORWARDER_CITY");
            builder.HasIndex(x => new { x.CityId, x.ForwarderName }).HasDatabaseName("IND_FORWARDER_NAME");
            builder.HasIndex(x => x.ForwarderName).HasDatabaseName("UQ_FORWARDER_NAME").IsUnique();
        }
    }
}
