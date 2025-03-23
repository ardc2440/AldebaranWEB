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
    public class ConfirmedPurchaseOrderConfiguration : IEntityTypeConfiguration<ConfirmedPurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<ConfirmedPurchaseOrder> builder)
        {
            builder.HasNoKey();
            builder.Property(x => x.AlarmId).HasColumnName(@"AUTOMATIC_IN_PROCESS_ID").HasColumnType("INT");
            builder.Property(x => x.DocumentType).HasColumnName(@"DOCUMENT_TYPE").HasColumnType("VARCHAR(1)");
            builder.Property(x => x.DocumentId).HasColumnName(@"DOCUMENT_ID").HasColumnType("INT");
            builder.Property(x => x.OrderNumber).HasColumnName(@"ORDER_NUMBER").HasColumnType("VARCHAR(10)") ;
            builder.Property(x => x.IdentityNumber).HasColumnName(@"IDENTITY_NUMBER").HasColumnType("VARCHAR(15)");
            builder.Property(x => x.ProviderName).HasColumnName(@"PROVIDER_NAME").HasColumnType("VARCHAR(50)");
            builder.Property(x => x.ProformaNumber).HasColumnName(@"PROFORMA_NUMBER").HasColumnType("VARCHAR(20)");
            builder.Property(x => x.ImportNumber).HasColumnName(@"IMPORT_NUMBER").HasColumnType("VARCHAR(20)");
            builder.Property(x => x.ReceptionDate).HasColumnName(@"REAL_RECEIPT_DATE").HasColumnType("DATE");
            builder.Property(x => x.ConfirmationDate).HasColumnName(@"CREATION_DATE").HasColumnType("DATE");
        }
    }
}
