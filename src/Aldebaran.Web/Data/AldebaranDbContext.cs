using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Aldebaran.Web.Models.AldebaranDb;

namespace Aldebaran.Web.Data
{
    public partial class AldebaranDbContext : DbContext
    {
        public AldebaranDbContext()
        {
        }

        public AldebaranDbContext(DbContextOptions<AldebaranDbContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.ItemsArea>().HasKey(table => new {
                table.ITEM_ID, table.AREA_ID
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.ItemReference>()
              .HasOne(i => i.Item)
              .WithMany(i => i.ItemReferences)
              .HasForeignKey(i => i.ITEM_ID)
              .HasPrincipalKey(i => i.ITEM_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.Item>()
              .HasOne(i => i.MeasureUnit)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.CIF_MEASURE_UNIT_ID)
              .HasPrincipalKey(i => i.MEASURE_UNIT_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.Item>()
              .HasOne(i => i.Currency)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.CURRENCY_ID)
              .HasPrincipalKey(i => i.CURRENCY_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.Item>()
              .HasOne(i => i.MeasureUnit1)
              .WithMany(i => i.Items1)
              .HasForeignKey(i => i.FOB_MEASURE_UNIT_ID)
              .HasPrincipalKey(i => i.MEASURE_UNIT_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.Item>()
              .HasOne(i => i.Line)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.LINE_ID)
              .HasPrincipalKey(i => i.LINE_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.ItemsArea>()
              .HasOne(i => i.Area)
              .WithMany(i => i.ItemsAreas)
              .HasForeignKey(i => i.AREA_ID)
              .HasPrincipalKey(i => i.AREA_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.ItemsArea>()
              .HasOne(i => i.Item)
              .WithMany(i => i.ItemsAreas)
              .HasForeignKey(i => i.ITEM_ID)
              .HasPrincipalKey(i => i.ITEM_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.City>()
              .HasOne(i => i.Department)
              .WithMany(i => i.Cities)
              .HasForeignKey(i => i.DEPARTMENT_ID)
              .HasPrincipalKey(i => i.DEPARTMENT_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.Department>()
              .HasOne(i => i.Country)
              .WithMany(i => i.Departments)
              .HasForeignKey(i => i.COUNTRY_ID)
              .HasPrincipalKey(i => i.COUNTRY_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.Forwarder>()
              .HasOne(i => i.City)
              .WithMany(i => i.Forwarders)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent>()
              .HasOne(i => i.City)
              .WithMany(i => i.ForwarderAgents)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent>()
              .HasOne(i => i.Forwarder)
              .WithMany(i => i.ForwarderAgents)
              .HasForeignKey(i => i.FORWARDER_ID)
              .HasPrincipalKey(i => i.FORWARDER_ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.ItemReference>()
              .Property(p => p.IS_ACTIVE)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.ItemReference>()
              .Property(p => p.IS_SOLD_OUT)
              .HasDefaultValueSql(@"((0))");

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.Item>()
              .Property(p => p.IS_ACTIVE)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<Aldebaran.Web.Models.AldebaranDb.Line>()
              .Property(p => p.IS_ACTIVE)
              .HasDefaultValueSql(@"((1))");
            this.OnModelBuilding(builder);
        }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.Area> Areas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.Currency> Currencies { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.ItemReference> ItemReferences { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.Item> Items { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.ItemsArea> ItemsAreas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.Line> Lines { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> MeasureUnits { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.City> Cities { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.Country> Countries { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.Department> Departments { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.Forwarder> Forwarders { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent> ForwarderAgents { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}