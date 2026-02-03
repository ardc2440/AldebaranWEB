using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.ConnectivityPages
{
    public enum ConnectivityType
    {
        InventoryAutomation = 1,
        FtpWriting = 2
    }

    public partial class Connectivity
    {
        #region Injections

        [Inject]
        protected ILogger<Connectivity> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IInventoryAutomationConnectionService InventoryAutomationConnectionService { get; set; }

        [Inject]
        protected IFtpWritingConnectionService FtpWritingConnectionService { get; set; }

        [Inject]
        protected IAutomataNotificationRecipientService AutomataNotificationRecipientService { get; set; }

        [Inject]
        protected IAutomataConnectivityErrorPatternService AutomataConnectivityErrorPatternService { get; set; }

        #endregion

        #region Variables

        protected bool isLoadingInProgress;

        protected ConnectivityType SelectedType = ConnectivityType.InventoryAutomation;

        protected IEnumerable<InventoryAutomationConnection> InventoryConnections;
        protected LocalizedDataGrid<InventoryAutomationConnection> InventoryConnectionsGrid;

        protected IEnumerable<FtpWritingConnection> FtpConnections;
        protected LocalizedDataGrid<FtpWritingConnection> FtpConnectionsGrid;

        protected IEnumerable<AutomataNotificationRecipient> NotificationRecipients;
        protected LocalizedDataGrid<AutomataNotificationRecipient> NotificationRecipientsGrid;

        protected IEnumerable<AutomataConnectivityErrorPattern> ConnectivityErrorPatterns;
        protected LocalizedDataGrid<AutomataConnectivityErrorPattern> ConnectivityErrorPatternsGrid;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await LoadCurrentTypeAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Loaders

        protected async Task OnTypeChanged(object value)
        {
            if (value is ConnectivityType type)
            {
                SelectedType = type;
            }

            await LoadCurrentTypeAsync();
        }

        protected async Task LoadCurrentTypeAsync(CancellationToken ct = default)
        {
            await Task.Yield();

            switch (SelectedType)
            {
                case ConnectivityType.InventoryAutomation:
                    InventoryConnections = await InventoryAutomationConnectionService.GetAllAsync(ct);
                    NotificationRecipients = await AutomataNotificationRecipientService.GetAllAsync(ct);
                    ConnectivityErrorPatterns = await AutomataConnectivityErrorPatternService.GetAllAsync(ct);
                    break;
                case ConnectivityType.FtpWriting:
                    FtpConnections = await FtpWritingConnectionService.GetAllAsync(ct);
                    break;
            }
        }

        #endregion

        #region Helpers

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null)
            => TooltipService.Open(elementReference, content, options);

        protected string GetConnectivityTypeText(ConnectivityType type) =>
            type switch
            {
                ConnectivityType.InventoryAutomation => "Automata de Existencias",
                ConnectivityType.FtpWriting => "FTP Writing Service",
                _ => type.ToString()
            };

        #endregion

        #region Inventory Automation events

        protected async Task AddInventoryConnection(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddInventoryAutomationConnection>("Nueva conexión Automata");

            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Conectividad",
                    Severity = NotificationSeverity.Success,
                    Detail = "Conexión Automata creada correctamente."
                });
            }

            await LoadCurrentTypeAsync();
            await InventoryConnectionsGrid.Reload();
        }

        protected async Task AddNotificationRecipient(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddAutomataNotificationRecipient>("Nuevo destinatario");

            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Conectividad",
                    Severity = NotificationSeverity.Success,
                    Detail = "Destinatario creado correctamente."
                });
            }

            await LoadCurrentTypeAsync();
            await NotificationRecipientsGrid.Reload();
        }

        protected async Task EditNotificationRecipient(AutomataNotificationRecipient recipient)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Id", recipient.Id }
            };

            var result = await DialogService.OpenAsync<EditAutomataNotificationRecipient>("Editar destinatario", parameters);

            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Conectividad",
                    Severity = NotificationSeverity.Success,
                    Detail = "Destinatario actualizado correctamente."
                });
            }

            await LoadCurrentTypeAsync();
            await NotificationRecipientsGrid.Reload();
        }

        protected async Task AddConnectivityErrorPattern(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddAutomataConnectivityErrorPattern>("Nuevo patrón");

            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Conectividad",
                    Severity = NotificationSeverity.Success,
                    Detail = "Patrón creado correctamente."
                });
            }

            await LoadCurrentTypeAsync();
            await ConnectivityErrorPatternsGrid.Reload();
        }

        protected async Task EditConnectivityErrorPattern(AutomataConnectivityErrorPattern pattern)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Id", pattern.Id }
            };

            var result = await DialogService.OpenAsync<EditAutomataConnectivityErrorPattern>("Editar patrón", parameters);

            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Conectividad",
                    Severity = NotificationSeverity.Success,
                    Detail = "Patrón actualizado correctamente."
                });
            }

            await LoadCurrentTypeAsync();
            await ConnectivityErrorPatternsGrid.Reload();
        }

        protected async Task EditInventoryConnection(InventoryAutomationConnection connection)
        {
            var parameters = new Dictionary<string, object>
            {
                { "INVENTORY_AUTOMATION_CONNECTION_ID", connection.InventoryAutomationConnectionId }
            };

            var result = await DialogService.OpenAsync<EditInventoryAutomationConnection>("Actualizar conexión Automata", parameters);

            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Conectividad",
                    Severity = NotificationSeverity.Success,
                    Detail = "Conexión Automata actualizada correctamente."
                });
            }

            await LoadCurrentTypeAsync();
            await InventoryConnectionsGrid.Reload();
        }

        protected async Task ToggleInventoryConnectionActive(MouseEventArgs args, InventoryAutomationConnection connection)
        {
            try
            {
                var action = connection.Active ? "desactivar" : "activar";

                if (await DialogService.Confirm($"¿Está seguro que desea {action} esta conexión Automata?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar cambio de estado") == true)
                {
                    await InventoryAutomationConnectionService.ChangeActivationAsync(connection.InventoryAutomationConnectionId, !connection.Active);
                    await LoadCurrentTypeAsync();
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Conectividad",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Conexión Automata {action}da correctamente."
                    });
                    await InventoryConnectionsGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(ToggleInventoryConnectionActive));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = "No se ha podido cambiar el estado de la conexión Automata."
                });
            }
        }

        #endregion

        #region FTP events

        protected async Task AddFtpConnection(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddFtpWritingConnection>("Nueva conexión FTP");

            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Conectividad",
                    Severity = NotificationSeverity.Success,
                    Detail = "Conexión FTP creada correctamente."
                });
            }

            await LoadCurrentTypeAsync();
            await FtpConnectionsGrid.Reload();
        }

        protected async Task EditFtpConnection(FtpWritingConnection connection)
        {
            var parameters = new Dictionary<string, object>
            {
                { "FTP_WRITING_CONNECTION_ID", connection.FtpWritingConnectionId }
            };

            var result = await DialogService.OpenAsync<EditFtpWritingConnection>("Actualizar conexión FTP", parameters);

            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Conectividad",
                    Severity = NotificationSeverity.Success,
                    Detail = "Conexión FTP actualizada correctamente."
                });
            }

            await LoadCurrentTypeAsync();
            await FtpConnectionsGrid.Reload();
        }

        protected async Task ToggleFtpConnectionActive(MouseEventArgs args, FtpWritingConnection connection)
        {
            try
            {
                var action = connection.Active ? "desactivar" : "activar";

                if (await DialogService.Confirm($"¿Está seguro que desea {action} esta conexión FTP?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar cambio de estado") == true)
                {
                    await FtpWritingConnectionService.ChangeActivationAsync(connection.FtpWritingConnectionId, !connection.Active);
                    await LoadCurrentTypeAsync();
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Conectividad",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Conexión FTP {action}da correctamente."
                    });
                    await FtpConnectionsGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(ToggleFtpConnectionActive));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = "No se ha podido cambiar el estado de la conexión FTP."
                });
            }
        }

        #endregion
    }
}
