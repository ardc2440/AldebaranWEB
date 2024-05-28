using Aldebaran.DataAccess.Infraestructure.Models;
using Aldebaran.Infraestructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class EmailNotificationProviderSettingsRepository : RepositoryBase<AldebaranDbContext>, IEmailNotificationProviderSettingsRepository
    {

        public EmailNotificationProviderSettingsRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<EmailNotificationProvider> GetAsync(string subject, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var provider = await dbContext.NotificationProviderSettings.AsNoTracking().FirstOrDefaultAsync(f => f.Subject == subject, ct);
                if (provider == null)
                    throw new ArgumentNullException($"Configuración con nombre {subject} no existe.");
                var emailProvider = JsonConvert.DeserializeObject<EmailNotificationProvider>(provider.Settings);
                if (emailProvider == null)
                    throw new ArgumentNullException($"Configuración con nombre {subject} no se encuentra correctamente almacenada.");
                return emailProvider;
            }, ct);
        }
        public async Task UpdateAsync(string subject, EmailNotificationProvider provider, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                if (provider == null)
                    throw new ArgumentNullException($"Proveedor debe contener un valor.");
                if (string.IsNullOrEmpty(subject))
                    throw new ArgumentNullException($"Llave de configuración debe contener un valor.");
                var currentProvider = await dbContext.NotificationProviderSettings.FirstOrDefaultAsync(f => f.Subject == subject, ct);
                if (currentProvider == null)
                    throw new ArgumentNullException($"Configuración con nombre {subject} no existe.");
                if (string.IsNullOrEmpty(provider.MailServer))
                    throw new ArgumentNullException($"Servidor de correo debe contener un valor.");
                if (provider.Port <= 0)
                    throw new ArgumentNullException($"Puerto del servidor de correo no puede ser menor o igual a 0.");
                if (string.IsNullOrEmpty(provider.SenderName))
                    throw new ArgumentNullException($"Nombre del remitente debe contener un valor.");
                if (string.IsNullOrEmpty(provider.SenderEmail))
                    throw new ArgumentNullException($"Correo del remitente debe contener un valor.");
                if (string.IsNullOrEmpty(provider.Password))
                    throw new ArgumentNullException($"Contraseña del correo del remitente debe contener un valor.");
                var settings = new Dictionary<string, string> {
                {"mail_server",provider.MailServer.Trim() },
                {"port",provider.Port.ToString() },
                {"sender_name",provider.SenderName.Trim() },
                {"sender_email",provider.SenderEmail.Trim() },
                {"password",provider.Password.Trim().Encrypt() },
                {"secure_socket_option",provider.SecureSocketOption.ToString() },
                        };
                currentProvider.Settings = JsonConvert.SerializeObject(settings);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }
    }
}
