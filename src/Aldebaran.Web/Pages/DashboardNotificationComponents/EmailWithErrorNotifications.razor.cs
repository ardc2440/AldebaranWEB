using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Infraestructure.Common.Extensions;
using Aldebaran.Web.Pages.CustomerOrderPages;
using Aldebaran.Web.Pages.CustomerReservationPages;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Radzen;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class EmailWithErrorNotifications
    {
        #region Injections       

        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IMemoryCache MemoryCache { get; set; }
        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }
        [Inject]
        protected ILogger<Index> Logger { get; set; }
        [Inject]
        protected ICacheHelper CacheHelper { get; set; }
        [Inject]
        protected IDashBoardService DashBoardService { get; set; }
        [Inject]
        protected IPurchaseOrderNotificationService PurchaseOrderNotificationService { get; set; }
        [Inject]
        protected ICustomerOrderNotificationService CustomerOrderNotificationService { get; set; }
        [Inject]
        protected ICustomerReservationNotificationService CustomerReservationNotificationService { get; set; }

        private static MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };

        #endregion

        #region properties

        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool emailNotificationsAlertVisible = false;
        protected int pageSize = 7;
        readonly GridTimer GridTimer = new GridTimer("EmailErrors-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";

        protected IEnumerable<NotificationWithError> notificationsWithError = new List<NotificationWithError>();
        protected LocalizedDataGrid<NotificationWithError> notificationsWithErrorGrid;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Timers = TimerPreferenceService.Timers;
                await InitializeGridTimers();
                await GridData_Update();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }
        #endregion

        #region Events

        #region Timer

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
        #endregion

        #region Cache

        protected string GetCacheKey(string key)
        {
            return $"{Security.User.Id}-{key}";
        }

        public async Task<List<T>> GetCache<T>(string key) where T : class
        {
            var loggedUserCache = GetCacheKey(key);
            if (!MemoryCache.TryGetValue(loggedUserCache, out List<T> list))
            {
                MemoryCache.Set(loggedUserCache, new List<T>(), _cacheEntryOptions);
                return new List<T>();
            }
            return list ?? new List<T>();
        }

        public async Task UpdateCache<T>(string key, List<T> list) where T : class
        {
            MemoryCache.Set(GetCacheKey(key), list, _cacheEntryOptions);
        }

        #endregion

        public async Task Update()
        {
            await GridData_Update();
        }

        private async Task GridData_Update()
        {
            try
            {
                isLoadingInProgress = true;
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
                await UpdateMailErrorNotificationsAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
            StateHasChanged();
        }

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await notificationsWithErrorGrid.GoToPage(0);

            await GridData_Update();
        }

        protected async Task UpdateMailErrorNotificationsAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<NotificationWithError>("NotificationWithError");

            notificationsWithError = (await DashBoardService.GetNotificationsWithError(search, ct)).ToList();
            emailNotificationsAlertVisible = !notificationsWithError.OrderBy(o => o.NotificationDate).ToList().IsEqual<NotificationWithError>(originalData.OrderBy(o => o.NotificationDate).ToList());
            await UpdateCache<NotificationWithError>("NotificationWithError", notificationsWithError.ToList());
            if (notificationsWithErrorGrid != null)
                await notificationsWithErrorGrid.Reload();
        }

        protected async Task ReSendMail(short EmailType, int EmailId, CancellationToken ct = default)
        {
            var alertVisible = emailNotificationsAlertVisible;

            if (await DialogService.Confirm("Desea reenviar la notificación seleccionada?. ", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Marcar alarma leída") == true)
            {
                await ResendEmailFromError(EmailType, EmailId, NavigationManager.BaseUri, ct);
                await UpdateMailErrorNotificationsAsync(ct);
                emailNotificationsAlertVisible = alertVisible;
            }
        }

        private void HandleBoolChange(bool newValue)
        {
            emailNotificationsAlertVisible = newValue;
        }

        #region EmailResernd

        internal async Task ResendEmailFromError(short emailType, int emailId, string baseUri, CancellationToken ct = default)
        {
            switch (emailType)
            {
                case 0: await SendPurchaseOrderEmail(emailId, baseUri, ct); break;
                case 1: await SendCustomerOrderEmail(emailId, ct); break;
                case 2: await SendCustomerReservationEmail(emailId, ct); break;
                default: break;
            }
        }

        internal async Task SendPurchaseOrderEmail(int emailId, string baseUri, CancellationToken ct = default)
        {
            var purchaseOrderNotification = await PurchaseOrderNotificationService.FindAsync(emailId, ct) ?? throw new Exception("No se encontró informacion pra e elnvio del correo");
            await PurchaseOrderNotificationService.AddAsync(new PurchaseOrderNotification { 
                                                                    CustomerOrderId= purchaseOrderNotification.CustomerOrderId,
                                                                    ModifiedPurchaseOrderId= purchaseOrderNotification.ModifiedPurchaseOrderId,
                                                                    NotificationState= NotificationStatus.Pending,
                                                                    NotifiedMailList=purchaseOrderNotification.NotifiedMailList,
                                                                    NotificationDate= DateTime.Now,
                                                                    },ct);

            await PurchaseOrderNotificationService.NotifyToCustomers(purchaseOrderNotification.ModifiedPurchaseOrderId, baseUri, ct);
            await PurchaseOrderNotificationService.UpdateAsync(emailId, purchaseOrderNotification.NotificationId, NotificationStatus.ReSend, ct);
        }

        internal async Task SendCustomerOrderEmail(int emailId, CancellationToken ct = default)
        {
            var customerOrderNotification = await CustomerOrderNotificationService.FindAsync(emailId, ct);
            var customerOrder = customerOrderNotification.CustomerOrder;
            var templateCode = customerOrderNotification.NotificationTemplate.Name ;

            var result = await DialogService.OpenAsync<CustomerOrderSummary>(null, new Dictionary<string, object> { { "Id", customerOrder.CustomerOrderId }, { "NotificationTemplateName", templateCode } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
            if ((bool)result)
            {
                await CustomerOrderNotificationService.UpdateAsync(customerOrderNotification.NotificationId, NotificationStatus.ReSend, "", DateTime.Now, ct);
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Notificación",
                    Detail = $"El reenvio del correo se realizó con exito."
                });
            }
        }

        internal async Task SendCustomerReservationEmail(int emailId, CancellationToken ct = default)
        {
            var customerReservationNotification = await CustomerReservationNotificationService.FindAsync(emailId, ct);
            var customerReservation = customerReservationNotification.CustomerReservation;
            var templateCode = customerReservationNotification.NotificationTemplate.Name;

            var result = await DialogService.OpenAsync<CustomerReservationSummary>(null, new Dictionary<string, object> { { "Id", customerReservation.CustomerReservationId }, { "NotificationTemplateName", templateCode } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
            if ((bool)result)
            {
                await CustomerReservationNotificationService.UpdateAsync(customerReservationNotification.NotificationId, NotificationStatus.ReSend, "", DateTime.Now, ct);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Notificación",
                    Detail = $"El reenvio del correo se realizó con exito."
                });
            }
        }

        #endregion

        #endregion
    }
}
