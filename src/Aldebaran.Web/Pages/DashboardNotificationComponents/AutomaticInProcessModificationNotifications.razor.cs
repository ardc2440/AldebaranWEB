using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Infraestructure.Common.Extensions;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Radzen;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class AutomaticInProcessModificationNotifications
    {
        #region Injections

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        private IMemoryCache MemoryCache { get; set; }

        private static readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };

        [Inject]
        public IDashBoardService DashBoardService { get; set; }

        [Inject]
        public IVisualizedAutomaticCustomerInProcessModificationService VisualizedAutomaticCustomerInProcessModificationService { get; set; }

        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }

        [Inject]
        public ICustomerOrdersInProcessService CustomerOrdersInProcessService { get; set; }

        [Inject]
        public ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected ILogger<Index> Logger { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICacheHelper CacheHelper { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters
        [Parameter] public EventCallback<(int, bool)> OnAlertVisibleChanged { get; set; }
        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool automaticModificationsAlertVisible = false;
        protected Employee employee;
        protected int pageSize = 10;
        readonly GridTimer GridTimer = new GridTimer("AutomaticModifications-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";

        private int currentPage = 1;
        protected IList<AutomaticCustomerOrderInProcessModification> selectedModifications = new List<AutomaticCustomerOrderInProcessModification>();
        protected IList<AutomaticCustomerOrderInProcessModification> visibleItems = new List<AutomaticCustomerOrderInProcessModification>();

        protected IEnumerable<AutomaticCustomerOrderInProcessModification> modifications = new List<AutomaticCustomerOrderInProcessModification>();
        protected LocalizedDataGrid<AutomaticCustomerOrderInProcessModification> modificationsGrid;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Timers = TimerPreferenceService.Timers;
                employee = await DashBoardService.FindByLoginUserIdAsync(Security.User.Id);
                await InitializeGridTimers();
                await GridData_Update();
                await LoadVisibleItems();
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
                        Logger.LogError(ex, "Unable to update data for Automatic Modifications");
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

        #region Data

        public async Task Update()
        {
            try
            {
                isLoadingInProgress = true;
                await GridData_Update();
                await LoadVisibleItems();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        private async Task GridData_Update()
        {
            try
            {
                isLoadingInProgress = true;
                selectedModifications = new List<AutomaticCustomerOrderInProcessModification>();
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
                await UpdateModificationsAsync();
                selectedModifications = new List<AutomaticCustomerOrderInProcessModification>();
                await LoadVisibleItems();
            }
            finally
            {
                isLoadingInProgress = false;
            }
            StateHasChanged();
        }

        async Task UpdateModificationsAsync(CancellationToken ct = default)
        {
            if (employee == null) return;

            var originalData = await GetCache<AutomaticCustomerOrderInProcessModification>("AutomaticCustomerOrderInProcessModification");
            modifications = await DashBoardService.GetModificatedAutomaticCustomerInProcessAlarmsAsync(employee.EmployeeId, search, ct);
            
            await AlertVisibleChange(!modifications.OrderBy(o => o.Order_Number).ToList().IsEqual<AutomaticCustomerOrderInProcessModification>(originalData.OrderBy(o => o.Order_Number).ToList()));
            await UpdateCache<AutomaticCustomerOrderInProcessModification>("AutomaticCustomerOrderInProcessModification", modifications.ToList());
            if (modificationsGrid != null)
                await modificationsGrid.Reload();
        }

        private async Task AlertVisibleChange(bool value)
        {
            automaticModificationsAlertVisible = value;
            await OnAlertVisibleChanged.InvokeAsync((12, automaticModificationsAlertVisible));
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await modificationsGrid.GoToPage(0);
            await GridData_Update();
        }

        private async Task OnPageChanged(object args)
        {
            if (args is PagerEventArgs pageArgs)
            {
                currentPage = pageArgs.PageIndex + 1;
                await LoadVisibleItems();
            }
        }

        private async Task LoadVisibleItems()
        {
            visibleItems = modifications
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        async Task ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null)
        {
            TooltipService.Open(elementReference, content, options);
            await Task.Delay(1000);
            TooltipService.Close();
        }

        private async Task<bool> IsAllPageSelected()
        {
            return visibleItems.All(item => selectedModifications.Contains(item)) && (selectedModifications.Any());
        }

        private async Task SelectAllItems(bool select)
        {
            if (select)
            {
                foreach (var item in visibleItems)
                {
                    if (!selectedModifications.Contains(item))
                    {
                        selectedModifications.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in visibleItems)
                {
                    selectedModifications.Remove(item);
                }
            }
        }

        private async Task ToggleSelection(AutomaticCustomerOrderInProcessModification item, bool isSelected)
        {
            if (isSelected)
            {
                if (!selectedModifications.Contains(item))
                {
                    selectedModifications.Add(item);
                }
            }
            else
            {
                selectedModifications.Remove(item);
            }
        }

        protected async Task DisableModifications(AutomaticCustomerOrderInProcessModification args)
        {
            var alertVisible = automaticModificationsAlertVisible;

            if (await DialogService.Confirm("Desea ocultar las modificaciones seleccionadas?. No volverán a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Ocultar modificaciones") == false)
                return;

            try
            {
                if (!selectedModifications.Any())
                {
                    await VisualizedAutomaticCustomerInProcessModificationService.AddAsync(new VisualizedAutomaticCustomerOrderInProcessModification
                    { 
                        Id = args.Id, 
                        ActionType = args.ActionType, 
                        Employee_Id = employee.EmployeeId 
                    });
                    await UpdateModificationsAsync();
                    await AlertVisibleChange(alertVisible);
                    await LoadVisibleItems();
                    return;
                }

                isLoadingInProgress = true;
                foreach (var modification in selectedModifications)
                    await VisualizedAutomaticCustomerInProcessModificationService.AddAsync(new VisualizedAutomaticCustomerOrderInProcessModification 
                    { 
                        Id = modification.Id, 
                        ActionType = modification.ActionType, 
                        Employee_Id = employee.EmployeeId 
                    });
            }
            finally
            {
                await UpdateModificationsAsync();
                await AlertVisibleChange(alertVisible);
                selectedModifications = new List<AutomaticCustomerOrderInProcessModification>();
                await LoadVisibleItems();
                isLoadingInProgress = false;
            }
        }

        protected async Task MarkAsVisualized(AutomaticCustomerOrderInProcessModification modification)
        {
            await DisableModifications(modification);
        }

        public async Task ShowOrderInfo(string orderNumber)
        {
            try
            {
                // Buscar la modificación por orderNumber para obtener el CUSTOMER_ORDER_ID
                var modification = modifications.FirstOrDefault(m => m.Order_Number == orderNumber);
                if (modification?.CUSTOMER_ORDER_ID > 0)
                {
                    // Usar directamente el CUSTOMER_ORDER_ID para abrir el diálogo de detalles del pedido
                    await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido", 
                        new Dictionary<string, object> { { "CustomerOrderId", modification.CUSTOMER_ORDER_ID } }, 
                        options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
                    return;
                }
                
                // Si no se puede obtener el CUSTOMER_ORDER_ID, mostrar información básica
                await DialogService.OpenAsync("Información del Pedido", ds => (RenderFragment)(builder =>
                {
                    builder.OpenElement(0, "div");
                    builder.AddAttribute(1, "style", "padding: 20px;");
                    
                    builder.OpenElement(2, "div");
                    builder.AddAttribute(3, "style", "margin-bottom: 15px;");
                    builder.OpenElement(4, "strong");
                    builder.AddContent(5, "Número de Pedido: ");
                    builder.CloseElement();
                    builder.AddContent(6, orderNumber);
                    builder.CloseElement();
                    
                    builder.OpenElement(7, "div");
                    builder.AddAttribute(8, "style", "margin-bottom: 10px; font-size: 14px; color: #666;");
                    builder.AddContent(9, "Para ver los detalles completos del pedido, navegue a la sección de 'Pedido de artículos' en el menú principal.");
                    builder.CloseElement();
                    
                    builder.CloseElement();
                }), new DialogOptions() 
                { 
                    CloseDialogOnOverlayClick = true,
                    Width = "400px"
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error showing order info for order: {OrderNumber}", orderNumber);
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Error",
                    Severity = NotificationSeverity.Error,
                    Detail = "No se pudo mostrar la información del pedido."
                });
            }
        }

        #endregion

        #endregion
    }

    public abstract class AutomaticInProcessModificationNotificationsBase : ComponentBase
    {
    }
}