using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedPurchaseOrderConfiguration : IEntityTypeConfiguration<ModifiedPurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<ModifiedPurchaseOrder> builder)
        {
            builder.ToTable("modified_purchase_orders", "dbo");
            builder.HasKey(x => x.ModifiedPurchaseOrderId).HasName("PK_MODIFIED_PURCHASE_ORDER").IsClustered();
            builder.Property(x => x.ModifiedPurchaseOrderId).HasColumnName(@"MODIFIED_PURCHASE_ORDER_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.PurchaseOrderId).HasColumnName(@"PURCHASE_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationReasonId).HasColumnName(@"MODIFICATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationDate).HasColumnName(@"MODIFICATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Employee).WithMany(b => b.ModifiedPurchaseOrders).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_PURCHASE_ORDER_EMPLOYEE");
            builder.HasOne(a => a.ModificationReason).WithMany(b => b.ModifiedPurchaseOrders).HasForeignKey(c => c.ModificationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_PURCHASE_ORDER_MODIFICATION_REASON");
            builder.HasOne(a => a.PurchaseOrder).WithMany(b => b.ModifiedPurchaseOrders).HasForeignKey(c => c.PurchaseOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_PURCHASE_ORDER_PURCHASE_ORDER");
        }
    }
}
