using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Pages.DashboardNotificationComponents;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Radzen;
using System.Configuration;

namespace Aldebaran.Web.Pages
{
    public partial class Index : IDisposable
    {
        #region Injections
        [Inject]
        protected ILogger<Index> Logger { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public IDashBoardService DashBoardService { get; set; }

        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        #endregion
                
        #region Global Variables
        protected StatusDocumentType pendingStatusOrder;
        List<DataTimer> Timers;
        readonly GridTimer GridTimer = new GridTimer("Dahsboard-GridTimer");
        private MemoryCacheEntryOptions _cacheEntryOptions;
        protected bool isLoadingInProgress;
        
        protected bool minimumQuantityNotificatioVisible;
        protected bool minimumLocalWarehouseQuantityNotificatioVisible;
        protected bool userAlarmNotificationVisible;
        protected bool outOfStockNotificatioVisible;
        protected bool expiredReservationNotificationVisible;
        protected bool customerOrderExpirationNotificationVisible;
        protected bool purchaseOrderExpirationNotificationVisible;
        protected bool purchaseOrderTransitAlarmNotificationsVisible;
        protected bool emailErrorNotificationsVisible;

        protected bool minimumQuantityAlertVisible;
        protected bool minimumLocalWarehouseQuantityAlertVisible;
        protected bool outOfStockAlertVisible;
        protected bool userAlarmAlertVisible;
        protected bool expiredReservationAlertVisible;
        protected bool customerOrderExpirationAlertVisible;
        protected bool purchaseOrderExpirationAlertVisible;
        protected bool purchaseOrderTransitAlertVisible;
        protected bool emailErrorAlertVisible;

        protected MinimumQuantityNotifications minimumQuantityNotifications;
        protected MinimumLocalWarehouseQuantityNotifications minimumLocalWarehouseQuantityNotifications;
        protected UserAlarmNotifications userAlarmNotifications;
        protected OutOfStockNotifications outOfStockNotifications;
        protected ExpiredReservationNotifications expiredReservationNotifications;
        protected CustomerOrderExpirationNotifications customerOrderExpirationNotifications;
        protected PurchaseOrderExpirationNotifications purchaseOrderExpirationNotifications;
        protected PurchaseOrderTransitAlarmNotifications purchaseOrderTransitAlarmNotifications;
        protected EmailWithErrorNotifications emailWithErrorNotifications;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await SetPermissions();
                isLoadingInProgress = true;
                Timers = TimerPreferenceService.Timers;
                await InitializeGridTimers();
                pendingStatusOrder = await DashBoardService.FindByDocumentAndOrderAsync((await DashBoardService.FindByCodeAsync("O")).DocumentTypeId, 1);
                _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }
        #endregion

        #region Events
        protected async Task SetPermissions()
        {
            minimumQuantityNotificatioVisible = Security.IsInRole("Administrador", "Consulta de notificaciones por cantidades mínimas");
            minimumLocalWarehouseQuantityNotificatioVisible = Security.IsInRole("Administrador", "Consulta de notificaciones por cantidad mínima bodega local");
            userAlarmNotificationVisible = Security.IsInRole("Administrador", "Consulta de notificaciones por alarmas del día");
            outOfStockNotificatioVisible = Security.IsInRole("Administrador", "Consulta de notificaciones por artículos sin disponible");
            //expiredReservationNotificationVisible = Security.IsInRole("Administrador", "Consulta de notificaciones por reservas vencidas");
            //customerOrderExpirationNotificationVisible = Security.IsInRole("Administrador", "Consulta de notificaciones por pedidos vencidos");
            //purchaseOrderExpirationNotificationVisible = Security.IsInRole("Administrador", "Consulta de notificaciones por órdenes próximas a su vencimiento");
            //purchaseOrderTransitAlarmNotificationsVisible = Security.IsInRole("Administrador", "Consulta de notificaciones por alarmas de órdenes modificadas con afectación en pedido");
            //emailErrorNotificationsVisible = Security.IsInRole("Administrador", "Consulta de notificaciones por envio de correo con error");
        }

