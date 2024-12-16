using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class LocalWarehouseAlarmConfiguration : IEntityTypeConfiguration<LocalWarehouseAlarm>
    {
        public void Configure(EntityTypeBuilder<LocalWarehouseAlarm> builder)
        {
            builder.ToTable("Local_Warehouse_Alarms", "dbo");
            builder.HasKey(x => x.LocalWarehouseAlarmId).HasName("PK_LOCAL_WAREHOUSE_ALARM").IsClustered();
            builder.Property(x => x.LocalWarehouseAlarmId).HasColumnName(@"LOCAL_WAREHOUSE_ALARM_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.DocumentTypeId).HasColumnName(@"DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.DocumentNumber).HasColumnName(@"DOCUMENT_NUMBER").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReferenceList).HasColumnName(@"REFERENCE_LIST").HasColumnType("nvarchar(MAX)").IsRequired();
            builder.Property(x => x.CustomerOrderList).HasColumnName(@"CUSTOMER_ORDER_LIST").HasColumnType("nvarchar(MAX)");
            builder.Property(x => x.AlarmDate).HasColumnName(@"ALARM_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.DocumentType).WithMany(b => b.LocalWarehouseAlarms).HasForeignKey(c => c.DocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_LOCAL_WAREHOUSE_ALARM_DOCUMENT_TYPE");
        }
    }
}
