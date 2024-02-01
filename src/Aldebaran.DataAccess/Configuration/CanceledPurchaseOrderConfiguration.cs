using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CanceledPurchaseOrderConfiguration : IEntityTypeConfiguration<CanceledPurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<CanceledPurchaseOrder> builder)
        {
            builder.ToTable("canceled_purchase_orders", "dbo");
            builder.HasKey(x => x.PurchaseOrderId).HasName("PK_CANCELED_PURCHASE_ORDER").IsClustered();
            builder.Property(x => x.PurchaseOrderId).HasColumnName(@"PURCHASE_ORDER_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.CancellationReasonId).HasColumnName(@"CANCELLATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CancellationDate).HasColumnName(@"CANCELLATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CancellationReason).WithMany(b => b.CanceledPurchaseOrders).HasForeignKey(c => c.CancellationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_PURCHASE_ORDER_CANCELLATION");
            builder.HasOne(a => a.Employee).WithMany(b => b.CanceledPurchaseOrders).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_PURCHASE_ORDER_EMPLOYEE");
            builder.HasOne(a => a.PurchaseOrder).WithOne(b => b.CanceledPurchaseOrder).HasForeignKey<CanceledPurchaseOrder>(c => c.PurchaseOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_PURCHASE_ORDER_PURCHASE_ORDER");
        }
    }
}
