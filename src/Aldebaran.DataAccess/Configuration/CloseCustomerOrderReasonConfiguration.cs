using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CloseCustomerOrderReasonConfiguration : IEntityTypeConfiguration<CloseCustomerOrderReason>
    {
        public void Configure(EntityTypeBuilder<CloseCustomerOrderReason> builder)
        {
            builder.ToTable("CLOSE_CUSTOMER_ORDER_REASONS", "dbo");
            builder.HasKey(x => x.CloseCustomerOrderReasonId).HasName("PK_CLOSE_CUSTOMER_ORDER_REASON").IsClustered();
            builder.Property(x => x.CloseCustomerOrderReasonId).HasColumnName(@"CLOSE_CUSTOMER_ORDER_REASON_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CloseReasonName).HasColumnName(@"CLOSE_REASON_NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.CloseReasonNotes).HasColumnName(@"CLOSE_REASON_NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.HasIndex(x => x.CloseReasonName).HasDatabaseName("UQ_CLOSE_CUSTOMER_ORDER_REASON_NAME").IsUnique();
        }
    }
}
