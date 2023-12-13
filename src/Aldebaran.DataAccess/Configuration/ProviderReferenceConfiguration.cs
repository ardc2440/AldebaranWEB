using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ProviderReferenceConfiguration : IEntityTypeConfiguration<ProviderReference>
    {
        public void Configure(EntityTypeBuilder<ProviderReference> builder)
        {
            builder.ToTable("provider_references", "dbo");
            builder.HasKey(x => new { x.ReferenceId, x.ProviderId }).HasName("PK_PROVIDER_REFERENCE").IsClustered();
            builder.Property(x => x.ReferenceId).HasColumnName(@"REFERENCE_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.ProviderId).HasColumnName(@"PROVIDER_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            // Foreign keys
            builder.HasOne(a => a.ItemReference).WithMany(b => b.ProviderReferences).HasForeignKey(c => c.ReferenceId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PROVIDER_REFERENCE_REFERENCE");
            builder.HasOne(a => a.Provider).WithMany(b => b.ProviderReferences).HasForeignKey(c => c.ProviderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PROVIDER_REFERENCE_PROVIDER");
        }
    }
}
