using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class AutomataConnectivityErrorPatternConfiguration : IEntityTypeConfiguration<AutomataConnectivityErrorPattern>
    {
        public void Configure(EntityTypeBuilder<AutomataConnectivityErrorPattern> builder)
        {
            builder.ToTable("AUTOMATA_CONNECTIVITY_ERROR_PATTERNS");
            builder.HasKey(e => e.Id).HasName("PK_AUTOMATA_CONNECTIVITY_ERROR_PATTERNS");
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("INT").IsRequired();
            builder.Property(e => e.Pattern).HasColumnName("Pattern").HasColumnType("NVARCHAR(1000)").IsRequired();
            builder.Property(e => e.Target).HasColumnName("Target").HasColumnType("NVARCHAR(1)").IsRequired().HasDefaultValue("D");
            builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnType("BIT").IsRequired().HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("DATETIME2(3)").IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("DATETIME2(3)").IsRequired(false);
            builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("NVARCHAR(1000)").IsRequired(false);
            builder.HasIndex(e => e.Pattern).HasDatabaseName("IX_AUTOMATA_CONNECTIVITY_ERROR_PATTERNS_Pattern");
        }
    }
}
