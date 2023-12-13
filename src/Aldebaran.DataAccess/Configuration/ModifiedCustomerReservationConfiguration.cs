using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedCustomerReservationConfiguration : IEntityTypeConfiguration<ModifiedCustomerReservation>
    {
        public void Configure(EntityTypeBuilder<ModifiedCustomerReservation> builder)
        {
            builder.ToTable("MODIFIED_CUSTOMER_RESERVATIONS", "dbo");
            builder.HasKey(x => x.ModifiedCustomerReservationId).HasName("PK_MODIFIED_CUSTOMER_RESERVATION").IsClustered();
            builder.Property(x => x.ModifiedCustomerReservationId).HasColumnName(@"MODIFIED_CUSTOMER_RESERVATION_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerReservationId).HasColumnName(@"CUSTOMER_RESERVATION_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationReasonId).HasColumnName(@"MODIFICATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ModificationDate).HasColumnName(@"MODIFICATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CustomerReservation).WithMany(b => b.ModifiedCustomerReservations).HasForeignKey(c => c.CustomerReservationId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_RESERVATION_CUSTOMER_RESERVATION");
            builder.HasOne(a => a.Employee).WithMany(b => b.ModifiedCustomerReservations).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_RESREVATION_EMPLOYEE");
            builder.HasOne(a => a.ModificationReason).WithMany(b => b.ModifiedCustomerReservations).HasForeignKey(c => c.ModificationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_MODIFIED_CUSTOMER_RESREVATION_MODIFICATION_REASON");
        }
    }
}
