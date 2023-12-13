using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ForwarderAgentConfiguration : IEntityTypeConfiguration<ForwarderAgent>
    {
        public void Configure(EntityTypeBuilder<ForwarderAgent> builder)
        {
            builder.ToTable("forwarder_agents", "dbo");
            builder.HasKey(x => x.ForwarderAgentId).HasName("PK_FORWARDER_AGENT").IsClustered();
            builder.Property(x => x.ForwarderAgentId).HasColumnName(@"FORWARDER_AGENT_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ForwarderId).HasColumnName(@"FORWARDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ForwarderAgentName).HasColumnName(@"FORWARDER_AGENT_NAME").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.Phone1).HasColumnName(@"PHONE1").HasColumnType("varchar(20)").IsRequired().IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Phone2).HasColumnName(@"PHONE2").HasColumnType("varchar(20)").IsRequired(false).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Fax).HasColumnName(@"FAX").HasColumnType("varchar(22)").IsRequired(false).IsUnicode(false).HasMaxLength(22);
            builder.Property(x => x.ForwarderAgentAddress).HasColumnName(@"FORWARDER_AGENT_ADDRESS").HasColumnType("varchar(52)").IsRequired().IsUnicode(false).HasMaxLength(52);
            builder.Property(x => x.CityId).HasColumnName(@"CITY_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.Contact).HasColumnName(@"CONTACT").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.Email1).HasColumnName(@"EMAIL1").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.Email2).HasColumnName(@"EMAIL2").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            // Foreign keys
            builder.HasOne(a => a.City).WithMany(b => b.ForwarderAgents).HasForeignKey(c => c.CityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_FORWARDER_AGENT_CITY");
            builder.HasOne(a => a.Forwarder).WithMany(b => b.ForwarderAgents).HasForeignKey(c => c.ForwarderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_FORWARDER_AGENT_FORWARDER");
            builder.HasIndex(x => x.ForwarderAgentName).HasDatabaseName("UQ_FORWARDER_AGENT_NAME").IsUnique();
        }
    }
}
