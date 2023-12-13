using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class AlarmConfiguration : IEntityTypeConfiguration<Alarm>
    {
        public void Configure(EntityTypeBuilder<Alarm> builder)
        {
            builder.ToTable("ALARMS", "dbo");
            builder.HasKey(x => x.AlarmId).HasName("PK_ALARM").IsClustered();
            builder.Property(x => x.AlarmId).HasColumnName(@"ALARM_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.AlarmTypeId).HasColumnName(@"ALARM_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.AlarmGenerationDate).HasColumnName(@"ALARM_GENERATION_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.DocumentId).HasColumnName(@"DOCUMENT_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.IsActive).HasColumnName(@"IS_ACTIVE").HasColumnType("bit").IsRequired();
            builder.Property(x => x.AlarmMessage).HasColumnName(@"ALARM_MESSAGE").HasColumnType("varchar(200)").IsRequired().IsUnicode(false).HasMaxLength(200);
            // Foreign keys
            builder.HasOne(a => a.AlarmType).WithMany(b => b.Alarms).HasForeignKey(c => c.AlarmTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ALARM_ALARM_TYPE");
        }
    }
}
