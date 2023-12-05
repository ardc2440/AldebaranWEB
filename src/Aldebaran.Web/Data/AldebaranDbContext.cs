using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

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

            builder.Entity<ActivityTypeArea>().HasKey(table => new
            {
                table.ACTIVITY_TYPE_ID,
                table.AREA_ID
            });

            builder.Entity<ItemsArea>().HasKey(table => new
            {
                table.ITEM_ID,
                table.AREA_ID
            });

            builder.Entity<ProviderReference>().HasKey(table => new
            {
                table.REFERENCE_ID,
                table.PROVIDER_ID
            });

            builder.Entity<ReferencesWarehouse>().HasKey(table => new
            {
                table.REFERENCE_ID,
                table.WAREHOUSE_ID
            });

            builder.Entity<CustomerOrderInProcessDetail>()
             .HasOne(i => i.CustomerOrderDetail)
             .WithMany(i => i.CustomerOrderInProcessDetails)
             .HasForeignKey(i => i.CUSTOMER_ORDER_DETAIL_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ORDER_DETAIL_ID);

            builder.Entity<Adjustment>()
             .HasOne(i => i.StatusDocumentType)
             .WithMany(i => i.Adjustments)
             .HasForeignKey(i => i.STATUS_DOCUMENT_TYPE_ID)
             .HasPrincipalKey(i => i.STATUS_DOCUMENT_TYPE_ID);

            builder.Entity<CustomerOrderInProcessDetail>()
             .HasOne(i => i.CustomerOrderInProcess)
             .WithMany(i => i.CustomerOrderInProcessDetails)
             .HasForeignKey(i => i.CUSTOMER_ORDER_IN_PROCESS_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ORDER_IN_PROCESS_ID);

            builder.Entity<CustomerOrderInProcessDetail>()
             .HasOne(i => i.Warehouse)
             .WithMany(i => i.CustomerOrderInProcessDetails)
             .HasForeignKey(i => i.WAREHOUSE_ID)
             .HasPrincipalKey(i => i.WAREHOUSE_ID);

            builder.Entity<CustomerOrderInProcess>()
             .HasOne(i => i.StatusDocumentType)
             .WithMany(i => i.CustomerOrderInProcesses)
             .HasForeignKey(i => i.STATUS_DOCUMENT_TYPE_ID)
             .HasPrincipalKey(i => i.STATUS_DOCUMENT_TYPE_ID);

            builder.Entity<CustomerOrderInProcess>()
             .HasOne(i => i.Employee)
             .WithMany(i => i.CustomerOrderInProcesses)
             .HasForeignKey(i => i.EMPLOYEE_RECIPIENT_ID)
             .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<CustomerOrderInProcess>()
             .HasOne(i => i.ProcessSatellite)
             .WithMany(i => i.CustomerOrderInProcesses)
             .HasForeignKey(i => i.PROCESS_SATELLITE_ID)
             .HasPrincipalKey(i => i.PROCESS_SATELLITE_ID);

            builder.Entity<CustomerOrderInProcess>()
             .HasOne(i => i.CustomerOrder)
             .WithMany(i => i.CustomerOrderInProcesses)
             .HasForeignKey(i => i.CUSTOMER_ORDER_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ORDER_ID);

            builder.Entity<ProcessSatellite>()
             .HasOne(i => i.IdentityType)
             .WithMany(i => i.ProcessSatellites)
             .HasForeignKey(i => i.IDENTITY_TYPE_ID)
             .HasPrincipalKey(i => i.IDENTITY_TYPE_ID);

            builder.Entity<ProcessSatellite>()
             .HasOne(i => i.City)
             .WithMany(i => i.ProcessSatellites)
             .HasForeignKey(i => i.CITY_ID)
             .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<ActivityTypeArea>()
             .HasOne(i => i.Area)
             .WithMany(i => i.ActivityTypesAreas)
             .HasForeignKey(i => i.AREA_ID)
             .HasPrincipalKey(i => i.AREA_ID);

            builder.Entity<ActivityTypeArea>()
             .HasOne(i => i.ActivityType)
             .WithMany(i => i.ActivityTypesAreas)
             .HasForeignKey(i => i.ACTIVITY_TYPE_ID)
             .HasPrincipalKey(i => i.ACTIVITY_TYPE_ID);

            builder.Entity<CustomerReservation>()
              .HasOne(i => i.Customer)
              .WithMany(i => i.CustomerReservations)
              .HasForeignKey(i => i.CUSTOMER_ID)
              .HasPrincipalKey(i => i.CUSTOMER_ID);

            builder.Entity<CustomerReservation>()
              .HasOne(i => i.StatusDocumentType)
              .WithMany(i => i.CustomerReservations)
              .HasForeignKey(i => i.STATUS_DOCUMENT_TYPE_ID)
              .HasPrincipalKey(i => i.STATUS_DOCUMENT_TYPE_ID);

            builder.Entity<CustomerReservation>()
              .HasOne(i => i.Employee)
              .WithMany(i => i.CustomerReservations)
              .HasForeignKey(i => i.EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<CustomerReservationDetail>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.CustomerReservationDetails)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<CustomerReservationDetail>()
              .HasOne(i => i.CustomerReservation)
              .WithMany(i => i.CustomerReservationDetails)
              .HasForeignKey(i => i.CUSTOMER_RESERVATION_ID)
              .HasPrincipalKey(i => i.CUSTOMER_RESERVATION_ID);

            builder.Entity<StatusDocumentType>()
              .HasOne(i => i.DocumentType)
              .WithMany(i => i.StatusDocumentTypes)
              .HasForeignKey(i => i.DOCUMENT_TYPE_ID)
              .HasPrincipalKey(i => i.DOCUMENT_TYPE_ID);

            builder.Entity<AdjustmentDetail>()
              .HasOne(i => i.Adjustment)
              .WithMany(i => i.AdjustmentDetails)
              .HasForeignKey(i => i.ADJUSTMENT_ID)
              .HasPrincipalKey(i => i.ADJUSTMENT_ID);

            builder.Entity<AdjustmentDetail>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.AdjustmentDetails)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<AdjustmentDetail>()
              .HasOne(i => i.Warehouse)
              .WithMany(i => i.AdjustmentDetails)
              .HasForeignKey(i => i.WAREHOUSE_ID)
              .HasPrincipalKey(i => i.WAREHOUSE_ID);

            builder.Entity<Adjustment>()
              .HasOne(i => i.AdjustmentReason)
              .WithMany(i => i.Adjustments)
              .HasForeignKey(i => i.ADJUSTMENT_REASON_ID)
              .HasPrincipalKey(i => i.ADJUSTMENT_REASON_ID);

            builder.Entity<Adjustment>()
              .HasOne(i => i.AdjustmentType)
              .WithMany(i => i.Adjustments)
              .HasForeignKey(i => i.ADJUSTMENT_TYPE_ID)
              .HasPrincipalKey(i => i.ADJUSTMENT_TYPE_ID);

            builder.Entity<Adjustment>()
              .HasOne(i => i.Employee)
              .WithMany(i => i.Adjustments)
              .HasForeignKey(i => i.EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

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

            builder.Entity<Employee>()
              .HasOne(i => i.Area)
              .WithMany(i => i.Employees)
              .HasForeignKey(i => i.AREA_ID)
              .HasPrincipalKey(i => i.AREA_ID);

            builder.Entity<Employee>()
              .HasOne(i => i.IdentityType)
              .WithMany(i => i.Employees)
              .HasForeignKey(i => i.IDENTITY_TYPE_ID)
              .HasPrincipalKey(i => i.IDENTITY_TYPE_ID);

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

            builder.Entity<ProviderReference>()
              .HasOne(i => i.Provider)
              .WithMany(i => i.ProviderReferences)
              .HasForeignKey(i => i.PROVIDER_ID)
              .HasPrincipalKey(i => i.PROVIDER_ID);

            builder.Entity<ProviderReference>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.ProviderReferences)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<Provider>()
              .HasOne(i => i.City)
              .WithMany(i => i.Providers)
              .HasForeignKey(i => i.CITY_ID)
              .HasPrincipalKey(i => i.CITY_ID);

            builder.Entity<Provider>()
              .HasOne(i => i.IdentityType)
              .WithMany(i => i.Providers)
              .HasForeignKey(i => i.IDENTITY_TYPE_ID)
              .HasPrincipalKey(i => i.IDENTITY_TYPE_ID);

            builder.Entity<PurchaseOrderActivity>()
              .HasOne(i => i.ActivityEmployee)
              .WithMany(i => i.PurchaseOrderActivities)
              .HasForeignKey(i => i.ACTIVITY_EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<PurchaseOrderActivity>()
              .HasOne(i => i.Employee)
              .WithMany(i => i.PurchaseOrderActivities1)
              .HasForeignKey(i => i.EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<PurchaseOrderActivity>()
              .HasOne(i => i.PurchaseOrder)
              .WithMany(i => i.PurchaseOrderActivities)
              .HasForeignKey(i => i.PURCHASE_ORDER_ID)
              .HasPrincipalKey(i => i.PURCHASE_ORDER_ID);

            builder.Entity<PurchaseOrderDetail>()
              .HasOne(i => i.PurchaseOrder)
              .WithMany(i => i.PurchaseOrderDetails)
              .HasForeignKey(i => i.PURCHASE_ORDER_ID)
              .HasPrincipalKey(i => i.PURCHASE_ORDER_ID);

            builder.Entity<PurchaseOrderDetail>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.PurchaseOrderDetails)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<PurchaseOrderDetail>()
              .HasOne(i => i.Warehouse)
              .WithMany(i => i.PurchaseOrderDetails)
              .HasForeignKey(i => i.WAREHOUSE_ID)
              .HasPrincipalKey(i => i.WAREHOUSE_ID);

            builder.Entity<PurchaseOrder>()
              .HasOne(i => i.Employee)
              .WithMany(i => i.PurchaseOrders)
              .HasForeignKey(i => i.EMPLOYEE_ID)
              .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<PurchaseOrder>()
              .HasOne(i => i.ForwarderAgent)
              .WithMany(i => i.PurchaseOrders)
              .HasForeignKey(i => i.FORWARDER_AGENT_ID)
              .HasPrincipalKey(i => i.FORWARDER_AGENT_ID);

            builder.Entity<PurchaseOrder>()
              .HasOne(i => i.Provider)
              .WithMany(i => i.PurchaseOrders)
              .HasForeignKey(i => i.PROVIDER_ID)
              .HasPrincipalKey(i => i.PROVIDER_ID);

            builder.Entity<PurchaseOrder>()
             .HasOne(i => i.StatusDocumentType)
             .WithMany(i => i.PurchaseOrders)
             .HasForeignKey(i => i.STATUS_DOCUMENT_TYPE_ID)
             .HasPrincipalKey(i => i.STATUS_DOCUMENT_TYPE_ID);

            builder.Entity<PurchaseOrder>()
              .HasOne(i => i.ShipmentForwarderAgentMethod)
              .WithMany(i => i.PurchaseOrders)
              .HasForeignKey(i => i.SHIPMENT_FORWARDER_AGENT_METHOD_ID)
              .HasPrincipalKey(i => i.SHIPMENT_FORWARDER_AGENT_METHOD_ID);

            builder.Entity<PurchaseOrder>()
                .Property(x => x.ORDER_NUMBER).ValueGeneratedOnAdd().HasValueGenerator<PurchaseOrderNumberValueGenerator>();

            builder.Entity<ReferencesWarehouse>()
              .HasOne(i => i.ItemReference)
              .WithMany(i => i.ReferencesWarehouses)
              .HasForeignKey(i => i.REFERENCE_ID)
              .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<ReferencesWarehouse>()
              .HasOne(i => i.Warehouse)
              .WithMany(i => i.ReferencesWarehouses)
              .HasForeignKey(i => i.WAREHOUSE_ID)
              .HasPrincipalKey(i => i.WAREHOUSE_ID);

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

            builder.Entity<CustomerReservation>()
              .HasOne(i => i.CustomerOrder)
              .WithMany(i => i.CustomerReservations)
              .HasForeignKey(i => i.CUSTOMER_ORDER_ID)
              .HasPrincipalKey(i => i.CUSTOMER_ORDER_ID);

            builder.Entity<CustomerOrder>()
             .HasOne(i => i.Customer)
             .WithMany(i => i.CustomerOrders)
             .HasForeignKey(i => i.CUSTOMER_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ID);

            builder.Entity<CustomerOrder>()
             .HasOne(i => i.StatusDocumentType)
             .WithMany(i => i.CustomerOrders)
             .HasForeignKey(i => i.STATUS_DOCUMENT_TYPE_ID)
             .HasPrincipalKey(i => i.STATUS_DOCUMENT_TYPE_ID);

            builder.Entity<CustomerOrder>()
             .HasOne(i => i.Employee)
             .WithMany(i => i.CustomerOrders)
             .HasForeignKey(i => i.EMPLOYEE_ID)
             .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<CustomerOrderDetail>()
             .HasOne(i => i.ItemReference)
             .WithMany(i => i.CustomerOrderDetails)
             .HasForeignKey(i => i.REFERENCE_ID)
             .HasPrincipalKey(i => i.REFERENCE_ID);

            builder.Entity<CustomerOrderDetail>()
             .HasOne(i => i.CustomerOrder)
             .WithMany(i => i.CustomerOrderDetails)
             .HasForeignKey(i => i.CUSTOMER_ORDER_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ORDER_ID);

            builder.Entity<CustomerOrderActivity>()
             .HasOne(i => i.CustomerOrder)
             .WithMany(i => i.CustomerOrderActivities)
             .HasForeignKey(i => i.CUSTOMER_ORDER_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ORDER_ID);

            builder.Entity<CustomerOrderActivity>()
             .HasOne(i => i.Area)
             .WithMany(i => i.CustomerOrderActivities)
             .HasForeignKey(i => i.AREA_ID)
             .HasPrincipalKey(i => i.AREA_ID);

            builder.Entity<CustomerOrderActivity>()
             .HasOne(i => i.Employee)
             .WithMany(i => i.CustomerOrderActivities)
             .HasForeignKey(i => i.EMPLOYEE_ID)
             .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<CustomerOrderActivityDetail>()
             .HasOne(i => i.CustomerOrderActivity)
             .WithMany(i => i.CustomerOrderActivityDetails)
             .HasForeignKey(i => i.CUSTOMER_ORDER_ACTIVITY_ID)
             .HasPrincipalKey(i => i.CUSTOMER_ORDER_ACTIVITY_ID);

            builder.Entity<CustomerOrderActivityDetail>()
             .HasOne(i => i.ActivityType)
             .WithMany(i => i.CustomerOrderActivityDetails)
             .HasForeignKey(i => i.ACTIVITY_TYPE_ID)
             .HasPrincipalKey(i => i.ACTIVITY_TYPE_ID);

            builder.Entity<CustomerOrderActivityDetail>()
             .HasOne(i => i.Employee)
             .WithMany(i => i.CustomerOrderActivityDetails)
             .HasForeignKey(i => i.EMPLOYEE_ID)
             .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<CustomerOrderActivityDetail>()
             .HasOne(i => i.EmployeeActivity)
             .WithMany(i => i.CustomerOrderActivityDetailsActivityEmployee)
             .HasForeignKey(i => i.ACTIVITY_EMPLOYEE_ID)
             .HasPrincipalKey(i => i.EMPLOYEE_ID);

            builder.Entity<CustomerOrderInProcess>()
                  .Property(p => p.PROCESS_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<CustomerOrderInProcess>()
                  .Property(p => p.TRANSFER_DATETIME)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<CustomerOrderInProcess>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Adjustment>()
                  .Property(p => p.ADJUSTMENT_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Adjustment>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<StatusDocumentType>()
                  .Property(p => p.EDIT_MODE)
                  .HasDefaultValueSql(@"((1))");

            builder.Entity<ItemReference>()
                  .Property(p => p.IS_ACTIVE)
                  .HasDefaultValueSql(@"((1))");

            builder.Entity<Item>()
                  .Property(p => p.IS_ACTIVE)
                  .HasDefaultValueSql(@"((1))");

            builder.Entity<Line>()
                  .Property(p => p.IS_ACTIVE)
                  .HasDefaultValueSql(@"((1))");

            builder.Entity<CustomerOrder>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<PurchaseOrderActivity>()
                  .Property(p => p.EXECUTION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<PurchaseOrderActivity>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<CustomerOrderDetail>()
                  .Property(p => p.PROCESSED_QUANTITY)
                  .HasDefaultValueSql(@"((0))");

            builder.Entity<CustomerOrderDetail>()
                  .Property(p => p.DELIVERED_QUANTITY)
                  .HasDefaultValueSql(@"((0))");

            builder.Entity<PurchaseOrderDetail>()
                  .Property(p => p.RECEIVED_QUANTITY)
                  .HasDefaultValueSql(@"((0))");

            builder.Entity<CustomerReservationDetail>()
                  .Property(p => p.SEND_TO_CUSTOMER_ORDER)
                  .HasDefaultValueSql(@"((0))");

            builder.Entity<PurchaseOrder>()
                  .Property(p => p.EMBARKATION_PORT)
                  .HasDefaultValueSql(@"(' ')");

            builder.Entity<PurchaseOrder>()
                  .Property(p => p.PROFORMA_NUMBER)
                  .HasDefaultValueSql(@"(' ')");

            builder.Entity<PurchaseOrder>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<CustomerReservation>()
                  .Property(p => p.CREATION_DATE)
                  .HasDefaultValueSql(@"(getdate())");

            builder.Entity<CustomerReservation>()
                  .Property(p => p.CREATION_DATE)
                  .HasColumnType("datetime");

            builder.Entity<Adjustment>()
                  .Property(p => p.CREATION_DATE)
                  .HasColumnType("datetime");

            builder.Entity<PurchaseOrderActivity>()
                  .Property(p => p.EXECUTION_DATE)
                  .HasColumnType("datetime");

            builder.Entity<PurchaseOrderActivity>()
                  .Property(p => p.CREATION_DATE)
                  .HasColumnType("datetime");

            builder.Entity<PurchaseOrder>()
                  .Property(p => p.CREATION_DATE)
                  .HasColumnType("datetime");
            this.OnModelBuilding(builder);

            builder.Entity<CustomerOrderActivity>()
                 .Property(p => p.ACTIVITY_DATE)
                 .HasDefaultValueSql(@"(getdate())");

            builder.Entity<CustomerOrderActivity>()
                  .Property(p => p.ACTIVITY_DATE)
                  .HasColumnType("datetime");

            builder.Entity<CustomerOrderInProcess>()
               .Property(p => p.TRANSFER_DATETIME)
               .HasDefaultValueSql(@"(getdate())");

            builder.Entity<CustomerOrderInProcess>()
                  .Property(p => p.CREATION_DATE)
            .HasColumnType("datetime");
        }

        public DbSet<AdjustmentDetail> AdjustmentDetails { get; set; }

        public DbSet<AdjustmentReason> AdjustmentReasons { get; set; }

        public DbSet<AdjustmentType> AdjustmentTypes { get; set; }

        public DbSet<Adjustment> Adjustments { get; set; }

        public DbSet<Area> Areas { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<ForwarderAgent> ForwarderAgents { get; set; }

        public DbSet<Forwarder> Forwarders { get; set; }

        public DbSet<IdentityType> IdentityTypes { get; set; }

        public DbSet<ItemReference> ItemReferences { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<ItemsArea> ItemsAreas { get; set; }

        public DbSet<Line> Lines { get; set; }

        public DbSet<MeasureUnit> MeasureUnits { get; set; }

        public DbSet<ProviderReference> ProviderReferences { get; set; }

        public DbSet<Provider> Providers { get; set; }

        public DbSet<PurchaseOrderActivity> PurchaseOrderActivities { get; set; }

        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<ReferencesWarehouse> ReferencesWarehouses { get; set; }

        public DbSet<ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }

        public DbSet<ShipmentMethod> ShipmentMethods { get; set; }

        public DbSet<ShippingMethod> ShippingMethods { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<CustomerReservation> CustomerReservations { get; set; }

        public DbSet<CustomerReservationDetail> CustomerReservationDetails { get; set; }

        public DbSet<StatusDocumentType> StatusDocumentTypes { get; set; }

        public DbSet<DocumentType> DocumentTypes { get; set; }

        public DbSet<CustomerOrder> CustomerOrders { get; set; }

        public DbSet<CustomerOrderDetail> CustomerOrderDetails { get; set; }

        public DbSet<ActivityType> ActivityTypes { get; set; }

        public DbSet<ActivityTypeArea> ActivityTypesAreas { get; set; }

        public DbSet<CustomerOrderActivity> CustomerOrderActivities { get; set; }

        public DbSet<CustomerOrderActivityDetail> CustomerOrderActivityDetails { get; set; }

        public DbSet<CustomerOrderInProcess> CustomerOrderInProcesses { get; set; }

        public DbSet<CustomerOrderInProcessDetail> CustomerOrderInProcessDetails { get; set; }

        public DbSet<ProcessSatellite> ProcessSatellites { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }

    public class PurchaseOrderNumberValueGenerator : ValueGenerator<string>
    {
        public override string Next(EntityEntry entry)
        {
            var dbContext = entry.Context;
            var currentRecords = dbContext.Set<PurchaseOrder>().AsNoTracking().Count();
            return (currentRecords + 1).ToString().PadLeft(10, '0');
        }
        public override bool GeneratesTemporaryValues => false;
    }
}