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
    public class NotificationAccessTokenConfiguration : IEntityTypeConfiguration<NotificationAccessToken>
    {
        public void Configure(EntityTypeBuilder<NotificationAccessToken> builder)
        {
            builder.ToTable("notification_access_tokens", "dbo");
            builder.HasKey(x => x.NotificationAccessTokenId).HasName("PK_NOTIFICATION_ACCESS_TOKEN").IsClustered();
            builder.Property(x => x.NotificationAccessTokenId).HasColumnName(@"NOTIFICATION_ACCESS_TOKEN_ID").HasColumnType("UNIQUEIDENTIFIER").IsRequired();
            builder.Property(x => x.NotificationTemplateId).HasColumnName(@"NOTIFICATION_TEMPLATE_ID").HasColumnType("SMALLINT").IsRequired();
            builder.Property(x => x.ExtraData).HasColumnName(@"EXTRA_DATA").HasColumnType("NVARCHAR(MAX)");
            builder.Property(x => x.GeneratedDate).HasColumnName(@"GENERATED_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ExpirationDate).HasColumnName(@"EXPIRATION_DATE").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.IsConsumed).HasColumnName(@"IS_CONSUMED").HasColumnType("bit").IsRequired();
            builder.Property(x => x.ConsumedDate).HasColumnName(@"CONSUMED_DATE").HasColumnType("datetime");
            
            // Foreign keys
            builder.HasOne(a => a.NotificationTemplate)
                .WithMany(b => b.NotificationAccessTokens)
                .HasForeignKey(c => c.NotificationTemplateId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_NOTIFICATION_ACCESS_TOKEN_TEMPLATE");            
        }
    }
}
