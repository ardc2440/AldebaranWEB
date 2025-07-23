using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldebaran.DataAccess.Configuration
{
    public class AutomaticCustomerOrderInProcessModificationConfiguration : IEntityTypeConfiguration<AutomaticCustomerOrderInProcessModification>
    {
        public void Configure(EntityTypeBuilder<AutomaticCustomerOrderInProcessModification> builder)
        {
            builder.HasNoKey();
            builder.Property(x => x.Id).HasColumnName(@"AUTOMATIC_IN_PROCESS_ID").HasColumnType("INT");
            builder.Property(x => x.CUSTOMER_ORDER_IN_PROCESS_ID).HasColumnName(@"DOCUMENT_TYPE").HasColumnType("VARCHAR(1)");
            builder.Property(x => x.ActionType).HasColumnName(@"DOCUMENT_ID").HasColumnType("INT");
            builder.Property(x => x.ActionDate).HasColumnName(@"ORDER_NUMBER").HasColumnType("VARCHAR(10)");
            builder.Property(x => x.ActionReason).HasColumnName(@"IDENTITY_NUMBER").HasColumnType("VARCHAR(15)");
            builder.Property(x => x.EmployeeName).HasColumnName(@"PROVIDER_NAME").HasColumnType("VARCHAR(50)");
            builder.Property(x => x.Order_Number).HasColumnName(@"ORDER_NUMBER").HasColumnType("VARCHAR(10)");
            builder.Property(x => x.Customer_Name).HasColumnName(@"CUSTOMER_NAME").HasColumnType("VARCHAR(50)");
        }
    }
}
