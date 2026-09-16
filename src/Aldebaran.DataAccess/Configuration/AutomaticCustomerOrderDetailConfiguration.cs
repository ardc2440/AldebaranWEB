using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Configuration
{
    public class AutomaticCustomerOrderDetailConfiguration : IEntityTypeConfiguration<AutomaticCustomerOrderDetail>
    {
        public void Configure(EntityTypeBuilder<AutomaticCustomerOrderDetail> builder)
        {
            builder.HasNoKey();
            builder.Property(x => x.ReferenceId).HasColumnName(@"REFERENCE_ID").HasColumnType("INT");
            builder.Property(x => x.ArticleName).HasColumnName(@"ARTICLE_NAME").HasColumnType("VARCHAR(150)");
            builder.Property(x => x.Requested).HasColumnName(@"REQUESTED_QUANTITY").HasColumnType("INT");
            builder.Property(x => x.Assigned).HasColumnName(@"ASSIGNED_QUANTITY").HasColumnType("INT");
            builder.Property(x => x.Pending).HasColumnName(@"PENDING_QUANTITY").HasColumnType("INT");            
        }
    }
}
