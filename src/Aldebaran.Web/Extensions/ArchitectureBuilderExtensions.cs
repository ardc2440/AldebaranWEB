using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Application.Services.Services;
using Aldebaran.DataAccess;
using Aldebaran.DataAccess.Core.Triggers.Adjustments;
using Aldebaran.DataAccess.Core.Triggers.OrderInProcesses;
using Aldebaran.DataAccess.Core.Triggers.Orders;
using Aldebaran.DataAccess.Core.Triggers.Purchases;
using Aldebaran.DataAccess.Core.Triggers.Reservations;
using Aldebaran.DataAccess.Core.Triggers.Shipments;
using Aldebaran.DataAccess.Core.Triggers.Transfers;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Infraestructure.Core.Model;
using Aldebaran.Infraestructure.Core.Queue;
using Aldebaran.Infraestructure.Core.Ssh;
using Aldebaran.Web.Data;
using Aldebaran.Web.Models;
using Aldebaran.Web.Settings;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Polly;
using Radzen;
using Notificator = Aldebaran.Application.Services.Notificator;

namespace Aldebaran.Web.Extensions
{
    public static class ArchitectureBuilderExtensions
    {
        public static IServiceCollection AddArchitecture(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            var services = builder.Services;
            var configuration = builder.Configuration;
            var amqpConnection = configuration.GetConnectionString("RabbitMqConnection") ?? throw new KeyNotFoundException("RabbitMqConnection");
            var dbConnection = configuration.GetConnectionString("AldebaranDbConnection") ?? throw new KeyNotFoundException("AldebaranDbConnection");

            // Add services to the container.
            services.AddRazorPages();
            services.AddServerSideBlazor().AddHubOptions(o =>
            {
                o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
            });
            services.Configure<IdentityOptions>(options =>
            {
                // Configuración de complejidad de contraseña
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            });
            // Data context
            services.AddDbContext<DataAccess.AldebaranDbContext>(options => { options.UseSqlServer(dbConnection).AddTriggers(); }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            //
            //services.AddDbContext<DataAccess.AldebaranDashBoardDbContext>(options => { options.UseSqlServer(dbConnection).AddTriggers(); }, ServiceLifetime.Transient, ServiceLifetime.Scoped);
            // Identity context
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options => { options.UseSqlServer(dbConnection); }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders().AddErrorDescriber<MultilanguageIdentityErrorDescriber>();
            builder.Services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();
            // Configuration
            services.Configure<FtpSettings>(configuration.GetSection("FtpSettings"));
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            builder.Services.AddControllers().AddOData(o =>
            {
                var oDataBuilder = new ODataConventionModelBuilder();
                oDataBuilder.EntitySet<ApplicationUser>("ApplicationUsers");
                var usersType = oDataBuilder.StructuralTypes.First(x => x.ClrType == typeof(ApplicationUser));
                usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Password)));
                usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.ConfirmPassword)));
                oDataBuilder.EntitySet<ApplicationRole>("ApplicationRoles");
                o.AddRouteComponents("odata/Identity", oDataBuilder.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
            });
            // RabbitMq
            services.AddSingleton(sp =>
            {
                return Policy.Handle<Exception>()
                    .WaitAndRetry(10, _ => TimeSpan.FromSeconds(5), (ex, ts) =>
                    {
                        Console.WriteLine($"Resilience Error [{ex.Message}] since [{ts.TotalSeconds}] sec using connection [{amqpConnection}]");
                    })
                    .Execute(() =>
                    {
                        var factory = new RabbitMQ.Client.ConnectionFactory()
                        {
                            Uri = new Uri(amqpConnection),
                            DispatchConsumersAsync = true
                        };
                        var conn = factory.CreateConnection();
                        return conn;
                    });
            });
            services.AddHttpClient("Aldebaran.Web").AddHeaderPropagation(o => o.Headers.Add("Cookie"));
            services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
            // Auth
            services.AddAuthentication();
            services.AddAuthorization();
            // Utils
            builder.Services.AddLocalization();
            builder.Services.AddTransient<ISharedStringLocalizer, SharedStringLocalizer>();
            builder.Services.AddTransient<IExportHelper, ExportHelper>();
            builder.Services.AddTransient<ICacheHelper, CacheHelper>();
            builder.Services.AddSingleton(AutoMapperConfiguration.Configure());
            builder.Services.AddTransient<IFileBytesGeneratorService, FileBytesGeneratorService>();
            builder.Services.AddTransient<ITimerPreferenceService, TimerPreferenceService>();            
            services.AddScoped<IContextConfiguration, ContextConfiguration>();
            // Logging
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            // Radzen
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            // BackgroundServices
            services.AddScoped<SecurityService>();
            return services;
        }
        private static DbContextOptionsBuilder AddTriggers(this DbContextOptionsBuilder dbContextBuilder)
        {
            ArgumentNullException.ThrowIfNull(dbContextBuilder);

            dbContextBuilder.UseTriggers(triggerOptions =>
            {
                triggerOptions.AddTrigger<AdjustInventoryFromNewAdjustmentDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromModifiedAdjustmentDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromDeletedAdjustmentDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromAdjustmentCancelled>();
                triggerOptions.AddTrigger<AdjustInventoryFromAdjustmentTypeChanged>();
                triggerOptions.AddTrigger<AdjustInventoryFromDeletedReservationDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromModifiedReservationDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromNewReservationDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromReservationCancelled>();
                triggerOptions.AddTrigger<AdjustInventoryFromReservationToOrder>();
                triggerOptions.AddTrigger<AdjustInventoryFromDeletedOrderDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromModifiedOrderDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromNewOrderDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromOrderCancelled>();
                triggerOptions.AddTrigger<AdjustInventoryFromOrderClosed>();
                triggerOptions.AddTrigger<AdjustCustomerOrderDetailFromDeletedOrderInProcessDetail>();
                triggerOptions.AddTrigger<AdjustCustomerOrderDetailFromModifiedOrderInProcessDetail>();
                triggerOptions.AddTrigger<AdjustCustomerOrderDetailFromNewOrderInProcessDetail>();
                triggerOptions.AddTrigger<AdjustCustomerOrderDetailFromOrderInProcessCancelled>();
                triggerOptions.AddTrigger<AdjustInventoryFromDeletedOrderInProcessDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromModifiedOrderInProcessDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromNewOrderInProcessDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromOrderInProcessCancelled>();
                triggerOptions.AddTrigger<ModifyCustomerOrderFromNewOrderInProcess>();
                triggerOptions.AddTrigger<ModifyCustomerOrderFromOrderInProcessCancelled>();
                triggerOptions.AddTrigger<AdjustCustomerOrderDetailFromDeletedOrderShipmentDetail>();
                triggerOptions.AddTrigger<AdjustCustomerOrderDetailFromModifiedOrderShipmentDetail>();
                triggerOptions.AddTrigger<AdjustCustomerOrderDetailFromNewOrderShipmentDetail>();
                triggerOptions.AddTrigger<AdjustCustomerOrderDetailFromOrderShipmentCancelled>();
                triggerOptions.AddTrigger<AdjustInventoryFromDeletedOrderShipmentDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromModifiedOrderShipmentDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromNewOrderShipmentDetail>();
                triggerOptions.AddTrigger<AdjustInventoryFromOrderShipmentCancelled>();
                triggerOptions.AddTrigger<ModifyCustomerOrderFromNewOrderShipment>();
                triggerOptions.AddTrigger<ModifyCustomerOrderFromOrderShipmentCancelled>();
                triggerOptions.AddTrigger<AdjustInventoryFromConfirmedPurchaseOrder>();
                triggerOptions.AddTrigger<AdjustWarehouseFromDeletedWarehouseTransferDetail>();
                triggerOptions.AddTrigger<AdjustWarehouseFromModifiedWarehouseTransferDetail>();
                triggerOptions.AddTrigger<AdjustWarehouseFromNewWarehouseTransferDetail>();
                triggerOptions.AddTrigger<AlarmWarehouseFromNewWarehouseTransfer>();
            });
            return dbContextBuilder;
        }
        public static IServiceCollection AddInfraestructure(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            var services = builder.Services;

            // Repositories
            #region Repositories
            services.AddTransient<IActivityTypeRepository, ActivityTypeRepository>();
            services.AddTransient<IActivityTypesAreaRepository, ActivityTypesAreaRepository>();
            services.AddTransient<IAdjustmentRepository, AdjustmentRepository>();
            services.AddTransient<IAdjustmentDetailRepository, AdjustmentDetailRepository>();
            services.AddTransient<IAdjustmentReasonRepository, AdjustmentReasonRepository>();
            services.AddTransient<IAdjustmentTypeRepository, AdjustmentTypeRepository>();
            services.AddTransient<IAlarmRepository, AlarmRepository>();
            services.AddTransient<IAlarmMessageRepository, AlarmMessageRepository>();
            services.AddTransient<IAlarmTypeRepository, AlarmTypeRepository>();
            services.AddTransient<IAreaRepository, AreaRepository>();
            services.AddTransient<ICancellationReasonRepository, CancellationReasonRepository>();
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<ICloseCustomerOrderReasonRepository, CloseCustomerOrderReasonRepository>();
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<ICurrencyRepository, CurrencyRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ICustomerContactRepository, CustomerContactRepository>();
            services.AddTransient<ICustomerOrderRepository, CustomerOrderRepository>();
            services.AddTransient<ICustomerOrderActivityRepository, CustomerOrderActivityRepository>();
            services.AddTransient<ICustomerOrderActivityDetailRepository, CustomerOrderActivityDetailRepository>();
            services.AddTransient<ICustomerOrderDetailRepository, CustomerOrderDetailRepository>();
            services.AddTransient<ICustomerOrderInProcessDetailRepository, CustomerOrderInProcessDetailRepository>();
            services.AddTransient<ICustomerOrderShipmentRepository, CustomerOrderShipmentRepository>();
            services.AddTransient<ICustomerOrderShipmentDetailRepository, CustomerOrderShipmentDetailRepository>();
            services.AddTransient<ICustomerOrdersInProcessRepository, CustomerOrdersInProcessRepository>();
            services.AddTransient<ICustomerReservationRepository, CustomerReservationRepository>();
            services.AddTransient<ICustomerReservationDetailRepository, CustomerReservationDetailRepository>();
            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IDocumentTypeRepository, DocumentTypeRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IForwarderRepository, ForwarderRepository>();
            services.AddTransient<IForwarderAgentRepository, ForwarderAgentRepository>();
            services.AddTransient<IIdentityTypeRepository, IdentityTypeRepository>();
            services.AddTransient<IItemRepository, ItemRepository>();
            services.AddTransient<IItemReferenceRepository, ItemReferenceRepository>();
            services.AddTransient<IItemAreaRepository, ItemAreaRepository>();
            services.AddTransient<ILineRepository, LineRepository>();
            services.AddTransient<IMeasureUnitRepository, MeasureUnitRepository>();
            services.AddTransient<IModificationReasonRepository, ModificationReasonRepository>();
            services.AddTransient<IProcessSatelliteRepository, ProcessSatelliteRepository>();
            services.AddTransient<IProviderRepository, ProviderRepository>();
            services.AddTransient<IProviderReferenceRepository, ProviderReferenceRepository>();
            services.AddTransient<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddTransient<IPurchaseOrderActivityRepository, PurchaseOrderActivityRepository>();
            services.AddTransient<IPurchaseOrderDetailRepository, PurchaseOrderDetailRepository>();
            services.AddTransient<IReferencesWarehouseRepository, ReferencesWarehouseRepository>();
            services.AddTransient<IShipmentForwarderAgentMethodRepository, ShipmentForwarderAgentMethodRepository>();
            services.AddTransient<IShipmentMethodRepository, ShipmentMethodRepository>();
            services.AddTransient<IShippingMethodRepository, ShippingMethodRepository>();
            services.AddTransient<IStatusDocumentTypeRepository, StatusDocumentTypeRepository>();
            services.AddTransient<IUsersAlarmTypeRepository, UsersAlarmTypeRepository>();
            services.AddTransient<IVisualizedAlarmRepository, VisualizedAlarmRepository>();
            services.AddTransient<IWarehouseRepository, WarehouseRepository>();
            services.AddTransient<IWarehouseTransferRepository, WarehouseTransferRepository>();
            services.AddTransient<IWarehouseTransferDetailRepository, WarehouseTransferDetailRepository>();
            services.AddTransient<INotificationProviderSettingsRepository, NotificationProviderSettingsRepository>();
            services.AddTransient<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddTransient<IInventoryAdjustmentReportRepository, InventoryAdjustmentReportRepository>();
            services.AddTransient<IInProcessInventoryReportRepository, InProcessInventoryReportRepository>();
            services.AddTransient<IInventoryReportRepository, InventoryReportRepository>();
            services.AddTransient<IProviderReferenceReportRepository, ProviderReferenceReportRepository>();
            services.AddTransient<IReferenceMovementReportRepository, ReferenceMovementReportRepository>();
            services.AddTransient<IWarehouseStockReportRepository, WarehouseStockReportRepository>();
            services.AddTransient<ICustomerOrderReportRepository, CustomerOrderReportRepository>();
            services.AddTransient<ICustomerReservationReportRepository, CustomerReservationReportRepository>();
            services.AddTransient<IPurchaseOrderReportRepository, PurchaseOrderReportRepository>();
            services.AddTransient<ICustomerOrderActivityReportRepository, CustomerOrderActivityReportRepository>();
            services.AddTransient<IWarehouseTransferReportRepository, WarehouseTransferReportRepository>();
            services.AddTransient<IFreezoneVsAvailableReportRepository, FreezoneVsAvailableReportRepository>();
            services.AddTransient<ICustomerSaleReportRepository, CustomerSaleReportRepository>();
            services.AddTransient<IEmailNotificationProviderSettingsRepository, EmailNotificationProviderSettingsRepository>();
            services.AddTransient<IDashBoardRepository, DashBoardRepository>();
            services.AddTransient<IPurchaseOrderNotificationRepository, PurchaseOrderNotificationRepository>();
            services.AddTransient<IVisualizedPurchaseOrderTransitAlarmRepository, VisualizedPurchaseOrderTransitAlarmRepository>();
            services.AddTransient<IPurchaseOrderTransitAlarmRepository, PurchaseOrderTransitAlarmRepository>();
            services.AddTransient<ICustomerOrderNotificationRepository, CustomerOrderNotificationRepository>();
            services.AddTransient<ICustomerReservationNotificationRepository, CustomerReservationNotificationRepository>();
            services.AddTransient<IPackagingRepository, PackagingRepository>();
            services.AddTransient<IVisualizedMinimumQuantityAlarmRepository, VisualizedMinimumQuantityAlarmRepository>();
            services.AddTransient<IVisualizedOutOfStockInventoryAlarmRepository, VisualizedOutOfStockInventoryAlarmRepository>();
            services.AddTransient<IVisualizedMinimumLocalWarehouseQuantityAlarmRepository, VisualizedMinimumLocalWarehouseQuantityAlarmRepository>();
            services.AddTransient<ICancellationRequestRepository, CancellationRequestRepository>();
            services.AddTransient<IVisualizedLocalWarehouseAlarmRepository, VisualizedLocalWarehouseAlarmRepository>();
            services.AddTransient<IVisualizedAutomaticInProcessAlarmRepository, VisualizedAutomaticInProcessAlarmRepository>();
            services.AddTransient<IAutomaticPurchaseOrderAssigmentReportRepository, AutomaticPurchaseOrderAssigmentReportRepository>();

            #endregion
            // Services
            #region Services
            services.AddTransient<IActivityTypeService, ActivityTypeService>();
            services.AddTransient<IActivityTypesAreaService, ActivityTypesAreaService>();
            services.AddTransient<IAdjustmentService, AdjustmentService>();
            services.AddTransient<IAdjustmentDetailService, AdjustmentDetailService>();
            services.AddTransient<IAdjustmentReasonService, AdjustmentReasonService>();
            services.AddTransient<IAdjustmentTypeService, AdjustmentTypeService>();
            services.AddTransient<IAlarmService, AlarmService>();
            services.AddTransient<IAlarmMessageService, AlarmMessageService>();
            services.AddTransient<IAlarmTypeService, AlarmTypeService>();
            services.AddTransient<IAreaService, AreaService>();
            services.AddTransient<ICancellationReasonService, CancellationReasonService>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<ICloseCustomerOrderReasonService, CloseCustomerOrderReasonService>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<ICurrencyService, CurrencyService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomerContactService, CustomerContactService>();
            services.AddTransient<ICustomerOrderService, CustomerOrderService>();
            services.AddTransient<ICustomerOrderActivityService, CustomerOrderActivityService>();
            services.AddTransient<ICustomerOrderActivityDetailService, CustomerOrderActivityDetailService>();
            services.AddTransient<ICustomerOrderDetailService, CustomerOrderDetailService>();
            services.AddTransient<ICustomerOrderInProcessDetailService, CustomerOrderInProcessDetailService>();
            services.AddTransient<ICustomerOrderShipmentService, CustomerOrderShipmentService>();
            services.AddTransient<ICustomerOrderShipmentDetailService, CustomerOrderShipmentDetailService>();
            services.AddTransient<ICustomerOrdersInProcessService, CustomerOrdersInProcessService>();
            services.AddTransient<ICustomerReservationService, CustomerReservationService>();
            services.AddTransient<ICustomerReservationDetailService, CustomerReservationDetailService>();
            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IDocumentTypeService, DocumentTypeService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IForwarderService, ForwarderService>();
            services.AddTransient<IForwarderAgentService, ForwarderAgentService>();
            services.AddTransient<IIdentityTypeService, IdentityTypeService>();
            services.AddTransient<IItemService, ItemService>();
            services.AddTransient<IItemReferenceService, ItemReferenceService>();
            services.AddTransient<IItemAreaService, ItemAreaService>();
            services.AddTransient<ILineService, LineService>();
            services.AddTransient<IMeasureUnitService, MeasureUnitService>();
            services.AddTransient<IModificationReasonService, ModificationReasonService>();
            services.AddTransient<IProcessSatelliteService, ProcessSatelliteService>();
            services.AddTransient<IProviderService, ProviderService>();
            services.AddTransient<IProviderReferenceService, ProviderReferenceService>();
            services.AddTransient<IPurchaseOrderService, PurchaseOrderService>();
            services.AddTransient<IPurchaseOrderActivityService, PurchaseOrderActivityService>();
            services.AddTransient<IPurchaseOrderDetailService, PurchaseOrderDetailService>();
            services.AddTransient<IReferencesWarehouseService, ReferencesWarehouseService>();
            services.AddTransient<IShipmentForwarderAgentMethodService, ShipmentForwarderAgentMethodService>();
            services.AddTransient<IShipmentMethodService, ShipmentMethodService>();
            services.AddTransient<IShippingMethodService, ShippingMethodService>();
            services.AddTransient<IStatusDocumentTypeService, StatusDocumentTypeService>();
            services.AddTransient<IUsersAlarmTypeService, UsersAlarmTypeService>();
            services.AddTransient<IVisualizedAlarmService, VisualizedAlarmService>();
            services.AddTransient<IWarehouseService, WarehouseService>();
            services.AddTransient<IWarehouseTransferService, WarehouseTransferService>();
            services.AddTransient<IWarehouseTransferDetailService, WarehouseTransferDetailService>();
            services.AddTransient<IInventoryAdjustmentReportService, InventoryAdjustmentReportService>();
            services.AddTransient<IInProcessInventoryReportService, InProcessInventoryReportService>();
            services.AddTransient<IInventoryReportService, InventoryReportService>();
            services.AddTransient<IProviderReferenceReportService, ProviderReferenceReportService>();
            services.AddTransient<IReferenceMovementReportService, ReferenceMovementReportService>();
            services.AddTransient<IWarehouseStockReportService, WarehouseStockReportService>();
            services.AddTransient<ICustomerOrderReportService, CustomerOrderReportService>();
            services.AddTransient<ICustomerReservationReportService, CustomerReservationReportService>();
            services.AddTransient<IPurchaseOrderReportService, PurchaseOrderReportService>();
            services.AddTransient<ICustomerOrderActivityReportService, CustomerOrderActivityReportService>();
            services.AddTransient<IWarehouseTransferReportService, WarehouseTransferReportService>();
            services.AddTransient<IFreezoneVsAvailableReportService, FreezoneVsAvailableReportService>();
            services.AddTransient<ICustomerSaleReportService, CustomerSaleReportService>();
            services.AddTransient<IEmailNotificationProviderSettingsService, EmailNotificationProviderSettingsService>();
            services.AddTransient<INotificationTemplateService, NotificationTemplateService>();
            services.AddTransient<IDashBoardService, DashBoardService>();
            services.AddTransient<IPurchaseOrderNotificationService, PurchaseOrderNotificationService>();
            services.AddTransient<IVisualizedPurchaseOrderTransitAlarmService, VisualizedPurchaseOrderTransitAlarmService>();
            services.AddTransient<IPurchaseOrderTransitAlarmService, PurchaseOrderTransitAlarmService>();
            services.AddTransient<ICustomerOrderNotificationService, CustomerOrderNotificationService>();
            services.AddTransient<ICustomerReservationNotificationService, CustomerReservationNotificationService>();
            services.AddTransient<IPackagingService,PackagingService>();
            services.AddTransient<IVisualizedMinimumQuantityAlarmService, VisualizedMinimumQuantityAlarmService>();
            services.AddTransient<IVisualizedOutOfStockInventoryAlarmService, VisualizedOutOfStockInventoryAlarmService>();
            services.AddTransient<IVisualizedMinimumLocalWarehouseQuantityAlarmService, VisualizedMinimumLocalWarehouseQuantityAlarmService>();
            services.AddTransient<ICancellationRequestService, CancellationRequestService>();
            services.AddTransient<IVisualizedLocalWarehouseAlarmService, VisualizedLocalWarehouseAlarmService>();
            services.AddTransient<IVisualizedAutomaticInProcessAlarmService, VisualizedAutomaticInProcessAlarmService>();
            services.AddTransient<IAutomaticPurchaseOrderAssigmentReportService, AutomaticPurchaseOrderAssigmentReportService>();

            #endregion

            services.AddTransient<IQueue, RabbitQueue>();
            services.AddTransient<IQueueSettings, QueueSettings>();
            services.AddTransient<Notificator.INotificationService, Notificator.NotificationService>();
            services.AddTransient<IFtpClient, FtpClient>();

            return services;
        }
    }
}
