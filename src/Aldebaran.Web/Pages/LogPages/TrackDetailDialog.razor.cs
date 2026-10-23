using Aldebaran.Application.Services.Models.TrackLogs;
using Aldebaran.Application.Services.TrackLogs;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>Detalle campo / anterior / nuevo de un movimiento del Track. Los campos que cambiaron se resaltan.</summary>
    public partial class TrackDetailDialog
    {
        [Inject] protected ILogger<TrackDetailDialog> Logger { get; set; }
        [Inject] protected ITrackLogService TrackLogService { get; set; }

        [Parameter] public TrackEntry Entry { get; set; }

        protected TrackDetail detail;
        protected bool onlyChanges = true;
        protected bool isLoading;

        protected IEnumerable<TrackChange> VisibleChanges =>
            detail == null ? Array.Empty<TrackChange>()
            : onlyChanges && detail.Action == TrackAction.Modified && detail.HasPrevious ? detail.Changes.Where(c => c.Changed)
            : detail.Changes;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = true;
                detail = await TrackLogService.GetDetailAsync(Entry.Id);
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

        protected void RowRender(RowRenderEventArgs<TrackChange> args)
        {
            if (args.Data.Changed && detail?.Action == TrackAction.Modified && detail.HasPrevious)
                args.Attributes.Add("style", "font-weight: 600; background-color: var(--rz-warning-lighter);");
        }
    }
}
