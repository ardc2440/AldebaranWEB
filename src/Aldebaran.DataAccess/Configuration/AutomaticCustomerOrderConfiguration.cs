using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Configuration
{
    public class AutomaticCustomerOrderConfiguration : IEntityTypeConfiguration<AutomaticCustomerOrder>
    {
        public void Configure(EntityTypeBuilder<AutomaticCustomerOrder> builder)
        {
            builder.HasNoKey();
            builder.Property(x => x.OrderId).HasColumnName(@"AUTOMATIC_IN_PROCESS_ORDER_ID").HasColumnType("INT");
            builder.Property(x => x.CustomerOrderId).HasColumnName(@"CUSTOMER_ORDER_ID").HasColumnType("INT");
            builder.Property(x => x.OrderNumber).HasColumnName(@"ORDER_NUMBER").HasColumnType("VARCHAR(10)");
            builder.Property(x => x.IdentityNumber).HasColumnName(@"IDENTITY_NUMBER").HasColumnType("VARCHAR(15)");
            builder.Property(x => x.CustomerName).HasColumnName(@"CUSTOMER_NAME").HasColumnType("VARCHAR(50)");
            builder.Property(x => x.OrderDate).HasColumnName(@"ORDER_DATE").HasColumnType("DATETIME");
            builder.Property(x => x.EstimatedDeliveryDate).HasColumnName(@"ESTIMATED_DELIVERY_DATE").HasColumnType("DATETIME");
            builder.Property(x => x.StatusDocumentTypeName).HasColumnName(@"STATUS_DOCUMENT_TYPE_NAME").HasColumnType("VARCHAR(30)");
        }
    }
}
