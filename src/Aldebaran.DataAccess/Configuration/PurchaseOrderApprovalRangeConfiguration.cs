using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aldebaran.DataAccess.Entities;


namespace Aldebaran.DataAccess.Configuration
{
    public class PurchaseOrderApprovalRangeConfiguration
        : IEntityTypeConfiguration<PurchaseOrderApprovalRange>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderApprovalRange> builder)
        {
            builder.ToTable("PURCHASE_ORDER_APPROVAL_RANGES",
                t => {t.HasTrigger("TR_PURCHASE_ORDER_APPROVAL_RANGES_VALIDATE");});

            builder.HasKey(x => x.PurchaseOrderApprovalRangeId);

            builder.Property(x => x.PurchaseOrderApprovalRangeId)
                   .HasColumnName("PURCHASE_ORDER_APPROVAL_RANGE_ID")
                   .ValueGeneratedOnAdd(); 

            builder.Property(x => x.RequestedQuantityFrom)
                   .HasColumnName("REQUESTED_QUANTITY_FROM")
                   .IsRequired();

            builder.Property(x => x.RequestedQuantityTo)
                   .HasColumnName("REQUESTED_QUANTITY_TO")
                   .IsRequired();

            builder.Property(x => x.AllowedDifferencePercent)
                   .HasColumnName("ALLOWED_DIFFERENCE_PERCENT")
                   .HasPrecision(5, 2)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .HasColumnName("IS_ACTIVE")
                   .IsRequired();

            builder.Property(x => x.CreatedDate)
                   .HasColumnName("CREATED_DATE")
                   .IsRequired();

            builder.Property(x => x.EmployeeId)
                   .HasColumnName("CREATED_BY_EMPLOYEE_ID");

            builder.HasOne(a => a.Employee)
                .WithMany(b => b.PurchaseOrderApprovalRanges)
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PO_APPROVAL_RANGE_EMPLOYEE");
        }
    }
}