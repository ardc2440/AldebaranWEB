using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
    {
        public void Configure(EntityTypeBuilder<CustomerContact> builder)
        {
            builder.ToTable("customer_contacts", "dbo");
            builder.HasKey(x => x.CustomerContactId).HasName("PK_CUSTOMER_CONTACT").IsClustered();
            builder.Property(x => x.CustomerContactId).HasColumnName(@"CUSTOMER_CONTACT_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerId).HasColumnName(@"CUSTOMER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CustomerContactName).HasColumnName(@"CUSTOMER_CONTACT_NAME").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.Title).HasColumnName(@"TITLE").HasColumnType("varchar(20)").IsRequired(false).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Phone).HasColumnName(@"PHONE").HasColumnType("varchar(15)").IsRequired(false).IsUnicode(false).HasMaxLength(15);
            builder.Property(x => x.Email).HasColumnName(@"EMAIL").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            // Foreign keys
            builder.HasOne(a => a.Customer).WithMany(b => b.CustomerContacts).HasForeignKey(c => c.CustomerId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_CONTACT_CUSTOMER");
            builder.HasIndex(x => new { x.CustomerId, x.CustomerContactName }).HasDatabaseName("UQ_CUSTOMER_CONTACT").IsUnique();
        }
    }
}
