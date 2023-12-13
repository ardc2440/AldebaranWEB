using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderActivityConfiguration : IEntityTypeConfiguration<PurchaseOrderActivity>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderActivity> builder)
        {
            builder.ToTable("purchase_order_activities", "dbo");
            builder.HasKey(x => x.PurchaseOrderActivityId).HasName("PK_ACTORDEN").IsClustered();
            builder.Property(x => x.PurchaseOrderActivityId).HasColumnName(@"PURCHASE_ORDER_ACTIVITY_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.PurchaseOrderId).HasColumnName(@"PURCHASE_ORDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ExecutionDate).HasColumnName(@"EXECUTION_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ActivityDescription).HasColumnName(@"ACTIVITY_DESCRIPTION").HasColumnType("varchar(500)").IsRequired().IsUnicode(false).HasMaxLength(500);
            builder.Property(x => x.CreationDate).HasColumnName(@"CREATION_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ActivityEmployeeId).HasColumnName(@"ACTIVITY_EMPLOYEE_ID").HasColumnType("int").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.ActivityEmployee).WithMany(b => b.PurchaseOrderActivities_ActivityEmployeeId).HasForeignKey(c => c.ActivityEmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_ACTIVITY_ACTIVITY_EMPLOYEE");
            builder.HasOne(a => a.Employee_EmployeeId).WithMany(b => b.PurchaseOrderActivities_EmployeeId).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_ACTIVITY_EMPLOYEE");
            builder.HasOne(a => a.PurchaseOrder).WithMany(b => b.PurchaseOrderActivities).HasForeignKey(c => c.PurchaseOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_ACTIVITY_PURCHASE_ORDER");
            builder.HasIndex(x => new { x.PurchaseOrderId, x.PurchaseOrderActivityId, x.ExecutionDate }).HasDatabaseName("IND_PURCHASE_ORDERS_ACTIVITIIES_EXECUTION_DATE");
        }
    }
}
