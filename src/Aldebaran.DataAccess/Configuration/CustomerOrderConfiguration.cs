using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderConfiguration : IEntityTypeConfiguration<CustomerOrder>
    {
        public void Configure(EntityTypeBuilder<CustomerOrder> builder)
        {
            builder.ToTable("customer_orders", "dbo");
            builder.HasKey(x => x.CustomerOrderId).HasName("PK_CUSTOMER_ORDER").IsClustered();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerId).HasColumnName(@"CUSTOMER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.OrderNumber).HasColumnName(@"ORDER_NUMBER").HasColumnType("varchar(10)").IsRequired().IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.OrderDate).HasColumnName(@"ORDER_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.EstimatedDeliveryDate).HasColumnName(@"ESTIMATED_DELIVERY_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.InternalNotes).HasColumnName(@"INTERNAL_NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.StatusDocumentTypeId).HasColumnName(@"STATUS_DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CREATION_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.CustomerNotes).HasColumnName(@"CUSTOMER_NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            // Foreign keys
            builder.HasOne(a => a.Customer).WithMany(b => b.CustomerOrders).HasForeignKey(c => c.CustomerId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_CUSTOMER");
            builder.HasOne(a => a.Employee).WithMany(b => b.CustomerOrders).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_EMPLOYEE");
            builder.HasOne(a => a.StatusDocumentType).WithMany(b => b.CustomerOrders).HasForeignKey(c => c.StatusDocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_ORDER_STATUS_DOCUMENT_TYPE");
            builder.HasIndex(x => x.CreationDate).HasDatabaseName("IND_CUSTOMER_ORDER_CREACION_DATE");
            builder.HasIndex(x => x.CustomerId).HasDatabaseName("IND_CUSTOMER_ORDER_CUSTOMER");
            builder.HasIndex(x => x.StatusDocumentTypeId).HasDatabaseName("IND_CUSTOMER_ORDER_ESTADO");
            builder.HasIndex(x => new { x.OrderDate, x.CustomerId, x.OrderNumber }).HasDatabaseName("IND_CUSTOMER_ORDER_FECHAPEDIDO");
            builder.HasIndex(x => x.OrderNumber).HasDatabaseName("IND_CUSTOMER_ORDER_ORDER_NUMBER");
            builder.HasIndex(x => x.OrderNumber).HasDatabaseName("UQ_CUSTOMER_ORDER_NUMBER").IsUnique();
        }
    }
}
