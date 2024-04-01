using Aldebaran.DataAccess.Core.Atributes;
using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Aldebaran.DataAccess
{
    // ****************************************************************************************************
    // This is not a commercial licence, therefore only a few tables/views/stored procedures are generated.
    // ****************************************************************************************************
    public class AldebaranDbContext : DbContext
    {
        public Dictionary<string, bool> Events = new();

        public AldebaranDbContext()
        {
        }
        public AldebaranDbContext(DbContextOptions<AldebaranDbContext> options)
            : base(options)
        {
        }

        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<ActivityTypesArea> ActivityTypesAreas { get; set; }
        public DbSet<Adjustment> Adjustments { get; set; }
        public DbSet<AdjustmentDetail> AdjustmentDetails { get; set; }
        public DbSet<AdjustmentReason> AdjustmentReasons { get; set; }
        public DbSet<AdjustmentType> AdjustmentTypes { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<AlarmMessage> AlarmMessages { get; set; }
        public DbSet<AlarmType> AlarmTypes { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<CanceledCustomerOrder> CanceledCustomerOrders { get; set; }
        public DbSet<CanceledCustomerReservation> CanceledCustomerReservations { get; set; }
        public DbSet<CanceledOrderShipment> CanceledOrderShipments { get; set; }
        public DbSet<CanceledOrdersInProcess> CanceledOrdersInProcesses { get; set; }
        public DbSet<CanceledPurchaseOrder> CanceledPurchaseOrders { get; set; }
        public DbSet<CancellationReason> CancellationReasons { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<CloseCustomerOrderReason> CloseCustomerOrderReasons { get; set; }
        public DbSet<ClosedCustomerOrder> ClosedCustomerOrders { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<CustomerOrderActivity> CustomerOrderActivities { get; set; }
        public DbSet<CustomerOrderActivityDetail> CustomerOrderActivityDetails { get; set; }
        public DbSet<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public DbSet<CustomerOrderInProcessDetail> CustomerOrderInProcessDetails { get; set; }
        public DbSet<CustomerOrderShipment> CustomerOrderShipments { get; set; }
        public DbSet<CustomerOrderShipmentDetail> CustomerOrderShipmentDetails { get; set; }
        public DbSet<CustomerOrdersInProcess> CustomerOrdersInProcesses { get; set; }
        public DbSet<CustomerReservation> CustomerReservations { get; set; }
        public DbSet<CustomerReservationDetail> CustomerReservationDetails { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Forwarder> Forwarders { get; set; }
        public DbSet<ForwarderAgent> ForwarderAgents { get; set; }
        public DbSet<IdentityType> IdentityTypes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemReference> ItemReferences { get; set; }
        public DbSet<ItemsArea> ItemsAreas { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<MeasureUnit> MeasureUnits { get; set; }
        public DbSet<ModificationReason> ModificationReasons { get; set; }
        public DbSet<ModifiedCustomerOrder> ModifiedCustomerOrders { get; set; }
        public DbSet<ModifiedCustomerReservation> ModifiedCustomerReservations { get; set; }
        public DbSet<ModifiedOrderShipment> ModifiedOrderShipments { get; set; }
        public DbSet<ModifiedOrdersInProcess> ModifiedOrdersInProcesses { get; set; }
        public DbSet<ModifiedPurchaseOrder> ModifiedPurchaseOrders { get; set; }
        public DbSet<Packaging> Packagings { get; set; }
        public DbSet<ProcessSatellite> ProcessSatellites { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderReference> ProviderReferences { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderActivity> PurchaseOrderActivities { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<ReferencesWarehouse> ReferencesWarehouses { get; set; }
        public DbSet<ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }
        public DbSet<ShipmentMethod> ShipmentMethods { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<StatusDocumentType> StatusDocumentTypes { get; set; }
        public DbSet<UsersAlarmType> UsersAlarmTypes { get; set; }
        public DbSet<VisualizedAlarm> VisualizedAlarms { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseTransfer> WarehouseTransfers { get; set; }
        public DbSet<WarehouseTransferDetail> WarehouseTransferDetails { get; set; }
        public DbSet<NotificationProviderSetting> NotificationProviderSettings { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ActivityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityTypesAreaConfiguration());
            modelBuilder.ApplyConfiguration(new AdjustmentConfiguration());
            modelBuilder.ApplyConfiguration(new AdjustmentDetailConfiguration());
            modelBuilder.ApplyConfiguration(new AdjustmentReasonConfiguration());
            modelBuilder.ApplyConfiguration(new AdjustmentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AlarmConfiguration());
            modelBuilder.ApplyConfiguration(new AlarmMessageConfiguration());
            modelBuilder.ApplyConfiguration(new AlarmTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AreaConfiguration());
            modelBuilder.ApplyConfiguration(new CanceledCustomerOrderConfiguration());
            modelBuilder.ApplyConfiguration(new CanceledCustomerReservationConfiguration());
            modelBuilder.ApplyConfiguration(new CanceledOrderShipmentConfiguration());
            modelBuilder.ApplyConfiguration(new CanceledOrdersInProcessConfiguration());
            modelBuilder.ApplyConfiguration(new CanceledPurchaseOrderConfiguration());
            modelBuilder.ApplyConfiguration(new CancellationReasonConfiguration());
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            modelBuilder.ApplyConfiguration(new CloseCustomerOrderReasonConfiguration());
            modelBuilder.ApplyConfiguration(new ClosedCustomerOrderConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerContactConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerOrderConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerOrderActivityConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerOrderActivityDetailConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerOrderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerOrderInProcessDetailConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerOrderShipmentConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerOrderShipmentDetailConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerOrdersInProcessConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerReservationConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerReservationDetailConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new ForwarderConfiguration());
            modelBuilder.ApplyConfiguration(new ForwarderAgentConfiguration());
            modelBuilder.ApplyConfiguration(new IdentityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new ItemReferenceConfiguration());
            modelBuilder.ApplyConfiguration(new ItemsAreaConfiguration());
            modelBuilder.ApplyConfiguration(new LineConfiguration());
            modelBuilder.ApplyConfiguration(new MeasureUnitConfiguration());
            modelBuilder.ApplyConfiguration(new ModificationReasonConfiguration());
            modelBuilder.ApplyConfiguration(new ModifiedCustomerOrderConfiguration());
            modelBuilder.ApplyConfiguration(new ModifiedCustomerReservationConfiguration());
            modelBuilder.ApplyConfiguration(new ModifiedOrderShipmentConfiguration());
            modelBuilder.ApplyConfiguration(new ModifiedOrdersInProcessConfiguration());
            modelBuilder.ApplyConfiguration(new ModifiedPurchaseOrderConfiguration());
            modelBuilder.ApplyConfiguration(new PackagingConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessSatelliteConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderReferenceConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderActivityConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new ReferencesWarehouseConfiguration());
            modelBuilder.ApplyConfiguration(new ShipmentForwarderAgentMethodConfiguration());
            modelBuilder.ApplyConfiguration(new ShipmentMethodConfiguration());
            modelBuilder.ApplyConfiguration(new ShippingMethodConfiguration());
            modelBuilder.ApplyConfiguration(new StatusDocumentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UsersAlarmTypeConfiguration());
            modelBuilder.ApplyConfiguration(new VisualizedAlarmConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseTransferConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseTransferDetailConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationProviderSettingConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationTemplateConfiguration());

            modelBuilder.Entity<InventoryAdjustmentReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<InProcessInventoryReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<InventoryReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<CustomerOrderReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<CustomerReservationReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<ProviderReferenceReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<ReferenceMovementReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<WarehouseStockReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<OrderShipmentReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<CustomerOrderActivityReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<WarehouseTransferReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<FreezoneVsAvailableReport>(iar => { iar.HasNoKey(); });
            modelBuilder.Entity<CustomerSaleReport>(iar => { iar.HasNoKey(); });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            UpdateSequenceProperties();
            return await base.SaveChangesAsync(ct);
        }

        private void UpdateSequenceProperties()
        {
            var dbContext = this;
            var entities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);

            foreach (var entity in entities)
            {
                var properties = entity.GetType().GetProperties()
                    .Where(p => p.GetCustomAttribute<SequenceAttribute>() != null);

                foreach (var property in properties)
                {
                    var sequenceAttribute = property.GetCustomAttribute<SequenceAttribute>();
                    var currentValue = property.GetValue(entity) as string;
                    var entityType = Model.FindEntityType(entity.GetType());
                    var setMethod = dbContext.GetType().GetMethod("Set", 1, Type.EmptyTypes);

                    if (entityType == null) continue;
                    if (sequenceAttribute == null) continue;
                    if (!string.IsNullOrEmpty(currentValue)) continue;
                    if (setMethod == null) continue;

                    var genericMethods = setMethod.MakeGenericMethod(entityType.ClrType);
                    var dbset = genericMethods.Invoke(dbContext, null);
                    if (dbset == null) continue;

                    var totalRecords = ((IQueryable<Object>)dbset).Count();
                    var newValue = (totalRecords + 1).ToString().PadLeft(sequenceAttribute.Length, '0');

                    property.SetValue(entity, newValue);
                }
            }
        }
    }
}
