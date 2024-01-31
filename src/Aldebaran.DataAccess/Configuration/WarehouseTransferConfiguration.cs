using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class WarehouseTransferConfiguration : IEntityTypeConfiguration<WarehouseTransfer>
    {
        public void Configure(EntityTypeBuilder<WarehouseTransfer> builder)
        {
            builder.ToTable("warehouse_transfers", "dbo");
            builder.HasKey(x => x.WarehouseTransferId).HasName("PK_WAREHOUSE_TRANSFER").IsClustered();
            builder.Property(x => x.WarehouseTransferId).HasColumnName(@"WAREHOUSE_TRANSFER_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.TransferDate).HasColumnName(@"TRANSFER_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.OriginWarehouseId).HasColumnName(@"ORIGIN_WAREHOUSE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.DestinationWarehouseId).HasColumnName(@"DESTINATION_WAREHOUSE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.CreationDate).HasColumnName(@"CREATION_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.Nationalization).HasColumnName(@"NATIONALIZATION").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.StatusDocumentTypeId).HasColumnName(@"STATUS_DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.OriginWarehouse).WithMany(b => b.OriginWarehouseTransfers).HasForeignKey(c => c.OriginWarehouseId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WAREHOUSE_TRANSFER_ORIGIN_WAREHOUSE");
            builder.HasOne(a => a.DestinationWarehouse).WithMany(b => b.DestinationWarehouseTransfers).HasForeignKey(c => c.DestinationWarehouseId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WAREHOUSE_TRANSFER_DESTINATION_WAREHOUSE");
            builder.HasOne(a => a.Employee).WithMany(b => b.WarehouseTransfers).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WAREHOUSE_TRANSFER_EMPLOYEE");
            builder.HasOne(a => a.StatusDocumentType).WithMany(b => b.WarehouseTransfers).HasForeignKey(c => c.StatusDocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WAREHOUSE_TRANSFER_STATUS_DOCUMENT_TYPE");
        }
    }
}