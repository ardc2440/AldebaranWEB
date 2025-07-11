using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Reports;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;
using Enums = Aldebaran.DataAccess.Enums;

namespace Aldebaran.Application.Services.Mappings
{
    public class ApplicationServicesProfile : Profile
    {
        public ApplicationServicesProfile()
        {
            CreateMap<ActivityType, Entities.ActivityType>().ReverseMap();
            CreateMap<ActivityTypesArea, Entities.ActivityTypesArea>().ReverseMap();
            CreateMap<Adjustment, Entities.Adjustment>().ReverseMap();
            CreateMap<AdjustmentDetail, Entities.AdjustmentDetail>().ReverseMap();
            CreateMap<AdjustmentReason, Entities.AdjustmentReason>().ReverseMap();
            CreateMap<AdjustmentType, Entities.AdjustmentType>().ReverseMap();
            CreateMap<Alarm, Entities.Alarm>().ReverseMap();
            CreateMap<AlarmMessage, Entities.AlarmMessage>().ReverseMap();
            CreateMap<AlarmType, Entities.AlarmType>().ReverseMap();
            CreateMap<Area, Entities.Area>().ReverseMap();
            CreateMap<CanceledCustomerOrder, Entities.CanceledCustomerOrder>().ReverseMap();
            CreateMap<CanceledCustomerReservation, Entities.CanceledCustomerReservation>().ReverseMap();
            CreateMap<CanceledOrderShipment, Entities.CanceledOrderShipment>().ReverseMap();
            CreateMap<CanceledOrdersInProcess, Entities.CanceledOrdersInProcess>().ReverseMap();
            CreateMap<CanceledPurchaseOrder, Entities.CanceledPurchaseOrder>().ReverseMap();
            CreateMap<CancellationReason, Entities.CancellationReason>().ReverseMap();
            CreateMap<City, Entities.City>().ReverseMap();
            CreateMap<CloseCustomerOrderReason, Entities.CloseCustomerOrderReason>().ReverseMap();
            CreateMap<ClosedCustomerOrder, Entities.ClosedCustomerOrder>().ReverseMap();
            CreateMap<Country, Entities.Country>().ReverseMap();
            CreateMap<Currency, Entities.Currency>().ReverseMap();
            CreateMap<Customer, Entities.Customer>().ReverseMap();
            CreateMap<CustomerContact, Entities.CustomerContact>().ReverseMap();
            CreateMap<CustomerOrder, Entities.CustomerOrder>().ReverseMap();
            CreateMap<CustomerOrderActivity, Entities.CustomerOrderActivity>().ReverseMap();
            CreateMap<CustomerOrderActivityDetail, Entities.CustomerOrderActivityDetail>().ReverseMap();
            CreateMap<CustomerOrderDetail, Entities.CustomerOrderDetail>().ReverseMap();
            CreateMap<CustomerOrderInProcessDetail, Entities.CustomerOrderInProcessDetail>().ReverseMap();
            CreateMap<CustomerOrderShipment, Entities.CustomerOrderShipment>().ReverseMap();
            CreateMap<CustomerOrderShipmentDetail, Entities.CustomerOrderShipmentDetail>().ReverseMap();
            CreateMap<CustomerOrdersInProcess, Entities.CustomerOrdersInProcess>().ReverseMap();
            CreateMap<CustomerReservation, Entities.CustomerReservation>().ReverseMap();
            CreateMap<CustomerReservationDetail, Entities.CustomerReservationDetail>().ReverseMap();
            CreateMap<Department, Entities.Department>().ReverseMap();
            CreateMap<DocumentType, Entities.DocumentType>().ReverseMap();
            CreateMap<Employee, Entities.Employee>().ReverseMap();
            CreateMap<Forwarder, Entities.Forwarder>().ReverseMap();
            CreateMap<ForwarderAgent, Entities.ForwarderAgent>().ReverseMap();
            CreateMap<IdentityType, Entities.IdentityType>().ReverseMap();
            CreateMap<Item, Entities.Item>().ReverseMap();
            CreateMap<ItemReference, Entities.ItemReference>()
                .ReverseMap()
                .ForMember(fm => fm.HavePurchaseOrderDetail, opt => opt.Ignore());
            CreateMap<ItemsArea, Entities.ItemsArea>().ReverseMap();
            CreateMap<Line, Entities.Line>().ReverseMap();
            CreateMap<MeasureUnit, Entities.MeasureUnit>().ReverseMap();
            CreateMap<ModificationReason, Entities.ModificationReason>().ReverseMap();
            CreateMap<ModifiedCustomerOrder, Entities.ModifiedCustomerOrder>().ReverseMap();
            CreateMap<ModifiedCustomerReservation, Entities.ModifiedCustomerReservation>().ReverseMap();
            CreateMap<ModifiedOrderShipment, Entities.ModifiedOrderShipment>().ReverseMap();
            CreateMap<ModifiedOrdersInProcess, Entities.ModifiedOrdersInProcess>().ReverseMap();
            CreateMap<ModifiedPurchaseOrder, Entities.ModifiedPurchaseOrder>().ReverseMap();
            CreateMap<Packaging, Entities.Packaging>().ReverseMap();
            CreateMap<ProcessSatellite, Entities.ProcessSatellite>().ReverseMap();
            CreateMap<Provider, Entities.Provider>().ReverseMap();
            CreateMap<ProviderReference, Entities.ProviderReference>().ReverseMap();
            CreateMap<PurchaseOrder, Entities.PurchaseOrder>().ReverseMap();
            CreateMap<PurchaseOrderActivity, Entities.PurchaseOrderActivity>().ReverseMap();
            CreateMap<PurchaseOrderDetail, Entities.PurchaseOrderDetail>().ReverseMap();
            CreateMap<ReferencesWarehouse, Entities.ReferencesWarehouse>().ReverseMap();
            CreateMap<ShipmentForwarderAgentMethod, Entities.ShipmentForwarderAgentMethod>().ReverseMap();
            CreateMap<ShipmentMethod, Entities.ShipmentMethod>().ReverseMap();
            CreateMap<ShippingMethod, Entities.ShippingMethod>().ReverseMap();
            CreateMap<StatusDocumentType, Entities.StatusDocumentType>().ReverseMap();
            CreateMap<UsersAlarmType, Entities.UsersAlarmType>().ReverseMap();
            CreateMap<VisualizedAlarm, Entities.VisualizedAlarm>().ReverseMap();
            CreateMap<Warehouse, Entities.Warehouse>().ReverseMap();
            CreateMap<WarehouseTransfer, Entities.WarehouseTransfer>().ReverseMap();
            CreateMap<WarehouseTransferDetail, Entities.WarehouseTransferDetail>().ReverseMap();
            CreateMap<Reason, DataAccess.Infraestructure.Models.Reason>().ReverseMap();
            CreateMap<InventoryAdjustmentReport, Entities.Reports.InventoryAdjustmentReport>().ReverseMap();
            CreateMap<InProcessInventoryReport, Entities.Reports.InProcessInventoryReport>().ReverseMap();
            CreateMap<InventoryReport, Entities.Reports.InventoryReport>().ReverseMap();
            CreateMap<ProviderReferenceReport, Entities.Reports.ProviderReferenceReport>().ReverseMap();
            CreateMap<ReferenceMovementReport, Entities.Reports.ReferenceMovementReport>().ReverseMap();
            CreateMap<WarehouseStockReport, Entities.Reports.WarehouseStockReport>().ReverseMap();
            CreateMap<CustomerOrderReport, Entities.Reports.CustomerOrderReport>().ReverseMap();
            CreateMap<CustomerReservationReport, Entities.Reports.CustomerReservationReport>().ReverseMap();
            CreateMap<PurchaseOrderReport, Entities.Reports.PurchaseOrderReport>().ReverseMap();
            CreateMap<CustomerOrderActivityReport, Entities.Reports.CustomerOrderActivityReport>().ReverseMap();
            CreateMap<WarehouseTransferReport, Entities.Reports.WarehouseTransferReport>().ReverseMap();
            CreateMap<FreezoneVsAvailableReport, Entities.Reports.FreezoneVsAvailableReport>().ReverseMap();
            CreateMap<CustomerSaleReport, Entities.Reports.CustomerSaleReport>().ReverseMap();
            CreateMap<CustomerOrderExport, Entities.Reports.CustomerOrderExport>().ReverseMap();
            CreateMap<AutomaticCustomerOrderAssigmentReport, Entities.Reports.AutomaticCustomerOrderAssigmentReport>().ReverseMap();
            CreateMap<EmailNotificationProvider, DataAccess.Infraestructure.Models.EmailNotificationProvider>().ReverseMap();
            CreateMap<NotificationTemplate, Entities.NotificationTemplate>().ReverseMap();
            CreateMap<PurchaseOrderNotification, Entities.PurchaseOrderNotification>().ReverseMap();
            CreateMap<CustomerOrderNotification, Entities.CustomerOrderNotification>().ReverseMap();
            CreateMap<CustomerReservationNotification, Entities.CustomerReservationNotification>().ReverseMap();
            CreateMap<CustomerOrderAffectedByPurchaseOrderUpdate, Entities.CustomerOrderAffectedByPurchaseOrderUpdate>().ReverseMap();
            CreateMap<PurchaseOrderTransitAlarm, Entities.PurchaseOrderTransitAlarm>().ReverseMap();
            CreateMap<VisualizedPurchaseOrderTransitAlarm, Entities.VisualizedPurchaseOrderTransitAlarm>().ReverseMap();
            CreateMap<NotificationStatus, Enums.NotificationStatus>().ReverseMap();
            CreateMap<NotificationWithError, Entities.NotificationWithError>().ReverseMap();
            CreateMap<PurchaseOrderVariation, Entities.PurchaseOrderVariation>().ReverseMap();
            CreateMap<MinimumQuantityArticle, Entities.MinimumQuantityArticle>().ReverseMap();
            CreateMap<OutOfStockArticle, Entities.OutOfStockArticle>().ReverseMap();
            CreateMap<VisualizedMinimumQuantityAlarm, Entities.VisualizedMinimumQuantityAlarm>().ReverseMap();
            CreateMap<VisualizedOutOfStockInventoryAlarm, Entities.VisualizedOutOfStockInventoryAlarm>().ReverseMap();
            CreateMap<MinimumLocalWarehouseQuantityArticle, Entities.MinimumLocalWarehouseQuantityArticle>().ReverseMap();
            CreateMap<VisualizedMinimumLocalWarehouseQuantityAlarm, Entities.VisualizedMinimumLocalWarehouseQuantityAlarm>().ReverseMap();
            CreateMap<CancellationRequest, Entities.CancellationRequest>().ReverseMap();
            CreateMap<CancellationRequestModel, Entities.CancellationRequestModel>().ReverseMap();
            CreateMap<VisualizedLocalWarehouseAlarm, Entities.VisualizedLocalWarehouseAlarm>().ReverseMap();
            CreateMap<LocalWarehouseAlarm, Entities.LocalWarehouseAlarm>().ReverseMap();
            CreateMap<ConfirmedPurchaseOrder, Entities.ConfirmedPurchaseOrder>().ReverseMap();    
            CreateMap<VisualizedAutomaticInProcess, Entities.VisualizedAutomaticInProcess>().ReverseMap();
            CreateMap<AutomaticCustomerOrder, Entities.AutomaticCustomerOrder>().ReverseMap();
            CreateMap<AutomaticCustomerOrderDetail, Entities.AutomaticCustomerOrderDetail>().ReverseMap();
        }
    }
}
