using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class AlarmConfiguration : IEntityTypeConfiguration<Alarm>
    {
        public void Configure(EntityTypeBuilder<Alarm> builder)
        {
            builder.ToTable("alarms", "dbo");
            builder.HasKey(x => x.AlarmId).HasName("PK_ALARM").IsClustered();
            builder.Property(x => x.AlarmId).HasColumnName(@"ALARM_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.AlarmMessageId).HasColumnName(@"ALARM_MESSAGE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CREATION_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.ExecutionDate).HasColumnName(@"EXECUTION_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.DocumentId).HasColumnName(@"DOCUMENT_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.IsActive).HasColumnName(@"IS_ACTIVE").HasColumnType("bit").IsRequired();            
            // Foreign keys
            builder.HasOne(a => a.AlarmMessage).WithMany(b => b.Alarms).HasForeignKey(c => c.AlarmMessageId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ALARM_ALARM_MESSAGE");
        }
    }
}
