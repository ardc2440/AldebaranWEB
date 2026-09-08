using Aldebaran.Web.Services.Notifications;
using Aldebaran.Application.Services.Notifications.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Aldebaran.Web.Shared
{
    public partial class NotificationHook : ComponentBase, IDisposable
    {
        [Inject]
        protected INotificationStore NotificationStore { get; set; } = null!;

        [Inject]
        protected SecurityService Security { get; set; } = null!;

        [Inject]
        protected IJSRuntime JS { get; set; } = null!;

        private PeriodicTimer? _timer;
        private CancellationTokenSource? _cts;
        private long _lastVersionRead;

        protected override Task OnInitializedAsync()
        {
            _cts = new CancellationTokenSource();

            _timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            _ = Task.Run(() => StartMonitoringAsync(_cts.Token));

            return Task.CompletedTask;
        }

        private async Task StartMonitoringAsync(CancellationToken ct)
        {
            while (_timer != null && await _timer.WaitForNextTickAsync(ct))
                await ProcessNotificationsAsync(ct);

        }

        private async Task ProcessNotificationsAsync(CancellationToken ct)
        {
            if (_lastVersionRead == NotificationStore.Version)
            {
                return;
            }

            _lastVersionRead = NotificationStore.Version;

            var notifications = NotificationStore.GetNotifications();

            foreach (var notification in notifications)
            {
                if (!ApplyToCurrentUser(notification))
                    continue;

                await JS.InvokeVoidAsync("aldebaranNotifications.show", notification.Name, notification.NotificationMessage);
            }
        }

        private bool ApplyToCurrentUser(NotificationEvent notification)
        {
            if (!Security.IsAuthenticated())
                return false;

            if (notification.Roles.Any() && !Security.IsInRole(notification.Roles.ToArray()))
                return false;

            return true;
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _timer?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}