using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ShipmentForwarderAgentMethodConfiguration : IEntityTypeConfiguration<ShipmentForwarderAgentMethod>
    {
        public void Configure(EntityTypeBuilder<ShipmentForwarderAgentMethod> builder)
        {
            builder.ToTable("shipment_forwarder_agent_methods", "dbo");
            builder.HasKey(x => x.ShipmentForwarderAgentMethodId).HasName("PK_SHIPMENT_FORWARDER_AGENT_METHOD").IsClustered();
            builder.Property(x => x.ShipmentForwarderAgentMethodId).HasColumnName(@"SHIPMENT_FORWARDER_AGENT_METHOD_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.ShipmentMethodId).HasColumnName(@"SHIPMENT_METHOD_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.ForwarderAgentId).HasColumnName(@"FORWARDER_AGENT_ID").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.ForwarderAgent).WithMany(b => b.ShipmentForwarderAgentMethods).HasForeignKey(c => c.ForwarderAgentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SHIPMENT_FORWARDER_AGENT_METHOD_FORWARDER_AGENT");
            builder.HasOne(a => a.ShipmentMethod).WithMany(b => b.ShipmentForwarderAgentMethods).HasForeignKey(c => c.ShipmentMethodId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SHIPMENT_FORWARDER_AGENT_METHOD_SHIPMENT_METHOD");
            builder.HasIndex(x => new { x.ShipmentMethodId, x.ForwarderAgentId }).HasDatabaseName("UQ_SHIPMENT_FORWARDER_AGENT_METHOD_FORWARDER_AGENT").IsUnique();
        }
    }
}
