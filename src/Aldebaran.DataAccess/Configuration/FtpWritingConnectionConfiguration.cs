using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Configuration
{
    public class FtpWritingConnectionConfiguration : IEntityTypeConfiguration<FtpWritingConnection>
    {
        public void Configure(EntityTypeBuilder<FtpWritingConnection> builder)
        {
            builder.ToTable("FTP_Writing_Connections");

            builder.HasKey(e => e.FtpWritingConnectionId)
                   .HasName("PK_FTP_Writing_Connections");

            builder.Property(e => e.FtpWritingConnectionId)
                   .HasColumnName("FTP_WRITING_CONNECTION_ID")
                   .HasColumnType("INT")
                   .IsRequired();

            builder.Property(e => e.HostName)
                   .HasColumnName("HOST_NAME")
                   .HasColumnType("VARCHAR(256)")
                   .IsRequired();

            builder.Property(e => e.PortNumber)
                   .HasColumnName("PORT_NUMBER")
                   .HasColumnType("VARCHAR(10)");

            builder.Property(e => e.UserName)
                   .HasColumnName("USER_NAME")
                   .HasColumnType("VARCHAR(50)")
                   .IsRequired();

            builder.Property(e => e.Password)
                   .HasColumnName("PASSWORD")
                   .HasColumnType("VARCHAR(50)")
                   .IsRequired();

            builder.Property(e => e.RewriteFile)
                   .HasColumnName("REWRITE_FILE")
                   .HasColumnType("BIT")
                   .IsRequired(false)
                   .HasDefaultValue(true);

            builder.Property(e => e.Active)
                   .HasColumnName("ACTIVE")
                   .HasColumnType("BIT")
                   .IsRequired()
                   .HasDefaultValue(false);
        }
    }
}