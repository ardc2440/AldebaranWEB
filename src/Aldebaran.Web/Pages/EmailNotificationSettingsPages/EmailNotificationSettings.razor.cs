using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.EmailNotificationSettingsPages
{
    public partial class EmailNotificationSettings
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ILogger<EmailNotificationSettings> Logger { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected INotificationTemplateService NotificationTemplateService { get; set; }

        [Inject]
        public IEmailNotificationProviderSettingsService EmailNotificationProviderSettingsService { get; set; }
        #endregion

        #region Variables
        protected ServiceModel.EmailNotificationProvider Provider;
        protected IEnumerable<ServiceModel.EmailNotificationProvider.SecureSocketOptions> SecureSocketOptions;
        protected ServiceModel.NotificationTemplate PurchaseOrderNew;
        protected ServiceModel.NotificationTemplate PurchaseOrderUpdate;
        protected ServiceModel.NotificationTemplate PurchaseOrderForwarding;
        protected ServiceModel.NotificationTemplate CustomerReservationNew;
        protected ServiceModel.NotificationTemplate CustomerReservationUpdate;
        protected ServiceModel.NotificationTemplate CustomerReservationForwarding;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        private static readonly string SettingsKey = "Sales";
        public string ConfirmPassword;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await GetEmailProviderSettings();
                await GetEmailTemplates();
                SecureSocketOptions = Enum.GetValues(typeof(ServiceModel.EmailNotificationProvider.SecureSocketOptions))
                                        .Cast<ServiceModel.EmailNotificationProvider.SecureSocketOptions>()
                                        .ToList();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        private async Task GetEmailProviderSettings()
        {
            Provider = await EmailNotificationProviderSettingsService.GetAsync(SettingsKey);
            Provider.Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
        private async Task GetEmailTemplates()
        {
            var templates = await NotificationTemplateService.GetAsync();
            PurchaseOrderNew = templates.FirstOrDefault(w => w.Name == "Customer:Order:New");
            PurchaseOrderUpdate = templates.FirstOrDefault(w => w.Name == "Customer:Order:Update");
            PurchaseOrderForwarding = templates.FirstOrDefault(w => w.Name == "Customer:Order:Forwarding");
            CustomerReservationNew = templates.FirstOrDefault(w => w.Name == "Customer:Reservation:New");
            CustomerReservationUpdate = templates.FirstOrDefault(w => w.Name == "Customer:Reservation:Update");
            CustomerReservationForwarding = templates.FirstOrDefault(w => w.Name == "Customer:Reservation:Forwarding");
        }
        protected async Task FormSubmit()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar las configuraciones del proveedor de correo?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await EmailNotificationProviderSettingsService.UpdateAsync(SettingsKey, Provider);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Proveedor de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Configuraciones del proveedor de correo han sido actualizadas correctamente.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar el proveedor de correo.",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailProviderSettings();
            }
        }

        protected async Task UpdatePurchaseOrderNew()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Creación del pedido\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(PurchaseOrderNew.NotificationTemplateId, PurchaseOrderNew);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Creación del pedido\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdatePurchaseOrderNew));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Creación del pedido\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdatePurchaseOrderUpdate()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Actualización del pedido\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(PurchaseOrderUpdate.NotificationTemplateId, PurchaseOrderUpdate);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Actualización del pedido\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdatePurchaseOrderUpdate));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Actualización del pedido\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdatePurchaseOrderForwarding()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Reenvío del pedido\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(PurchaseOrderForwarding.NotificationTemplateId, PurchaseOrderForwarding);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Reenvío del pedido\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdatePurchaseOrderForwarding));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Reenvío del pedido\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdateCustomerReservationNew()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Confirmación de la reserva\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerReservationNew.NotificationTemplateId, CustomerReservationNew);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Confirmación de la reserva\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerReservationNew));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Confirmación de la reserva\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdateCustomerReservationUpdate()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Actualizacion de la reserva\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerReservationUpdate.NotificationTemplateId, CustomerReservationUpdate);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Actualizacion de la reserva\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerReservationUpdate));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Actualizacion de la reserva\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdateCustomerReservationForwarding()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Reenvío de la reserva\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerReservationForwarding.NotificationTemplateId, CustomerReservationForwarding);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Reenvío de la reserva\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerReservationForwarding));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Reenvío de la reserva\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        #endregion
    }
}