        async Task InitializeGridTimers()
        {
            await GridTimer.InitializeTimer(TimerPreferenceService.GetTimerPreferences(GridTimer.Key), async (sender, e) =>
            {
                await InvokeAsync(async () =>
                {
                    GridTimer.IsLoading = true;
                    try
                    {
                        StateHasChanged();
                        await GridData_Update();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Unable to update data for Dashboard Data");
                        NotificationService.Notify(new NotificationMessage
                        {
                            Summary = "Actualización de información",
                            Severity = NotificationSeverity.Error,
                            Detail = $"No se ha podido actualizar la información, favor intente manualmente."
                        });
                    }
                    finally
                    {
                        GridTimer.IsLoading = false;
                        StateHasChanged();
                    }
                });
            });
        }

        private async Task GridData_Update()
        {
            GridTimer.LastUpdate = DateTime.Now;
            Console.WriteLine($"{GridTimer.LastUpdate}");
            if (minimumQuantityNotificatioVisible) await minimumQuantityNotifications.Update();
            if (minimumLocalWarehouseQuantityNotificatioVisible) await minimumLocalWarehouseQuantityNotifications.Update();
            if (userAlarmNotificationVisible) await userAlarmNotifications.Update();
            if (outOfStockNotificatioVisible) await outOfStockNotifications.Update();
            if (expiredReservationNotificationVisible) await expiredReservationNotifications.Update();
            if (customerOrderExpirationNotificationVisible) await customerOrderExpirationNotifications.Update();
            if (purchaseOrderExpirationNotificationVisible) await purchaseOrderExpirationNotifications.Update();
            if (purchaseOrderTransitAlarmNotificationsVisible) await purchaseOrderTransitAlarmNotifications.Update();
            if (emailErrorNotificationsVisible) await emailWithErrorNotifications.Update();
        }
        protected async Task GridaData_UpdateOnTimerChange(object value)
        {
            var milliseconds = (double)value;
            GridTimer.UpdateTimerInterval(milliseconds);
            TimerPreferenceService.UpdateTimerPreferences(GridTimer.Key, milliseconds);
        }

        public void Dispose()
        {
            GridTimer.Dispose();
        }
        #endregion               

        protected async Task AlertClick(int idObject)
        {
            switch (idObject)
            {
                case 1:
                    minimumQuantityAlertVisible = false;
                    break;
                case 2:
                    outOfStockAlertVisible = false;
                    break;
                case 3:
                    userAlarmAlertVisible = false;
                    break;
                case 4:
                    expiredReservationAlertVisible = false;
                    break;
                case 5:
                    customerOrderExpirationAlertVisible = false;
                    break;
                case 6:
                    purchaseOrderExpirationAlertVisible = false;
                    break;
                case 7:
                    purchaseOrderTransitAlertVisible = false;
                    break;
                case 8:
                    emailErrorAlertVisible = false;
                    break;
                case 9:
                    minimumLocalWarehouseQuantityAlertVisible = false;
                    break;
                default:
                    break;
            }
        }

        protected async Task UpdateAlert((int idObject, bool value) data) 
        {
            var idObject = data.idObject;
            var value = data.value;

            switch (idObject)
            {
                case 1:
                    minimumQuantityAlertVisible = value;
                    break;
                case 2:
                    outOfStockAlertVisible = value;
                    break;
                case 3:
                    userAlarmAlertVisible = value;
                    break;
                case 4:
                    expiredReservationAlertVisible = value;
                    break;
                case 5:
                    customerOrderExpirationAlertVisible = value;
                    break;
                case 6:
                    purchaseOrderExpirationAlertVisible = value;
                    break;
                case 7:
                    purchaseOrderTransitAlertVisible = value;
                    break;
                case 8:
                    emailErrorAlertVisible = value;
                    break;
                case 9:
                    minimumLocalWarehouseQuantityAlertVisible = value;
                    break;
                default:
                    break;
            }
        }
    }
}