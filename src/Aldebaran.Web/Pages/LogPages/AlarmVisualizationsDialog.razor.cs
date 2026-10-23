using Aldebaran.Application.Services.AlarmLogs;
using Aldebaran.Application.Services.Models.AlarmLogs;
using Microsoft.AspNetCore.Components;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>Quién vio una alarma activa y cuándo (visualizaciones vigentes).</summary>
    public partial class AlarmVisualizationsDialog
    {
        [Inject] protected ILogger<AlarmVisualizationsDialog> Logger { get; set; }
        [Inject] protected IAlarmLogService AlarmLogService { get; set; }

        [Parameter] public string TypeCode { get; set; }
        [Parameter] public AlarmLogItem Alarm { get; set; }

        protected IReadOnlyList<AlarmVisualization> visualizations = Array.Empty<AlarmVisualization>();
        protected bool isLoading;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = true;
                visualizations = await AlarmLogService.GetVisualizationsAsync(TypeCode, Alarm.AlarmKey);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
