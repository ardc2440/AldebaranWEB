using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerReservationConfiguration : IEntityTypeConfiguration<CustomerReservation>
    {
        public void Configure(EntityTypeBuilder<CustomerReservation> builder)
        {
            builder.ToTable("customer_reservations", "dbo");
            builder.HasKey(x => x.CustomerReservationId).HasName("PK_CUSTOMER_RESERVATION").IsClustered();
            builder.Property(x => x.CustomerReservationId).HasColumnName(@"CUSTOMER_RESERVATION_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerId).HasColumnName(@"CUSTOMER_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReservationNumber).HasColumnName(@"RESERVATION_NUMBER").HasColumnType("varchar(10)").IsRequired().IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.ReservationDate).HasColumnName(@"RESERVATION_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.ExpirationDate).HasColumnName(@"EXPIRATION_DATE").HasColumnType("date").IsRequired();
            builder.Property(x => x.Notes).HasColumnName(@"NOTES").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.EmployeeId).HasColumnName(@"EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("int").IsRequired(false);
            builder.Property(x => x.StatusDocumentTypeId).HasColumnName(@"STATUS_DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CREATION_DATE").HasColumnType("datetime").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.Customer).WithMany(b => b.CustomerReservations).HasForeignKey(c => c.CustomerId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_RESERVATION_CUSTOMER");
            builder.HasOne(a => a.CustomerOrder).WithMany(b => b.CustomerReservations).HasForeignKey(c => c.CustomerOrderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_RESERVATION_CUSTOMER_ORDER");
            builder.HasOne(a => a.Employee).WithMany(b => b.CustomerReservations).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_RESERVATION_EMPLOYEE");
            builder.HasOne(a => a.StatusDocumentType).WithMany(b => b.CustomerReservations).HasForeignKey(c => c.StatusDocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_RESERVATION_STATUS_DOCUMENT_TYPE");
            builder.HasIndex(x => x.CreationDate).HasDatabaseName("IND_CUSTOMER_RESERVATIONS_CREATION_DATE");
            builder.HasIndex(x => x.CustomerId).HasDatabaseName("IND_CUSTOMER_RESERVATIONS_CUSTOMER");
            builder.HasIndex(x => x.CustomerOrderId).HasDatabaseName("IND_CUSTOMER_RESERVATIONS_CUSTOMER_ORDER");
            builder.HasIndex(x => x.StatusDocumentTypeId).HasDatabaseName("IND_CUSTOMER_RESERVATIONS_ESTADO");
            builder.HasIndex(x => x.ReservationDate).HasDatabaseName("IND_CUSTOMER_RESERVATIONS_RESERVATION_DATE");
            builder.HasIndex(x => x.ReservationNumber).HasDatabaseName("UQ_CUSTOMER_RESERVATION_RESERVATION_NUMBER").IsUnique();
        }
    }
}
