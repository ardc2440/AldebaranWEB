using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class PurchaseOrderAdjustmentLogConfiguration: IEntityTypeConfiguration<PurchaseOrderAdjustmentLog>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderAdjustmentLog> builder)
        {
            builder.ToTable("PURCHASE_ORDER_ADJUSTMENT_LOG");

            builder.HasKey(x => x.PurchaseOrderAdjustmentLogId);

            builder.Property(x => x.PurchaseOrderAdjustmentLogId)
                .HasColumnName("PURCHASE_ORDER_ADJUSTMENT_LOG_ID")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.PurchaseOrderId)
                .HasColumnName("PURCHASE_ORDER_ID")
                .IsRequired();

            builder.Property(x => x.NewStatusDocumentTypeId)
                .HasColumnName("NEW_STATUS_DOCUMENT_TYPE_ID")
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasColumnName("REASON")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.EmployeeId)
                .HasColumnName("EMPLOYE_ID")
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .HasColumnName("CREATED_DATE")
                .HasColumnType("datetime")
                .IsRequired();

            // FK PurchaseOrder
            builder.HasOne<PurchaseOrder>()
                .WithMany()
                .HasForeignKey(x => x.PurchaseOrderId)
                .HasConstraintName("FK_PO_ADJ_LOG_PURCHASE_ORDER");

            // FK StatusDocumentType
            builder.HasOne<StatusDocumentType>()
                .WithMany()
                .HasForeignKey(x => x.NewStatusDocumentTypeId)
                .HasConstraintName("FK_PO_ADJ_LOG_NEW_STATUS");

            // FK Employee
            builder.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .HasConstraintName("FK_PO_ADJ_LOG_USER");
        }
    }
}