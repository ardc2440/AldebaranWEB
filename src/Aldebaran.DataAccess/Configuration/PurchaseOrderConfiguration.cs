using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.ToTable("purchase_orders", "dbo");
            builder.HasKey(x => x.PurchaseOrderId).HasName("PK_PURCHASE_ORDER").IsClustered();
            builder.Property(x => x.PurchaseOrderId).HasColumnName(@"PURCHASE_ORDER_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.OrderNumber).HasColumnName(@"ORDER_NUMBER").HasColumnType("varchar(10)").IsRequired().IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.RequestDate).HasColumnName(@"REQUEST_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.ExpectedReceiptDate).HasColumnName(@"EXPECTED_RECEIPT_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.RealReceiptDate).HasColumnName(@"REAL_RECEIPT_DATE").HasColumnType("date").IsRequired(false);
            builder.Property(x => x.ProviderId).HasColumnName(@"PROVIDER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ForwarderAgentId).HasColumnName(@"FORWARDER_AGENT_ID").HasColumnType("int").IsRequired(false);
            builder.Property(x => x.ShipmentForwarderAgentMethodId).HasColumnName(@"SHIPMENT_FORWARDER_AGENT_METHOD_ID").HasColumnType("smallint").IsRequired(false);
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.StatusDocumentTypeId).HasColumnName(@"STATUS_DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.ImportNumber).HasColumnName(@"IMPORT_NUMBER").HasColumnType("varchar(20)").IsRequired(false).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.EmbarkationPort).HasColumnName(@"EMBARKATION_PORT").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.ProformaNumber).HasColumnName(@"PROFORMA_NUMBER").HasColumnType("varchar(20)").IsRequired().IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.CreationDate).HasColumnName(@"CREATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Employee).WithMany(b => b.PurchaseOrders).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_EMPLOYEE");
            builder.HasOne(a => a.ForwarderAgent).WithMany(b => b.PurchaseOrders).HasForeignKey(c => c.ForwarderAgentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_FORWARDER_AGENT");
            builder.HasOne(a => a.Provider).WithMany(b => b.PurchaseOrders).HasForeignKey(c => c.ProviderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_PROVIDER");
            builder.HasOne(a => a.ShipmentForwarderAgentMethod).WithMany(b => b.PurchaseOrders).HasForeignKey(c => c.ShipmentForwarderAgentMethodId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PURCHASE_ORDER_SHIPMENT_FORWARDER_AGENT_METHOD");
            builder.HasOne(a => a.StatusDocumentType).WithMany(b => b.PurchaseOrders).HasForeignKey(c => c.StatusDocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_purchase_orders_status_document_types");
            builder.HasIndex(x => x.CreationDate).HasDatabaseName("IND_PURCHASE_ORDERS_CREATION_DATE");
            builder.HasIndex(x => x.RequestDate).HasDatabaseName("IND_PURCHASE_ORDERS_REQUEST_DATE");
            builder.HasIndex(x => x.StatusDocumentTypeId).HasDatabaseName("IND_PURCHASE_ORDERS_STATUS_DOCUMENT");
            builder.HasIndex(x => x.OrderNumber).HasDatabaseName("UQ_PURCHASE_ORDER_ORDER_NUMBER").IsUnique();
            builder.ToTable(tb => tb.HasTrigger("TRGINSERTSTRANSITO_ORDERS"));
        }
    }
}
