using Aldebaran.Application.Services.NotificationsAccessToken;
using Aldebaran.Application.Services.InventoryMinimumAlerts.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Aldebaran.Application.Services.Services;
using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.InventoryMinimumAlerts
{
    public class InventoryMinimumAlertTokenHandler : INotificationAccessTokenHandler
    {
        private readonly IOptions<InventoryMinimumAlertSettings> _settings;
        private readonly IVisualizedMinimumQuantityAlarmService _visualizedMinimumQuantityAlarmService;
        private readonly IEmployeeService _employeeService;

        public string NotificationTemplateName =>   _settings.Value.NotificationSubject;

        public InventoryMinimumAlertTokenHandler(IEmployeeService employeeService, IVisualizedMinimumQuantityAlarmService visualizedMinimumQuantityAlarmService, IOptions<InventoryMinimumAlertSettings> settings)
        {
            _employeeService = employeeService;
            _visualizedMinimumQuantityAlarmService = visualizedMinimumQuantityAlarmService;
            _settings = settings;
        }

        public async Task<bool> ExecuteAsync(string? extraData, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(extraData)) return false;

            var data = JsonConvert.DeserializeObject<TokenExtraDataDto>(extraData!);

            if (data == null) return false;

            if (!data.AlarmIds.Any())return false;

            if (data.EmployeeId <= 0) return false;

            var employee = await _employeeService.FindAsync(data.EmployeeId, ct);   
            
            if (employee == null) return false;

            foreach (var alert in data!.AlarmIds)
            {
                var alarm = new VisualizedMinimumQuantityAlarm
                {
                    MinimumQuantityAlarmId = alert,
                    EmployeeId = data.EmployeeId
                };

                await _visualizedMinimumQuantityAlarmService.AddAsync(alarm, ct);
            }

            return true;
        }
    }
}
