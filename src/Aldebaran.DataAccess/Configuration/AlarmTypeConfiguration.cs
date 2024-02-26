using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class AlarmTypeConfiguration : IEntityTypeConfiguration<AlarmType>
    {
        public void Configure(EntityTypeBuilder<AlarmType> builder)
        {
            builder.ToTable("alarm_types", "dbo");
            builder.HasKey(x => x.AlarmTypeId).HasName("PK_ALARM_TYPE").IsClustered();
            builder.Property(x => x.AlarmTypeId).HasColumnName(@"ALARM_TYPE_ID").HasColumnType("smallint").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.DocumentTypeId).HasColumnName(@"DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Name).HasColumnName(@"NAME").HasColumnType("varchar(30)").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.Description).HasColumnName(@"DESCRIPTION").HasColumnType("varchar(250)").IsRequired().IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.IsManualMessage).HasColumnName(@"IS_MANUAL_MESSAGE").HasColumnType("bit").IsRequired();
            builder.Property(x => x.TableName).HasColumnName(@"TABLE_NAME").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.FieldName).HasColumnName(@"FIELD_NAME").HasColumnType("varchar(30)").IsRequired(false).IsUnicode(false).HasMaxLength(30);
            // Foreign keys
            builder.HasOne(a => a.DocumentType).WithMany(b => b.AlarmTypes).HasForeignKey(c => c.DocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ALARM_TYPE_DOCUMENT_TYPE");
        }
    }
}
