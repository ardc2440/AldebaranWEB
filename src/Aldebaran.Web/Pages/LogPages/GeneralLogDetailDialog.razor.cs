using Aldebaran.Application.Services.GeneralLogs;
using Aldebaran.Application.Services.Models.GeneralLogs;
using Microsoft.AspNetCore.Components;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>Detalle técnico de una ocurrencia del log general.</summary>
    public partial class GeneralLogDetailDialog
    {
        [Inject] protected ILogger<GeneralLogDetailDialog> Logger { get; set; }
        [Inject] protected IGeneralLogService GeneralLogService { get; set; }

        [Parameter] public long LogId { get; set; }

        protected GeneralLogDetail detail;
        protected bool loading = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                detail = await GeneralLogService.GetDetailAsync(LogId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
            finally
            {
                loading = false;
            }
        }
    }
}
