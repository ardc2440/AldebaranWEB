using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class CancellationRequestConfiguration : IEntityTypeConfiguration<CancellationRequest>
    {
        public void Configure(EntityTypeBuilder<CancellationRequest> builder)
        {
            builder.ToTable("cancellation_requests", "dbo");
            builder.HasKey(x => x.RequestId).HasName("PK_CANCELLATION_REQUEST").IsClustered();
            builder.Property(x => x.RequestId).HasColumnName(@"CANCELLATION_REQUEST_ID").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.DocumentTypeId).HasColumnName(@"DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.DocumentNumber).HasColumnName(@"DOCUMENT_NUMBER").HasColumnType("int").IsRequired();
            builder.Property(x => x.RequestEmployeeId).HasColumnName(@"REQUEST_EMPLOYEE_ID").HasColumnType("int").IsRequired();
            builder.Property(x => x.ResponseEmployeeId).HasColumnName(@"RESPONSE_EMPLOYEE_ID").HasColumnType("int");
            builder.Property(x => x.StatusDocumentTypeId).HasColumnName(@"STATUS_DOCUMENT_TYPE_ID").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.RequestDate).HasColumnName(@"REQUEST_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ResponseDate).HasColumnName(@"RESPONSE_DATE").HasColumnType("datetime");
            builder.Property(x => x.ResponseReason).HasColumnName(@"RESPONSE_REASON").HasColumnType("VARCHAR(250)").IsUnicode(false).HasMaxLength(250); ;
            
            // Foreign keys
            builder.HasOne(a => a.DocumentType).WithMany(b => b.CancellationRequests).HasForeignKey(c => c.DocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELLATION_REQUEST_DOCUMENT_TYPE");
            builder.HasOne(a => a.RequestEmployee).WithMany(b => b.CancellationEmployeeRequests).HasForeignKey(c => c.RequestEmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELLATION_REQUESTS_REQUESTED_BY");
            builder.HasOne(a => a.ResponseEmployee).WithMany(b => b.CancellationEmployeeResponses).HasForeignKey(c => c.ResponseEmployeeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELLATION_REQUESTS_RESPONDED_BY");
            builder.HasOne(a => a.StatusDocumentType).WithMany(b => b.CancellationRequests).HasForeignKey(c => c.StatusDocumentTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CANCELLATION_REQUEST_STATUS_DOCUMENT_TYPE");
        }
    }
}
