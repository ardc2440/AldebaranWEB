using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Radzen;

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


        #endregion

        #region Global Variables
        protected StatusDocumentType pendingStatusOrder;
        List<DataTimer> Timers;
        readonly GridTimer GridTimer = new GridTimer("Dahsboard-GridTimer");
        private MemoryCacheEntryOptions _cacheEntryOptions;
        protected bool isLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Timers = TimerPreferenceService.Timers;
                await InitializeGridTimers();
                pendingStatusOrder = await DashBoardService.FindByDocumentAndOrderAsync((await DashBoardService.FindByCodeAsync("O")).DocumentTypeId, 1);
                _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };
                await GridData_Update();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }
        #endregion

        #region Events

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
            try
            {
                isLoadingInProgress = true;
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        private async Task GridaData_UpdateOnTimerChange(object value)
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
    }
}