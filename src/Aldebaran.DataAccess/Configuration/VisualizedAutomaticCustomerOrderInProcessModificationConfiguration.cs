using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Configuration
{
    public class VisualizedAutomaticCustomerOrderInProcessModificationConfiguration : IEntityTypeConfiguration<VisualizedAutomaticCustomerOrderInProcessModification>
    {
        public void Configure(EntityTypeBuilder<VisualizedAutomaticCustomerOrderInProcessModification> builder)
        {
            builder.ToTable("visualized_automatic_customer_in_process_modifications", "dbo");
            builder.HasKey(x => new { x.Id, x.ActionType, x.Employee_Id}).HasName("PK_visualized_automatic_customer_in_process_modification").IsClustered();
            builder.Property(x => x.Id).HasColumnName(@"ID").HasColumnType("INT").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.ActionType).HasColumnName(@"ACTIONTYPE").HasColumnType("CHAR(1)").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Employee_Id).HasColumnName(@"EMPLOYEE_ID").HasColumnType("INT").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Visualized_Date).HasColumnName(@"VISUALIZED_DATE").HasColumnType("DATETIME").IsRequired();         
        }
    }
}
