using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aldebaran.DataAccess.Entities;


namespace Aldebaran.DataAccess.Configuration
{
    public class PurchaseOrderApprovalRangeLogConfiguration : IEntityTypeConfiguration<PurchaseOrderApprovalRangeLog>
    {
        public void Configure(
            EntityTypeBuilder<PurchaseOrderApprovalRangeLog> builder)
        {
            builder.ToTable("PURCHASE_ORDER_APPROVAL_RANGE_LOG");

            builder.HasKey(x => x.PurchaseOrderApprovalRangeLogId);

            builder.Property(x => x.PurchaseOrderApprovalRangeLogId)
                .HasColumnName("PURCHASE_ORDER_APPROVAL_RANGE_LOG_ID");

            builder.Property(x => x.PurchaseOrderApprovalRangeId)
                .HasColumnName("PURCHASE_ORDER_APPROVAL_RANGE_ID");

            builder.Property(x => x.PreviousRequestedQuantityFrom)
                .HasColumnName("PREVIOUS_REQUESTED_QUANTITY_FROM");

            builder.Property(x => x.PreviousRequestedQuantityTo)
                .HasColumnName("PREVIOUS_REQUESTED_QUANTITY_TO");

            builder.Property(x => x.PreviousAllowedDifferencePercent)
                .HasColumnName("PREVIOUS_ALLOWED_DIFFERENCE_PERCENT")
                .HasPrecision(5, 2);

            builder.Property(x => x.PreviousIsActive)
                .HasColumnName("PREVIOUS_IS_ACTIVE");

            builder.Property(x => x.ChangeReason)
                .HasColumnName("CHANGE_REASON")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.ChangedByEmployeeId)
                .HasColumnName("CHANGED_BY_EMPLOYEE_ID");

            builder.Property(x => x.ChangedDate)
                .HasColumnName("CHANGED_DATE");

            builder.HasOne(x => x.ChangedByEmployee)
                .WithMany()
                .HasForeignKey(x => x.ChangedByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}