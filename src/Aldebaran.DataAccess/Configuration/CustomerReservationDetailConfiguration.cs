using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerReservationDetailConfiguration : IEntityTypeConfiguration<CustomerReservationDetail>
    {
        public void Configure(EntityTypeBuilder<CustomerReservationDetail> builder)
        {
            builder.ToTable("customer_reservation_details", "dbo");
            builder.HasKey(x => x.CustomerReservationDetailId).HasName("PK_ITEMRESERVAS").IsClustered();
            builder.Property(x => x.CustomerReservationDetailId).HasColumnName(@"CUSTOMER_RESERVATION_DETAIL_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CustomerReservationId).HasColumnName(@"CUSTOMER_RESERVATION_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReferenceId).HasColumnName(@"REFERENCE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReservedQuantity).HasColumnName(@"RESERVED_QUANTITY").HasColumnType("int").IsRequired();
            builder.Property(x => x.Brand).HasColumnName(@"BRAND").HasColumnType("varchar(250)").IsRequired(false).IsUnicode(false).HasMaxLength(250);
            builder.Property(x => x.SendToCustomerOrder).HasColumnName(@"SEND_TO_CUSTOMER_ORDER").HasColumnType("bit").IsRequired();
            // Foreign keys
            builder.HasOne(a => a.CustomerReservation).WithMany(b => b.CustomerReservationDetails).HasForeignKey(c => c.CustomerReservationId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_RESERVATION_DETAILS_CUSTOMER_RESERVATION");
            builder.HasOne(a => a.ItemReference).WithMany(b => b.CustomerReservationDetails).HasForeignKey(c => c.ReferenceId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CUSTOMER_RESERVATION_DETAILS_REFERENCE");
        }
    }
}
