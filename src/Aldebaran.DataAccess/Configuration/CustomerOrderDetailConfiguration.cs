using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderDetailConfiguration : IEntityTypeConfiguration<CustomerOrderDetail>
    {
        public void Configure(EntityTypeBuilder<CustomerOrderDetail> builder)
        {
            builder.ToTable("customer_order_details", "dbo");
            builder.HasKey(x => x.CustomerOrderDetailId).HasName("PK_CUSTOMER_ORDER_DETAIL").IsClustered();
            builder.Property(x => x.CustomerOrderDetailId).HasColumnName(@"CUSTOMER_ORDER_DETAIL_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReferenceId).HasColumnName(@"REFERENCE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.RequestedQuantity).HasColumnName(@"REQUESTED_QUANTITY").HasColumnType("int").IsRequired();
            builder.Property(x => x.ProcessedQuantity).HasColumnName(@"PROCESSED_QUANTITY").HasColumnType("int").IsRequired();
            builder.Property(x => x.DeliveredQuantity).HasColumnName(@"DELIVERED_QUANTITY").HasColumnType("int").IsRequired();
            builder.Property(x => x.Brand).HasColumnName(@"BRAND").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            // Foreign keys
            builder.HasOne(a => a.CustomerOrder).WithMany(b => b.CustomerOrderDetails).HasForeignKey(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_DETAIL_CUSTOMER_ORDER");
            builder.HasOne(a => a.ItemReference).WithMany(b => b.CustomerOrderDetails).HasForeignKey(c => c.ReferenceId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_DETAIL_ITEM_REFERENCE");
            builder.HasIndex(x => new { x.CustomerOrderId, x.ReferenceId }).HasDatabaseName("UQIND_CUSTOMER_ORDER_DETAILS").IsUnique();
        }
    }
}
