using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CanceledCustomerReservationConfiguration : IEntityTypeConfiguration<CanceledCustomerReservation>
    {
        public void Configure(EntityTypeBuilder<CanceledCustomerReservation> builder)
        {
            builder.ToTable("CANCELED_CUSTOMER_RESERVATIONS", "dbo");
            builder.HasKey(x => x.CustomerReservationId).HasName("PK_CANCELED_CUSTOMER_RESERVATION").IsClustered();
            builder.Property(x => x.CustomerReservationId).HasColumnName(@"CUSTOMER_RESERVATION_ID").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.CancellationReasonId).HasColumnName(@"CANCELLATION_REASON_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CancellationDate).HasColumnName(@"CANCELLATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CancellationReason).WithMany(b => b.CanceledCustomerReservations).HasForeignKey(c => c.CancellationReasonId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_RESERVATION_CANCELLATION");
            builder.HasOne(a => a.CustomerReservation).WithOne(b => b.CanceledCustomerReservation).HasForeignKey<CanceledCustomerReservation>(c => c.CustomerReservationId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_RESERVATION_CUSTOMER_RESERVATION");
            builder.HasOne(a => a.Employee).WithMany(b => b.CanceledCustomerReservations).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELED_CUSTOMER_RESERVATION_EMPLOYEE");
        }
    }
}
