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

            builder.Entity<Models.AldebaranDb.ActivityTypeArea>().HasKey(table => new
            {
                table.ACTIVITY_TYPE_ID,
                table.AREA_ID
            });

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

            builder.Entity<Models.AldebaranDb.ReferencesWarehouse>().HasKey(table => new
            {
                table.REFERENCE_ID,
                table.WAREHOUSE_ID
            });

            builder.Entity<Models.AldebaranDb.ActivityTypeArea>()
             .HasOne(i => i.Area)
             .WithMany(i => i.ActivityTypesAreas)
             .HasForeignKey(i => i.AREA_ID)
             .HasPrincipalKey(i => i.AREA_ID);

            builder.Entity<Models.AldebaranDb.ActivityTypeArea>()
             .HasOne(i => i.ActivityType)
             .WithMany(i => i.ActivityTypesAreas)
             .HasForeignKey(i => i.ACTIVITY_TYPE_ID)
             .HasPrincipalKey(i => i.ACTIVITY_TYPE_ID);

            builder.Entity<Models.AldebaranDb.CustomerReservation>()
              .HasOne(i => i.Customer)
              .WithMany(i => i.CustomerReservations)
              .HasForeignKey(i => i.CUSTOMER_ID)
              .HasPrincipalKey(i => i.CUSTOMER_ID);

            builder.Entity<Models.AldebaranDb.CustomerReservation>()
              .HasOne(i => i.StatusDocumentType)
              .WithMany(i => i.CustomerReservations)
              .HasForeignKey(i => i.STATUS_DOCUMENT_TYPE_ID)
              .HasPrincipalKey(i => i.STATUS_DOCUMENT_TYPE_ID);

            builder.Entity<Models.AldebaranDb.CustomerReservation>()
              .HasOne(i => i.Employee)
              .WithMany(i => i.CustomerReservations)
              .HasForeignKey(i => i.EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<Models.AldebaranDb.CustomerReservationDetail>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.CustomerReservationDetails)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<Models.AldebaranDb.CustomerReservationDetail>()
              .HasOne(i => i.CustomerReservation)
              .WithMany(i => i.CustomerReservationDetails)
              .HasForeignKey(i => i.CUSTOMER_RESERVATION_ID)
              .HasPrincipalKey(i => i.CUSTOMER_RESERVATION_ID);

            builder.Entity<Models.AldebaranDb.StatusDocumentType>()
              .HasOne(i => i.DocumentType)
              .WithMany(i => i.StatusDocumentTypes)
              .HasForeignKey(i => i.DOCUMENT_TYPE_ID)
              .HasPrincipalKey(i => i.DOCUMENT_TYPE_ID);

            builder.Entity<Models.AldebaranDb.AdjustmentDetail>()
              .HasOne(i => i.Adjustment)
              .WithMany(i => i.AdjustmentDetails)
              .HasForeignKey(i => i.ADJUSTMENT_ID)
              .HasPrincipalKey(i => i.ADJUSTMENT_ID);

            builder.Entity<Models.AldebaranDb.AdjustmentDetail>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.AdjustmentDetails)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<Models.AldebaranDb.AdjustmentDetail>()
              .HasOne(i => i.Warehouse)
              .WithMany(i => i.AdjustmentDetails)
              .HasForeignKey(i => i.WAREHOUSE_ID)
              .HasPrincipalKey(i => i.WAREHOUSE_ID);

            builder.Entity<Models.AldebaranDb.Adjustment>()
              .HasOne(i => i.AdjustmentReason)
              .WithMany(i => i.Adjustments)
              .HasForeignKey(i => i.ADJUSTMENT_REASON_ID)
              .HasPrincipalKey(i => i.ADJUSTMENT_REASON_ID);

            builder.Entity<Models.AldebaranDb.Adjustment>()
              .HasOne(i => i.AdjustmentType)
              .WithMany(i => i.Adjustments)
              .HasForeignKey(i => i.ADJUSTMENT_TYPE_ID)
              .HasPrincipalKey(i => i.ADJUSTMENT_TYPE_ID);

            builder.Entity<Models.AldebaranDb.Adjustment>()
              .HasOne(i => i.Employee)
              .WithMany(i => i.Adjustments)
              .HasForeignKey(i => i.EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

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

            builder.Entity<Models.AldebaranDb.Employee>()
              .HasOne(i => i.Area)
              .WithMany(i => i.Employees)
              .HasForeignKey(i => i.AREA_ID)
              .HasPrincipalKey(i => i.AREA_ID);

            builder.Entity<Models.AldebaranDb.Employee>()
              .HasOne(i => i.IdentityType)
              .WithMany(i => i.Employees)
              .HasForeignKey(i => i.IDENTITY_TYPE_ID)
              .HasPrincipalKey(i => i.IDENTITY_TYPE_ID);

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

            builder.Entity<Models.AldebaranDb.PurchaseOrderActivity>()
              .HasOne(i => i.Employee)
              .WithMany(i => i.PurchaseOrderActivities)
              .HasForeignKey(i => i.ACTIVITY_EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<Models.AldebaranDb.PurchaseOrderActivity>()
              .HasOne(i => i.Employee1)
              .WithMany(i => i.PurchaseOrderActivities1)
              .HasForeignKey(i => i.EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<Models.AldebaranDb.PurchaseOrderActivity>()
              .HasOne(i => i.PurchaseOrder)
              .WithMany(i => i.PurchaseOrderActivities)
              .HasForeignKey(i => i.PURCHASE_ORDER_ID)
              .HasPrincipalKey(i => i.PURCHASE_ORDER_ID);

            builder.Entity<Models.AldebaranDb.PurchaseOrderDetail>()
              .HasOne(i => i.PurchaseOrder)
              .WithMany(i => i.PurchaseOrderDetails)
              .HasForeignKey(i => i.PURCHASE_ORDER_ID)
              .HasPrincipalKey(i => i.PURCHASE_ORDER_ID);

            builder.Entity<Models.AldebaranDb.PurchaseOrderDetail>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.PurchaseOrderDetails)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<Models.AldebaranDb.PurchaseOrderDetail>()
              .HasOne(i => i.Warehouse)
              .WithMany(i => i.PurchaseOrderDetails)
              .HasForeignKey(i => i.WAREHOUSE_ID)
              .HasPrincipalKey(i => i.WAREHOUSE_ID);

            builder.Entity<Models.AldebaranDb.PurchaseOrder>()
              .HasOne(i => i.Employee)
              .WithMany(i => i.PurchaseOrders)
              .HasForeignKey(i => i.EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<Models.AldebaranDb.PurchaseOrder>()
              .HasOne(i => i.ForwarderAgent)
              .WithMany(i => i.PurchaseOrders)
              .HasForeignKey(i => i.FORWARDER_AGENT_ID)
              .HasPrincipalKey(i => i.FORWARDER_AGENT_ID);

            builder.Entity<Models.AldebaranDb.PurchaseOrder>()
              .HasOne(i => i.Provider)
              .WithMany(i => i.PurchaseOrders)
              .HasForeignKey(i => i.PROVIDER_ID)
              .HasPrincipalKey(i => i.PROVIDER_ID);

            builder.Entity<Models.AldebaranDb.PurchaseOrder>()
              .HasOne(i => i.ShipmentForwarderAgentMethod)
              .WithMany(i => i.PurchaseOrders)
              .HasForeignKey(i => i.SHIPMENT_FORWARDER_AGENT_METHOD_ID)
              .HasPrincipalKey(i => i.SHIPMENT_FORWARDER_AGENT_METHOD_ID);

            builder.Entity<Models.AldebaranDb.ReferencesWarehouse>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.ReferencesWarehouses)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<Models.AldebaranDb.ReferencesWarehouse>()
              .HasOne(i => i.Warehouse)
              .WithMany(i => i.ReferencesWarehouses)
              .HasForeignKey(i => i.WAREHOUSE_ID)
              .HasPrincipalKey(i => i.WAREHOUSE_ID);

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

            builder.Entity<Models.AldebaranDb.CustomerReservation>()
              .HasOne(i => i.CustomerOrder)
              .WithMany(i => i.CustomerReservations)
              .HasForeignKey(i => i.CUSTOMER_ORDER_ID)
              .HasPrincipalKey(i => i.CUSTOMER_ORDER_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrder>()
             .HasOne(i => i.Customer)
             .WithMany(i => i.CustomerOrders)
             .HasForeignKey(i => i.CUSTOMER_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrder>()
             .HasOne(i => i.StatusDocumentType)
             .WithMany(i => i.CustomerOrders)
             .HasForeignKey(i => i.STATUS_DOCUMENT_TYPE_ID)
             .HasPrincipalKey(i => i.STATUS_DOCUMENT_TYPE_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrder>()
             .HasOne(i => i.Employee)
             .WithMany(i => i.CustomerOrders)
             .HasForeignKey(i => i.EMPLOYEE_ID)
             .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrderDetail>()
             .HasOne(i => i.ItemReference)
             .WithMany(i => i.CustomerOrderDetails)
             .HasForeignKey(i => i.REFERENCE_ID)
             .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrderDetail>()
             .HasOne(i => i.CustomerOrder)
             .WithMany(i => i.CustomerOrderDetails)
             .HasForeignKey(i => i.CUSTOMER_ORDER_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ORDER_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrderActivity>()
             .HasOne(i => i.CustomerOrder)
             .WithMany(i => i.CustomerOrderActivities)
             .HasForeignKey(i => i.CUSTOMER_ORDER_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ORDER_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrderActivity>()
             .HasOne(i => i.Area)
             .WithMany(i => i.CustomerOrderActivities)
             .HasForeignKey(i => i.AREA_ID)
             .HasPrincipalKey(i => i.AREA_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrderActivity>()
             .HasOne(i => i.Employee)
             .WithMany(i => i.CustomerOrderActivities)
             .HasForeignKey(i => i.EMPLOYEE_ID)
             .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrderActivityDetail>()
             .HasOne(i => i.CustomerOrderActivity)
             .WithMany(i => i.CustomerOrderActivityDetails)
             .HasForeignKey(i => i.CUSTOMER_ORDER_ACTIVITY_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ORDER_ACTIVITY_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrderActivityDetail>()
             .HasOne(i => i.ActivityType)
             .WithMany(i => i.CustomerOrderActivityDetails)
             .HasForeignKey(i => i.ACTIVITY_TYPE_ID)
             .HasPrincipalKey(i => i.ACTIVITY_TYPE_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrderActivityDetail>()
             .HasOne(i => i.Employee)
             .WithMany(i => i.CustomerOrderActivityDetails)
             .HasForeignKey(i => i.EMPLOYEE_ID)
             .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<Models.AldebaranDb.CustomerOrderActivityDetail>()
             .HasOne(i => i.EmployeeActivity)
             .WithMany(i => i.CustomerOrderActivityDetailsActivityEmployee)
             .HasForeignKey(i => i.ACTIVITY_EMPLOYEE_ID)
             .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<Models.AldebaranDb.Adjustment>()
                  .Property(p => p.ADJUSTMENT_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Models.AldebaranDb.Adjustment>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Models.AldebaranDb.StatusDocumentType>()
                  .Property(p => p.EDIT_MODE)
                  .HasDefaultValueSql(@"((1))");

            builder.Entity<Models.AldebaranDb.ItemReference>()
                  .Property(p => p.IS_ACTIVE)
                  .HasDefaultValueSql(@"((1))");

            builder.Entity<Models.AldebaranDb.Item>()
                  .Property(p => p.IS_ACTIVE)
                  .HasDefaultValueSql(@"((1))");

            builder.Entity<Models.AldebaranDb.Line>()
                  .Property(p => p.IS_ACTIVE)
                  .HasDefaultValueSql(@"((1))");

            builder.Entity<Models.AldebaranDb.CustomerOrder>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Models.AldebaranDb.PurchaseOrderActivity>()
                  .Property(p => p.EXECUTION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Models.AldebaranDb.PurchaseOrderActivity>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Models.AldebaranDb.CustomerOrderDetail>()
                  .Property(p => p.PROCESSED_QUANTITY)
                  .HasDefaultValueSql(@"((0))");

            builder.Entity<Models.AldebaranDb.CustomerOrderDetail>()
                  .Property(p => p.DELIVERED_QUANTITY)
                  .HasDefaultValueSql(@"((0))");

            builder.Entity<Models.AldebaranDb.PurchaseOrderDetail>()
                  .Property(p => p.RECEIVED_QUANTITY)
                  .HasDefaultValueSql(@"((0))");

            builder.Entity<Models.AldebaranDb.CustomerReservationDetail>()
                  .Property(p => p.SEND_TO_CUSTOMER_ORDER)
                  .HasDefaultValueSql(@"((0))");

            builder.Entity<Models.AldebaranDb.PurchaseOrder>()
                  .Property(p => p.EMBARKATION_PORT)
                  .HasDefaultValueSql(@"(' ')");

            builder.Entity<Models.AldebaranDb.PurchaseOrder>()
                  .Property(p => p.PROFORMA_NUMBER)
                  .HasDefaultValueSql(@"(' ')");

            builder.Entity<Models.AldebaranDb.PurchaseOrder>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Models.AldebaranDb.CustomerReservation>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Models.AldebaranDb.CustomerReservation>()
                  .Property(p => p.CREATION_DATE)
                  .HasColumnType("datetime");

            builder.Entity<Models.AldebaranDb.Adjustment>()
                  .Property(p => p.CREATION_DATE)
                  .HasColumnType("datetime");

            builder.Entity<Models.AldebaranDb.PurchaseOrderActivity>()
                  .Property(p => p.EXECUTION_DATE)
                  .HasColumnType("datetime");

            builder.Entity<Models.AldebaranDb.PurchaseOrderActivity>()
                  .Property(p => p.CREATION_DATE)
                  .HasColumnType("datetime");

            builder.Entity<Models.AldebaranDb.PurchaseOrder>()
                  .Property(p => p.CREATION_DATE)
                  .HasColumnType("datetime");
            this.OnModelBuilding(builder);

            builder.Entity<Models.AldebaranDb.CustomerOrderActivity>()
                 .Property(p => p.ACTIVITY_DATE)
                 .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Models.AldebaranDb.CustomerOrderActivity>()
                  .Property(p => p.ACTIVITY_DATE)
                  .HasColumnType("datetime");
        }

        public DbSet<Models.AldebaranDb.AdjustmentDetail> AdjustmentDetails { get; set; }

        public DbSet<Models.AldebaranDb.AdjustmentReason> AdjustmentReasons { get; set; }

        public DbSet<Models.AldebaranDb.AdjustmentType> AdjustmentTypes { get; set; }

        public DbSet<Models.AldebaranDb.Adjustment> Adjustments { get; set; }

        public DbSet<Models.AldebaranDb.Area> Areas { get; set; }

        public DbSet<Models.AldebaranDb.City> Cities { get; set; }

        public DbSet<Models.AldebaranDb.Country> Countries { get; set; }

        public DbSet<Models.AldebaranDb.Currency> Currencies { get; set; }

        public DbSet<Models.AldebaranDb.CustomerContact> CustomerContacts { get; set; }

        public DbSet<Models.AldebaranDb.Customer> Customers { get; set; }

        public DbSet<Models.AldebaranDb.Department> Departments { get; set; }

        public DbSet<Models.AldebaranDb.Employee> Employees { get; set; }

        public DbSet<Models.AldebaranDb.ForwarderAgent> ForwarderAgents { get; set; }

        public DbSet<Models.AldebaranDb.Forwarder> Forwarders { get; set; }

        public DbSet<Models.AldebaranDb.IdentityType> IdentityTypes { get; set; }

        public DbSet<Models.AldebaranDb.ItemReference> ItemReferences { get; set; }

        public DbSet<Models.AldebaranDb.Item> Items { get; set; }

        public DbSet<Models.AldebaranDb.ItemsArea> ItemsAreas { get; set; }

        public DbSet<Models.AldebaranDb.Line> Lines { get; set; }

        public DbSet<Models.AldebaranDb.MeasureUnit> MeasureUnits { get; set; }

        public DbSet<Models.AldebaranDb.ProviderReference> ProviderReferences { get; set; }

        public DbSet<Models.AldebaranDb.Provider> Providers { get; set; }

        public DbSet<Models.AldebaranDb.PurchaseOrderActivity> PurchaseOrderActivities { get; set; }

        public DbSet<Models.AldebaranDb.PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public DbSet<Models.AldebaranDb.PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<Models.AldebaranDb.ReferencesWarehouse> ReferencesWarehouses { get; set; }

        public DbSet<Models.AldebaranDb.ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }

        public DbSet<Models.AldebaranDb.ShipmentMethod> ShipmentMethods { get; set; }

        public DbSet<Models.AldebaranDb.ShippingMethod> ShippingMethods { get; set; }

        public DbSet<Models.AldebaranDb.Warehouse> Warehouses { get; set; }

        public DbSet<Models.AldebaranDb.CustomerReservation> CustomerReservations { get; set; }

        public DbSet<Models.AldebaranDb.CustomerReservationDetail> CustomerReservationDetails { get; set; }

        public DbSet<Models.AldebaranDb.StatusDocumentType> StatusDocumentTypes { get; set; }

        public DbSet<Models.AldebaranDb.DocumentType> DocumentTypes { get; set; }

        public DbSet<Models.AldebaranDb.CustomerOrder> CustomerOrders { get; set; }

        public DbSet<Models.AldebaranDb.CustomerOrderDetail> CustomerOrderDetails { get; set; }

        public DbSet<Models.AldebaranDb.ActivityType> ActivityTypes { get; set; }

        public DbSet<Models.AldebaranDb.ActivityTypeArea> ActivityTypesAreas { get; set; }

        public DbSet<Models.AldebaranDb.CustomerOrderActivity> CustomerOrderActivities { get; set; }

        public DbSet<Models.AldebaranDb.CustomerOrderActivityDetail> CustomerOrderActivityDetails { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }

    }
}