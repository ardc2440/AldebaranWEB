using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Configuration
{
    public class InventoryAutomationConnectionConfiguration : IEntityTypeConfiguration<InventoryAutomationConnection>
    {
        public void Configure(EntityTypeBuilder<InventoryAutomationConnection> builder)
        {
            builder.ToTable("Inventory_Automation_Connections");

            builder.HasKey(e => e.InventoryAutomationConnectionId)
                   .HasName("PK_Inventory_Automation_Connections");

            builder.Property(e => e.InventoryAutomationConnectionId)
                   .HasColumnName("INVENTORY_AUTOMATION_CONNECTION_ID")
                   .HasColumnType("INT")
                   .IsRequired();

            builder.Property(e => e.ServerName)
                   .HasColumnName("SERVER_NAME")
                   .HasColumnType("VARCHAR(256)")
                   .IsRequired();

            builder.Property(e => e.PortNumber)
                   .HasColumnName("PORT_NUMBER")
                   .HasColumnType("VARCHAR(10)");

            builder.Property(e => e.DatabaseName)
                   .HasColumnName("DATABASE_NAME")
                   .HasColumnType("VARCHAR(100)")
                   .IsRequired();

            builder.Property(e => e.UserId)
                   .HasColumnName("USER_ID")
                   .HasColumnType("VARCHAR(50)")
                   .IsRequired();

            builder.Property(e => e.Password)
                   .HasColumnName("PASSWORD")
                   .HasColumnType("VARCHAR(20)")
                   .IsRequired();

            builder.Property(e => e.Active)
                   .HasColumnName("ACTIVE")
                   .HasColumnType("BIT")
                   .IsRequired()
                   .HasDefaultValue(false);
        }
    }
}