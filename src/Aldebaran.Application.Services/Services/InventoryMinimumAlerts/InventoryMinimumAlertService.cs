using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Notificator;
using Aldebaran.Application.Services.Notificator.Model;
using Aldebaran.Application.Services.NotificationsAccessToken;
using Aldebaran.Infraestructure.Common.Security;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
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
        private readonly IArticleInventoryService _articleInventoryService;
        private readonly IImageThumbnailService _imageThumbnailService;
        private readonly ILogger<InventoryMinimumAlertService> _logger;

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
            INotificationTemplateService notificationTemplateService,
            IArticleInventoryService articleInventoryService,
            IImageThumbnailService imageThumbnailService,
            ILogger<InventoryMinimumAlertService> logger)
        {
            _imageThumbnailService = imageThumbnailService ?? throw new ArgumentNullException(nameof(imageThumbnailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _articleInventoryService = articleInventoryService ?? throw new ArgumentNullException(nameof(articleInventoryService));
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

                var (excel, withoutImages) = await GenerateExcelWithinLimitAsync(employee.EmployeeId, alarms, ct);
                var (markAsReadLink, tokenId) = GenerateMarkAsReadLinkAsync();
                var message = BuildMessage(employee, excel);
                var aditionalBodyMessage = BuildAdditionalBodyMessage(markAsReadLink, withoutImages);

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
                    IsBombPrice = x.IsBombPrice,
                    ReferenceId = x.ReferenceId,
                    AvailableQuantity = x.AvailableQuantity,
                    MinimumQuantity = x.MinimumQuantity,
                    InTransitQuantity = x.InTransitQuantity,
                    OrderedQuantity = x.OrderedQuantity,
                    ReservedQuantity = x.ReservedQuantity
                })
                .ToList();
        }

        /// <summary>
        /// Excel con miniaturas; si aun así supera el máximo configurado (límite del servidor de correo),
        /// se genera sin imágenes para que el correo siempre salga.
        /// </summary>
        private async Task<(byte[] Excel, bool WithoutImages)> GenerateExcelWithinLimitAsync(int employeeId, List<InventoryMinimumDto> alarms, CancellationToken ct)
        {
            var excel = await GenerateExcelAsync(alarms, ct);
            var maxBytes = (long)_settings.Value.MaxAttachmentMegabytes * 1024 * 1024;

            _logger.LogInformation("InventoryMinimumAlert: empleado {EmployeeId}, {AlarmCount} alarmas, Excel {SizeKb} KB",
                employeeId, alarms.Count, excel.Length / 1024);

            if (excel.Length <= maxBytes)
                return (excel, false);

            _logger.LogWarning("InventoryMinimumAlert: empleado {EmployeeId}, Excel de {SizeKb} KB supera {MaxMb} MB; se envía sin imágenes",
                employeeId, excel.Length / 1024, _settings.Value.MaxAttachmentMegabytes);

            alarms.ForEach(alarm => alarm.ImagePath = null);
            return (await GenerateExcelAsync(alarms, ct), true);
        }

        /// <summary>Excel agrupado por artículo con "+" (D6–D10): alarma con imagen + inventario de las referencias del artículo.</summary>
        private async Task<byte[]> GenerateExcelAsync(List<InventoryMinimumDto> data, CancellationToken ct = default)
        {
            var groups = await GetArticleAlarmGroupsAsync(data, ct);
            var exportData = InventoryMinimumGroupedExportBuilder.Build(data, groups);

            return await _fileBytesGeneratorService.GetExcelBytesWithChildRows(exportData, row => row.Children);
        }

        /// <summary>
        /// Un grupo por artículo: la primera alarma (por nombre) consulta el inventario del artículo;
        /// las demás alarmas del mismo artículo ya quedan cubiertas en ese detalle (D9).
        /// </summary>
        private async Task<List<ArticleAlarmGroup>> GetArticleAlarmGroupsAsync(List<InventoryMinimumDto> alarms, CancellationToken ct)
        {
            var groups = new List<ArticleAlarmGroup>();
            var coveredReferences = new HashSet<int>();

            foreach (var alarm in alarms.OrderBy(alarm => alarm.ArticleName))
            {
                if (coveredReferences.Contains(alarm.ReferenceId))
                    continue;

                var inventory = await _articleInventoryService.GetByReferenceAsync(alarm.ReferenceId, ct);

                coveredReferences.Add(alarm.ReferenceId);
                coveredReferences.UnionWith(inventory.Select(row => row.ReferenceId));
                groups.Add(new ArticleAlarmGroup(alarm, inventory));
            }

            return groups;
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

        private static string BuildAdditionalBodyMessage(string markAsReadLink, bool withoutImages)
        {
            var withoutImagesNote = withoutImages
                ? @"<p>El archivo adjunto se envía sin imágenes por su tamaño; puede consultarlas en el tablero de notificaciones.</p>"
                : string.Empty;

            return $@"  <br/>
                        {withoutImagesNote}
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

            /* Miniatura: la imagen original (hasta ~1,2 MB) hacía que el correo superara el límite de Gmail */
            return File.Exists(imagePath) ? _imageThumbnailService.GetThumbnailPath(imagePath, _settings.Value.ThumbnailMaxSize) : null;
        }

        private async Task<bool> SaveNotificationToken(int employeeId, short templateId, List<InventoryMinimumDto> alarms, Guid tokenId, CancellationToken ct)
        { 
            var safedToken = await _notificationAccessTokenService.AddAsync(employeeId, templateId, alarms.Select(a => a.AlarmId).ToList(), tokenId, ct);
            return safedToken;
        }

        internal sealed class MarkAlarmsAsReadPayload { public Guid NotificationAccessTokenId { get; set; } }
    }
}
