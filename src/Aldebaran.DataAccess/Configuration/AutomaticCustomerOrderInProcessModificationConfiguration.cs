using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Configuration
{
    public class AutomaticCustomerOrderInProcessModificationConfiguration : IEntityTypeConfiguration<AutomaticCustomerOrderInProcessModification>
    {
        public void Configure(EntityTypeBuilder<AutomaticCustomerOrderInProcessModification> builder)
        {
            builder.HasNoKey();
            builder.Property(x => x.Id).HasColumnName(@"ID").HasColumnType("INT");
            builder.Property(x => x.CUSTOMER_ORDER_IN_PROCESS_ID).HasColumnName(@"CUSTOMER_ORDER_IN_PROCESS_ID").HasColumnType("INT");
            builder.Property(x => x.CUSTOMER_ORDER_ID).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("INT");
            builder.Property(x => x.ActionType).HasColumnName(@"ACTIONTYPE").HasColumnType("VARCHAR(15)");
            builder.Property(x => x.ActionDate).HasColumnName(@"ACTIONDATE").HasColumnType("DATETIME");
            builder.Property(x => x.ActionReason).HasColumnName(@"ACTIONREASON").HasColumnType("VARCHAR(30)");
            builder.Property(x => x.EmployeeName).HasColumnName(@"EMPLOYEENAME").HasColumnType("VARCHAR(50)");
            builder.Property(x => x.Order_Number).HasColumnName(@"ORDER_NUMBER").HasColumnType("VARCHAR(10)");
            builder.Property(x => x.Customer_Name).HasColumnName(@"CUSTOMER_NAME").HasColumnType("VARCHAR(50)");
        }
    }
}
