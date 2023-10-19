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

            builder.Entity<Models.AldebaranDb.ItemsArea>().HasKey(table => new
            {
                table.ITEM_ID,
                table.AREA_ID
            });

            builder.Entity<Models.AldebaranDb.ProviderReference>().HasKey(table => new
            {
                table.REFERENCE_ID,
                table.PROVIDER_ID
            });

            builder.Entity<Models.AldebaranDb.City>()
              .HasOne(i => i.Department)
              .WithMany(i => i.Cities)
              .HasForeignKey(i => i.DEPARTMENT_ID)
              .HasPrincipalKey(i => i.DEPARTMENT_ID);

            builder.Entity<Models.AldebaranDb.CustomerContact>()
              .HasOne(i => i.Customer)
              .WithMany(i => i.CustomerContacts)
              .HasForeignKey(i => i.CUSTOMER_ID)
              .HasPrincipalKey(i => i.CUSTOMER_ID);

            builder.Entity<Models.AldebaranDb.Customer>()
              .HasOne(i => i.City)
              .WithMany(i => i.Customers)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<Models.AldebaranDb.Customer>()
              .HasOne(i => i.IdentityType)
              .WithMany(i => i.Customers)
              .HasForeignKey(i => i.IDENTITY_TYPE_ID)
              .HasPrincipalKey(i => i.IDENTITY_TYPE_ID);

            builder.Entity<Models.AldebaranDb.Department>()
              .HasOne(i => i.Country)
              .WithMany(i => i.Departments)
              .HasForeignKey(i => i.COUNTRY_ID)
              .HasPrincipalKey(i => i.COUNTRY_ID);

            builder.Entity<Models.AldebaranDb.ForwarderAgent>()
              .HasOne(i => i.City)
              .WithMany(i => i.ForwarderAgents)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<Models.AldebaranDb.ForwarderAgent>()
              .HasOne(i => i.Forwarder)
              .WithMany(i => i.ForwarderAgents)
              .HasForeignKey(i => i.FORWARDER_ID)
              .HasPrincipalKey(i => i.FORWARDER_ID);

            builder.Entity<Models.AldebaranDb.Forwarder>()
              .HasOne(i => i.City)
              .WithMany(i => i.Forwarders)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<Models.AldebaranDb.ItemReference>()
              .HasOne(i => i.Item)
              .WithMany(i => i.ItemReferences)
              .HasForeignKey(i => i.ITEM_ID)
              .HasPrincipalKey(i => i.ITEM_ID);

            builder.Entity<Models.AldebaranDb.Item>()
              .HasOne(i => i.MeasureUnit)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.CIF_MEASURE_UNIT_ID)
              .HasPrincipalKey(i => i.MEASURE_UNIT_ID);

            builder.Entity<Models.AldebaranDb.Item>()
              .HasOne(i => i.Currency)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.CURRENCY_ID)
              .HasPrincipalKey(i => i.CURRENCY_ID);

            builder.Entity<Models.AldebaranDb.Item>()
              .HasOne(i => i.MeasureUnit1)
              .WithMany(i => i.Items1)
              .HasForeignKey(i => i.FOB_MEASURE_UNIT_ID)
              .HasPrincipalKey(i => i.MEASURE_UNIT_ID);

            builder.Entity<Models.AldebaranDb.Item>()
              .HasOne(i => i.Line)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.LINE_ID)
              .HasPrincipalKey(i => i.LINE_ID);

            builder.Entity<Models.AldebaranDb.ItemsArea>()
              .HasOne(i => i.Area)
              .WithMany(i => i.ItemsAreas)
              .HasForeignKey(i => i.AREA_ID)
              .HasPrincipalKey(i => i.AREA_ID);

            builder.Entity<Models.AldebaranDb.ItemsArea>()
              .HasOne(i => i.Item)
              .WithMany(i => i.ItemsAreas)
              .HasForeignKey(i => i.ITEM_ID)
              .HasPrincipalKey(i => i.ITEM_ID);

            builder.Entity<Models.AldebaranDb.ShipmentForwarderAgentMethod>()
              .HasOne(i => i.ForwarderAgent)
              .WithMany(i => i.ShipmentForwarderAgentMethods)
              .HasForeignKey(i => i.FORWARDER_AGENT_ID)
              .HasPrincipalKey(i => i.FORWARDER_AGENT_ID);

            builder.Entity<Models.AldebaranDb.ShipmentForwarderAgentMethod>()
              .HasOne(i => i.ShipmentMethod)
              .WithMany(i => i.ShipmentForwarderAgentMethods)
              .HasForeignKey(i => i.SHIPMENT_METHOD_ID)
              .HasPrincipalKey(i => i.SHIPMENT_METHOD_ID);

            builder.Entity<Models.AldebaranDb.Provider>()
              .HasOne(i => i.City)
              .WithMany(i => i.Providers)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<Models.AldebaranDb.Provider>()
              .HasOne(i => i.IdentityType)
              .WithMany(i => i.Providers)
              .HasForeignKey(i => i.IDENTITY_TYPE_ID)
              .HasPrincipalKey(i => i.IDENTITY_TYPE_ID);

            builder.Entity<Models.AldebaranDb.ProviderReference>()
              .HasOne(i => i.Provider)
              .WithMany(i => i.ProviderReferences)
              .HasForeignKey(i => i.PROVIDER_ID)
              .HasPrincipalKey(i => i.PROVIDER_ID);

            builder.Entity<Models.AldebaranDb.ProviderReference>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.ProviderReferences)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<Models.AldebaranDb.ItemReference>()
              .Property(p => p.IS_ACTIVE)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<Models.AldebaranDb.ItemReference>()
              .Property(p => p.IS_SOLD_OUT)
              .HasDefaultValueSql(@"((0))");

            builder.Entity<Models.AldebaranDb.Item>()
              .Property(p => p.IS_ACTIVE)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<Models.AldebaranDb.Line>()
              .Property(p => p.IS_ACTIVE)
              .HasDefaultValueSql(@"((1))");
            this.OnModelBuilding(builder);
        }

        public DbSet<Models.AldebaranDb.Area> Areas { get; set; }

        public DbSet<Models.AldebaranDb.City> Cities { get; set; }

        public DbSet<Models.AldebaranDb.Country> Countries { get; set; }

        public DbSet<Models.AldebaranDb.Currency> Currencies { get; set; }

        public DbSet<Models.AldebaranDb.CustomerContact> CustomerContacts { get; set; }

        public DbSet<Models.AldebaranDb.Customer> Customers { get; set; }

        public DbSet<Models.AldebaranDb.Department> Departments { get; set; }

        public DbSet<Models.AldebaranDb.ForwarderAgent> ForwarderAgents { get; set; }

        public DbSet<Models.AldebaranDb.Forwarder> Forwarders { get; set; }

        public DbSet<Models.AldebaranDb.IdentityType> IdentityTypes { get; set; }

        public DbSet<Models.AldebaranDb.ItemReference> ItemReferences { get; set; }

        public DbSet<Models.AldebaranDb.Item> Items { get; set; }

        public DbSet<Models.AldebaranDb.ItemsArea> ItemsAreas { get; set; }

        public DbSet<Models.AldebaranDb.Line> Lines { get; set; }

        public DbSet<Models.AldebaranDb.MeasureUnit> MeasureUnits { get; set; }

        public DbSet<Models.AldebaranDb.ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }

        public DbSet<Models.AldebaranDb.ShipmentMethod> ShipmentMethods { get; set; }

        public DbSet<Models.AldebaranDb.ShippingMethod> ShippingMethods { get; set; }

        public DbSet<Models.AldebaranDb.Provider> Providers { get; set; }

        public DbSet<Models.AldebaranDb.ProviderReference> ProviderReferences { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }

    }
}