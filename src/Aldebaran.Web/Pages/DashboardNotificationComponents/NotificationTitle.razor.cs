using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class NotificationTitle
    {
        [Parameter]
        public EventCallback<MouseEventArgs> TogglePanel { get; set; }        
        [Parameter]
        public EventCallback<MouseEventArgs> DataUpdate { get; set; }
        [Parameter]
        public EventCallback<bool> OnBoolChange { get; set; }
        [Parameter]
        public bool IsPanelCollapsed { get; set; }
        [Parameter]
        public bool AlertVisible { get; set; }
        [Parameter]
        public GridTimer GridTimer { get; set; }
        [Parameter]
        public List<DataTimer> Timers { get; set; }
        [Parameter]
        public string Title { get; set; }

        protected async Task AlertClick()
        {
            AlertVisible = false;
            await OnBoolChange.InvokeAsync(AlertVisible);
        }
    }
}
