using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class AlarmMessageConfiguration : IEntityTypeConfiguration<AlarmMessage>
    {
        public void Configure(EntityTypeBuilder<AlarmMessage> builder)
        {
            builder.ToTable("ALARM_MESSAGES", "dbo");
            builder.HasKey(x => x.AlarmMessageId).HasName("PK_ALARM_MESSAGE").IsClustered();
            builder.Property(x => x.AlarmMessageId).HasColumnName(@"ALARM_MESSAGE_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.AlarmTypeId).HasColumnName(@"ALARM_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Message).HasColumnName(@"ALARM_MESSAGE").HasColumnType("varchar(200)").IsRequired().IsUnicode(false).HasMaxLength(200);
            // Foreign keys
            builder.HasOne(a => a.AlarmType).WithMany(b => b.AlarmMessages).HasForeignKey(c => c.AlarmTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ALARM_MESSAGE_ALARM_TYPE");
        }
    }
}
