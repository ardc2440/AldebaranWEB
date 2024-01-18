using Aldebaran.Application.Services;
using Aldebaran.DataAccess.Core.Triggers.Adjustments;
using Aldebaran.DataAccess.Core.Triggers.OrderInProcesses;
using Aldebaran.DataAccess.Core.Triggers.Orders;
using Aldebaran.DataAccess.Core.Triggers.Purchases;
using Aldebaran.DataAccess.Core.Triggers.Reservations;
using Aldebaran.DataAccess.Core.Triggers.Shipments;
using Aldebaran.DataAccess.Core.Triggers.Transfers;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Aldebaran.Web.Data;
using Aldebaran.Web.Models;
using Aldebaran.Web.Resources;
using Aldebaran.Web.Settings;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Radzen;

namespace Aldebaran.Web.Extensions
{
    public static class ArchitectureBuilderExtensions
    {
        public static IServiceCollection AddArchitecture(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            var services = builder.Services;
            var configuration = builder.Configuration;

            // Add services to the container.
            services.AddRazorPages();
            services.AddServerSideBlazor().AddHubOptions(o =>
            {
                o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
            });
            // Data context
            services.AddDbContext<DataAccess.AldebaranDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("AldebaranDbConnection")).AddTriggers();
            }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            // Identity context
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AldebaranDbConnection"));
            }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();
            builder.Services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();
            // Configuration
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
            services.AddHttpClient("Aldebaran.Web").AddHeaderPropagation(o => o.Headers.Add("Cookie"));
            services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
            // Auth
            services.AddAuthentication();
            services.AddAuthorization();
            // Utils
            builder.Services.AddLocalization();
            builder.Services.AddTransient<ISharedStringLocalizer, SharedStringLocalizer>();
            builder.Services.AddTransient<IExportHelper, ExportHelper>();
            builder.Services.AddSingleton(AutoMapperConfiguration.Configure());
            // Logging
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            // Radzen
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            // Temp
            services.AddDbContext<AldebaranDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("AldebaranDbConnection"));
            }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            services.AddScoped<AldebaranDbService>();

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
                triggerOptions.AddTrigger<AdjustWarehouseFromWarehousesModified>();
                triggerOptions.AddTrigger<AdjustWarehouseFromWarehouseTransferCancelled>();
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
            services.AddTransient<ICanceledCustomerOrderRepository, CanceledCustomerOrderRepository>();
            services.AddTransient<ICanceledCustomerReservationRepository, CanceledCustomerReservationRepository>();
            services.AddTransient<ICanceledOrderShipmentRepository, CanceledOrderShipmentRepository>();
            services.AddTransient<ICanceledOrdersInProcessRepository, CanceledOrdersInProcessRepository>();
            services.AddTransient<ICanceledPurchaseOrderRepository, CanceledPurchaseOrderRepository>();
            services.AddTransient<ICancellationReasonRepository, CancellationReasonRepository>();
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<ICloseCustomerOrderReasonRepository, CloseCustomerOrderReasonRepository>();
            services.AddTransient<IClosedCustomerOrderRepository, ClosedCustomerOrderRepository>();
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
            services.AddTransient<IModifiedCustomerOrderRepository, ModifiedCustomerOrderRepository>();
            services.AddTransient<IModifiedCustomerReservationRepository, ModifiedCustomerReservationRepository>();
            services.AddTransient<IModifiedOrderShipmentRepository, ModifiedOrderShipmentRepository>();
            services.AddTransient<IModifiedOrdersInProcessRepository, ModifiedOrdersInProcessRepository>();
            services.AddTransient<IModifiedPurchaseOrderRepository, ModifiedPurchaseOrderRepository>();
            services.AddTransient<IPackagingRepository, PackagingRepository>();
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
            services.AddTransient<ICanceledCustomerOrderService, CanceledCustomerOrderService>();
            services.AddTransient<ICanceledCustomerReservationService, CanceledCustomerReservationService>();
            services.AddTransient<ICanceledOrderShipmentService, CanceledOrderShipmentService>();
            services.AddTransient<ICanceledOrdersInProcessService, CanceledOrdersInProcessService>();
            services.AddTransient<ICanceledPurchaseOrderService, CanceledPurchaseOrderService>();
            services.AddTransient<ICancellationReasonService, CancellationReasonService>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<ICloseCustomerOrderReasonService, CloseCustomerOrderReasonService>();
            services.AddTransient<IClosedCustomerOrderService, ClosedCustomerOrderService>();
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
            services.AddTransient<IModifiedCustomerOrderService, ModifiedCustomerOrderService>();
            services.AddTransient<IModifiedCustomerReservationService, ModifiedCustomerReservationService>();
            services.AddTransient<IModifiedOrderShipmentService, ModifiedOrderShipmentService>();
            services.AddTransient<IModifiedOrdersInProcessService, ModifiedOrdersInProcessService>();
            services.AddTransient<IModifiedPurchaseOrderService, ModifiedPurchaseOrderService>();
            services.AddTransient<IPackagingService, PackagingService>();
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
            #endregion

            return services;
        }
    }
}
