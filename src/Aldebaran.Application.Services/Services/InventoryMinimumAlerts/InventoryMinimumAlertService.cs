using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Notificator;
using Aldebaran.Application.Services.Notificator.Model;
using Aldebaran.Infraestructure.Common.Security;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Aldebaran.Application.Services.InventoryMinimumAlerts
{
    public partial class InventoryMinimumAlertService : IInventoryMinimumAlertService
    {
        private readonly INotificationService _notificationService;
        private readonly IEmployeeService _employeeService;
        private readonly IOptions<InventoryMinimumAlertSettings> _settings;
        private readonly IDashBoardService _dashboardService;
        private readonly IFileBytesGeneratorService _fileBytesGeneratorService;
        private readonly IEncryptionService _encryptionService;
        private string _imageRepositoryPath;
        private string _applicationUrl;
        private static readonly Regex _articleCodeRegex = new(@"\[(.*?)\]", RegexOptions.Compiled);

        public InventoryMinimumAlertService(IEncryptionService encryptionService, IFileBytesGeneratorService fileBytesGeneratorService, IOptions<InventoryMinimumAlertSettings> settings, INotificationService notificationService, IEmployeeService employeeService, IDashBoardService dashboardService)
        {
            _encryptionService = encryptionService;
            _notificationService = notificationService;
            _employeeService = employeeService;
            _settings = settings;
            _dashboardService = dashboardService;
            _fileBytesGeneratorService = fileBytesGeneratorService;
        }

        public async Task ExecuteAsync(string imagePath, string applicationUrl, CancellationToken ct = default)
        {
            var employees = await GetRecipientsAsync(ct);

            _imageRepositoryPath = imagePath;
            _applicationUrl = applicationUrl;

            foreach (var employee in employees)
            {
                var alarms = await GetInventoryMinimumDataAsync(employee.EmployeeId, ct);

                if (!alarms.Any()) continue;

                var excel = await GenerateExcelAsync(alarms, ct);
                var markAsReadLink = GenerateMarkAsReadLinkAsync(employee, alarms, ct);

                var message = BuildMessage(employee, excel, markAsReadLink, ct);

                await _notificationService.Send(message, ct);
            }
        }

        private async Task<ICollection<EmployeeMail>> GetRecipientsAsync(CancellationToken ct = default)
        {
            var recipients = await _employeeService.GetEmployeeMailsByRoleNameAsync(_settings.Value.RoleName, ct);
            return recipients;
        }

        private async Task<List<InventoryMinimumDto>> GetInventoryMinimumDataAsync(int employeeId, CancellationToken ct = default)
        {
            var alarms = await _dashboardService.GetMinimumQuantityAlarmsAsync(employeeId, ct: ct);

            return alarms
                .Select(x => new InventoryMinimumDto
                {
                    AlarmId = x.AlarmId,
                    ArticleName = x.ArticleName,
                    ImagePath = GetImagePath(x.ArticleName),
                    ReferenceId = x.ReferenceId,
                    AvailableQuantity = x.AvailableQuantity,
                    MinimumQuantity = x.MinimumQuantity,
                    InTransitQuantity = x.InTransitQuantity,
                    OrderedQuantity = x.OrderedQuantity,
                    ReservedQuantity = x.ReservedQuantity
                })
                .ToList();
        }

        private Task<byte[]> GenerateExcelAsync(List<InventoryMinimumDto> data, CancellationToken ct = default)
        {
            return _fileBytesGeneratorService.GetExcelBytes(data);
        }

        private string GenerateMarkAsReadLinkAsync(EmployeeMail employee, List<InventoryMinimumDto> alarms, CancellationToken ct = default)
        {
            if (!alarms.Any()) return string.Empty;

            var payload = new MarkAlarmsAsReadPayload
            {
                EmployeeId = employee.EmployeeId,
                AlarmIds = alarms
                    .Select(s => s.AlarmId)
                    .Distinct()
                    .ToList(),
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            var json = JsonConvert.SerializeObject(payload);

            var encryptedToken = _encryptionService.Encrypt(json);

            return $"{_applicationUrl}/Notification/MarkMinimumQuantityAlarmsAsRead?token={Uri.EscapeDataString(encryptedToken)}";
        }

        private static MessageModel BuildMessage(EmployeeMail employeeData, byte[] excelData, string markAsReadLink, CancellationToken ct = default)
        {
            return new MessageModel
            {
                Body = new MessageModel.EnvelopeBody
                {
                    Subject = "Inventory Minimum Alert",
                    Template = "InventoryMinimumAlertTemplate"
                },
                Header = new MessageModel.EnvelopeHeader
                {
                    Subject = "Inventory Minimum Alert",
                    MessageUid = "",
                    ReceiverUrn = "".Split(',')
                }
            };
        }

        private string? GetImagePath(string articleName)
        {
            if (string.IsNullOrWhiteSpace(articleName))
                return null;

            var match = _articleCodeRegex.Match(articleName);

            if (!match.Success) return null;

            var code = match.Groups[1].Value;

            var imagePath = Path.Combine(_imageRepositoryPath, $"{code}.jpg");

            return File.Exists(imagePath) ? imagePath : null;
        }

        internal sealed class MarkAlarmsAsReadPayload
        {
            public int EmployeeId { get; set; }

            public List<int> AlarmIds { get; set; } = new List<int>();

            public DateTime ExpirationDate { get; set; }
        }
    }
}
