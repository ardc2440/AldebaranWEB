using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.EntityFrameworkCore;

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

            builder.Entity<ItemsArea>().HasKey(table => new
            {
                table.ITEM_ID,
                table.AREA_ID
            });

            builder.Entity<City>()
              .HasOne(i => i.Department)
              .WithMany(i => i.Cities)
              .HasForeignKey(i => i.DEPARTMENT_ID)
              .HasPrincipalKey(i => i.DEPARTMENT_ID);

            builder.Entity<CustomerContact>()
              .HasOne(i => i.Customer)
              .WithMany(i => i.CustomerContacts)
              .HasForeignKey(i => i.CUSTOMER_ID)
              .HasPrincipalKey(i => i.CUSTOMER_ID);

            builder.Entity<Customer>()
              .HasOne(i => i.City)
              .WithMany(i => i.Customers)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<Customer>()
              .HasOne(i => i.IdentityType)
              .WithMany(i => i.Customers)
              .HasForeignKey(i => i.IDENTITY_TYPE_ID)
              .HasPrincipalKey(i => i.IDENTITY_TYPE_ID);

            builder.Entity<Department>()
              .HasOne(i => i.Country)
              .WithMany(i => i.Departments)
              .HasForeignKey(i => i.COUNTRY_ID)
              .HasPrincipalKey(i => i.COUNTRY_ID);

            builder.Entity<ForwarderAgent>()
              .HasOne(i => i.City)
              .WithMany(i => i.ForwarderAgents)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<ForwarderAgent>()
              .HasOne(i => i.Forwarder)
              .WithMany(i => i.ForwarderAgents)
              .HasForeignKey(i => i.FORWARDER_ID)
              .HasPrincipalKey(i => i.FORWARDER_ID);

            builder.Entity<Forwarder>()
              .HasOne(i => i.City)
              .WithMany(i => i.Forwarders)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<ItemReference>()
              .HasOne(i => i.Item)
              .WithMany(i => i.ItemReferences)
              .HasForeignKey(i => i.ITEM_ID)
              .HasPrincipalKey(i => i.ITEM_ID);

            builder.Entity<Item>()
              .HasOne(i => i.MeasureUnit)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.CIF_MEASURE_UNIT_ID)
              .HasPrincipalKey(i => i.MEASURE_UNIT_ID);

            builder.Entity<Item>()
              .HasOne(i => i.Currency)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.CURRENCY_ID)
              .HasPrincipalKey(i => i.CURRENCY_ID);

            builder.Entity<Item>()
              .HasOne(i => i.MeasureUnit1)
              .WithMany(i => i.Items1)
              .HasForeignKey(i => i.FOB_MEASURE_UNIT_ID)
              .HasPrincipalKey(i => i.MEASURE_UNIT_ID);

            builder.Entity<Item>()
              .HasOne(i => i.Line)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.LINE_ID)
              .HasPrincipalKey(i => i.LINE_ID);

            builder.Entity<ItemsArea>()
              .HasOne(i => i.Area)
              .WithMany(i => i.ItemsAreas)
              .HasForeignKey(i => i.AREA_ID)
              .HasPrincipalKey(i => i.AREA_ID);

            builder.Entity<ItemsArea>()
              .HasOne(i => i.Item)
              .WithMany(i => i.ItemsAreas)
              .HasForeignKey(i => i.ITEM_ID)
              .HasPrincipalKey(i => i.ITEM_ID);

            builder.Entity<ShipmentForwarderAgentMethod>()
              .HasOne(i => i.ForwarderAgent)
              .WithMany(i => i.ShipmentForwarderAgentMethods)
              .HasForeignKey(i => i.FORWARDER_AGENT_ID)
              .HasPrincipalKey(i => i.FORWARDER_AGENT_ID);

            builder.Entity<ShipmentForwarderAgentMethod>()
              .HasOne(i => i.ShipmentMethod)
              .WithMany(i => i.ShipmentForwarderAgentMethods)
              .HasForeignKey(i => i.SHIPMENT_METHOD_ID)
              .HasPrincipalKey(i => i.SHIPMENT_METHOD_ID);

            builder.Entity<ItemReference>()
              .Property(p => p.IS_ACTIVE)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<ItemReference>()
              .Property(p => p.IS_SOLD_OUT)
              .HasDefaultValueSql(@"((0))");

            builder.Entity<Item>()
              .Property(p => p.IS_ACTIVE)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<Line>()
              .Property(p => p.IS_ACTIVE)
              .HasDefaultValueSql(@"((1))");
            this.OnModelBuilding(builder);
        }

        public DbSet<Area> Areas { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<ForwarderAgent> ForwarderAgents { get; set; }

        public DbSet<Forwarder> Forwarders { get; set; }

        public DbSet<IdentityType> IdentityTypes { get; set; }

        public DbSet<ItemReference> ItemReferences { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<ItemsArea> ItemsAreas { get; set; }

        public DbSet<Line> Lines { get; set; }

        public DbSet<MeasureUnit> MeasureUnits { get; set; }

        public DbSet<ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }

        public DbSet<ShipmentMethod> ShipmentMethods { get; set; }

        public DbSet<ShippingMethod> ShippingMethods { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }

    }
}