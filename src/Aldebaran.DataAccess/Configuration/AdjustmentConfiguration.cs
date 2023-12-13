using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class AdjustmentConfiguration : IEntityTypeConfiguration<Adjustment>
    {
        public void Configure(EntityTypeBuilder<Adjustment> builder)
        {
            builder.ToTable("adjustments", "dbo");
            builder.HasKey(x => x.AdjustmentId).HasName("PK_ADJUSTMENTS").IsClustered();
            builder.Property(x => x.AdjustmentId).HasColumnName(@"ADJUSTMENT_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.AdjustmentDate).HasColumnName(@"ADJUSTMENT_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.AdjustmentReasonId).HasColumnName(@"ADJUSTMENT_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.AdjustmentTypeId).HasColumnName(@"ADJUSTMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.CreationDate).HasColumnName(@"CREATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.AdjustmentReason).WithMany(b => b.Adjustments).HasForeignKey(c => c.AdjustmentReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ADJUSTMENT_ADJUSTMENT_RESON");
            builder.HasOne(a => a.AdjustmentType).WithMany(b => b.Adjustments).HasForeignKey(c => c.AdjustmentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ADJUSTMENT_ADJUSTMENT_TYPE");
            builder.HasOne(a => a.Employee).WithMany(b => b.Adjustments).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ADJUSTMENT_EMPLOYEE");
        }
    }
}
