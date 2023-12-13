using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderInProcessDetailConfiguration : IEntityTypeConfiguration<CustomerOrderInProcessDetail>
    {
        public void Configure(EntityTypeBuilder<CustomerOrderInProcessDetail> builder)
        {
            builder.ToTable("customer_order_in_process_details", "dbo");
            builder.HasKey(x => x.CustomerOrderInProcessDetailId).HasName("PK_CUSTMER_ORDER_IN_PROCESS_DETAIL").IsClustered();
            builder.Property(x => x.CustomerOrderInProcessDetailId).HasColumnName(@"CUSTOMER_ORDER_IN_PROCESS_DETAIL_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerOrderInProcessId).HasColumnName(@"CUSTOMER_ORDER_IN_PROCESS_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CustomerOrderDetailId).HasColumnName(@"CUSTOMER_ORDER_DETAIL_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.WarehouseId).HasColumnName(@"WAREHOUSE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.ProcessedQuantity).HasColumnName(@"PROCESSED_QUANTITY").HasColumnType("int").IsRequired();
            builder.Property(x => x.Brand).HasColumnName(@"BRAND").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            // Foreign keys
            builder.HasOne(a => a.CustomerOrderDetail).WithMany(b => b.CustomerOrderInProcessDetails).HasForeignKey(c => c.CustomerOrderDetailId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTMER_ORDER_IN_PROCESS_DETAIL_CUSTOMER_ORDER_DETAIL");
            builder.HasOne(a => a.CustomerOrdersInProcess).WithMany(b => b.CustomerOrderInProcessDetails).HasForeignKey(c => c.CustomerOrderInProcessId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTMER_ORDER_IN_PROCESS_DETAIL_ORDER_IN_PROCESS");
            builder.HasOne(a => a.Warehouse).WithMany(b => b.CustomerOrderInProcessDetails).HasForeignKey(c => c.WarehouseId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTMER_ORDER_IN_PROCESS_DETAIL_WAREHOUSES");
            builder.HasIndex(x => new { x.CustomerOrderInProcessId, x.CustomerOrderDetailId }).HasDatabaseName("UQ_CUSTMER_ORDER_IN_PROCESS_DETAIL").IsUnique();
        }
    }
}
