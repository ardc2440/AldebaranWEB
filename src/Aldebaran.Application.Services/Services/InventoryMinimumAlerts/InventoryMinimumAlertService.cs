using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Notificator;
using Aldebaran.Application.Services.Notificator.Model;
using Aldebaran.Application.Services.NotificationsAccessToken;
using Aldebaran.Infraestructure.Common.Security;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Aldebaran.Application.Services.InventoryMinimumAlerts.Models;
using Aldebaran.Application.Services.InventoryMinimumAlerts.Models;

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
        private readonly INotificationAccessTokenService _notificationAccessTokenService;
        private readonly INotificationTemplateService _notificationTemplateService;

        private static readonly Regex _articleCodeRegex = new(@"\[(.*?)\]", RegexOptions.Compiled);

        private string _imageRepositoryPath = "";
        private string _markAsReadUrl = "";
        
        public InventoryMinimumAlertService(
            INotificationAccessTokenService notificationAccessTokenService, 
            IEncryptionService encryptionService, 
            IFileBytesGeneratorService fileBytesGeneratorService, 
            IOptions<InventoryMinimumAlertSettings> settings, 
            INotificationService notificationService, 
            IEmployeeService employeeService, 
            IDashBoardService dashboardService,
            INotificationTemplateService notificationTemplateService)
        {
            _encryptionService = encryptionService;
            _notificationService = notificationService;
            _employeeService = employeeService;
            _settings = settings;
            _notificationTemplateService = notificationTemplateService;
            _dashboardService = dashboardService;
            _fileBytesGeneratorService = fileBytesGeneratorService;
            _notificationAccessTokenService = notificationAccessTokenService;
            _markAsReadUrl = _settings.Value.MarkAsReadUrl;
        }

        public async Task ExecuteAsync(string imagePath, CancellationToken ct = default)
        {
            var employees = await GetRecipientsAsync(ct);

            if (!employees.Any()) return;

            _imageRepositoryPath = imagePath;

            var notificationTemplate = await _notificationTemplateService.FindAsync(_settings.Value.NotificationSubject, ct) ?? throw new InvalidOperationException("Notification template not found");

            foreach (var employee in employees)
            {
                var alarms = await GetInventoryMinimumDataAsync(employee.EmployeeId, ct);

                if (!alarms.Any()) continue;

                var excel = await GenerateExcelAsync(alarms, ct);
                var (markAsReadLink, tokenId) = GenerateMarkAsReadLinkAsync();
                var message = BuildMessage(employee, excel);
                var aditionalBodyMessage = BuildAdditionalBodyMessage(markAsReadLink);

                /* Despues de pasar todos los metodos se persiste el token el EmployeeId y la Lista de Alarmas 
                   Es preferible un token perdido y no un link huerfano */

                if (await SaveNotificationToken(employee.EmployeeId, notificationTemplate.NotificationTemplateId, alarms, tokenId, ct))
                    await _notificationService.Send(message, aditionalBodyMessage, ct);                
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
            var exportData = data.Select(x => new InventoryMinimumExportDto
            {
                ArticleName = x.ArticleName,
                ImagePath = x.ImagePath,
                AvailableQuantity = x.AvailableQuantity,
                MinimumQuantity = x.MinimumQuantity,
                InTransitQuantity = x.InTransitQuantity,
                OrderedQuantity = x.OrderedQuantity,
                ReservedQuantity = x.ReservedQuantity
            }).OrderBy(x => x.ArticleName).ToList();

            return _fileBytesGeneratorService.GetExcelBytes(exportData);
        }

        private (string url,Guid tokenId) GenerateMarkAsReadLinkAsync()
        {
            var _tokenId = Guid.NewGuid();
            
            var encryptedToken = _encryptionService.Encrypt(_tokenId.ToString());

            return ($"{_markAsReadUrl}?token={Uri.EscapeDataString(encryptedToken)}", _tokenId);
        }

        private MessageModel BuildMessage(EmployeeMail employeeData, byte[] excelData)
        {
            var excelBase64 = Convert.ToBase64String(excelData);

            return new MessageModel
            {
                Header = new MessageModel.EnvelopeHeader
                {
                    MessageUid = Guid.NewGuid().ToString(),
                    ReceiverUrn = new[] { employeeData.Email },
                    // Debe coincidir con NotificationSettings
                    Subject = _settings.Value.NotificationSettings
                },

                Body = new MessageModel.EnvelopeBody
                {
                    Template = _settings.Value.NotificationSubject,
                    Medias = new List<MessageModel.EnvelopeBody.MediaContent>{
                        new MessageModel.EnvelopeBody.MediaContent{
                            ContentType ="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            FileName = $"InventarioMinimo_{DateTime.Now:yyyyMMdd HHmm}.xlsx",
                            Hash =excelBase64
                        }
                    }
                }
            };
        }

        private static string BuildAdditionalBodyMessage(string markAsReadLink)
        {
            return $@"  <br/>
                        <br/>
                        <p>
                            Puede marcar todas las alarmas incluidas
                            en esta notificación como leídas
                            haciendo clic en el siguiente enlace:
                        </p>
                        <p>
                            <a href=""{markAsReadLink}""> 
                                Marcar alarmas como leídas
                            </a>
                        </p>";
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

        private async Task<bool> SaveNotificationToken(int employeeId, short templateId, List<InventoryMinimumDto> alarms, Guid tokenId, CancellationToken ct)
        { 
            var safedToken = await _notificationAccessTokenService.AddAsync(employeeId, templateId, alarms.Select(a => a.AlarmId).ToList(), tokenId, ct);
            return safedToken;
        }

        internal sealed class MarkAlarmsAsReadPayload { public Guid NotificationAccessTokenId { get; set; } }
    }
}
